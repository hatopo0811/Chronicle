
using UnityEngine;
using UnityEngine.UI;

public class NinjaRunAnimation : MonoBehaviour
{
    // =========================================
    // 画像設定
    // =========================================

    [Header("走るアニメーション画像（16枚）")]
    [SerializeField]
    private Sprite[] runSprites = new Sprite[16];

    [Header("表示するImage")]
    [SerializeField]
    private Image characterImage;

    // =========================================
    // アニメーション設定
    // =========================================

    [Header("アニメーション設定")]
    [SerializeField]
    [Min(1f)]
    private float animationFPS = 16f;

    [SerializeField]
    private bool playOnStart = true;

    [SerializeField]
    private bool loop = true;

    // =========================================
    // 移動設定
    // =========================================

    [Header("キャラクター移動")]
    [SerializeField]
    private bool moveCharacter = false;

    [SerializeField]
    private float moveSpeed = 100f;

    [Header("向き")]
    [SerializeField]
    private bool faceLeft = false;

    // =========================================
    // 内部変数
    // =========================================

    private RectTransform rectTransform;

    private int currentFrame = 0;

    private float animationTimer = 0f;

    private bool isPlaying = false;

    // =========================================
    // 初期化
    // =========================================

    private void Awake()
    {
        if (characterImage == null)
        {
            characterImage = GetComponent<Image>();
        }

        rectTransform = GetComponent<RectTransform>();

        if (characterImage == null)
        {
            Debug.LogError(
                "NinjaRunAnimation: Imageがありません！"
            );

            enabled = false;
            return;
        }

        if (runSprites == null || runSprites.Length == 0)
        {
            Debug.LogError(
                "NinjaRunAnimation: 画像が登録されていません！"
            );

            enabled = false;
            return;
        }

        // 最初のフレームを表示
        currentFrame = 0;
        ShowFrame();

        // 向きを設定
        SetDirection(faceLeft);

        isPlaying = playOnStart;
    }

    // =========================================
    // 毎フレーム更新
    // =========================================

    private void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        UpdateAnimation();

        if (moveCharacter)
        {
            MoveCharacter();
        }
    }

    // =========================================
    // アニメーション再生
    // =========================================

    private void UpdateAnimation()
    {
        float frameDuration = 1f / animationFPS;

        animationTimer += Time.deltaTime;

        while (animationTimer >= frameDuration)
        {
            animationTimer -= frameDuration;

            currentFrame++;

            // 最後のフレームに到達
            if (currentFrame >= runSprites.Length)
            {
                if (loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame = runSprites.Length - 1;
                    isPlaying = false;
                }
            }

            ShowFrame();

            if (!isPlaying)
            {
                break;
            }
        }
    }

    // =========================================
    // 画像切り替え
    // =========================================

    private void ShowFrame()
    {
        if (currentFrame < 0 ||
            currentFrame >= runSprites.Length)
        {
            return;
        }

        if (runSprites[currentFrame] == null)
        {
            return;
        }

        characterImage.sprite = runSprites[currentFrame];
    }

    // =========================================
    // キャラクター移動
    // =========================================

    private void MoveCharacter()
    {
        if (rectTransform == null)
        {
            return;
        }

        float direction = faceLeft ? -1f : 1f;

        Vector2 position = rectTransform.anchoredPosition;

        position.x +=
            direction * moveSpeed * Time.deltaTime;

        rectTransform.anchoredPosition = position;
    }

    // =========================================
    // 再生開始
    // =========================================

    public void StartRun()
    {
        currentFrame = 0;
        animationTimer = 0f;

        ShowFrame();

        isPlaying = true;
    }

    // =========================================
    // 再生停止
    // =========================================

    public void StopRun()
    {
        isPlaying = false;
    }

    // =========================================
    // FPS変更
    // =========================================

    public void SetFPS(float fps)
    {
        animationFPS = Mathf.Max(1f, fps);
    }

    // =========================================
    // 移動速度変更
    // =========================================

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = Mathf.Max(0f, speed);
    }

    // =========================================
    // 向き変更
    // =========================================

    public void SetDirection(bool left)
    {
        faceLeft = left;

        if (rectTransform != null)
        {
            Vector3 scale = rectTransform.localScale;

            scale.x = Mathf.Abs(scale.x) *
                      (faceLeft ? -1f : 1f);

            rectTransform.localScale = scale;
        }
    }
}
