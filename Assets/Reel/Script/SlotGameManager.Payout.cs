using UnityEngine;

public partial class SlotGameManager
{
    // =========================================
    // 払い出し
    // =========================================

    private void ProcessPayout()
    {
        LastPayout =
            0;


        // =====================================
        // BONUS中
        // =========================================

        if (
            ActiveBonus !=
            BonusType.None
        )
        {
            if (
                CurrentHitRole ==
                HitRoleType.Grape
            )
            {
                LastPayout =
                    Bonusgrape;


                AddCredit(
                    LastPayout
                );


                BonusTotalPayout +=
                    LastPayout;


                UpdateCountDisplay();


                // =================================
                // ★ BONUS中ぶどう払い出し音
                // =================================

                if (slotAudio != null)
                {
                    slotAudio
                        .PlayGrapePayoutSound();
                }


                RemainingBonusGrapes--;


                Debug.Log(
                    "★ BonusGrape入賞！"
                );


                Debug.Log(
                    "今回の払い出し：" +
                    LastPayout +
                    "枚"
                );


                Debug.Log(
                    "累計払い出し：" +
                    BonusTotalPayout +
                    "枚"
                );


                Debug.Log(
                    "残り：" +
                    RemainingBonusGrapes
                );


                if (
                    RemainingBonusGrapes <=
                    0
                )
                {
                    EndBonus();
                }
                else
                {
                    UpdateBonusDebugText();
                }
            }


            UpdatePayDisplay();

            UpdateCreditDisplay();


            return;
        }


        // =====================================
        // 通常時
        // =========================================

        switch (CurrentHitRole)
        {
            // =================================
            // Grape
            // =================================

            case HitRoleType.Grape:

                LastPayout =
                    grapePayout;


                AddCredit(
                    LastPayout
                );


                if (slotAudio != null)
                {
                    slotAudio
                        .PlayGrapePayoutSound();
                }

                break;


            // =================================
            // Bell
            // =================================

            case HitRoleType.Bell:

                LastPayout =
                    bellPayout;


                AddCredit(
                    LastPayout
                );


                if (slotAudio != null)
                {
                    slotAudio
                        .PlayBellPayoutSound();
                }

                break;


            // =================================
            // Cherry
            // =================================

            case HitRoleType.Cherry:

                LastPayout =
                    cherryPayout;


                AddCredit(
                    LastPayout
                );


                if (slotAudio != null)
                {
                    slotAudio
                        .PlayCherryHitSound();
                }

                break;


            // =================================
            // Hibiscus
            // =================================

            case HitRoleType.Hibiscus:

                LastPayout =
                    hibiscusPayout;


                AddCredit(
                    LastPayout
                );


                if (slotAudio != null)
                {
                    slotAudio
                        .PlayHibiscusPayoutSound();
                }

                break;


            // =================================
            // Replay
            // =================================

            case HitRoleType.Replay:

                replayNextGame =
                    true;


                if (slotAudio != null)
                {
                    slotAudio
                        .PlayReplaySound();
                }

                break;
        }


        UpdatePayDisplay();

        UpdateCreditDisplay();
    }


    // =========================================
    // CREDIT追加
    // =========================================

    private void AddCredit(
        int amount
    )
    {
        Credit +=
            amount;


        if (
            Credit >
            maxCredit
        )
        {
            Credit =
                maxCredit;
        }
    }


    // =========================================
    // CREDIT表示
    // =========================================

    private void UpdateCreditDisplay()
    {
        if (creditText == null)
        {
            return;
        }


        creditText.text =
            Credit.ToString(
                "00"
            );
    }


    // =========================================
    // PAY表示
    // =========================================

    private void UpdatePayDisplay()
    {
        if (payText == null)
        {
            return;
        }


        payText.text =
            LastPayout.ToString(
                "00"
            );
    }
}