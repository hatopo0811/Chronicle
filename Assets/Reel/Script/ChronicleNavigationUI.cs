using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 閃撃クロニクル：2択ATベルの「第一停止ナビ」画像だけを表示する。
/// 既存の SlotGameManager / ReelController は変更しない。
///
/// 必要な公開API（提供された SlotGameManager.AT.cs に存在）:
///   ATBellFirstStopNavigationReady (Action<string> : "L" / "C" / "R")
///   ChronicleReelStopPressed (Action<int, ReelController.ReelType>)
///   IsChronicleAT / IsATBellGame / CurrentATBellFirstStopNavigation
///   ATBellStopInputCount
/// </summary>
[DisallowMultipleComponent]
public sealed class ChronicleNavigationUI : MonoBehaviour
{
    [Header("既存の SlotGameManager を指定")]
    [SerializeField] private SlotGameManager slotGameManager;

    [Header("2択第一停止ナビ画像（UI > Image）")]
    [SerializeField] private Image leftFirstImage;
    [SerializeField] private Image centerFirstImage;
    [SerializeField] private Image rightFirstImage;

    [Header("デバッグ")]
    [SerializeField] private bool logNavigationUI = false;

    private string shownFirstStop = "";
    private bool subscribed;

    private void Awake()
    {
        HideAll();
    }

    private void OnEnable()
    {
        Subscribe();
        RefreshFromGameState();
    }

    private void Start()
    {
        // UIの方が先に初期化されても表示状態を同期する。
        RefreshFromGameState();
    }

    private void Update()
    {
        if (!subscribed) Subscribe();

        // ゲーム終了、AT解除、ナビなし、第一停止済みの間は必ず非表示。
        if (!ShouldShowFirstStopNavigation())
        {
            if (!string.IsNullOrEmpty(shownFirstStop)) HideAll();
            return;
        }

        // イベントを取り逃しても現在のゲーム状態から復元できる。
        string nextFirst = slotGameManager.CurrentATBellFirstStopNavigation;
        if (shownFirstStop != nextFirst)
        {
            ShowFirstStop(nextFirst);
        }
    }

    private void OnDisable()
    {
        Unsubscribe();
        HideAll();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (subscribed || slotGameManager == null) return;
        slotGameManager.ATBellFirstStopNavigationReady += OnFirstStopNavigationReady;
        slotGameManager.ChronicleReelStopPressed += OnReelStopPressed;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed) return;
        if (slotGameManager != null)
        {
            slotGameManager.ATBellFirstStopNavigationReady -= OnFirstStopNavigationReady;
            slotGameManager.ChronicleReelStopPressed -= OnReelStopPressed;
        }
        subscribed = false;
    }

    private void OnFirstStopNavigationReady(string firstStop)
    {
        // フル正解順のイベントは使わない（2択の正解を画面に出さない）。
        if (ShouldShowFirstStopNavigation())
            ShowFirstStop(firstStop);
        else
            HideAll();
    }

    private void OnReelStopPressed(int stopNumber, ReelController.ReelType reel)
    {
        if (stopNumber == 1)
        {
            HideAll();
        }
    }

    private bool ShouldShowFirstStopNavigation()
    {
        return slotGameManager != null
            && slotGameManager.IsChronicleAT
            && slotGameManager.IsATBellGame
            && slotGameManager.ATBellStopInputCount == 0
            && !string.IsNullOrEmpty(slotGameManager.CurrentATBellFirstStopNavigation);
    }

    private void RefreshFromGameState()
    {
        if (ShouldShowFirstStopNavigation())
            ShowFirstStop(slotGameManager.CurrentATBellFirstStopNavigation);
        else
            HideAll();
    }

    private void ShowFirstStop(string firstStop)
    {
        if (firstStop != "L" && firstStop != "C" && firstStop != "R")
        {
            HideAll();
            if (logNavigationUI)
                Debug.LogWarning("[ChronicleNavigationUI] 未知の第一停止ナビ: " + firstStop);
            return;
        }

        shownFirstStop = firstStop;
        if (leftFirstImage != null) leftFirstImage.enabled = firstStop == "L";
        if (centerFirstImage != null) centerFirstImage.enabled = firstStop == "C";
        if (rightFirstImage != null) rightFirstImage.enabled = firstStop == "R";

        if (logNavigationUI)
            Debug.Log("[ChronicleNavigationUI] 第一停止ナビ表示: " + firstStop);
    }

    private void HideAll()
    {
        shownFirstStop = "";
        if (leftFirstImage != null) leftFirstImage.enabled = false;
        if (centerFirstImage != null) centerFirstImage.enabled = false;
        if (rightFirstImage != null) rightFirstImage.enabled = false;
    }
}
