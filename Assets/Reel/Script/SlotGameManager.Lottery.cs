using UnityEngine;

public partial class SlotGameManager
{
    private RoleType DrawNormalRole()
    {
        if (chronicleUseTestRole)
        {
            return (RoleType)System.Enum.Parse(typeof(RoleType), chronicleTestRole.ToString());
        }

        float[] w = {
            Mathf.Max(0f, chronicleLoseWeight),
            Mathf.Max(0f, chronicleBellWeight),
            Mathf.Max(0f, chronicleReplayWeight),
            Mathf.Max(0f, chronicleSuikaWeight),
            Mathf.Max(0f, chronicleCherryWeight),
            Mathf.Max(0f, chronicleStrongCherryWeight)
        };
        RoleType[] roles = {
            RoleType.Lose, RoleType.Bell, RoleType.Replay,
            RoleType.Suika, RoleType.Cherry, RoleType.StrongCherry
        };
        float total = 0f;
        foreach (float value in w) total += value;
        if (total <= 0f) { Debug.LogError("通常役の抽選重みが全部0です"); return RoleType.Lose; }
        float draw = Random.value * total;
        for (int i = 0; i < w.Length; i++)
        {
            draw -= w[i];
            if (draw < 0f) return roles[i];
        }
        return RoleType.StrongCherry;
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
            case RoleType.Suika:
                return "スイカ";
            case RoleType.StrongCherry:
                return "強チェリー";

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
            case HitRoleType.Suika:
                return "スイカ";
            case HitRoleType.StrongCherry:
                return "強チェリー";

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
