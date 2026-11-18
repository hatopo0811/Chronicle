
using UnityEngine;

public class PushOrderNaviAnimation : MonoBehaviour
{
    // =========================================
    // アニメーション設定
    // =========================================

    [Header("待機アニメーション")]
    [SerializeField]
    private bool playOnEnable = true;

    [SerializeField]
    private float animationSpeed = 4f;

    [Header("拡大・縮小")]
    [SerializeField]
    [Range(1f, 1.5f)]
    private float maxScale = 1.12f;

    [Header("透明度変化")]
    [SerializeField]
    private bool useAlphaAnimation = false;

    [SerializeField]
    [Range(0f, 1f)]
    private float minAlpha = 0.7f;

    [Header("時間設定")]
    [SerializeField]
    private bool useUnscaledTime = false;

    // =========================================
    // 内部変数
    // =========================================

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector3 originalScale;
    private float originalAlpha = 1f;

    private float animationTime = 0f;
    private bool isWaiting = false;
    private bool initialized = false;

    // =========================================
    // 初期化
    // =========================================

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized) return;

        rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError(
                "PushOrderNaviAnimation: RectTransformがありません"
            );

            enabled = false;
            return;
        }

        canvasGroup = GetComponent<CanvasGroup>();

        originalScale = rectTransform.localScale;

        if (canvasGroup != null)
        {
            originalAlpha = canvasGroup.alpha;
        }

        initialized = true;
    }

    private void OnEnable()
    {
        Initialize();

        if (!initialized) return;

        if (playOnEnable)
        {
            StartWaiting();
        }
        else
        {
            ResetAnimation();
        }
    }

    // =========================================
    // アニメーション更新
    // =========================================

    private void Update()
    {
        if (!isWaiting) return;

        float deltaTime = useUnscaledTime
            ? Time.unscaledDeltaTime
            : Time.deltaTime;

        animationTime += deltaTime * animationSpeed;

        // 0～1～0 の滑らかな変化
        float wave =
            (Mathf.Sin(animationTime) + 1f) * 0.5f;

        // 拡大・縮小
        float scale =
            Mathf.Lerp(1f, maxScale, wave);

        rectTransform.localScale =
            originalScale * scale;

        // 必要なら透明度も変化
        if (useAlphaAnimation && canvasGroup != null)
        {
            canvasGroup.alpha = Mathf.Lerp(
                minAlpha,
                originalAlpha,
                wave
            );
        }
    }

    // =========================================
    // 待機アニメーション開始
    // =========================================

    public void StartWaiting()
    {
        Initialize();

        if (!initialized) return;

        ResetAnimation();

        animationTime = -Mathf.PI / 2f;
        isWaiting = true;
    }

    // =========================================
    // 待機アニメーション停止
    // =========================================

    public void StopWaiting()
    {
        isWaiting = false;

        ResetAnimation();
    }

    // =========================================
    // 押されたとき
    // =========================================

    public void OnNaviPressed()
    {
        StopWaiting();
    }

    // =========================================
    // 元の大きさに戻す
    // =========================================

    private void ResetAnimation()
    {
        if (!initialized) return;

        rectTransform.localScale = originalScale;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = originalAlpha;
        }
    }

    // =========================================
    // 非表示時
    // =========================================

    private void OnDisable()
    {
        isWaiting = false;

        ResetAnimation();
    }

    // =========================================
    // 速度変更
    // =========================================

    public void SetAnimationSpeed(float speed)
    {
        animationSpeed = Mathf.Max(0f, speed);
    }
}
