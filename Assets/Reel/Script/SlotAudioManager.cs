using System.Collections;
using UnityEngine;

public class SlotAudioManager : MonoBehaviour
{
    // =========================================
    // AudioSource
    // =========================================

    [Header("AudioSource")]
    [SerializeField]
    private AudioSource seSource;

    [SerializeField]
    private AudioSource fanfareSource;

    [SerializeField]
    private AudioSource bgmSource;


    // =========================================
    // BET
    // =========================================

    [Header("BET")]
    [SerializeField]
    private AudioClip betSound;

    [SerializeField]
    private AudioClip maxBetSound;


    // =========================================
    // リール
    // =========================================

    [Header("リール")]
    [SerializeField]
    private AudioClip leverOnSound;

    [SerializeField]
    private AudioClip reelStopSound;

    [SerializeField]
    private AudioClip thirdStopSound;


    // =========================================
    // 告知
    // =========================================

    [Header("告知")]
    [SerializeField]
    private AudioClip hibiscusNotifySound;

    [SerializeField]
    private AudioClip advanceNotifySound;

    [SerializeField]
    private AudioClip premiumNotifySound;


    // =========================================
    // 小役・払い出し
    // =========================================

    [Header("小役・払い出し")]
    [SerializeField]
    private AudioClip grapePayoutSound;

    [SerializeField]
    private AudioClip bellPayoutSound;

    [SerializeField]
    private AudioClip cherryHitSound;

    [SerializeField]
    private AudioClip replaySound;

    [SerializeField]
    private AudioClip hibiscusPayoutSound;


    // =========================================
    // リーチ目
    // =========================================

    [Header("リーチ目")]
    [SerializeField]
    private AudioClip reachSound;


    // =========================================
    // BONUSファンファーレ
    // =========================================

    [Header("BONUS ファンファーレ")]
    [SerializeField]
    private AudioClip bigFanfare;

    [SerializeField]
    private AudioClip regFanfare;


    // =========================================
    // BONUS BGM
    // =========================================

    [Header("BONUS BGM")]
    [SerializeField]
    private AudioClip bigBonusBgm;

    [SerializeField]
    private AudioClip regBonusBgm;


    // =========================================
    // BONUS終了
    // =========================================

    [Header("BONUS終了")]
    [SerializeField]
    private AudioClip bonusEndSound;


    // =========================================
    // CREDIT
    // =========================================

    [Header("CREDIT")]
    [SerializeField]
    private AudioClip creditShortageSound;

    [SerializeField]
    private AudioClip medalAddSound;


    // =========================================
    // BONUSシーケンス
    // =========================================

    private Coroutine bonusSoundCoroutine = null;


    // =========================================
    // 共通SE
    // =========================================

    private void PlaySE(
        AudioClip clip
    )
    {
        if (clip == null)
        {
            return;
        }


        if (seSource == null)
        {
            Debug.LogWarning(
                "SlotAudioManagerのSE用AudioSourceが設定されていません。"
            );

            return;
        }


        seSource.PlayOneShot(
            clip
        );
    }


    // =========================================
    // BET
    // =========================================

    public void PlayBetSound()
    {
        PlaySE(
            betSound
        );
    }


    public void PlayMaxBetSound()
    {
        PlaySE(
            maxBetSound
        );
    }


    public void PlayBetSound(
        int bet
    )
    {
        if (bet >= 3)
        {
            PlayMaxBetSound();

            return;
        }


        PlayBetSound();
    }


    // =========================================
    // レバーON
    // =========================================

    public void PlayLeverOnSound()
    {
        PlaySE(
            leverOnSound
        );
    }


    // =========================================
    // リール停止
    // =========================================

    public void PlayReelStopSound()
    {
        PlaySE(
            reelStopSound
        );
    }


    // =========================================
    // 第3停止
    // =========================================

    public void PlayThirdStopSound()
    {
        PlaySE(
            thirdStopSound
        );
    }


    // =========================================
    // ハイビスカス告知
    // =========================================

    public void PlayHibiscusNotifySound()
    {
        PlaySE(
            hibiscusNotifySound
        );
    }


    // =========================================
    // 先告知
    // =========================================

    public void PlayAdvanceNotifySound()
    {
        PlaySE(
            advanceNotifySound
        );
    }


    // =========================================
    // プレミア告知
    // =========================================

    public void PlayPremiumNotifySound()
    {
        PlaySE(
            premiumNotifySound
        );
    }


    // =========================================
    // ぶどう払い出し
    // =========================================

    public void PlayGrapePayoutSound()
    {
        PlaySE(
            grapePayoutSound
        );
    }


    // =========================================
    // ベル払い出し
    // =========================================

    public void PlayBellPayoutSound()
    {
        PlaySE(
            bellPayoutSound
        );
    }


    // =========================================
    // Cherry
    // =========================================

    public void PlayCherryHitSound()
    {
        PlaySE(
            cherryHitSound
        );
    }


    // =========================================
    // Replay
    // =========================================

    public void PlayReplaySound()
    {
        PlaySE(
            replaySound
        );
    }


    // =========================================
    // Hibiscus払い出し
    // =========================================

    public void PlayHibiscusPayoutSound()
    {
        PlaySE(
            hibiscusPayoutSound
        );
    }


    // =========================================
    // Reach
    // =========================================

    public void PlayReachSound()
    {
        PlaySE(
            reachSound
        );
    }


    // =========================================
    // BIG BONUS開始
    //
    // ファンファーレ
    // ↓
    // 終了後にBIG BGM
    // =========================================

    public void PlayBigBonusStart()
    {
        StopBonusSounds();


        bonusSoundCoroutine =
            StartCoroutine(
                PlayBonusStartSequence(
                    bigFanfare,
                    bigBonusBgm
                )
            );
    }


    // =========================================
    // REG BONUS開始
    // =========================================

    public void PlayRegBonusStart()
    {
        StopBonusSounds();


        bonusSoundCoroutine =
            StartCoroutine(
                PlayBonusStartSequence(
                    regFanfare,
                    regBonusBgm
                )
            );
    }


    // =========================================
    // BONUS開始シーケンス
    // =========================================

    private IEnumerator PlayBonusStartSequence(
        AudioClip fanfare,
        AudioClip bgm
    )
    {
        // =====================================
        // ファンファーレ
        // =========================================

        if (
            fanfare != null &&
            fanfareSource != null
        )
        {
            fanfareSource.Stop();

            fanfareSource.loop =
                false;

            fanfareSource.clip =
                fanfare;

            fanfareSource.Play();


            // ファンファーレ終了まで待つ
            yield return new WaitForSeconds(
                fanfare.length
            );
        }


        // =====================================
        // BONUS BGM
        // =========================================

        if (
            bgm != null &&
            bgmSource != null
        )
        {
            bgmSource.Stop();

            bgmSource.clip =
                bgm;

            bgmSource.loop =
                true;

            bgmSource.Play();
        }


        bonusSoundCoroutine =
            null;
    }


    // =========================================
    // BIGファンファーレ単体
    // =========================================

    public void PlayBigFanfare()
    {
        PlayFanfare(
            bigFanfare
        );
    }


    // =========================================
    // REGファンファーレ単体
    // =========================================

    public void PlayRegFanfare()
    {
        PlayFanfare(
            regFanfare
        );
    }


    // =========================================
    // ファンファーレ共通
    // =========================================

    private void PlayFanfare(
        AudioClip clip
    )
    {
        if (clip == null)
        {
            return;
        }


        if (fanfareSource == null)
        {
            PlaySE(
                clip
            );

            return;
        }


        fanfareSource.Stop();

        fanfareSource.loop =
            false;

        fanfareSource.clip =
            clip;

        fanfareSource.Play();
    }


    // =========================================
    // BIG BGM単体
    // =========================================

    public void PlayBigBonusBgm()
    {
        PlayBgm(
            bigBonusBgm
        );
    }


    // =========================================
    // REG BGM単体
    // =========================================

    public void PlayRegBonusBgm()
    {
        PlayBgm(
            regBonusBgm
        );
    }


    // =========================================
    // BGM共通
    // =========================================

    private void PlayBgm(
        AudioClip clip
    )
    {
        if (clip == null)
        {
            return;
        }


        if (bgmSource == null)
        {
            Debug.LogWarning(
                "SlotAudioManagerのBGM用AudioSourceが設定されていません。"
            );

            return;
        }


        bgmSource.Stop();

        bgmSource.clip =
            clip;

        bgmSource.loop =
            true;

        bgmSource.Play();
    }


    // =========================================
    // BONUS関連音すべて停止
    // =========================================

    public void StopBonusSounds()
    {
        if (bonusSoundCoroutine != null)
        {
            StopCoroutine(
                bonusSoundCoroutine
            );


            bonusSoundCoroutine =
                null;
        }


        if (fanfareSource != null)
        {
            fanfareSource.Stop();
        }


        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }


    // =========================================
    // ファンファーレ停止
    // =========================================

    public void StopFanfare()
    {
        if (fanfareSource == null)
        {
            return;
        }


        fanfareSource.Stop();
    }


    // =========================================
    // BGM停止
    // =========================================

    public void StopBonusBgm()
    {
        if (bgmSource == null)
        {
            return;
        }


        bgmSource.Stop();
    }


    // =========================================
    // BONUS終了音
    // =========================================

    public void PlayBonusEndSound()
    {
        PlaySE(
            bonusEndSound
        );
    }


    // =========================================
    // CREDIT不足
    // =========================================

    public void PlayCreditShortageSound()
    {
        PlaySE(
            creditShortageSound
        );
    }


    // =========================================
    // メダル補充
    // =========================================

    public void PlayMedalAddSound()
    {
        PlaySE(
            medalAddSound
        );
    }
}