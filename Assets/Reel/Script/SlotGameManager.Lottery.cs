using UnityEngine;

public partial class SlotGameManager
{
    private RoleType DrawNormalRole()
    {
        float total =
          loseProbability +
          replayProbability +
          grapeProbability +
          bellProbability +
          cherryProbability +
          hibiscusProbability +
          bigProbability +
          regProbability;


        float value =
            UnityEngine.Random.Range(
                0f,
                total
            );


        float border =
            loseProbability;


        if (value < border)
        {
            return
                RoleType.Lose;
        }


        border +=
            replayProbability;


        if (value < border)
        {
            return
                RoleType.Replay;
        }


        border +=
            grapeProbability;


        if (value < border)
        {
            return
                RoleType.Grape;
        }


        border +=
            bellProbability;


        if (value < border)
        {
            return
                RoleType.Bell;
        }


        border +=
            cherryProbability;


        if (value < border)
        {
            return
                RoleType.Cherry;
        }

        border +=
            hibiscusProbability;

        if (value < border)
        {
            return RoleType.Hibiscus;
        }

        border +=
            bigProbability;


        if (value < border)
        {
            return
                RoleType.BigBonus;
        }


        return
            RoleType.RegBonus;
    }

    private RoleType DrawRoleDuringBonusPending()
    {
        float total =
            loseProbability +
            replayProbability +
            grapeProbability;


        float value =
            UnityEngine.Random.Range(
                0f,
                total
            );


        float border =
            loseProbability;


        if (value < border)
        {
            return
                RoleType.Lose;
        }


        border +=
            replayProbability;


        if (value < border)
        {
            return
                RoleType.Replay;
        }


        border +=
            grapeProbability;


        if (value < border)
        {
            return
                RoleType.Grape;
        }



        return
            RoleType.Lose;
    }

    private string GetRoleName(
     RoleType role
 )
    {
        switch (role)
        {
            case RoleType.Lose:
                return "ハズレ";

            case RoleType.Replay:
                return "リプレイ";

            case RoleType.Grape:
                return "ぶどう";

            case RoleType.Bell:
                return "ベル";

            case RoleType.Cherry:
                return "チェリー";

            case RoleType.Hibiscus:
                return "ハイビスカス";

            case RoleType.BigBonus:
                return "BIG BONUS";

            case RoleType.RegBonus:
                return "REG BONUS";
        }

        return "不明";
    }

    private string GetHitRoleName(
        HitRoleType role
    )
    {
        switch (role)
        {
            case HitRoleType.None:
                return "入賞なし";

            case HitRoleType.Replay:
                return "リプレイ";

            case HitRoleType.Grape:
                return "ぶどう";

            case HitRoleType.Bell:
                return "ベル";

            case HitRoleType.Cherry:
                return "チェリー";

            case HitRoleType.BigBonus:
                return "BIG BONUS";

            case HitRoleType.RegBonus:
                return "REG BONUS";
        }


        return "不明";
    }

    private string GetBonusName(
        BonusType bonus
    )
    {
        switch (bonus)
        {
            case BonusType.None:
                return "なし";

            case BonusType.Big:
                return "BIG BONUS";

            case BonusType.Reg:
                return "REG BONUS";
        }


        return "不明";
    }
}
