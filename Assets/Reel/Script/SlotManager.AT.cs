using System;
using UnityEngine;

// チーム連携用。相手側の優出・ATクラスを参照しない独立API。
public partial class SlotGameManager
{
    public enum ChronicleMode { Normal, Yushutsu, AT }
    public enum ATBellOrder { Random, LCR, LRC, CLR, CRL, RLC, RCL }

    [Header("AT押し順ベル（プロトタイプ）")]
    [SerializeField] private ATBellOrder atBellOrderSetting = ATBellOrder.Random;
    [Min(0)][SerializeField] private int atBellWrongPayout = 1;
    [SerializeField] private bool logATBellNavigation = true;

    private bool chronicleIsAT;
    private bool chronicleIsYushutsu;
    private string requiredATBellOrder = "";
    private string pressedATBellOrder = "";
    private bool atBellMissed;
    private int atBellStopInputCount = 0;
    private string chronicleGeneralStopOrder = "";

    // チームのシャッター演出等：引数は停止順(1/2/3)、リール種類
    public event Action<int, ReelController.ReelType> ChronicleReelStopPressed;
    public string ChronicleGeneralStopOrder { get { return chronicleGeneralStopOrder; } }
    private bool chronicleBellDiversionPlanned = false;

    // 外部演出スクリプトはこのイベントを購読できる。
    public event Action<ChronicleMode> ChronicleModeChanged;
    // UI連携: 2択ナビは第1停止だけ渡す。既存のフルナビイベントも互換維持。
    public event Action<string> ATBellFirstStopNavigationReady;
    public event Action<string> ATBellNavigationReady;
    public event Action<bool, int> ATBellResolved; // 正解か、払い出し枚数

    public bool IsChronicleAT { get { return chronicleIsAT; } }
    public bool IsChronicleYushutsu { get { return chronicleIsYushutsu; } }
    public ChronicleMode CurrentChronicleMode { get { return chronicleIsAT ? ChronicleMode.AT : chronicleIsYushutsu ? ChronicleMode.Yushutsu : ChronicleMode.Normal; } }
    public bool IsATBellGame { get { return CurrentRole == RoleType.ATBell && GameStarted; } }
    public string CurrentATBellNavigation { get { return requiredATBellOrder; } }
    public string CurrentATBellFirstStopNavigation
    {
        get
        {
            return requiredATBellOrder.Length == 3
            ? requiredATBellOrder.Substring(0, 1) : "";
        }
    }

    // 互換用。旧ベル分岐フラグ（新停止テーブルでは使用しない）。
    // StopSpinで停止予定を決定した時点で予約する。
    // 停止アニメーション完了前に次のボタンが押されても二重崩しを防ぐ。
    public bool HasChronicleBellDiversion()
    {
        return chronicleBellDiversionPlanned;
    }

    public void ReserveChronicleBellDiversion()
    {
        chronicleBellDiversionPlanned = true;
    }
    public string PressedATBellOrder { get { return pressedATBellOrder; } }
    public bool IsATBellOrderFailed { get { return atBellMissed; } }
    public int ATBellStopInputCount { get { return atBellStopInputCount; } }
    public bool IsATBellOrderCorrect { get { return requiredATBellOrder.Length == 3 && pressedATBellOrder == requiredATBellOrder && !atBellMissed; } }

    // チームのAT開始・終了処理から呼ぶ。ゲーム回転中は切替を受け付けない。
    public void SetChronicleATMode(bool enabled)
    {
        SetChronicleMode(enabled ? ChronicleMode.AT : ChronicleMode.Normal);
    }

    // チームの優出モード開始・終了処理から呼ぶ。
    public void SetChronicleYushutsuMode(bool enabled)
    {
        SetChronicleMode(enabled ? ChronicleMode.Yushutsu : ChronicleMode.Normal);
    }

    public void SetChronicleMode(ChronicleMode mode)
    {
        if (GameStarted)
        {
            Debug.LogWarning("[クロニクル] 回転中はモード変更できません。3リール停止後に切替してください。");
            return;
        }
        ChronicleMode old = CurrentChronicleMode;
        chronicleIsAT = mode == ChronicleMode.AT;
        chronicleIsYushutsu = mode == ChronicleMode.Yushutsu;
        if (old == mode) return;
        if (DebugText != null) DebugText.text = mode.ToString().ToUpperInvariant() + " MODE";
        Debug.Log("[クロニクル] モード切替 " + old + " -> " + mode);
        if (ChronicleModeChanged != null) ChronicleModeChanged(mode);
    }

    // 毎ゲーム役抽選後に呼び、押し順を確定。
    private void PrepareChronicleATBellGame()
    {
        requiredATBellOrder = "";
        pressedATBellOrder = "";
        atBellMissed = false;
        atBellStopInputCount = 0;
        chronicleBellDiversionPlanned = false;
        chronicleGeneralStopOrder = "";
        if (CurrentRole != RoleType.ATBell)
        {
            if (chronicleIsAT && CurrentRole == RoleType.Bell)
                Debug.LogError("[クロニクル] AT中なのに通常Bellが成立。Lottery/GameFlowの役変換を確認してください。");
            return;
        }
        string[] all = { "LCR", "LRC", "CLR", "CRL", "RLC", "RCL" };
        requiredATBellOrder = atBellOrderSetting == ATBellOrder.Random
            ? all[UnityEngine.Random.Range(0, all.Length)]
            : atBellOrderSetting.ToString();
        // ナビ通知はゲーム開始（GameStarted=true）後に一度だけ行う。
        // ここでは抽選と内部状態の初期化だけを行う。
    }

    // StartAllReels() の後に呼ぶ。次ゲームのナビを毎回独立して通知する。
    // 非ベルのATゲームでも成立役をログに残し、ナビ非表示が正常だと判別できる。
    private void NotifyChronicleNavigationAfterSpinStarted()
    {
        if (!GameStarted)
        {
            Debug.LogWarning("[クロニクル] ナビ通知時にゲームが開始されていません。");
            return;
        }

        if (CurrentRole != RoleType.ATBell)
        {
            if (chronicleIsAT && logATBellNavigation)
            {
                Debug.Log("[クロニクル][ATナビ] 今ゲームは " + CurrentRole
                    + " のため、第一停止ナビなし。次ゲームでATベル成立時に再抽選します。");
            }
            return;
        }

        if (requiredATBellOrder.Length != 3)
        {
            Debug.LogError("[クロニクル][ATナビ] ATベルなのに正解押し順が未設定です。");
            return;
        }

        // 押し順記録は PrepareChronicleATBellGame() で毎ゲーム初期化済み。
        if (logATBellNavigation)
        {
            Debug.Log("[クロニクル][ATナビ] 新ゲーム 第1停止="
                + CurrentATBellFirstStopNavigation
                + " / 2択 / 入力待機中");
        }

        // 表示用イベントには第1停止だけを渡す。
        if (ATBellFirstStopNavigationReady != null)
            ATBellFirstStopNavigationReady(CurrentATBellFirstStopNavigation);

        // 旧API互換：こちらはフル正解順を渡すため演出表示に使わない。
        if (ATBellNavigationReady != null)
            ATBellNavigationReady(requiredATBellOrder);
    }

    // =========================================
    // 第一停止の受付制御
    // ナビが有効なATベルのみ指定リールから停止可能。
    // それ以外（通常・優出・ATナビなし）は必ず左第一停止。
    // 第二・第三停止は残りのリールを自由に押せる。
    // =========================================
    public bool CanChronicleStopReel(ReelController.ReelType reel)
    {
        if (!GameStarted) return false;

        // 第二・第三停止に第一停止制限は適用しない。
        if (chronicleGeneralStopOrder.Length > 0) return true;

        ReelController.ReelType expected = ReelController.ReelType.Left;

        // 今ゲームでATベルのナビが実際に準備されている場合のみ、
        // ナビされたリールを第一停止に指定する。
        bool hasCurrentGameNavigation =
            chronicleIsAT &&
            CurrentRole == RoleType.ATBell &&
            requiredATBellOrder.Length == 3;

        if (hasCurrentGameNavigation)
        {
            switch (requiredATBellOrder[0])
            {
                case 'L':
                    expected = ReelController.ReelType.Left;
                    break;
                case 'C':
                    expected = ReelController.ReelType.Center;
                    break;
                case 'R':
                    expected = ReelController.ReelType.Right;
                    break;
            }
        }

        if (reel == expected) return true;

        Debug.Log("[クロニクル] 第一停止受付不可: " + reel
            + " / 必要=" + expected
            + (hasCurrentGameNavigation ? "（ATベルナビ）" : "（ナビなし・左第一停止）"));
        return false;
    }

    // ATベル：第一停止ナビ別の「11枚／1枚」目標段を返す。
    // 第1停止は両結果で共通の段にし、第2停止以降は確定した正誤で分岐する。
    // L/C first: 11=LU+CC+RU / 1=LU+CC+RL
    // R first:   11=LL+CC+RL / 1=LU+CC+RL
    // ※ C-firstのR第2停止、R-firstのL第2停止では第2停止で出目差が表れる。
    public bool TryGetChronicleATBellTargetRow(
        ReelController.ReelType reel, out ReelController.TargetRow row)
    {
        row = ReelController.TargetRow.Center;
        if (CurrentRole != RoleType.ATBell || !GameStarted ||
            requiredATBellOrder.Length != 3 || atBellStopInputCount < 1)
            return false;

        char first = requiredATBellOrder[0];
        // 第1停止は正誤どちらも同じ位置。
        // L => 左上段、C => 中中段、R => 右下段。
        bool useOneBell = atBellStopInputCount >= 2 && atBellMissed;
        if (reel == ReelController.ReelType.Center)
            row = ReelController.TargetRow.Center;
        else if (reel == ReelController.ReelType.Left)
            row = (!useOneBell && first == 'R')
                ? ReelController.TargetRow.Lower
                : ReelController.TargetRow.Upper;
        else
            row = (!useOneBell && first != 'R')
                ? ReelController.TargetRow.Upper
                : ReelController.TargetRow.Lower;
        return true;
    }

    // 旧APIを使っているチームコードとの互換を維持。
    public bool TryGetThirdStopBellRow(
        ReelController.ReelType reel, out ReelController.TargetRow row)
    {
        return TryGetChronicleATBellTargetRow(reel, out row);
    }

    // StopSpinが押された瞬間に呼ぶ。停止完了通知では順序がずれる場合がある。
    public void NotifyChronicleStopPressed(ReelController.ReelType reel)
    {
        if (!GameStarted) return;
        char key = reel == ReelController.ReelType.Left ? 'L'
            : reel == ReelController.ReelType.Center ? 'C' : 'R';
        if (chronicleGeneralStopOrder.IndexOf(key) < 0)
        {
            chronicleGeneralStopOrder += key;
            if (ChronicleReelStopPressed != null)
                ChronicleReelStopPressed(chronicleGeneralStopOrder.Length, reel);
        }
        if (CurrentRole != RoleType.ATBell) return;
        if (pressedATBellOrder.IndexOf(key) >= 0)
        {
            Debug.LogWarning("[クロニクル] 同じリールの停止入力を重複受信: " + key);
            return;
        }
        pressedATBellOrder += key;
        atBellStopInputCount++;
        if (string.IsNullOrEmpty(requiredATBellOrder) ||
            !requiredATBellOrder.StartsWith(pressedATBellOrder, StringComparison.Ordinal))
            atBellMissed = true;
        Debug.Log("[クロニクル] ATベル押し順: 正解=" + requiredATBellOrder
            + " / 入力=" + pressedATBellOrder
            + (atBellMissed ? "（不正解）" : "（ここまでは正解）"));
    }

    private void NotifyChronicleATBellResolved(bool correct, int payout)
    {
        Debug.Log("[クロニクル] AT押し順ベル " + (correct ? "正解" : "不正解") + " / PAY=" + payout);
        if (ATBellResolved != null) ATBellResolved(correct, payout);
    }
}
