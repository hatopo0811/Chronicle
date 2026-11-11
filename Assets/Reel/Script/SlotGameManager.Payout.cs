using UnityEngine;

public partial class SlotGameManager
{
    // =========================================
    // 払い出し
    // =========================================

    private void ProcessPayout()
    {
        LastPayout = 0;
        switch (CurrentHitRole)
        {
            case HitRoleType.Bell:
                LastPayout = chronicleBellPayout;
                if (slotAudio != null) slotAudio.PlayBellPayoutSound();
                break;
            case HitRoleType.Suika:
                LastPayout = chronicleSuikaPayout;
                break;
            case HitRoleType.Cherry:
                LastPayout = chronicleCherryPayout;
                if (slotAudio != null) slotAudio.PlayCherryHitSound();
                break;
            case HitRoleType.StrongCherry:
                LastPayout = chronicleStrongCherryPayout;
                if (slotAudio != null) slotAudio.PlayCherryHitSound();
                break;
            case HitRoleType.Replay:
                replayNextGame = true;
                if (slotAudio != null) slotAudio.PlayReplaySound();
                break;
        }
        if (LastPayout > 0) AddCredit(LastPayout);
        UpdatePayDisplay();
        UpdateCreditDisplay();
        Debug.Log("[クロニクル] 入賞=" + GetHitRoleName(CurrentHitRole)
            + " / PAY=" + LastPayout);
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