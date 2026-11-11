using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SlotGameManager
{
    // CSVは表示/調査用。停止候補の評価はリール配列と実際の成立役から行う。
    private static readonly int[] FixedBottoms = { 19, 14, 9, 4 };

    private struct StopTriple
    {
        public string U, C, L;
        public StopTriple(string upper, string center, string lower)
        { U = upper; C = center; L = lower; }
    }

    private static StopTriple[] GetTriples(IList<string> reel)
    {
        if (reel == null || reel.Count != 20) return new StopTriple[0];
        var triples = new StopTriple[20];
        for (int i = 0; i < 20; i++)
            triples[i] = new StopTriple(reel[(i + 19) % 20], reel[i], reel[(i + 1) % 20]);
        return triples;
    }

    private static StopTriple Captured(ReelController reel)
    {
        return new StopTriple(reel.GetUpperSymbol(), reel.GetCenterSymbol(), reel.GetLowerSymbol());
    }

    private static bool AnyLine(string symbol, StopTriple l, StopTriple c, StopTriple r)
    {
        return (l.U == symbol && c.U == symbol && r.U == symbol)
            || (l.C == symbol && c.C == symbol && r.C == symbol)
            || (l.L == symbol && c.L == symbol && r.L == symbol)
            || (l.U == symbol && c.C == symbol && r.L == symbol)
            || (l.L == symbol && c.C == symbol && r.U == symbol);
    }

    private static bool IsCherryLeft(StopTriple l)
    {
        // 青V中下段2連は5番代用。通常Cherryも判定対象。
        return l.L == "Cherry" || (l.C == "BlueV" && l.L == "BlueV");
    }

    private static HitRoleType Classify(StopTriple l, StopTriple c, StopTriple r)
    {
        if (IsCherryLeft(l))
            return r.C == "Replay" ? HitRoleType.Cherry : HitRoleType.StrongCherry;
        if (l.U == "Suika" && c.C == "Suika" && r.L == "Suika")
            return HitRoleType.Suika;
        if (AnyLine("Replay", l, c, r)) return HitRoleType.Replay;
        if (AnyLine("Bell", l, c, r)) return HitRoleType.Bell;
        return HitRoleType.None;
    }

    private HitRoleType WantedHit()
    {
        switch (CurrentRole)
        {
            case RoleType.Bell: return HitRoleType.Bell;
            case RoleType.Replay: return HitRoleType.Replay;
            case RoleType.Suika: return HitRoleType.Suika;
            case RoleType.Cherry: return HitRoleType.Cherry;
            case RoleType.StrongCherry: return HitRoleType.StrongCherry;
            default: return HitRoleType.None;
        }
    }

    private static bool IsWanted(HitRoleType actual, HitRoleType wanted)
    {
        return actual == wanted;
    }

    // 指定した1リールの停止候補が、残りリールのいずれかの停止位置で
    // 成立役に対応する停止目を作れるか探索する。非停止リールの押下タイミングは
    // まだ不明なので、将来の20位置を候補として扱う（保証ではない）。
    public int EvaluateChronicleStop(ReelController.ReelType candidate,
        string upper, string center, string lower)
    {
        StopTriple cur = new StopTriple(upper, center, lower);
        StopTriple[] l = candidate == ReelController.ReelType.Left
            ? new[] { cur } : leftStopped ? new[] { Captured(leftReel) } : GetTriples(ReelMasterLoader.LeftReel);
        StopTriple[] c = candidate == ReelController.ReelType.Center
            ? new[] { cur } : centerStopped ? new[] { Captured(centerReel) } : GetTriples(ReelMasterLoader.CenterReel);
        StopTriple[] r = candidate == ReelController.ReelType.Right
            ? new[] { cur } : rightStopped ? new[] { Captured(rightReel) } : GetTriples(ReelMasterLoader.RightReel);
        if (l.Length == 0 || c.Length == 0 || r.Length == 0) return -1000;

        HitRoleType wanted = WantedHit();
        bool anyWanted = false;
        bool anySafe = false;
        bool anyWrong = false;
        foreach (StopTriple a in l)
            foreach (StopTriple b in c)
                foreach (StopTriple d in r)
                {
                    HitRoleType outcome = Classify(a, b, d);
                    if (IsWanted(outcome, wanted)) anyWanted = true;
                    else if (outcome == HitRoleType.None) anySafe = true;
                    else anyWrong = true;
                }
        // まず成立役の停止形が可能な候補を優先、ハズレ時は安全候補を優先。
        if (anyWanted) return 100;
        if (anySafe) return wanted == HitRoleType.None ? 80 : 10;
        return anyWrong ? -100 : -200;
    }

    private HitRoleType EvaluateFinalRole()
    {
        return Classify(Captured(leftReel), Captured(centerReel), Captured(rightReel));
    }
}
