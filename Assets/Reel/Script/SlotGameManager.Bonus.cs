using UnityEngine;

public partial class SlotGameManager
{
    // =========================================
    // BONUS告知
    // =========================================

    private void CheckBonusLampNotification()
    {
        if (
            ActiveBonus !=
            BonusType.None
        )
        {
            return;
        }


        if (
            PendingBonus ==
            BonusType.None
        )
        {
            return;
        }


        if (bonusLampNotified)
        {
            return;
        }


        if (bonusLamp != null)
        {
            bonusLamp.PlayHitEffect();
        }


        if (slotAudio != null)
        {
            slotAudio
                .PlayHibiscusNotifySound();
        }


        bonusLampNotified =
            true;


        Debug.Log(
            "★★★★★★★★★★★★★★★★"
        );


        Debug.Log(
            "ボーナス告知！"
        );


        Debug.Log(
            "成立ボーナス：" +
            GetBonusName(
                PendingBonus
            )
        );


        Debug.Log(
            "★★★★★★★★★★★★★★★★"
        );
    }


    // =========================================
    // BONUS図柄入賞確認
    // =========================================

    private void CheckBonusResult()
    {
        if (
            CurrentHitRole ==
                HitRoleType.BigBonus
            &&
            PendingBonus ==
                BonusType.Big
        )
        {
            StartBonus(
                BonusType.Big
            );


            return;
        }


        if (
            CurrentHitRole ==
                HitRoleType.RegBonus
            &&
            PendingBonus ==
                BonusType.Reg
        )
        {
            StartBonus(
                BonusType.Reg
            );


            return;
        }


        if (
            PendingBonus !=
            BonusType.None
        )
        {
            Debug.Log(
                "ボーナス未入賞 → 持ち越し"
            );


            Debug.Log(
                "保持中：" +
                GetBonusName(
                    PendingBonus
                )
            );
        }
    }


    // =========================================
    // BONUS開始
    // =========================================

    private void StartBonus(
        BonusType bonusType
    )
    {
        // =====================================
        // BONUS状態
        // =========================================

        ActiveBonus =
            bonusType;


        PendingBonus =
            BonusType.None;


        replayNextGame =
            false;


        // =====================================
        // 疑似遊戯解除
        // =========================================

        SetPseudoPlayModeForAllReels(
            false
        );


        // =====================================
        // ボーナス累計払い出しリセット
        // =========================================

        BonusTotalPayout =
            0;


        if (BonusPlusCount != null)
        {
            BonusPlusCount
                .gameObject
                .SetActive(
                    true
                );
        }


        UpdateCountDisplay();


        // =====================================
        // Reach / Lose状態解除
        // =========================================

        selectedReachPattern =
            null;


        selectedLosePattern =
            null;


        currentReachPattern =
            null;


        currentLosePattern =
            null;


        reachShownForCurrentBonus =
            false;


        // =====================================
        // ランプ消灯
        // =========================================

        if (bonusLamp != null)
        {
            bonusLamp.TurnOff();
        }


        bonusLampNotified =
            false;


        // =====================================
        // BIG
        // =========================================

        if (
            bonusType ==
            BonusType.Big
        )
        {
            // =================================
            // ★ BIG回数 +1
            // =========================================

            if (dataCounter != null)
            {
                dataCounter.AddBigBonus();
            }


            RemainingBonusGrapes =
                BIG_BGrapeCount;


            if (slotAudio != null)
            {
                slotAudio
                    .PlayBigBonusStart();
            }


            Debug.Log(
                "★★★★★★★★★★★★★★★★"
            );


            Debug.Log(
                "BIG BONUS START！"
            );


            Debug.Log(
                "BonusGrape：" +
                RemainingBonusGrapes +
                "回"
            );


            Debug.Log(
                "★★★★★★★★★★★★★★★★"
            );
        }


        // =====================================
        // REG
        // =========================================

        else if (
            bonusType ==
            BonusType.Reg
        )
        {
            // =================================
            // ★ REG回数 +1
            // =========================================

            if (dataCounter != null)
            {
                dataCounter.AddRegBonus();
            }


            RemainingBonusGrapes =
                REG_BGrapeCount;


            if (slotAudio != null)
            {
                slotAudio
                    .PlayRegBonusStart();
            }


            Debug.Log(
                "★★★★★★★★★★★★★★★★"
            );


            Debug.Log(
                "REG BONUS START！"
            );


            Debug.Log(
                "BonusGrape：" +
                RemainingBonusGrapes +
                "回"
            );


            Debug.Log(
                "★★★★★★★★★★★★★★★★"
            );
        }


        UpdateBonusDebugText();
    }


    // =========================================
    // BONUS終了
    // =========================================

    private void EndBonus()
    {
        BonusType finished =
            ActiveBonus;


        ActiveBonus =
            BonusType.None;


        RemainingBonusGrapes =
            0;


        replayNextGame =
            false;


        bonusLampNotified =
            false;


        if (bonusLamp != null)
        {
            bonusLamp.TurnOff();
        }


        if (slotAudio != null)
        {
            slotAudio
                .StopBonusSounds();


            slotAudio
                .PlayBonusEndSound();
        }


        if (BonusPlusCount != null)
        {
            BonusPlusCount
                .gameObject
                .SetActive(
                    false
                );
        }


        Debug.Log(
            "★★★★★★★★★★★★★★★★"
        );


        Debug.Log(
            GetBonusName(
                finished
            ) +
            " BONUS終了！"
        );


        Debug.Log(
            "最終払い出し：" +
            BonusTotalPayout +
            "枚"
        );


        Debug.Log(
            "★★★★★★★★★★★★★★★★"
        );


        if (DebugText != null)
        {
            DebugText.text =
                GetBonusName(
                    finished
                ) +
                " END";
        }
    }


    // =========================================
    // BONUSデバッグ表示
    // =========================================

    private void UpdateBonusDebugText()
    {
        if (DebugText == null)
        {
            return;
        }
        

        DebugText.text =
            GetBonusName(
                ActiveBonus
            ) +
            "\nBonusGrape 残り " +
            RemainingBonusGrapes +
            "\n払い出し " +
            BonusTotalPayout +
            "枚";
    }
}