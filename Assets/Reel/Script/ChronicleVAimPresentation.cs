using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Vを狙え演出の「見た目と抽選」を独立管理するコンポーネント。
// 既存のSlotGameManager / ReelControllerを変更せず追加できます。
// 注意：抽選したV結果を実際の停止出目に反映する処理は別途必要です。
public class ChronicleVAimPresentation : MonoBehaviour
{
    public enum VAimOutcome
    {
        Fake,
        SingleV,
        DoubleV
    }

    [Serializable]
    public class CharacterSetting
    {
        [Tooltip("チーム内で識別するための名前")]
        public string characterId = "Character";

        [Tooltip("キャラクターUIに表示する画像")]
        public Sprite characterSprite;

        [Min(0f)]
        [Tooltip("出現しやすさの重み。確率ではなく、ほかのキャラクターとの比率です")]
        public float appearanceWeight = 1f;

        [Range(0f, 100f)]
        [Tooltip("このキャラ登場時にシングルまたはダブルVに成功する確率")]
        public float successChancePercent = 30f;

        [Range(0f, 100f)]
        [Tooltip("成功した場合、そのうちダブルVになる確率")]
        public float doubleVChanceOnSuccessPercent = 10f;
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
    [Tooltip("ONなら第3停止ボタンが入力された時点で演出UIを非表示にする。リールの停止完了を待つ処理ではありません")]
    [SerializeField] private bool autoHideOnThirdStopInput = false;
    [SerializeField] private SlotGameManager slotGameManager;

    [Header("デバッグ")]
    [SerializeField] private bool logResult = true;

    public bool IsPlaying { get; private set; }
    public VAimOutcome CurrentOutcome { get; private set; } = VAimOutcome.Fake;
    public string CurrentCharacterId { get; private set; } = "";

    // チームのV停止制御と連携する場合はこのイベントを購読してください。
    // 引数: 抽選結果、キャラクターID
    public event Action<VAimOutcome, string> VAimPrepared;

    private RectTransform characterRect;
    private Vector2 initialCharacterPosition;
    private Vector2 uvScrollOffset;

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
        if (!IsPlaying || backgroundRawImage == null)
            return;

        // UVの位置をずらして1枚のテクスチャをループさせる。
        // Texture Import Settings の Wrap Mode = Repeat が必要。
        uvScrollOffset += backgroundScrollSpeed * Time.deltaTime;
        uvScrollOffset.x = Mathf.Repeat(uvScrollOffset.x, 1f);
        uvScrollOffset.y = Mathf.Repeat(uvScrollOffset.y, 1f);
        backgroundRawImage.uvRect = new Rect(
            uvScrollOffset.x,
            uvScrollOffset.y,
            Mathf.Max(0.01f, backgroundTiling.x),
            Mathf.Max(0.01f, backgroundTiling.y)
        );
    }

    // AT管理側から「Vを狙え演出を開始する」ときに呼ぶ。
    // この関数はリールを回転・停止させません。
    public void BeginVAim()
    {
        CharacterSetting selected = SelectCharacter();
        if (selected == null)
        {
            Debug.LogWarning("[Vを狙え] キャラクターが未登録、または出現重みがすべて0です。", this);
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

        if (aimTextObject != null)
            aimTextObject.SetActive(true);

        if (characterImage != null)
        {
            characterImage.sprite = selected.characterSprite;
            characterImage.gameObject.SetActive(selected.characterSprite != null);
            if (characterRect != null)
                characterRect.anchoredPosition = initialCharacterPosition;
        }

        if (logResult)
            Debug.Log("[Vを狙え] 開始 / キャラ=" + CurrentCharacterId +
                      " / 抽選結果=" + CurrentOutcome +
                      " / 成功期待度=" + selected.successChancePercent + "%");

        VAimPrepared?.Invoke(CurrentOutcome, CurrentCharacterId);
    }

    // 第3停止後や、チームのAT演出終了処理から呼ぶ。
    public void EndVAim()
    {
        IsPlaying = false;
        HideVisuals();
    }

    [ContextMenu("テスト：Vを狙えを開始")]
    private void TestBeginVAim()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[Vを狙え] Playモードでテストしてください。", this);
            return;
        }
        BeginVAim();
    }

    [ContextMenu("テスト：Vを狙えを終了")]
    private void TestEndVAim()
    {
        EndVAim();
    }

    private void OnReelStopPressed(int stopNumber, ReelController.ReelType reel)
    {
        if (IsPlaying && autoHideOnThirdStopInput && stopNumber == 3)
            EndVAim();
    }

    private void HideVisuals()
    {
        if (backgroundRawImage != null)
            backgroundRawImage.gameObject.SetActive(false);
        if (aimTextObject != null)
            aimTextObject.SetActive(false);
        if (characterImage != null)
            characterImage.gameObject.SetActive(false);
    }

    private CharacterSetting SelectCharacter()
    {
        float totalWeight = 0f;
        foreach (CharacterSetting character in characters)
        {
            if (character != null)
                totalWeight += Mathf.Max(0f, character.appearanceWeight);
        }

        if (totalWeight <= 0f)
            return null;

        float roll = UnityEngine.Random.value * totalWeight;
        foreach (CharacterSetting character in characters)
        {
            if (character == null) continue;
            roll -= Mathf.Max(0f, character.appearanceWeight);
            if (roll < 0f) return character;
        }
        return characters[characters.Count - 1];
    }

    private VAimOutcome DrawOutcome(CharacterSetting selected)
    {
        float success = Mathf.Clamp01(selected.successChancePercent / 100f);
        if (UnityEngine.Random.value >= success)
            return VAimOutcome.Fake;

        float doubleChance = Mathf.Clamp01(selected.doubleVChanceOnSuccessPercent / 100f);
        return UnityEngine.Random.value < doubleChance
            ? VAimOutcome.DoubleV
            : VAimOutcome.SingleV;
    }
}
