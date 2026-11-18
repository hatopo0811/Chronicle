
using UnityEngine;

public class LoopScrollBackground : MonoBehaviour
{
    [Header("背景画像")]
    [SerializeField] private RectTransform background1;
    [SerializeField] private RectTransform background2;

    [Header("スクロール設定")]
    [SerializeField] private float scrollSpeed = 100f;

    [SerializeField] private bool isScrolling = true;

    private float backgroundWidth;
    private float startX;

    private void Start()
    {
        if (background1 == null || background2 == null)
        {
            Debug.LogError("背景画像が設定されていません！");
            enabled = false;
            return;
        }

        backgroundWidth = background1.rect.width;

        if (backgroundWidth <= 0f)
        {
            Debug.LogError("背景画像の横幅が不正です！");
            enabled = false;
            return;
        }

        startX = background1.anchoredPosition.x;

        // 2枚目を1枚目の右側に配置
        background2.anchoredPosition = new Vector2(
            startX + backgroundWidth,
            background1.anchoredPosition.y
        );
    }

    private void Update()
    {
        if (!isScrolling) return;

        float move = Mathf.Max(0f, scrollSpeed) * Time.deltaTime;

        // 両方の背景を左に移動
        background1.anchoredPosition += Vector2.left * move;
        background2.anchoredPosition += Vector2.left * move;

        // 左に流れた背景を右側へ移動
        if (background1.anchoredPosition.x <= startX - backgroundWidth)
        {
            background1.anchoredPosition = new Vector2(
                background2.anchoredPosition.x + backgroundWidth,
                background1.anchoredPosition.y
            );
        }

        if (background2.anchoredPosition.x <= startX - backgroundWidth)
        {
            background2.anchoredPosition = new Vector2(
                background1.anchoredPosition.x + backgroundWidth,
                background2.anchoredPosition.y
            );
        }
    }

    // スクロール速度を変更
    public void SetScrollSpeed(float speed)
    {
        scrollSpeed = Mathf.Max(0f, speed);
    }

    // スクロール開始
    public void StartScroll()
    {
        isScrolling = true;
    }

    // スクロール停止
    public void StopScroll()
    {
        isScrolling = false;
    }
}
