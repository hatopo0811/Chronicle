using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI-only AT Bell navigation. Does not change lottery, reel stops or payout.
/// Uses the three parent Image components, swapping their sprite and enabled state.
/// </summary>
public sealed class ChronicleBellNavUIFixed : MonoBehaviour
{
    public enum DisplayMode { Random, ForceTwoChoice, ForceFull }

    [Header("既存 SlotGameManager")]
    [SerializeField] private SlotGameManager slotGameManager;

    [Header("表示先：親のImageを3つだけ登録")]
    [SerializeField] private Image leftImage;
    [SerializeField] private Image centerImage;
    [SerializeField] private Image rightImage;

    [Header("素材（Sprite）")]
    [SerializeField] private Sprite oneSprite;
    [SerializeField] private Sprite twoSprite;
    [SerializeField] private Sprite threeSprite;
    [SerializeField] private Sprite lineSprite;

    [Header("表示モード（表示だけを切り替える）")]
    [SerializeField] private DisplayMode displayMode = DisplayMode.ForceFull;
    [Range(0, 100)][SerializeField] private int fullNavigationPercent = 30;
    [SerializeField] private bool logNavigation = true;

    private bool currentFullNavigation;

    private void Awake()
    {
        HideAll();
    }

    private void OnEnable()
    {
        HideAll();
        if (slotGameManager == null)
        {
            Debug.LogWarning("[ベルナビUI] Slot Game Managerが未設定です。", this);
            return;
        }
        // ATBellNavigationReady is fired AFTER the first-stop-only event.
        // It provides the real correct three-character order.
        slotGameManager.ATBellNavigationReady += OnBellNavigation;
        slotGameManager.ChronicleReelStopPressed += OnReelStopPressed;
        slotGameManager.ChronicleModeChanged += OnModeChanged;
    }

    private void OnDisable()
    {
        if (slotGameManager != null)
        {
            slotGameManager.ATBellNavigationReady -= OnBellNavigation;
            slotGameManager.ChronicleReelStopPressed -= OnReelStopPressed;
            slotGameManager.ChronicleModeChanged -= OnModeChanged;
        }
        HideAll();
    }

    private void OnModeChanged(SlotGameManager.ChronicleMode mode)
    {
        HideAll();
    }

    private void OnBellNavigation(string order)
    {
        HideAll();
        if (string.IsNullOrEmpty(order) || order.Length != 3)
        {
            Debug.LogWarning("[ベルナビUI] 不正な押し順: " + order, this);
            return;
        }

        if (slotGameManager == null || !slotGameManager.IsChronicleAT)
            return;

        bool full = displayMode == DisplayMode.ForceFull ||
            (displayMode == DisplayMode.Random && UnityEngine.Random.Range(0, 100) < fullNavigationPercent);

        currentFullNavigation = full;
        if (full)
        {
            ShowForOrder(order, true);
        }
        else
        {
            ShowForOrder(order, false);
        }
        if (logNavigation)
            Debug.Log("[ベルナビUI] " + (full ? "全ナビ" : "2択ナビ") + " / first=" + order[0] + " / order=" + order);
    }

    private void ShowForOrder(string order, bool full)
    {
        if (!IsValidOrder(order))
        {
            Debug.LogWarning("[ベルナビUI] 対応していない押し順: " + order, this);
            return;
        }
        Image[] images = { leftImage, centerImage, rightImage };
        string labels = "LCR";
        for (int i = 0; i < images.Length; i++)
        {
            int place = order.IndexOf(labels[i]);
            Sprite sprite;
            if (!full && place > 0)
                sprite = lineSprite;
            else
                sprite = place == 0 ? oneSprite : place == 1 ? twoSprite : threeSprite;
            SetSprite(images[i], sprite);
        }
    }

    private static bool IsValidOrder(string order)
    {
        if (order.Length != 3) return false;
        return order.IndexOf('L') >= 0 && order.IndexOf('C') >= 0 && order.IndexOf('R') >= 0;
    }

    private void OnReelStopPressed(int stopNumber, ReelController.ReelType reel)
    {
        if (!currentFullNavigation)
        {
            if (stopNumber == 1) HideAll();
            return;
        }

        // 全ナビは停止したリールの数字だけ消し、残りの数字は表示を続ける。
        Image target = null;
        switch (reel)
        {
            case ReelController.ReelType.Left: target = leftImage; break;
            case ReelController.ReelType.Center: target = centerImage; break;
            case ReelController.ReelType.Right: target = rightImage; break;
        }
        if (target != null) target.enabled = false;
        if (stopNumber >= 3) HideAll();
    }

    private static void SetSprite(Image image, Sprite sprite)
    {
        if (image == null) return;
        image.sprite = sprite;
        image.enabled = sprite != null;
    }

    private void HideAll()
    {
        currentFullNavigation = false;
        if (leftImage != null) leftImage.enabled = false;
        if (centerImage != null) centerImage.enabled = false;
        if (rightImage != null) rightImage.enabled = false;
    }
}
