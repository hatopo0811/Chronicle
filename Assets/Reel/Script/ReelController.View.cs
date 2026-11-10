using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 忍者スロット用の図柄表示。ReelController.cs / .Stop.cs / .Pattern.cs と併用。
/// </summary>
public partial class ReelController
{
    // =========================================
    // 図柄画像（8種類）
    // =========================================
    [Header("忍者スロット 図柄画像")]
    [SerializeField] private Sprite bellSprite;
    [SerializeField] private Sprite replaySprite;
    [SerializeField] private Sprite blueVSprite;
    [SerializeField] private Sprite barSprite;
    [SerializeField] private Sprite suikaSprite;
    [SerializeField] private Sprite cherrySprite;
    [SerializeField] private Sprite loseSprite;
    [SerializeField] private Sprite pinkVSprite;

    // =========================================
    // リール表示設定
    // =========================================
    [Header("リール表示設定")]
    [SerializeField] private RectTransform symbolsParent;
    [SerializeField] private float symbolHeight = 100f;
    [SerializeField] private float symbolWidth = 200f;

    // 他の partial ファイルから使うため名称は維持する。
    private List<string> reelData;
    private List<RectTransform> reelSymbols = new List<RectTransform>();

    // =========================================
    // CSV配列をリールに設定
    // =========================================
    private void SetReelData()
    {
        ReelMasterLoader.EnsureLoaded();

        switch (reelType)
        {
            case ReelType.Left:
                reelData = ReelMasterLoader.LeftReel;
                break;
            case ReelType.Center:
                reelData = ReelMasterLoader.CenterReel;
                break;
            case ReelType.Right:
                reelData = ReelMasterLoader.RightReel;
                break;
        }

        if (reelData == null || reelData.Count == 0)
            Debug.LogError(GetReelName() + " のリール配列が読み込めていません。");
    }

    // =========================================
    // 図柄オブジェクト生成
    // =========================================
    private void CreateReel()
    {
        if (symbolsParent == null)
        {
            Debug.LogError(GetReelName() + ": Symbols Parent を設定してください。");
            return;
        }
        if (reelData == null || reelData.Count == 0)
            return;

        // 以前のコードと同様、既存の子は非表示にする。
        foreach (Transform child in symbolsParent)
            child.gameObject.SetActive(false);

        reelSymbols.Clear();

        for (int i = 0; i < reelData.Count; i++)
        {
            string symbolName = reelData[i];
            GameObject symbolObject = new GameObject(
                "Reel_" + i + "_" + symbolName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));

            symbolObject.transform.SetParent(symbolsParent, false);

            RectTransform rect = symbolObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(symbolWidth, symbolHeight);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            Image image = symbolObject.GetComponent<Image>();
            image.sprite = GetSprite(symbolName);
            image.preserveAspect = true;
            image.raycastTarget = false;

            reelSymbols.Add(rect);
        }
    }

    // =========================================
    // 画面上の図柄位置を更新（既存方式を維持）
    // =========================================
    private void UpdateSymbolPositions()
    {
        if (reelLength <= 0f) return;

        for (int i = 0; i < reelSymbols.Count; i++)
        {
            float symbolPosition = i * symbolHeight;
            float wrappedPosition = Mathf.Repeat(symbolPosition + reelOffset, reelLength);
            float y = symbolHeight - wrappedPosition;
            reelSymbols[i].anchoredPosition = new Vector2(0f, y);
        }
    }

    // =========================================
    // 停止図柄取得（他の partial ファイルと互換）
    // =========================================
    public string GetUpperSymbol()
    {
        if (reelData == null || reelData.Count == 0) return "";
        return reelData[WrapIndex(GetCenterSymbolIndex() - 1)];
    }

    public string GetCenterSymbol()
    {
        if (reelData == null || reelData.Count == 0) return "";
        return reelData[GetCenterSymbolIndex()];
    }

    public string GetLowerSymbol()
    {
        if (reelData == null || reelData.Count == 0) return "";
        return reelData[WrapIndex(GetCenterSymbolIndex() + 1)];
    }

    private int GetCenterSymbolIndex()
    {
        if (reelSymbols.Count == 0) return 0;

        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < reelSymbols.Count; i++)
        {
            float distance = Mathf.Abs(reelSymbols[i].anchoredPosition.y);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    private int GetCenterIndexFromOffset(float offset)
    {
        int offsetCount = Mathf.RoundToInt(offset / symbolHeight);
        return WrapIndex(1 - offsetCount);
    }

    private int WrapIndex(int index)
    {
        int length = reelData.Count;
        return ((index % length) + length) % length;
    }

    // =========================================
    // CSVの図柄名 → Sprite
    // =========================================
    private Sprite GetSprite(string symbolName)
    {
        Sprite sprite;
        switch (symbolName)
        {
            case "Bell": sprite = bellSprite; break;
            case "Replay": sprite = replaySprite; break;
            case "BlueV": sprite = blueVSprite; break;
            case "BAR": sprite = barSprite; break;
            case "Suika": sprite = suikaSprite; break;
            case "Cherry": sprite = cherrySprite; break;
            case "Lose": sprite = loseSprite; break;
            case "PinkV": sprite = pinkVSprite; break;
            default:
                Debug.LogError("CSVに未対応の図柄名: " + symbolName);
                return null;
        }

        if (sprite == null)
            Debug.LogWarning(symbolName + " のSpriteがInspectorで未設定です。");
        return sprite;
    }
}
