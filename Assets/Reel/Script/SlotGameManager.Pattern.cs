using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SlotGameManager
{
    private void SelectLosePatternForThisGame()
    {
        List<
            HitPatternMasterLoader.HitPatternData
        > losePatterns =
            HitPatternMasterLoader
                .GetPatternsByResult(
                    "Lose"
                );


        if (
            losePatterns == null ||
            losePatterns.Count == 0
        )
        {
            Debug.LogWarning(
                "HitPatternMaster.csvに " +
                "Result=Lose がありません。"
            );

            return;
        }


        List<
            HitPatternMasterLoader.HitPatternData
        > safePatterns =
            new List<
                HitPatternMasterLoader.HitPatternData
            >();


        foreach (
            HitPatternMasterLoader.HitPatternData
            losePattern in losePatterns
        )
        {
            if (
                IsSafeLosePattern(
                    losePattern
                )
            )
            {
                safePatterns.Add(
                    losePattern
                );
            }

            else
            {
                Debug.LogWarning(
                    "危険なLose目を除外：" +
                    losePattern.ID +
                    " / " +
                    losePattern.Name
                );
            }
        }


        if (
            safePatterns.Count == 0
        )
        {
            Debug.LogError(
                "安全なLose出目がありません。"
            );

            return;
        }


        int index =
            UnityEngine.Random.Range(
                0,
                safePatterns.Count
            );


        selectedLosePattern =
            safePatterns[index];


        Debug.Log(
            "★ ハズレ目停止制御：" +
            selectedLosePattern.ID +
            " / " +
            selectedLosePattern.Name
        );
    }

    private bool IsSafeLosePattern(
        HitPatternMasterLoader.HitPatternData
            losePattern
    )
    {
        foreach (
            HitPatternMasterLoader.HitPatternData
            otherPattern
            in HitPatternMasterLoader.Patterns
        )
        {
            if (
                !IsForbiddenNormalLoseResult(
                    otherPattern.Result
                )
            )
            {
                continue;
            }


            if (
                CanPatternsOverlap(
                    losePattern,
                    otherPattern
                )
            )
            {
                return false;
            }
        }


        return true;
    }

    private bool IsForbiddenNormalLoseResult(
        string result
    )
    {
        return
            string.Equals(
                result,
                "Grape",
                StringComparison.OrdinalIgnoreCase
            )
            ||
            string.Equals(
                result,
                "Replay",
                StringComparison.OrdinalIgnoreCase
            )
            ||
            string.Equals(
                result,
                "Bell",
                StringComparison.OrdinalIgnoreCase
            )
            ||
            string.Equals(
                result,
                "Cherry",
                StringComparison.OrdinalIgnoreCase
            )
            ||
            string.Equals(
                result,
                "BigBonus",
                StringComparison.OrdinalIgnoreCase
            )
            ||
            string.Equals(
                result,
                "RegBonus",
                StringComparison.OrdinalIgnoreCase
            )
            ||
            string.Equals(
                result,
                "Hibiscus",
                StringComparison.OrdinalIgnoreCase
            )
            ||
            IsReachResult(
                result
            );
    }

    private bool CanPatternsOverlap(
        HitPatternMasterLoader.HitPatternData a,
        HitPatternMasterLoader.HitPatternData b
    )
    {
        return
            CellsCompatible(
                a.LU,
                b.LU
            )
            &&
            CellsCompatible(
                a.LC,
                b.LC
            )
            &&
            CellsCompatible(
                a.LL,
                b.LL
            )
            &&
            CellsCompatible(
                a.CU,
                b.CU
            )
            &&
            CellsCompatible(
                a.CC,
                b.CC
            )
            &&
            CellsCompatible(
                a.CL,
                b.CL
            )
            &&
            CellsCompatible(
                a.RU,
                b.RU
            )
            &&
            CellsCompatible(
                a.RC,
                b.RC
            )
            &&
            CellsCompatible(
                a.RL,
                b.RL
            );
    }

    private bool CellsCompatible(
        string a,
        string b
    )
    {
        if (
            IsWildcard(a) ||
            IsWildcard(b)
        )
        {
            return true;
        }


        return
            string.Equals(
                a,
                b,
                StringComparison.OrdinalIgnoreCase
            );
    }

    // =========================================
    // Reach系Result判定
    //
    // Reach / ReachCommon / ReachBig /
    // ReachReg などをまとめてReach扱いにする
    // =========================================

    private bool IsReachResult(
        string result
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                result
            )
        )
        {
            return false;
        }


        return
            result.StartsWith(
                "Reach",
                StringComparison.OrdinalIgnoreCase
            );
    }


    // =========================================
    // ★ 通常ハズレ時の自動安全停止判定
    //
    // ReelControllerから、
    // 「この3コマで止めても大丈夫？」
    // と問い合わせるための処理。
    //
    // すでに停止済みのリール +
    // 今回止めようとしているリールを使って、
    // 小役 / ボーナス / Reachが完成する候補を除外する。
    // =========================================

    public bool IsSafeLoseStopCandidate(
        ReelController.ReelType candidateReel,
        string upper,
        string center,
        string lower
    )
    {
        // 通常ハズレ専用。
        // ボーナス持ち越し中は777 / 77BARを狙うため、
        // この安全停止は使わない。
        if (
            CurrentRole !=
                RoleType.Lose
            ||
            PendingBonus !=
                BonusType.None
        )
        {
            return true;
        }


        bool leftKnown =
            leftStopped ||
            candidateReel ==
                ReelController.ReelType.Left;

        bool centerKnown =
            centerStopped ||
            candidateReel ==
                ReelController.ReelType.Center;

        bool rightKnown =
            rightStopped ||
            candidateReel ==
                ReelController.ReelType.Right;


        string lu = null;
        string lc = null;
        string ll = null;

        string cu = null;
        string cc = null;
        string cl = null;

        string ru = null;
        string rc = null;
        string rl = null;


        // =====================================
        // 左
        // =========================================

        if (
            candidateReel ==
            ReelController.ReelType.Left
        )
        {
            lu = upper;
            lc = center;
            ll = lower;
        }
        else if (leftStopped)
        {
            lu =
                leftReel.GetUpperSymbol();

            lc =
                leftReel.GetCenterSymbol();

            ll =
                leftReel.GetLowerSymbol();
        }


        // =====================================
        // 中
        // =========================================

        if (
            candidateReel ==
            ReelController.ReelType.Center
        )
        {
            cu = upper;
            cc = center;
            cl = lower;
        }
        else if (centerStopped)
        {
            cu =
                centerReel.GetUpperSymbol();

            cc =
                centerReel.GetCenterSymbol();

            cl =
                centerReel.GetLowerSymbol();
        }


        // =====================================
        // 右
        // =========================================

        if (
            candidateReel ==
            ReelController.ReelType.Right
        )
        {
            ru = upper;
            rc = center;
            rl = lower;
        }
        else if (rightStopped)
        {
            ru =
                rightReel.GetUpperSymbol();

            rc =
                rightReel.GetCenterSymbol();

            rl =
                rightReel.GetLowerSymbol();
        }


        // =====================================
        // 危険パターン検索
        // =========================================

        foreach (
            HitPatternMasterLoader.HitPatternData
            pattern in HitPatternMasterLoader.Patterns
        )
        {
            if (
                !IsForbiddenNormalLoseResult(
                    pattern.Result
                )
            )
            {
                continue;
            }


            if (
                PatternAlreadyMatchesKnownReels(
                    pattern,

                    leftKnown,
                    lu,
                    lc,
                    ll,

                    centerKnown,
                    cu,
                    cc,
                    cl,

                    rightKnown,
                    ru,
                    rc,
                    rl
                )
            )
            {
                Debug.Log(
                    "通常ハズレ安全停止：除外 → " +
                    pattern.ID +
                    " / " +
                    pattern.Name
                );


                return false;
            }
        }


        return true;
    }


    // =========================================
    // 既知の停止リールだけで
    // 禁止パターンが完成しているか
    // =========================================

    private bool PatternAlreadyMatchesKnownReels(
        HitPatternMasterLoader.HitPatternData pattern,

        bool leftKnown,
        string lu,
        string lc,
        string ll,

        bool centerKnown,
        string cu,
        string cc,
        string cl,

        bool rightKnown,
        string ru,
        string rc,
        string rl
    )
    {
        return
            KnownCellMatches(
                pattern.LU,
                lu,
                leftKnown
            )
            &&
            KnownCellMatches(
                pattern.LC,
                lc,
                leftKnown
            )
            &&
            KnownCellMatches(
                pattern.LL,
                ll,
                leftKnown
            )
            &&
            KnownCellMatches(
                pattern.CU,
                cu,
                centerKnown
            )
            &&
            KnownCellMatches(
                pattern.CC,
                cc,
                centerKnown
            )
            &&
            KnownCellMatches(
                pattern.CL,
                cl,
                centerKnown
            )
            &&
            KnownCellMatches(
                pattern.RU,
                ru,
                rightKnown
            )
            &&
            KnownCellMatches(
                pattern.RC,
                rc,
                rightKnown
            )
            &&
            KnownCellMatches(
                pattern.RL,
                rl,
                rightKnown
            );
    }


    // =========================================
    // 既知セルとの一致
    //
    // まだ止まっていないリールの場合は、
    // そのセルが "*" の時だけ
    // 「すでにパターン成立」とみなせる。
    // =========================================

    private bool KnownCellMatches(
        string patternSymbol,
        string actualSymbol,
        bool reelKnown
    )
    {
        if (!reelKnown)
        {
            return
                IsWildcard(
                    patternSymbol
                );
        }


        if (
            IsWildcard(
                patternSymbol
            )
        )
        {
            return true;
        }


        return
            string.Equals(
                patternSymbol,
                actualSymbol,
                StringComparison.OrdinalIgnoreCase
            );
    }


    private void SelectReachPatternForThisGame()
    {
        selectedReachPattern =
            HitPatternMasterLoader
                .GetRandomPatternByResult(
                    "Reach"
                );


        if (
            selectedReachPattern ==
            null
        )
        {
            Debug.LogWarning(
                "Result=Reach がありません。"
            );

            return;
        }


        Debug.Log(
            "★ リーチ目停止制御：" +
            selectedReachPattern.ID +
            " / " +
            selectedReachPattern.Name
        );
    }

    private void CheckHitRole()
    {
        CurrentHitRole =
            HitRoleType.None;


        CurrentHitLine =
            PayLine.None;


        currentHitPattern =
            null;

        currentReachPattern =
            null;

        currentLosePattern =
            null;


        string lu =
            leftReel.GetUpperSymbol();

        string lc =
            leftReel.GetCenterSymbol();

        string ll =
            leftReel.GetLowerSymbol();


        string cu =
            centerReel.GetUpperSymbol();

        string cc =
            centerReel.GetCenterSymbol();

        string cl =
            centerReel.GetLowerSymbol();


        string ru =
            rightReel.GetUpperSymbol();

        string rc =
            rightReel.GetCenterSymbol();

        string rl =
            rightReel.GetLowerSymbol();


        List<
            HitPatternMasterLoader.HitPatternData
        > matches =
            HitPatternMasterLoader
                .FindAllPatterns(
                    lu,
                    lc,
                    ll,
                    cu,
                    cc,
                    cl,
                    ru,
                    rc,
                    rl
                );


        // =====================================
        // Lose
        // =========================================

        foreach (
            HitPatternMasterLoader.HitPatternData
            pattern in matches
        )
        {
            if (
                string.Equals(
                    pattern.Result,
                    "Lose",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                currentLosePattern =
                    pattern;

                break;
            }
        }


        // =====================================
        // 入賞役
        // =========================================

        foreach (
            HitPatternMasterLoader.HitPatternData
            pattern in matches
        )
        {
            HitRoleType role;


            if (
                !TryConvertResultToHitRole(
                    pattern.Result,
                    out role
                )
            )
            {
                continue;
            }


            if (
                !IsHitAllowedByCurrentState(
                    role
                )
            )
            {
                continue;
            }


            CurrentHitRole =
                role;


            currentHitPattern =
                pattern;


            CurrentHitLine =
                DetectStandardPayLine(
                    pattern
                );


            break;
        }


        // =====================================
        // Reach
        // =========================================

        if (
            PendingBonus !=
            BonusType.None
        )
        {
            foreach (
                HitPatternMasterLoader.HitPatternData
                pattern in matches
            )
            {
                if (
                    IsReachResult(
                        pattern.Result
                    )
                )
                {
                    currentReachPattern =
                        pattern;


                    reachShownForCurrentBonus =
                        true;


                    // ★ リーチ目演出音
                    if (slotAudio != null)
                    {
                        slotAudio
                            .PlayReachSound();
                    }


                    break;
                }
            }
        }


        ShowHitResult();
    }

    private bool TryConvertResultToHitRole(
        string result,
        out HitRoleType role
    )
    {
        role =
            HitRoleType.None;


        if (
            string.Equals(
                result,
                "Grape",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            role =
                HitRoleType.Grape;

            return true;
        }


        if (
            string.Equals(
                result,
                "Replay",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            role =
                HitRoleType.Replay;

            return true;
        }


        if (
            string.Equals(
                result,
                "Bell",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            role =
                HitRoleType.Bell;

            return true;
        }


        if (
            string.Equals(
                result,
                "Cherry",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            role =
                HitRoleType.Cherry;

            return true;
        }




        if (
            string.Equals(
                result,
                "Hibiscus",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            role =
                HitRoleType.Hibiscus;

            return true;
        }

        if (
            string.Equals(
                result,
                "BigBonus",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            role =
                HitRoleType.BigBonus;

            return true;
        }


        if (
            string.Equals(
                result,
                "RegBonus",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            role =
                HitRoleType.RegBonus;

            return true;
        }


        return false;
    }

    private bool IsHitAllowedByCurrentState(
        HitRoleType role
    )
    {
        if (
            ActiveBonus !=
            BonusType.None
        )
        {
            return
                role ==
                HitRoleType.Grape;
        }


        switch (role)
        {
            case HitRoleType.Grape:

                return
                    CurrentRole ==
                    RoleType.Grape;


            case HitRoleType.Replay:

                return
                    CurrentRole ==
                    RoleType.Replay;


            case HitRoleType.Bell:

                return
                    CurrentRole ==
                    RoleType.Bell;


            case HitRoleType.Cherry:

                return
                    CurrentRole ==
                    RoleType.Cherry;




            case HitRoleType.Hibiscus:

                return
                    CurrentRole ==
                    RoleType.Hibiscus;

            case HitRoleType.BigBonus:

                return
                    PendingBonus ==
                        BonusType.Big
                    &&
                    (
                        CurrentRole ==
                            RoleType.BigBonus
                        ||
                        CurrentRole ==
                            RoleType.Lose
                    );


            case HitRoleType.RegBonus:

                return
                    PendingBonus ==
                        BonusType.Reg
                    &&
                    (
                        CurrentRole ==
                            RoleType.RegBonus
                        ||
                        CurrentRole ==
                            RoleType.Lose
                    );
        }


        return false;
    }

    private PayLine DetectStandardPayLine(
        HitPatternMasterLoader.HitPatternData
            pattern
    )
    {
        // 上段
        if (
            IsSpecified(pattern.LU) &&
            IsSpecified(pattern.CU) &&
            IsSpecified(pattern.RU) &&

            IsWildcard(pattern.LC) &&
            IsWildcard(pattern.LL) &&
            IsWildcard(pattern.CC) &&
            IsWildcard(pattern.CL) &&
            IsWildcard(pattern.RC) &&
            IsWildcard(pattern.RL)
        )
        {
            return PayLine.Upper;
        }


        // 中段
        if (
            IsSpecified(pattern.LC) &&
            IsSpecified(pattern.CC) &&
            IsSpecified(pattern.RC) &&

            IsWildcard(pattern.LU) &&
            IsWildcard(pattern.LL) &&
            IsWildcard(pattern.CU) &&
            IsWildcard(pattern.CL) &&
            IsWildcard(pattern.RU) &&
            IsWildcard(pattern.RL)
        )
        {
            return PayLine.Center;
        }


        // 下段
        if (
            IsSpecified(pattern.LL) &&
            IsSpecified(pattern.CL) &&
            IsSpecified(pattern.RL) &&

            IsWildcard(pattern.LU) &&
            IsWildcard(pattern.LC) &&
            IsWildcard(pattern.CU) &&
            IsWildcard(pattern.CC) &&
            IsWildcard(pattern.RU) &&
            IsWildcard(pattern.RC)
        )
        {
            return PayLine.Lower;
        }


        // 右下がり
        if (
            IsSpecified(pattern.LU) &&
            IsSpecified(pattern.CC) &&
            IsSpecified(pattern.RL)
        )
        {
            return PayLine.DiagonalDown;
        }


        // 右上がり
        if (
            IsSpecified(pattern.LL) &&
            IsSpecified(pattern.CC) &&
            IsSpecified(pattern.RU)
        )
        {
            return PayLine.DiagonalUp;
        }


        return PayLine.None;
    }

    private bool IsWildcard(
        string value
    )
    {
        return
            string.IsNullOrWhiteSpace(
                value
            )
            ||
            value == "*";
    }

    private bool IsSpecified(
        string value
    )
    {
        return
            !IsWildcard(
                value
            );
    }

    private void ShowHitResult()
    {
        Debug.Log(
            "========== 入賞判定 =========="
        );


        Debug.Log(
            "内部成立：" +
            GetRoleName(
                CurrentRole
            )
        );


        Debug.Log(
            "実際の入賞：" +
            GetHitRoleName(
                CurrentHitRole
            )
        );


        if (
            currentLosePattern !=
            null
        )
        {
            Debug.Log(
                "★ Loseマスター一致：" +
                currentLosePattern.ID +
                " / " +
                currentLosePattern.Name
            );
        }


        if (
            currentHitPattern !=
            null
        )
        {
            Debug.Log(
                "HitPattern：" +
                currentHitPattern.ID +
                " / " +
                currentHitPattern.Name
            );
        }


        if (
            currentReachPattern !=
            null
        )
        {
            Debug.Log(
                "★★★★★★★★★★★★★★★★"
            );


            Debug.Log(
                "★ リーチ目出現！"
            );


            Debug.Log(
                currentReachPattern.ID +
                " / " +
                currentReachPattern.Name
            );


            Debug.Log(
                "★★★★★★★★★★★★★★★★"
            );
        }


        Debug.Log(
            "=============================="
        );
    }
}
