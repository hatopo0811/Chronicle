using UnityEngine;

public partial class SlotGameManager
{
    // 新6役は実出目と内部役の一致で入賞を確定する。
    private void CheckHitRole()
    {
        HitRoleType actual = EvaluateFinalRole();
        HitRoleType wanted = WantedHit();
        CurrentHitRole = actual == wanted ? actual : HitRoleType.None;
        CurrentHitLine = PayLine.None;

        if (actual != wanted)
        {
            Debug.LogWarning("[クロニクル] 停止形と成立役が不一致：成立="
                + GetRoleName(CurrentRole) + " / 実出目=" + GetHitRoleName(actual)
                + "。払い出しを無効化しました。");
        }
        Debug.Log("[クロニクル] 成立=" + GetRoleName(CurrentRole)
            + " / 入賞=" + GetHitRoleName(CurrentHitRole));
    }

    // ReelController.Stop.cs の旧安全停止フォールバックとの互換用。
    // 新しい停止制御は EvaluateChronicleStop() を使う。
    public bool IsSafeLoseStopCandidate(
        ReelController.ReelType candidate,
        string upper, string center, string lower)
    {
        return EvaluateChronicleStop(candidate, upper, center, lower) > 0;
    }
}
