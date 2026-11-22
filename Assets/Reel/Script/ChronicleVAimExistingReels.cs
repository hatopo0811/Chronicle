using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 既存3リールを直接借りる完全疑似遊技。追加のリールUIは不要。
// AT側はStartVAim()を呼び、VAimFinishedイベントで報酬を受け取る。
public class ChronicleVAimExistingReels : MonoBehaviour
{
    public enum TestOutcome { CharacterLottery, Fake, SingleV, DoubleV, PinkV }
    public enum VAimReward { None, Single, Double, Premium }

    [Header("既存オブジェクト")]
    [SerializeField] private SlotGameManager slotGameManager;
    [SerializeField] private ReelController leftReel;
    [SerializeField] private ReelController centerReel;
    [SerializeField] private ReelController rightReel;
    [SerializeField] private ChronicleVAimPresentation presentation;

    [Header("背景切替（未設定なら現在の背景を使用）")]
    [SerializeField] private RawImage backgroundRawImage;
    [SerializeField] private Texture blueBackground;
    [SerializeField] private Texture redBackground;
    [SerializeField] private Texture rainbowBackground;
    [SerializeField] private Texture premiumBackground;

    [Header("カットイン（任意）")]
    [SerializeField] private Image cutInImage;
    [SerializeField] private Sprite blueCutInSprite;
    [SerializeField] private Sprite redCutInSprite;
    [Range(0, 100)][SerializeField] private int fakeRedUpgradePercent = 3;
    [Range(0, 100)][SerializeField] private int singleRedUpgradePercent = 25;
    [Range(0, 100)][SerializeField] private int doubleRedUpgradePercent = 75;
    [Range(0, 100)][SerializeField] private int pinkRedUpgradePercent = 100;
    [SerializeField] private bool rainbowOnDoubleReverseScissors = true;
    [SerializeField] private bool premiumOnPinkFirstStop = true;

    [Header("演出時間・デバッグ")]
    [Min(0f)][SerializeField] private float resultHoldSeconds = 1.5f;
    [SerializeField] private TestOutcome testOutcome = TestOutcome.CharacterLottery;
    [SerializeField] private bool logDetails = true;

    public bool IsPlaying { get; private set; }
    public ChronicleVAimPresentation.VAimOutcome Outcome { get; private set; }
    public event Action<ChronicleVAimPresentation.VAimOutcome, VAimReward> VAimFinished;

    private bool oldSlotManagerEnabled;
    private bool waitingForReel;
    private int stopCount;
    private ReelController pendingReel;
    private string pressedOrder = "";
    private bool isFinishing;
    private bool willUpgradeRed;
    private bool didUpgradeRed;
    private Texture originalTexture;
    private Sprite originalCutInSprite;

    // 20→1のReelMaster.csvに対応する0-based中段Index。
    // Double: 左中・下V / 中中V / 右上・中V
    // Single: 左下V / 中中V / 右上V（右上がり1ラインのみ）
    // Pink: 左中PinkV / 中中PinkV / 右中PinkV
    private int TargetCenterIndex(ReelController.ReelType reel)
    {
        switch (Outcome)
        {
            case ChronicleVAimPresentation.VAimOutcome.DoubleV:
                return reel == ReelController.ReelType.Left ? 14
                     : reel == ReelController.ReelType.Center ? 16 : 15;
            case ChronicleVAimPresentation.VAimOutcome.SingleV:
                // 左中段=7番(Replay)、下段=6番(BlueV)
                // 中中段=4番(BlueV)
                // 右中段=5番(BlueV)、上段=6番(BlueV)
                // 左中段がBlueVではないため横一列は成立しない。
                return reel == ReelController.ReelType.Left ? 13
                     : reel == ReelController.ReelType.Center ? 16 : 15;
            case ChronicleVAimPresentation.VAimOutcome.PinkV:
                return reel == ReelController.ReelType.Left ? 4
                     : reel == ReelController.ReelType.Center ? 1 : 5;
            default:
                return reel == ReelController.ReelType.Left ? 14
                     : reel == ReelController.ReelType.Center ? 0 : 15;
        }
    }

    [ContextMenu("テスト：既存リールでV疑似遊技開始")]
    private void TestStart() { if (Application.isPlaying) StartVAim(); }

    public bool StartVAim()
    {
        if (IsPlaying || slotGameManager == null || presentation == null ||
            leftReel == null || centerReel == null || rightReel == null)
        {
            Debug.LogWarning("[V疑似遊技] 参照不足または実行中です", this);
            return false;
        }
        if (slotGameManager.GameStarted || leftReel.isSpinning ||
            centerReel.isSpinning || rightReel.isSpinning ||
            leftReel.isStopping || centerReel.isStopping || rightReel.isStopping)
        {
            Debug.LogWarning("[V疑似遊技] 通常ゲーム終了後、3リール完全停止状態で開始してください", this);
            return false;
        }
        // 既存のGameManager.Update()によるSpace/A/S/Dの重複処理を防ぐ。
        oldSlotManagerEnabled = slotGameManager.enabled;
        slotGameManager.enabled = false;
        bool l = leftReel.BeginVAimReelControl();
        bool c = centerReel.BeginVAimReelControl();
        bool r = rightReel.BeginVAimReelControl();
        if (!l || !c || !r)
        {
            leftReel.EndVAimReelControl();
            centerReel.EndVAimReelControl();
            rightReel.EndVAimReelControl();
            slotGameManager.enabled = oldSlotManagerEnabled;
            Debug.LogError("[V疑似遊技] リール制御の取得に失敗しました", this);
            return false;
        }

        if (backgroundRawImage != null)
            originalTexture = backgroundRawImage.texture;
        if (cutInImage != null)
            originalCutInSprite = cutInImage.sprite;

        // キャラクターごとの出現率／成功率で抽選（従来Presentation）。
        presentation.BeginVAim();
        if (!presentation.IsPlaying)
        {
            RestoreWithoutSpin();
            return false;
        }
        Outcome = testOutcome == TestOutcome.CharacterLottery
            ? presentation.CurrentOutcome
            : (ChronicleVAimPresentation.VAimOutcome)((int)testOutcome - 1);
        willUpgradeRed = UnityEngine.Random.Range(0, 100) < UpgradePercent();
        didUpgradeRed = false;
        if (backgroundRawImage != null && blueBackground != null)
            backgroundRawImage.texture = blueBackground;
        if (cutInImage != null && blueCutInSprite != null)
            cutInImage.sprite = blueCutInSprite;
        if (cutInImage != null)
            cutInImage.gameObject.SetActive(true);

        stopCount = 0;
        pressedOrder = "";
        waitingForReel = false;
        isFinishing = false;
        IsPlaying = true;
        leftReel.StartSpin();
        centerReel.StartSpin();
        rightReel.StartSpin();
        if (!leftReel.isSpinning || !centerReel.isSpinning || !rightReel.isSpinning)
        {
            Debug.LogError("[V疑似遊技] リールが回りません。遊戯説明パネル等を確認してください。", this);
            // 通常処理へ誤復帰しないよう、再生を止めて調査を促す。
            return false;
        }
        if (logDetails) Debug.Log("[V疑似遊技] 開始 / " + Outcome + " / 右第一停止 Dキー固定");
        return true;
    }

    private int UpgradePercent()
    {
        switch (Outcome)
        {
            case ChronicleVAimPresentation.VAimOutcome.Fake: return fakeRedUpgradePercent;
            case ChronicleVAimPresentation.VAimOutcome.SingleV: return singleRedUpgradePercent;
            case ChronicleVAimPresentation.VAimOutcome.DoubleV: return doubleRedUpgradePercent;
            default: return pinkRedUpgradePercent;
        }
    }

    private void Update()
    {
        if (!IsPlaying || isFinishing) return;
        if (waitingForReel)
        {
            if (pendingReel != null && !pendingReel.isSpinning && !pendingReel.isStopping)
            {
                waitingForReel = false;
                stopCount++;
                if (logDetails) Debug.Log("[V疑似遊技] " + stopCount + "停止完了 / 押し順=" + pressedOrder);
                if (stopCount == 1)
                {
                    if (Outcome == ChronicleVAimPresentation.VAimOutcome.PinkV && premiumOnPinkFirstStop)
                        SetBackground(premiumBackground);
                    else if (willUpgradeRed) SetRedCutIn();
                }
                if (stopCount == 2 && willUpgradeRed && !didUpgradeRed)
                    SetRedCutIn();
                if (stopCount == 3) StartCoroutine(FinishAfterHold());
            }
            return;
        }
        if (Keyboard.current == null || PlayGuidePanelManager.IsGuideOpen) return;
        if (stopCount == 0)
        {
            if (Keyboard.current.dKey.wasPressedThisFrame)
                RequestStop(rightReel, ReelController.ReelType.Right, "R");
            // D以外の第一停止は許可しない。
            return;
        }
        if (stopCount == 1)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
                RequestStop(leftReel, ReelController.ReelType.Left, "L");
            else if (Keyboard.current.sKey.wasPressedThisFrame)
                RequestStop(centerReel, ReelController.ReelType.Center, "C");
            return;
        }
        if (stopCount == 2)
        {
            if (pressedOrder == "RL" && Keyboard.current.sKey.wasPressedThisFrame)
                RequestStop(centerReel, ReelController.ReelType.Center, "C");
            else if (pressedOrder == "RC" && Keyboard.current.aKey.wasPressedThisFrame)
                RequestStop(leftReel, ReelController.ReelType.Left, "L");
        }
    }

    private void RequestStop(ReelController reel, ReelController.ReelType reelType, string key)
    {
        if (waitingForReel || !IsPlaying) return;
        int extraRevolution = Outcome == ChronicleVAimPresentation.VAimOutcome.PinkV &&
                              reelType == ReelController.ReelType.Right ? 1 : 0;
        if (!reel.StopVAimAtCenterIndex(TargetCenterIndex(reelType), extraRevolution))
        {
            Debug.LogError("[V疑似遊技] 停止要求に失敗: " + reelType, this);
            return;
        }
        pressedOrder += key;
        pendingReel = reel;
        waitingForReel = true;
    }

    private void SetBackground(Texture tex)
    {
        if (backgroundRawImage != null && tex != null)
            backgroundRawImage.texture = tex;
    }

    private void SetRedCutIn()
    {
        didUpgradeRed = true;
        if (cutInImage != null && redCutInSprite != null)
            cutInImage.sprite = redCutInSprite;
        SetBackground(redBackground);
        if (logDetails) Debug.Log("[V疑似遊技] 青→赤カットイン昇格");
    }

    private IEnumerator FinishAfterHold()
    {
        isFinishing = true;
        bool isReverseScissors = pressedOrder == "RLC";
        if (Outcome == ChronicleVAimPresentation.VAimOutcome.DoubleV &&
            rainbowOnDoubleReverseScissors && isReverseScissors)
            SetBackground(rainbowBackground);
        if (Outcome == ChronicleVAimPresentation.VAimOutcome.PinkV)
            SetBackground(premiumBackground);

        // 実リールの表示結果が想定に一致するか検証。
        ValidateStoppedSymbols();
        VAimReward reward = Outcome == ChronicleVAimPresentation.VAimOutcome.SingleV ? VAimReward.Single
            : Outcome == ChronicleVAimPresentation.VAimOutcome.DoubleV ? VAimReward.Double
            : Outcome == ChronicleVAimPresentation.VAimOutcome.PinkV ? VAimReward.Premium : VAimReward.None;
        if (logDetails) Debug.Log("[V疑似遊技] 成功結果=" + Outcome + " / 報酬=" + reward
            + " / 押し順=" + pressedOrder + " ※報酬付与はAT側で連携");
        yield return new WaitForSeconds(Mathf.Max(0f, resultHoldSeconds));
        RestoreWithoutSpin();
        VAimFinished?.Invoke(Outcome, reward);
    }

    private void ValidateStoppedSymbols()
    {
        bool success;
        if (Outcome == ChronicleVAimPresentation.VAimOutcome.DoubleV)
            success = leftReel.GetCenterSymbol() == "BlueV"
                && leftReel.GetLowerSymbol() == "BlueV"
                && centerReel.GetCenterSymbol() == "BlueV"
                && rightReel.GetUpperSymbol() == "BlueV"
                && rightReel.GetCenterSymbol() == "BlueV";
        else if (Outcome == ChronicleVAimPresentation.VAimOutcome.SingleV)
            // 左下・中中・右上の右上がり斜め1ライン。
            // 横一列の青Vを禁止し、ダブルVと区別する。
            success = leftReel.GetLowerSymbol() == "BlueV"
                && centerReel.GetCenterSymbol() == "BlueV"
                && rightReel.GetUpperSymbol() == "BlueV"
                && !(leftReel.GetCenterSymbol() == "BlueV"
                     && rightReel.GetCenterSymbol() == "BlueV");
        else if (Outcome == ChronicleVAimPresentation.VAimOutcome.PinkV)
            success = leftReel.GetCenterSymbol() == "PinkV"
                && centerReel.GetCenterSymbol() == "PinkV"
                && rightReel.GetCenterSymbol() == "PinkV";
        else success = centerReel.GetCenterSymbol() != "BlueV";
        if (!success)
            Debug.LogError("[V疑似遊技] 停止形が想定と違います。ReelMaster.csv配列／UpdateReelのインデックス方向を確認", this);
    }

    private void RestoreWithoutSpin()
    {
        // 3リール完全停止後にManager参照と通常4コマ制御を復元。
        leftReel.EndVAimReelControl();
        centerReel.EndVAimReelControl();
        rightReel.EndVAimReelControl();
        if (presentation != null) presentation.EndVAim();
        if (backgroundRawImage != null) backgroundRawImage.texture = originalTexture;
        if (cutInImage != null)
        {
            cutInImage.sprite = originalCutInSprite;
            cutInImage.gameObject.SetActive(false);
        }
        if (slotGameManager != null)
            slotGameManager.enabled = oldSlotManagerEnabled;
        IsPlaying = false;
        waitingForReel = false;
        isFinishing = false;
    }
}
