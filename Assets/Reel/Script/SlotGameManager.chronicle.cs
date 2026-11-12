using System;
using System.Collections.Generic;
using UnityEngine;

// 閃撃クロニクル：停止候補スコアと実際の停止役判定。
// 他のSlotGameManager partialファイルに追加する（同名重複に注意）。
public partial class SlotGameManager
{
    private struct StopTriple
    {
        public string U, C, L;
        public StopTriple(string upper, string center, string lower)
        { U = upper; C = center; L = lower; }
    }

    private static StopTriple[] GetTriples(IList<string> reel)
    {
        if (reel == null || reel.Count != 20) return new StopTriple[0];
        StopTriple[] triples = new StopTriple[20];
        for (int i = 0; i < 20; i++)
        {
            triples[i] = new StopTriple(
                reel[(i + 19) % 20], reel[i], reel[(i + 1) % 20]);
        }
        return triples;
    }

    private static StopTriple Captured(ReelController reel)
    {
        return new StopTriple(
            reel.GetUpperSymbol(), reel.GetCenterSymbol(), reel.GetLowerSymbol());
    }

    private static bool AnyLine(
        string symbol, StopTriple l, StopTriple c, StopTriple r)
    {
        return (l.U == symbol && c.U == symbol && r.U == symbol)
            || (l.C == symbol && c.C == symbol && r.C == symbol)
            || (l.L == symbol && c.L == symbol && r.L == symbol)
            || (l.U == symbol && c.C == symbol && r.L == symbol)
            || (l.L == symbol && c.C == symbol && r.U == symbol);
    }

    private static bool IsCherryLeft(StopTriple l)
    {
        // 通常の左下段Cherryと5番下段+6番中段BlueVだけ。
        return l.L == "Cherry" ||
               (l.C == "BlueV" && l.L == "BlueV");
    }

    private HitRoleType Classify(
        StopTriple l, StopTriple c, StopTriple r)
    {
        if (IsCherryLeft(l))
        {
            // 弱チェリーは右中段Replay。
            if (r.C == "Replay") return HitRoleType.Cherry;

            // 強チェリーは右中段BAR/BlueV/PinkV、かつ
            // 中リール中段がその3図柄ではない停止形のみ認める。
            bool strongRight = r.C == "BAR" ||
                               r.C == "BlueV" || r.C == "PinkV";
            bool centerConflict = c.C == "BAR" ||
                                  c.C == "BlueV" || c.C == "PinkV";
            if (strongRight && !centerConflict)
                return HitRoleType.StrongCherry;

            // 停止候補が条件を満たさない場合は強チェリー扱いにしない。
            return HitRoleType.None;
        }

        if (l.U == "Suika" && c.C == "Suika" && r.L == "Suika")
            return HitRoleType.Suika;

        // ATベルは第一停止別の入賞形として独立検出。
        if (CurrentRole == RoleType.ATBell)
            return (IsElevenBell(l, c, r) || IsOneBell(l, c, r))
                ? HitRoleType.Bell : HitRoleType.None;
        if (CurrentRole == RoleType.Bell)
            return IsOneBell(l, c, r) ? HitRoleType.Bell : HitRoleType.None;

        if (AnyLine("Replay", l, c, r)) return HitRoleType.Replay;
        // 他役でベルが偶然揃っても小役の誤入賞を防ぐ。
        if (IsOneBell(l, c, r) || IsElevenBell(l, c, r))
            return HitRoleType.Bell;
        return HitRoleType.None;
    }

    private HitRoleType WantedHit()
    {
        switch (CurrentRole)
        {
            case RoleType.Bell: return HitRoleType.Bell;
            case RoleType.ATBell: return HitRoleType.Bell;
            case RoleType.Replay: return HitRoleType.Replay;
            case RoleType.Suika: return HitRoleType.Suika;
            case RoleType.Cherry: return HitRoleType.Cherry;
            case RoleType.StrongCherry: return HitRoleType.StrongCherry;
            default: return HitRoleType.None;
        }
    }

    // 完成した出目があるか、まだ停止していないリールに
    // 可能性が残っているかを評価する。未停止リールの将来の
    // ボタン押下タイミングは不明なので入賞保証ではない。
    public int EvaluateChronicleStop(
        ReelController.ReelType candidate,
        string upper, string center, string lower)
    {
        StopTriple cur = new StopTriple(upper, center, lower);
        StopTriple[] ls = candidate == ReelController.ReelType.Left
            ? new[] { cur }
            : leftStopped ? new[] { Captured(leftReel) }
            : GetTriples(ReelMasterLoader.LeftReel);
        StopTriple[] cs = candidate == ReelController.ReelType.Center
            ? new[] { cur }
            : centerStopped ? new[] { Captured(centerReel) }
            : GetTriples(ReelMasterLoader.CenterReel);
        StopTriple[] rs = candidate == ReelController.ReelType.Right
            ? new[] { cur }
            : rightStopped ? new[] { Captured(rightReel) }
            : GetTriples(ReelMasterLoader.RightReel);

        if (ls.Length == 0 || cs.Length == 0 || rs.Length == 0)
            return -1000;

        HitRoleType wanted = WantedHit();
        bool anyWanted = false;
        bool anySafe = false;
        bool anyWrong = false;

        foreach (StopTriple l in ls)
            foreach (StopTriple c in cs)
                foreach (StopTriple r in rs)
                {
                    HitRoleType actual = Classify(l, c, r);
                    if (actual == wanted) anyWanted = true;
                    else if (actual == HitRoleType.None) anySafe = true;
                    else anyWrong = true;
                }

        if (anyWanted) return 100;
        if (anySafe) return wanted == HitRoleType.None ? 80 : 10;
        return anyWrong ? -100 : -200;
    }

    // ユーザー指定の各第一停止ごとの停止形を厳密判定。
    // 11枚：左/中ファースト LU・CC・RU / 右ファースト LL・CC・RL
    //  1枚：全ファースト LU・CC・RL
    // 通常ベルも LU・CC・RL を使用。
    private bool IsElevenBell(StopTriple l, StopTriple c, StopTriple r)
    {
        if (CurrentRole != RoleType.ATBell ||
            CurrentATBellFirstStopNavigation.Length != 1) return false;

        if (CurrentATBellFirstStopNavigation == "R")
            return l.L == "Bell" && c.C == "Bell" && r.L == "Bell";
        return l.U == "Bell" && c.C == "Bell" && r.U == "Bell";
    }

    private bool IsOneBell(StopTriple l, StopTriple c, StopTriple r)
    {
        return l.U == "Bell" && c.C == "Bell" && r.L == "Bell";
    }

    public bool IsChronicleATBellFullStop()
    {
        return IsElevenBell(Captured(leftReel), Captured(centerReel), Captured(rightReel));
    }

    public bool IsChronicleNormalBellStop()
    {
        return IsOneBell(Captured(leftReel), Captured(centerReel), Captured(rightReel));
    }

    public string CurrentChronicleBellStopName()
    {
        StopTriple l = Captured(leftReel);
        StopTriple c = Captured(centerReel);
        StopTriple r = Captured(rightReel);
        if (IsElevenBell(l, c, r))
            return CurrentATBellFirstStopNavigation == "R"
                ? "11枚ベル(左下・中中・右下)"
                : "11枚ベル(左上・中中・右上)";
        if (IsOneBell(l, c, r)) return "1枚ベル(左上・中中・右下)";
        return "非入賞";
    }

    private HitRoleType EvaluateFinalRole()
    {
        return Classify(
            Captured(leftReel), Captured(centerReel), Captured(rightReel));
    }
}
