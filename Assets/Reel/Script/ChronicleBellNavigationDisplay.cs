using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 閃撃クロニクル：2択ナビ / 11枚ベル用全ナビを表示する独立UI。
/// SlotGameManager / ReelController / チームメンバーのコードは変更しない。
///
/// ATBell 成立時の内部正解順を既存APIから取得する。
/// ・2択: 最初の1リールだけ「1」、残り2リールは線。
/// ・全ナビ: 正解の停止順に「1」「2」「3」を表示する。
/// 正解順どおりに押した場合、既存のATベル判定が11枚を払い出す。
/// ※押し順を無視した場合は既存制御により1枚等になる。
/// </summary>
[DisallowMultipleComponent]
public sealed class ChronicleBellNavigationDisplay : MonoBehaviour
{
    public enum NavigationTestMode
    {
        Random,
        ForceTwoChoice,
        ForceFull
    }

    private enum DisplayMode
    {
        None,
        TwoChoice,
        Full
    }

    [Header("既存スロット管理（必須）")]
    [SerializeField] private SlotGameManager slotGameManager;

    [Header("ナビ抽選：ATBell成立ゲームのうち全ナビにする割合")]
    [Range(0, 100)]
    [SerializeField] private int fullNavigationPercent = 30;
    [SerializeField] private NavigationTestMode testMode = NavigationTestMode.Random;

    [Header("2択：今ある第一停止ナビ画像")]
    [SerializeField] private Image leftFirstImage;
    [SerializeField] private Image centerFirstImage;
    [SerializeField] private Image rightFirstImage;

    [Header("2択：第一停止以外の2リールに表示する線画像（任意）")]
    [SerializeField] private Image leftLineImage;
    [SerializeField] private Image centerLineImage;
    [SerializeField] private Image rightLineImage;

    [Header("全ナビ：左・中・右の表示位置（UI Image）")]
    [SerializeField] private Image leftFullImage;
    [SerializeField] private Image centerFullImage;
    [SerializeField] private Image rightFullImage;

    [Header("全ナビ：共通の数字素材（Sprite）")]
    [SerializeField] private Sprite numberOneSprite;
    [SerializeField] private Sprite numberTwoSprite;
    [SerializeField] private Sprite numberThreeSprite;

    [Header("全ナビ：数字画像が未完成の場合の文字表示（任意・TextMeshProUGUI）")]
    [SerializeField] private TMP_Text leftFullNumberText;
    [SerializeField] private TMP_Text centerFullNumberText;
    [SerializeField] private TMP_Text rightFullNumberText;

    [Header("デバッグ")]
    [SerializeField] private bool logNavigation = true;

    private DisplayMode currentDisplay = DisplayMode.None;
    private string currentOrder = "";
    private bool decidedForCurrentGame;
    private bool subscribed;

    private void Awake()
    {
        HideAll();
    }

    private void OnEnable()
    {
        Subscribe();
        SyncFromSlot();
    }

    private void Start()
    {
        // スロット側と起動順が前後しても状態を合わせる。
        SyncFromSlot();
    }

    private void Update()
    {
        if (!subscribed) Subscribe();
        SyncFromSlot();
    }

    private void OnDisable()
    {
        Unsubscribe();
        HideAll();
        ResetSelection();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (subscribed || slotGameManager == null) return;

        slotGameManager.ATBellFirstStopNavigationReady += OnFirstStopNavigationReady;
        slotGameManager.ATBellNavigationReady += OnFullOrderAvailable;
        slotGameManager.ChronicleReelStopPressed += OnReelStopPressed;
        slotGameManager.ATBellResolved += OnBellResolved;
        slotGameManager.ChronicleModeChanged += OnModeChanged;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed) return;

        if (slotGameManager != null)
        {
            slotGameManager.ATBellFirstStopNavigationReady -= OnFirstStopNavigationReady;
            slotGameManager.ATBellNavigationReady -= OnFullOrderAvailable;
            slotGameManager.ChronicleReelStopPressed -= OnReelStopPressed;
            slotGameManager.ATBellResolved -= OnBellResolved;
            slotGameManager.ChronicleModeChanged -= OnModeChanged;
        }

        subscribed = false;
    }

    private void OnFirstStopNavigationReady(string firstStop)
    {
        // 現行の AT.cs はこのイベントとフル順イベントを連続発行する。
        // 先にゲーム状態から正解順を取得し、抽選は1ゲームにつき1回だけ行う。
        SyncFromSlot();
    }

    private void OnFullOrderAvailable(string order)
    {
        SyncFromSlot();
    }

    private void OnReelStopPressed(int stopNumber, ReelController.ReelType reel)
    {
        // AT.cs 側は停止入力を受けた瞬間に停止履歴を記録する。
        // 表示は履歴を参照するため、すでに停止したリールの数字を消せる。
        if (decidedForCurrentGame) Render();
    }

    private void OnBellResolved(bool correct, int payout)
    {
        if (decidedForCurrentGame && currentDisplay == DisplayMode.Full && logNavigation)
        {
            if (correct && payout == 11)
            {
                Debug.Log("[クロニクル][全ナビ] 指示順どおりに停止 / 11枚獲得");
            }
            else
            {
                Debug.LogWarning("[クロニクル][全ナビ] 11枚未獲得: 正解判定="
                    + correct + " / PAY=" + payout
                    + " / 指示=" + currentOrder
                    + " / 入力=" + slotGameManager.PressedATBellOrder);
            }
        }

        HideAll();
        // 次ゲーム用の抽選は終了後 Update のゲーム非稼働判定で初期化する。
    }

    private void OnModeChanged(SlotGameManager.ChronicleMode mode)
    {
        if (mode != SlotGameManager.ChronicleMode.AT)
        {
            HideAll();
            ResetSelection();
        }
    }

    private bool HasActiveATBell()
    {
        return slotGameManager != null
            && slotGameManager.IsChronicleAT
            && slotGameManager.IsATBellGame
            && IsValidOrder(slotGameManager.CurrentATBellNavigation);
    }

    private void SyncFromSlot()
    {
        if (!HasActiveATBell())
        {
            if (decidedForCurrentGame)
            {
                HideAll();
                ResetSelection();
            }
            return;
        }

        string order = slotGameManager.CurrentATBellNavigation;

        if (!decidedForCurrentGame || currentOrder != order)
        {
            currentOrder = order;
            decidedForCurrentGame = true;

            bool useFull = testMode == NavigationTestMode.ForceFull
                || (testMode == NavigationTestMode.Random
                    && Random.Range(0, 100) < fullNavigationPercent);

            currentDisplay = useFull ? DisplayMode.Full : DisplayMode.TwoChoice;
            if (logNavigation)
            {
                if (useFull)
                    Debug.Log("[クロニクル][ナビUI] 全ナビ(11枚狙い) / 正解押し順=" + order);
                else
                    Debug.Log("[クロニクル][ナビUI] 2択ナビ / 第1停止=" + order[0]);
            }
        }

        Render();
    }

    private static bool IsValidOrder(string order)
    {
        if (string.IsNullOrEmpty(order) || order.Length != 3) return false;
        return order.IndexOf('L') >= 0
            && order.IndexOf('C') >= 0
            && order.IndexOf('R') >= 0;
    }

    private void Render()
    {
        HideAll();
        if (!decidedForCurrentGame || !HasActiveATBell()) return;

        string stopped = slotGameManager.ChronicleGeneralStopOrder;
        if (stopped == null) stopped = "";

        if (currentDisplay == DisplayMode.TwoChoice)
        {
            // 2択は第1停止を受けた時点で全て消す。
            if (stopped.Length > 0) return;

            char first = currentOrder[0];
            SetVisible(leftFirstImage, first == 'L');
            SetVisible(centerFirstImage, first == 'C');
            SetVisible(rightFirstImage, first == 'R');
            SetVisible(leftLineImage, first != 'L');
            SetVisible(centerLineImage, first != 'C');
            SetVisible(rightLineImage, first != 'R');
            return;
        }

        if (currentDisplay == DisplayMode.Full)
        {
            // 全ナビは押したリールだけ消し、次の停止番号を残す。
            ShowFullNumber('L', leftFullImage, leftFullNumberText, stopped);
            ShowFullNumber('C', centerFullImage, centerFullNumberText, stopped);
            ShowFullNumber('R', rightFullImage, rightFullNumberText, stopped);
        }
    }

    private void ShowFullNumber(char reelKey, Image targetImage, TMP_Text fallbackText, string stopped)
    {
        int index = currentOrder.IndexOf(reelKey);
        if (index < 0 || stopped.IndexOf(reelKey) >= 0) return;

        Sprite sprite = index == 0 ? numberOneSprite
            : index == 1 ? numberTwoSprite : numberThreeSprite;

        if (targetImage != null && sprite != null)
        {
            targetImage.sprite = sprite;
            targetImage.enabled = true;
        }
        else if (fallbackText != null)
        {
            fallbackText.text = (index + 1).ToString();
            fallbackText.enabled = true;
        }
    }

    private static void SetVisible(Image image, bool visible)
    {
        if (image != null) image.enabled = visible;
    }

    private void HideAll()
    {
        SetVisible(leftFirstImage, false);
        SetVisible(centerFirstImage, false);
        SetVisible(rightFirstImage, false);
        SetVisible(leftLineImage, false);
        SetVisible(centerLineImage, false);
        SetVisible(rightLineImage, false);
        SetVisible(leftFullImage, false);
        SetVisible(centerFullImage, false);
        SetVisible(rightFullImage, false);

        if (leftFullNumberText != null) leftFullNumberText.enabled = false;
        if (centerFullNumberText != null) centerFullNumberText.enabled = false;
        if (rightFullNumberText != null) rightFullNumberText.enabled = false;
    }

    private void ResetSelection()
    {
        decidedForCurrentGame = false;
        currentDisplay = DisplayMode.None;
        currentOrder = "";
    }
}
