using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class SlotGameManager : MonoBehaviour
{
    public enum RoleType
    {
        Lose,
        Replay,
        Grape,
        Bell,
        Cherry,
        Hibiscus,
        BigBonus,
        RegBonus
    }


    public enum HitRoleType
    {
        None,
        Replay,
        Grape,
        Bell,
        Cherry,
        Hibiscus,
        BigBonus,
        RegBonus
    }


    public enum BonusType
    {
        None,
        Big,
        Reg
    }


    public enum PayLine
    {
        None,
        Upper,
        Center,
        Lower,
        DiagonalDown,
        DiagonalUp
    }


    // =========================================
    // リール
    // =========================================

    [Header("リール")]
    [SerializeField]
    private ReelController leftReel;

    [SerializeField]
    private ReelController centerReel;

    [SerializeField]
    private ReelController rightReel;


    // =========================================
    // 表示
    // =========================================

    [Header("表示")]
    [SerializeField]
    private TMP_Text creditText;

    [SerializeField]
    private TMP_Text payText;

    [SerializeField]
    private TMP_Text DebugText;

    [SerializeField]
    private TMP_Text BonusPlusCount;


    // =========================================
    // サウンド
    // =========================================

    [Header("サウンド")]
    [SerializeField]
    private SlotAudioManager slotAudio;


    // =========================================
    // ボーナス告知
    // =========================================

    [Header("ボーナス告知")]
    [SerializeField]
    private BonusLampManager bonusLamp;


    // =========================================
    // 通常時抽選
    // =========================================

    [Header("通常時 抽選確率（%）")]
    [SerializeField]
    private float loseProbability = 30f;

    [SerializeField]
    private float replayProbability = 15f;

    [SerializeField]
    private float grapeProbability = 15f;

    [SerializeField]
    private float bellProbability = 10f;

    [SerializeField]
    private float cherryProbability = 10f;

    [SerializeField]
    private float hibiscusProbability = 5f;

    [SerializeField]
    private float bigProbability = 10f;

    [SerializeField]
    private float regProbability = 10f;


    // =========================================
    // CREDIT
    // =========================================

    [Header("クレジット設定")]
    [SerializeField]
    private int startCredit = 50;

    [SerializeField]
    private int betCount = 3;

    [SerializeField]
    private int bonusBetCount = 2;

    [SerializeField]
    private int maxCredit = 19000;


    // =========================================
    // 払い出し
    // =========================================

    [Header("払い出し枚数")]
    [SerializeField]
    private int grapePayout = 8;

    [SerializeField]
    private int bellPayout = 14;

    [SerializeField]
    private int cherryPayout = 2;

    [SerializeField]
    private int Bonusgrape = 14;

    [SerializeField]
    private int hibiscusPayout = 10;


    // =========================================
    // BONUS
    // =========================================

    [Header("ボーナス設定")]
    [SerializeField]
    private int BIG_BGrapeCount = 20;

    [SerializeField]
    private int REG_BGrapeCount = 8;


    // =========================================
    // メダル追加
    // =========================================

    [Header("メダル追加")]
    [SerializeField]
    private int PlusMedal = 50;


    // =========================================
    // 停止受付
    // =========================================

    [Header("リール停止受付")]
    [SerializeField]
    private float stopEnableDelay = 0.5f;


    // =========================================
    // メダル追加Panel
    // =========================================

    [Header("メダル追加テキスト表示パネル")]
    [SerializeField]
    public GameObject MedalAdditionPanel;


    // =========================================
    // 遊戯説明Panel
    // =========================================

    [Header("遊戯説明")]
    [SerializeField]
    private GameObject playGuidePanel;


    // =========================================
    // 遊戯説明Panelが開いているか
    // =========================================

    public bool IsPlayGuideOpen()
    {
        return
            playGuidePanel != null &&
            playGuidePanel.activeInHierarchy;
    }


    // =========================================
    // BONUS累計払い出し
    // =========================================

    public int BonusTotalPayout
    {
        get;
        private set;
    }


    public RoleType CurrentRole
    {
        get;
        private set;
    }


    public HitRoleType CurrentHitRole
    {
        get;
        private set;
    }


    public PayLine CurrentHitLine
    {
        get;
        private set;
    }
        = PayLine.None;


    public BonusType PendingBonus
    {
        get;
        private set;
    }
        = BonusType.None;


    public BonusType ActiveBonus
    {
        get;
        private set;
    }
        = BonusType.None;


    public int Credit
    {
        get;
        private set;
    }


    public int LastPayout
    {
        get;
        private set;
    }


    public int RemainingBonusGrapes
    {
        get;
        private set;
    }


    // =========================================
    // マスター
    // =========================================

    private HitPatternMasterLoader.HitPatternData
        currentHitPattern = null;

    private HitPatternMasterLoader.HitPatternData
        currentReachPattern = null;

    private HitPatternMasterLoader.HitPatternData
        currentLosePattern = null;


    private HitPatternMasterLoader.HitPatternData
        selectedReachPattern = null;

    private HitPatternMasterLoader.HitPatternData
        selectedLosePattern = null;


    // =========================================
    // 告知状態
    // =========================================

    private bool bonusLampNotified =
        false;

    private bool reachShownForCurrentBonus =
        false;


    private bool replayNextGame =
        false;


    private PayLine targetPayLine =
        PayLine.Center;


    // =========================================
    // ゲーム状態
    // =========================================

    private bool gameStarted =
        false;

    private bool leftStopped =
        false;

    private bool centerStopped =
        false;

    private bool rightStopped =
        false;


    private bool canStopReels =
        false;

    private float spinStartTime =
        0f;


    // =========================================
    // メダル追加Panel OFF
    // =========================================

    public void PanelOFF()
    {
        if (MedalAdditionPanel == null)
        {
            return;
        }


        MedalAdditionPanel.SetActive(
            false
        );
    }


    // =========================================
    // メダル追加Panel ON
    // =========================================

    public void PanelON()
    {
        if (MedalAdditionPanel == null)
        {
            return;
        }


        MedalAdditionPanel.SetActive(
            true
        );
    }


    // =========================================
    // COUNT表示
    // =========================================

    private void UpdateCountDisplay()
    {
        if (BonusPlusCount == null)
        {
            return;
        }


        BonusPlusCount.text =
            BonusTotalPayout.ToString(
                "000"
            );
    }


    // =========================================
    // Start
    // =========================================

    private void Start()
    {
        Credit =
            startCredit;


        LastPayout =
            0;


        BonusTotalPayout =
            0;


        RemainingBonusGrapes =
            0;


        bonusLampNotified =
            false;


        // =====================================
        // 最初はメダル追加Panel非表示
        // =========================================

        PanelOFF();


        UpdateCreditDisplay();

        UpdatePayDisplay();

        UpdateCountDisplay();


        // =====================================
        // COUNTは通常時非表示
        // =========================================

        if (BonusPlusCount != null)
        {
            BonusPlusCount
                .gameObject
                .SetActive(
                    false
                );
        }


        // =====================================
        // BONUSランプ消灯
        // =========================================

        if (bonusLamp != null)
        {
            bonusLamp.TurnOff();
        }
    }


    // =========================================
    // Update
    // =========================================

    private void Update()
    {
        if (Keyboard.current == null)
        {
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

        if (!gameStarted)
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