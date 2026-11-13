using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class SlotGameManager : MonoBehaviour
{
    // Chronicle: 通常6役のみ。旧BB/RB・7揃い・持越しボーナスは廃止。
    public enum RoleType
    {
        Lose,
        Bell,
        Replay,
        Suika,
        Cherry,
        StrongCherry,
        ATBell
    }
    public enum HitRoleType { None, Replay, Bell, Cherry, Suika, StrongCherry }
    public enum PayLine { None, Upper, Center, Lower, DiagonalDown, DiagonalUp }
    public enum ChronicleTestRole
    {
        Lose,
        Bell,
        Replay,
        Suika,
        Cherry,
        StrongCherry,
        ATBell
    }

    [Header("リール")]
    [SerializeField] private ReelController leftReel;
    [SerializeField] private ReelController centerReel;
    [SerializeField] private ReelController rightReel;

    [Header("表示")]
    [SerializeField] private TMP_Text creditText;
    [SerializeField] private TMP_Text payText;
    [SerializeField] private TMP_Text DebugText;

    [Header("サウンド")]
    [SerializeField] private SlotAudioManager slotAudio;

    [Header("クロニクル 通常6役抽選の重み（0以上）")]
    [SerializeField] private float chronicleLoseWeight = 54f;
    [SerializeField] private float chronicleBellWeight = 20f;
    [SerializeField] private float chronicleReplayWeight = 20f;
    [SerializeField] private float chronicleSuikaWeight = 3f;
    [SerializeField] private float chronicleCherryWeight = 2f;
    [SerializeField] private float chronicleStrongCherryWeight = 1f;

    // =========================================
    // ATベル抽選重み
    // =========================================

    [Header("ATベル抽選設定")]
    [SerializeField]
    private float chronicleATBellWeight = 60f;

    [Header("クロニクル 払い出し（試験値）")]
    [SerializeField] private int chronicleBellPayout = 8;
    [SerializeField] private int chronicleSuikaPayout = 3;
    [SerializeField] private int chronicleCherryPayout = 1;
    [SerializeField] private int chronicleStrongCherryPayout = 1;
    // =========================================
    // AT中のベル払い出し枚数
    // =========================================
    [Header("クロニクル ATベル設定")]
    [SerializeField]
    private int chronicleATBellPayout = 15;



    [Header("クロニクル デバッグ")]
    [SerializeField] private bool chronicleUseTestRole = false;
    [SerializeField] private ChronicleTestRole chronicleTestRole = ChronicleTestRole.Lose;

    [Header("クレジット設定")]
    [SerializeField] private int startCredit = 50;
    [SerializeField] private int betCount = 3;
    [SerializeField] private int maxCredit = 19000;

    [Header("メダル追加")]
    [SerializeField] private int PlusMedal = 50;
    [Header("リール停止受付")]
    [SerializeField] private float stopEnableDelay = 0.5f;
    [Header("メダル追加テキスト表示パネル")]
    public GameObject MedalAdditionPanel;
    [Header("遊戯説明")]
    [SerializeField] private GameObject playGuidePanel;

    public RoleType CurrentRole { get; private set; }
    public HitRoleType CurrentHitRole { get; private set; }
    public PayLine CurrentHitLine { get; private set; } = PayLine.None;
    public int Credit { get; private set; }
    public int LastPayout { get; private set; }

    public bool GameStarted { get; private set; } = false;
    
    private bool replayNextGame = false;
    private bool leftStopped = false;
    private bool centerStopped = false;
    private bool rightStopped = false;
    private bool canStopReels = false;
    private float spinStartTime = 0f;

    public bool IsPlayGuideOpen()
    {
        return playGuidePanel != null && playGuidePanel.activeInHierarchy;
    }

    public void PanelOFF()
    {
        if (MedalAdditionPanel != null) MedalAdditionPanel.SetActive(false);
    }
    public void PanelON()
    {
        if (MedalAdditionPanel != null) MedalAdditionPanel.SetActive(true);
    }

    private void Start()
    {
        Credit = startCredit;
        LastPayout = 0;
        PanelOFF();
        UpdateCreditDisplay();
        UpdatePayDisplay();
    }

    private void Update()
    {
        // =========================================
        // クロニクル ATモード切り替え（デバッグ用）
        // =========================================

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            SetChronicleATMode(!IsChronicleAT);
            return;
        }


        // =====================================
        // 遊戯説明Panel表示中
        // スロットの入力を全部無効化
        // =========================================

        if (PlayGuidePanelManager.IsGuideOpen)
        {
            return;
        }


        // =====================================
        // Space
        // =========================================

        if (
            Keyboard.current
                .spaceKey
                .wasPressedThisFrame
        )
        {
            // =================================
            // メダル追加Panel表示中
            // Spaceでは閉じるだけ
            // =================================

            if (
                MedalAdditionPanel != null &&
                MedalAdditionPanel.activeInHierarchy
            )
            {
                PanelOFF();

                return;
            }


            // =================================
            // ゲーム開始
            // =========================================

            StartGame();
        }


        // =====================================
        // ゲーム中でなければ終了
        // =========================================

        if (!GameStarted)
        {
            return;
        }


        // =====================================
        // 停止受付ウェイト
        // =========================================

        if (!canStopReels)
        {
            float elapsed =
                Time.time -
                spinStartTime;


            if (
                elapsed >=
                stopEnableDelay
            )
            {
                canStopReels =
                    true;
            }
        }


        if (!canStopReels)
        {
            return;
        }


        // =====================================
        // A 左リール停止
        //
        // ★停止処理を行ったらreturn
        // 同じフレームで他のリールを止めない
        // =========================================

        if (
            Keyboard.current
                .aKey
                .wasPressedThisFrame
        )
        {
            leftReel.StopSpin();

            return;
        }


        // =====================================
        // S 中リール停止
        //
        // ★停止処理を行ったらreturn
        // =========================================

        if (
            Keyboard.current
                .sKey
                .wasPressedThisFrame
        )
        {
            centerReel.StopSpin();

            return;
        }


        // =====================================
        // D 右リール停止
        //
        // ★停止処理を行ったらreturn
        // =========================================

        if (
            Keyboard.current
                .dKey
                .wasPressedThisFrame
        )
        {
            rightReel.StopSpin();

            return;
        }
    }
}