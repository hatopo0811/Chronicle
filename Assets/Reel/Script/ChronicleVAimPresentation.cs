using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 既存のInspector参照と公開APIを維持し、PinkVとキャラ別抽選を拡張。
public class ChronicleVAimPresentation : MonoBehaviour
{
    public enum VAimOutcome { Fake, SingleV, DoubleV, PinkV }

    [Serializable]
    public class CharacterSetting
    {
        public string characterId = "Character";
        public Sprite characterSprite;
        [Min(0f)] public float appearanceWeight = 1f;
        [Range(0f, 100f)] public float successChancePercent = 30f;
        [Range(0f, 100f)] public float doubleVChanceOnSuccessPercent = 10f;
        [Range(0f, 100f)]
        [Tooltip("成功時、ダブルV抽選より先に判定するピンクV確率")]
        public float pinkVChanceOnSuccessPercent = 1f;
    }

    [Header("背景：RawImageを1枚使う")]
    [SerializeField] private RawImage backgroundRawImage;
    [SerializeField] private Vector2 backgroundScrollSpeed = new Vector2(0.06f, 0f);
    [SerializeField] private Vector2 backgroundTiling = Vector2.one;
    [Header("文字とキャラクターは別UI")]
    [SerializeField] private GameObject aimTextObject;
    [SerializeField] private Image characterImage;
    [Header("キャラクター別・出現率／期待度")]
    [SerializeField] private List<CharacterSetting> characters = new List<CharacterSetting>();
    [Header("演出の終了方法")]
    [SerializeField] private bool autoHideOnThirdStopInput = false;
    [SerializeField] private SlotGameManager slotGameManager;
    [Header("デバッグ")]
    [SerializeField] private bool logResult = true;

    public bool IsPlaying { get; private set; }
    public VAimOutcome CurrentOutcome { get; private set; } = VAimOutcome.Fake;
    public string CurrentCharacterId { get; private set; } = "";
    public event Action<VAimOutcome, string> VAimPrepared;

    private Vector2 uvScrollOffset;
    private RectTransform characterRect;
    private Vector2 initialCharacterPosition;

    private void Awake()
    {
        if (characterImage != null)
        {
            characterRect = characterImage.rectTransform;
            initialCharacterPosition = characterRect.anchoredPosition;
        }
        HideVisuals();
    }

    private void OnEnable()
    {
        if (slotGameManager != null)
            slotGameManager.ChronicleReelStopPressed += OnReelStopPressed;
    }

    private void OnDisable()
    {
        if (slotGameManager != null)
            slotGameManager.ChronicleReelStopPressed -= OnReelStopPressed;
    }

    private void Update()
    {
        if (!IsPlaying || backgroundRawImage == null) return;
        uvScrollOffset += backgroundScrollSpeed * Time.deltaTime;
        uvScrollOffset.x = Mathf.Repeat(uvScrollOffset.x, 1f);
        uvScrollOffset.y = Mathf.Repeat(uvScrollOffset.y, 1f);
        backgroundRawImage.uvRect = new Rect(
            uvScrollOffset.x, uvScrollOffset.y,
            Mathf.Max(0.01f, backgroundTiling.x),
            Mathf.Max(0.01f, backgroundTiling.y));
    }

    public void BeginVAim()
    {
        if (IsPlaying) return;
        CharacterSetting selected = SelectCharacter();
        if (selected == null)
        {
            Debug.LogWarning("[Vを狙え] キャラクター未登録または出現重みが全て0です", this);
            return;
        }
        CurrentCharacterId = selected.characterId;
        CurrentOutcome = DrawOutcome(selected);
        IsPlaying = true;
        uvScrollOffset = Vector2.zero;
        if (backgroundRawImage != null)
        {
            backgroundRawImage.gameObject.SetActive(true);
            backgroundRawImage.uvRect = new Rect(0f, 0f,
                Mathf.Max(0.01f, backgroundTiling.x),
                Mathf.Max(0.01f, backgroundTiling.y));
        }
        if (aimTextObject != null) aimTextObject.SetActive(true);
        if (characterImage != null)
        {
            characterImage.sprite = selected.characterSprite;
            characterImage.gameObject.SetActive(selected.characterSprite != null);
            if (characterRect != null)
                characterRect.anchoredPosition = initialCharacterPosition;
        }
        if (logResult)
            Debug.Log("[Vを狙え] 開始 / キャラ=" + CurrentCharacterId
                + " / 結果=" + CurrentOutcome);
        VAimPrepared?.Invoke(CurrentOutcome, CurrentCharacterId);
    }

    public void EndVAim()
    {
        IsPlaying = false;
        HideVisuals();
    }

    [ContextMenu("テスト：Vを狙えを開始")]
    private void TestBeginVAim()
    {
        if (Application.isPlaying) BeginVAim();
    }

    [ContextMenu("テスト：Vを狙えを終了")]
    private void TestEndVAim() { EndVAim(); }

    private void OnReelStopPressed(int stopNumber, ReelController.ReelType reel)
    {
        if (IsPlaying && autoHideOnThirdStopInput && stopNumber == 3)
            EndVAim();
    }

    private void HideVisuals()
    {
        if (backgroundRawImage != null) backgroundRawImage.gameObject.SetActive(false);
        if (aimTextObject != null) aimTextObject.SetActive(false);
        if (characterImage != null) characterImage.gameObject.SetActive(false);
    }

    private CharacterSetting SelectCharacter()
    {
        float total = 0f;
        foreach (var ch in characters)
            if (ch != null) total += Mathf.Max(0f, ch.appearanceWeight);
        if (total <= 0f) return null;
        float roll = UnityEngine.Random.value * total;
        foreach (var ch in characters)
        {
            if (ch == null) continue;
            roll -= Mathf.Max(0f, ch.appearanceWeight);
            if (roll < 0f) return ch;
        }
        for (int i = characters.Count - 1; i >= 0; i--)
            if (characters[i] != null && characters[i].appearanceWeight > 0f)
                return characters[i];
        return null;
    }

    private VAimOutcome DrawOutcome(CharacterSetting ch)
    {
        if (UnityEngine.Random.value >= Mathf.Clamp01(ch.successChancePercent / 100f))
            return VAimOutcome.Fake;
        if (UnityEngine.Random.value < Mathf.Clamp01(ch.pinkVChanceOnSuccessPercent / 100f))
            return VAimOutcome.PinkV;
        return UnityEngine.Random.value < Mathf.Clamp01(ch.doubleVChanceOnSuccessPercent / 100f)
            ? VAimOutcome.DoubleV : VAimOutcome.SingleV;
    }
}
