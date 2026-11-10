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

    // =========================================
    // 図柄の見た目調整
    // =========================================
    [Header("共通表示倍率")]
    [SerializeField] private float commonScale = 0.90f;

    [Header("図柄ごとの表示倍率")]
    [SerializeField] private float bellScale = 0.90f;
    [SerializeField] private float replayScale = 0.90f;
    [SerializeField] private float blueVScale = 1.00f;
    [SerializeField] private float barScale = 1.00f;
    [SerializeField] private float suikaScale = 0.95f;
    [SerializeField] private float cherryScale = 0.92f;
    [SerializeField] private float loseScale = 0.92f;
    [SerializeField] private float pinkVScale = 1.00f;

    [Header("図柄ごとのY位置補正")]
    [SerializeField] private float bellYOffset = 0f;
    [SerializeField] private float replayYOffset = 0f;
    [SerializeField] private float blueVYOffset = 0f;
    [SerializeField] private float barYOffset = 0f;
    [SerializeField] private float suikaYOffset = 0f;
    [SerializeField] private float cherryYOffset = 0f;
    [SerializeField] private float loseYOffset = 0f;
    [SerializeField] private float pinkVYOffset = 0f;

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
            Debug.LogError(GetReelName() + ": Symbols Parent を設定してください。", gameObject);
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

            // -----------------------------
            // ルート（位置管理用）
            // -----------------------------
            GameObject rootObject = new GameObject(
                "Reel_" + i + "_" + symbolName,
                typeof(RectTransform));

            rootObject.transform.SetParent(symbolsParent, false);

            RectTransform rootRect = rootObject.GetComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(symbolWidth, symbolHeight);
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);

            // -----------------------------
            // 画像本体（見た目調整用）
            // -----------------------------
            GameObject imageObject = new GameObject(
                "Image",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));

            imageObject.transform.SetParent(rootObject.transform, false);

            RectTransform imageRect = imageObject.GetComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0.5f, 0.5f);
            imageRect.anchorMax = new Vector2(0.5f, 0.5f);
            imageRect.pivot = new Vector2(0.5f, 0.5f);

            float finalScale = commonScale * GetSymbolScale(symbolName);
            float finalYOffset = GetSymbolYOffset(symbolName);

            imageRect.sizeDelta = new Vector2(
                symbolWidth * finalScale,
                symbolHeight * finalScale
            );
            imageRect.anchoredPosition = new Vector2(0f, finalYOffset);

            Image image = imageObject.GetComponent<Image>();
            image.sprite = GetSprite(symbolName);
            image.preserveAspect = true;
            image.raycastTarget = false;

            reelSymbols.Add(rootRect);
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
    // 図柄名 → Sprite
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

    // =========================================
    // 図柄ごとの表示倍率
    // =========================================
    private float GetSymbolScale(string symbolName)
    {
        switch (symbolName)
        {
            case "Bell": return bellScale;
            case "Replay": return replayScale;
            case "BlueV": return blueVScale;
            case "BAR": return barScale;
            case "Suika": return suikaScale;
            case "Cherry": return cherryScale;
            case "Lose": return loseScale;
            case "PinkV": return pinkVScale;
            default: return 1f;
        }
    }

    // =========================================
    // 図柄ごとのY補正
    // =========================================
    private float GetSymbolYOffset(string symbolName)
    {
        switch (symbolName)
        {
            case "Bell": return bellYOffset;
            case "Replay": return replayYOffset;
            case "BlueV": return blueVYOffset;
            case "BAR": return barYOffset;
            case "Suika": return suikaYOffset;
            case "Cherry": return cherryYOffset;
            case "Lose": return loseYOffset;
            case "PinkV": return pinkVYOffset;
            default: return 0f;
        }
    }
}