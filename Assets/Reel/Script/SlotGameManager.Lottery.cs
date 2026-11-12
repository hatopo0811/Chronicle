using UnityEngine;

public partial class SlotGameManager
{
    private RoleType DrawNormalRole()
    {
        // テスト役enumと内部RoleTypeの並び順は異なるため、数値キャストしない。
        if (chronicleUseTestRole)
        {
            switch (chronicleTestRole)
            {
                case ChronicleTestRole.Lose: return RoleType.Lose;
                case ChronicleTestRole.Bell: return chronicleIsAT ? RoleType.ATBell : RoleType.Bell;
                case ChronicleTestRole.ATBell:
                    return chronicleIsAT ? RoleType.ATBell : RoleType.Bell;
                case ChronicleTestRole.Replay: return RoleType.Replay;
                case ChronicleTestRole.Suika: return RoleType.Suika;
                case ChronicleTestRole.Cherry: return RoleType.Cherry;
                case ChronicleTestRole.StrongCherry: return RoleType.StrongCherry;
                default: return RoleType.Lose;
            }
        }
        float[] weights = {
            Mathf.Max(0f, chronicleLoseWeight),
            Mathf.Max(0f, chronicleIsAT ? chronicleATBellWeight : chronicleBellWeight),
            Mathf.Max(0f, chronicleReplayWeight),
            Mathf.Max(0f, chronicleSuikaWeight),
            Mathf.Max(0f, chronicleCherryWeight),
            Mathf.Max(0f, chronicleStrongCherryWeight)
        };
        RoleType[] roles = {
            RoleType.Lose, chronicleIsAT ? RoleType.ATBell : RoleType.Bell, RoleType.Replay,
            RoleType.Suika, RoleType.Cherry, RoleType.StrongCherry
        };
        float total = 0f;
        foreach (float weight in weights) total += weight;
        if (total <= 0f)
        {
            Debug.LogError("[クロニクル] 抽選重みがすべて0。ハズレにします。");
            return RoleType.Lose;
        }
        float draw = Random.value * total;
        for (int i = 0; i < weights.Length; i++)
        {
            draw -= weights[i];
            if (draw < 0f) return roles[i];
        }
        return RoleType.StrongCherry;
    }

    // AT/通常の境界でBellを明示的に正規化する。
    // DrawNormalRoleの今後の変更やテストモードにも対応。
    private RoleType NormalizeChronicleBellRole(RoleType role)
    {
        if (role == RoleType.Bell || role == RoleType.ATBell)
        {
            return IsChronicleAT ? RoleType.ATBell : RoleType.Bell;
        }
        return role;
    }

    private string GetRoleName(RoleType role)
    {
        switch (role)
        {
            case RoleType.Lose: return "ハズレ";
            case RoleType.Bell: return "通常ベル";
            case RoleType.ATBell: return "ATベル";
            case RoleType.Replay: return "リプレイ";
            case RoleType.Suika: return "スイカ";
            case RoleType.Cherry: return "弱チェリー";
            case RoleType.StrongCherry: return "強チェリー";
            default: return "不明";
        }
    }

    private string GetHitRoleName(HitRoleType role)
    {
        switch (role)
        {
            case HitRoleType.None: return "入賞なし";
            case HitRoleType.Bell: return "ベル";
            case HitRoleType.Replay: return "リプレイ";
            case HitRoleType.Suika: return "スイカ";
            case HitRoleType.Cherry: return "弱チェリー";
            case HitRoleType.StrongCherry: return "強チェリー";
            default: return "不明";
        }
    }
}
