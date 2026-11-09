using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BonusLampManager : MonoBehaviour
{
    [Header("ランプ")]
    [SerializeField] private Image leftLampImage;
    [SerializeField] private Image rightLampImage;

    [SerializeField] private CanvasGroup canvasGroup;

    [Header("告知SE")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;


    [Header("演出設定")]
    [SerializeField] private float flashDuration = 0.12f;
    [SerializeField] private int flashCount = 4;

    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float hitScale = 1.15f;

    [Header("明るさ")]
    [Range(0f, 1f)]
    [SerializeField] private float offAlpha = 0.15f;

    [Range(0f, 1f)]
    [SerializeField] private float onAlpha = 1f;


    private Coroutine effectCoroutine;
    private bool isLampOn = false;


    // =========================================
    // Start
    // =========================================

    private void Start()
    {
        TurnOff();
    }


    // =========================================
    // 当選告知
    // =========================================

    public void PlayHitEffect()
    {
        if (isLampOn)
        {
            return;
        }

        if (effectCoroutine != null)
        {
            StopCoroutine( effectCoroutine);
        }

        effectCoroutine =StartCoroutine(HitEffectCoroutine());
    }


    // =========================================
    // 告知演出
    // =========================================

    private IEnumerator HitEffectCoroutine()
    {
        isLampOn =true;

        Debug.Log( " BONUS LAMP 告知！" );


        // =====================================
        // SE
        // =========================================

        if (
            audioSource != null && hitSound != null
        )
        {
            audioSource.PlayOneShot( hitSound);
        }


        // =====================================
        // 2個同時に拡大
        // =====================================

        SetLampScale(hitScale);

        yield return new WaitForSeconds( 0.10f);

        SetLampScale( normalScale);


        // =====================================
        // 2個同時点滅
        // =====================================

        for ( int i = 0;i < flashCount; i++ ){
            SetLampAlpha( onAlpha);
            yield return new WaitForSeconds( flashDuration);

            SetLampAlpha(offAlpha );
            yield return new WaitForSeconds( flashDuration );
        }


        // =====================================
        // 最後は2個とも点灯
        // =========================================

        SetLampAlpha( onAlpha);
        SetLampScale( normalScale );

        effectCoroutine = null;
    }


    // =========================================
    // 即点灯
    // =========================================

    public void TurnOn()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine( effectCoroutine);
            effectCoroutine =null;
        }


        isLampOn = true;

        SetLampScale(normalScale );
        SetLampAlpha( onAlpha );
    }


    // =========================================
    // 消灯
    // =========================================

    public void TurnOff()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine( effectCoroutine);
            effectCoroutine = null;
        }


        isLampOn =false;


        SetLampScale(normalScale);
        SetLampAlpha(offAlpha);
    }


    // =========================================
    // 明るさ変更
    // =========================================

    private void SetLampAlpha( float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }


        if (leftLampImage != null)
        {
            leftLampImage.enabled = true;
        }


        if (rightLampImage != null)
        {
            rightLampImage.enabled = true;
        }
    }


    // =========================================
    // 2個同時にサイズ変更
    // =========================================

    private void SetLampScale(
        float scale
    )
    {
        if (leftLampImage != null)
        {
            leftLampImage.rectTransform.localScale = Vector3.one * scale;
        }

        if (rightLampImage != null)
        {
            rightLampImage.rectTransform.localScale = Vector3.one * scale;
        }
    }


    // =========================================
    // 点灯中か
    // =========================================

    public bool IsLampOn()
    {
        return isLampOn;
    }
}