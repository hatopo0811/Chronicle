using UnityEngine;

public partial class SlotGameManager
{
    // =========================================
    // 払い出し
    // =========================================

    private const int ElevenBellPayout = 11;
    private const int OneBellPayout = 1;
    private const int SuikaPayout = 3;
    private const int CherryPayout = 1;

    private void ProcessPayout()
    {
        LastPayout = 0;
        if (CurrentRole == RoleType.ATBell)
        {
            bool complete = ATBellStopInputCount == 3 &&
                PressedATBellOrder.Length == 3;
            bool correctOrder = complete && IsATBellOrderCorrect;
            bool fullBell = IsChronicleATBellFullStop();
            bool oneBell = IsChronicleNormalBellStop();

            if (correctOrder && fullBell && CurrentHitRole == HitRoleType.Bell)
                LastPayout = ElevenBellPayout;
            else if (!correctOrder && complete && oneBell && CurrentHitRole == HitRoleType.Bell)
                LastPayout = OneBellPayout;
            else
            {
                Debug.LogWarning("[クロニクル] ATベル停止判定不一致: 正解="
                    + CurrentATBellNavigation + " / 入力=" + PressedATBellOrder
                    + " / 順番一致=" + correctOrder
                    + " / 出目=" + CurrentChronicleBellStopName());
                CurrentHitRole = HitRoleType.None;
            }

            if (LastPayout > 0)
            {
                if (slotAudio != null) slotAudio.PlayBellPayoutSound();
                AddCredit(LastPayout);
            }
            UpdatePayDisplay();
            UpdateCreditDisplay();
            Debug.Log("[クロニクル][AT2択ベル] 正解=" + CurrentATBellNavigation
                + " / 入力=" + PressedATBellOrder + " / 一致=" + correctOrder
                + " / 停止形=" + CurrentChronicleBellStopName()
                + " / PAY=" + LastPayout);
            NotifyChronicleATBellResolved(correctOrder && fullBell, LastPayout);
            return;
        }

        switch (CurrentHitRole)
        {
            case HitRoleType.Bell:
                // 通常時の1枚役。5枚ベルは一切実装しない。
                if (IsChronicleNormalBellStop())
                    LastPayout = OneBellPayout;
                else
                {
                    Debug.LogWarning("[クロニクル] 通常ベルが1枚停止形と一致しません。");
                    CurrentHitRole = HitRoleType.None;
                }
                if (LastPayout > 0 && slotAudio != null)
                    slotAudio.PlayBellPayoutSound();
                break;
            case HitRoleType.Suika:
                LastPayout = SuikaPayout;
                break;
            case HitRoleType.Cherry:
            case HitRoleType.StrongCherry:
                LastPayout = CherryPayout;
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
        Debug.Log("[クロニクル] モード=" + (chronicleIsAT ? "AT" : "通常")
            + " / 内部役=" + GetRoleName(CurrentRole)
            + " / 入賞=" + GetHitRoleName(CurrentHitRole)
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