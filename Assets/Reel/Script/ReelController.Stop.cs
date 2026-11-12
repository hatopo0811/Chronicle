using UnityEngine;

public partial class ReelController
{
    // =========================================
    // スベリ設定
    // =========================================

    [Header("通常時スベリ設定")]
    [SerializeField]
    private int maxSlipCount = 4;


    [Header("ボーナス確定・疑似遊戯スベリ設定")]
    [SerializeField]
    private int pseudoMaxSlipCount = 20;


    // =========================================
    // 疑似遊戯状態
    // =========================================

    private bool pseudoPlayMode = false;


    private int CurrentMaxSlipCount
    {
        get
        {
            return pseudoPlayMode
                ? pseudoMaxSlipCount
                : maxSlipCount;
        }
    }


    // =========================================
    // 疑似遊戯 ON / OFF
    // =========================================

    public void SetPseudoPlayMode(
        bool enabled
    )
    {
        pseudoPlayMode = enabled;


        Debug.Log(
            GetReelName() +
            " 疑似遊戯=" +
            pseudoPlayMode +
            " / 最大スベリ=" +
            CurrentMaxSlipCount +
            "コマ"
        );
    }


    // =========================================
    // 停止要求
    // =========================================

    // =========================================
    // CHRONICLE: 成立役と停止位置の連動
    // =========================================
    public void StopSpin()
    {
        if (!isSpinning || isStopping) return;

        // 通常/優出は左から。ATベルはナビされた第1停止から。
        if (gameManager != null && !gameManager.CanChronicleStopReel(reelType))
            return;

        if (gameManager != null)
        {
            gameManager.NotifyChronicleStopPressed(reelType);
        }
        else
        {
            Debug.LogWarning(GetReelName() + " のGame Manager参照がNoneです。押し順を記録できません。");
        }

        float remainder = Mathf.Repeat(reelOffset, symbolHeight);
        float distanceToBoundary = remainder < 0.001f
            ? 0f : symbolHeight - remainder;

        float boundaryOffset = Mathf.Repeat(
            reelOffset + distanceToBoundary, reelLength);
        int baseCenterIndex = GetCenterIndexFromOffset(boundaryOffset);

        // 新作の通常6役では、旧OHANA GOGOのターゲット設定より
        // 成立役・4コマ・リール位置に基づく停止選択を優先する。
        if (gameManager != null)
        {
            int chronicleSlip = FindChronicleSlip(baseCenterIndex);
            SetStopDistance(distanceToBoundary, chronicleSlip);

            Debug.Log(GetReelName() +
                " CHRONICLE: " + gameManager.CurrentRole +
                " / " + chronicleSlip + "コマ滑り");
            return;
        }

        // 従来のターゲット方式（ゲームマネージャ参照がない場合のみ）
        if (useReachPattern)
        {
            int slip = FindReachPatternSlip(baseCenterIndex);
            if (slip >= 0)
            {
                SetStopDistance(distanceToBoundary, slip);
                return;
            }
        }
        if (useLoseSafetyFallback)
        {
            int slip = FindSafeLoseSlip(baseCenterIndex);
            if (slip >= 0)
            {
                SetStopDistance(distanceToBoundary, slip);
                return;
            }
        }
        if (reelType == ReelType.Left && targetSymbol == "Cherry")
        {
            int slip = FindCherrySlip(baseCenterIndex);
            if (slip >= 0)
            {
                SetStopDistance(distanceToBoundary, slip);
                return;
            }
        }
        if (!string.IsNullOrEmpty(targetSymbol))
        {
            int slip = FindTargetSlip(baseCenterIndex);
            if (slip >= 0)
            {
                SetStopDistance(distanceToBoundary, slip);
                return;
            }
        }
        SetStopDistance(distanceToBoundary, 0);
    }

    private int FindChronicleSlip(int baseCenterIndex)
    {
        SlotGameManager.RoleType role = gameManager.CurrentRole;
        int max = Mathf.Clamp(maxSlipCount, 0, 4);

        // BellとATBellは、成立役・押し順によって独立した停止テーブルを使用。
        if (role == SlotGameManager.RoleType.Bell ||
            role == SlotGameManager.RoleType.ATBell)
            return FindChronicleBellSlip(baseCenterIndex, role, max);

        int bestSlip = -1;
        int bestScore = int.MinValue;

        for (int slip = 0; slip <= max; slip++)
        {
            int centerIndex = GetIndexAfterSlip(baseCenterIndex, slip);
            int upperIndex = WrapIndex(centerIndex - 1);
            int lowerIndex = WrapIndex(centerIndex + 1);

            string upper = reelData[upperIndex];
            string center = reelData[centerIndex];
            string lower = reelData[lowerIndex];

            // 左リール：通常役は下段1・6・11・16番。
            // チェリー系は下段Cherry（20/15/10）か
            // 下段5番BlueV（中段6番BlueV）のみ。
            if (reelType == ReelType.Left)
            {
                if (role == SlotGameManager.RoleType.Lose ||
                    (role == SlotGameManager.RoleType.Bell ||
                     role == SlotGameManager.RoleType.ATBell) ||
                    role == SlotGameManager.RoleType.Replay)
                {
                    if (!IsChronicleNormalLower(lowerIndex)) continue;
                }
                else if (role == SlotGameManager.RoleType.Cherry ||
                         role == SlotGameManager.RoleType.StrongCherry)
                {
                    if (!IsChronicleCherryLower(centerIndex, lowerIndex))
                        continue;
                }
            }

            // 弱チェリー: 右中段Replay。
            // 強チェリー: 右中段は BAR / BlueV / PinkV のみ。
            if (reelType == ReelType.Right)
            {
                if (role == SlotGameManager.RoleType.Cherry &&
                    center != "Replay") continue;
                if (role == SlotGameManager.RoleType.StrongCherry &&
                    !IsChronicleStrongCherryRight(center)) continue;
            }

            // 強チェリーの中段チェリー風の重複停止形を回避する。
            // 中リール中段は BAR / BlueV / PinkV を禁止。
            if (reelType == ReelType.Center &&
                role == SlotGameManager.RoleType.StrongCherry &&
                IsChronicleStrongCherryRight(center))
            {
                continue;
            }

            int score = gameManager.EvaluateChronicleStop(
                reelType, upper, center, lower);

            // 実機の非公開停止テーブルではなく、今回の配列から
            // 成立役を作りやすい段を優先する設計。
            if ((role == SlotGameManager.RoleType.Bell ||
                 (role == SlotGameManager.RoleType.ATBell && !gameManager.IsATBellOrderFailed)) &&
                reelType != ReelType.Left && upper == "Bell") score += 30;

            if (role == SlotGameManager.RoleType.Replay &&
                reelType != ReelType.Left && center == "Replay") score += 30;

            if (role == SlotGameManager.RoleType.Suika)
            {
                if (reelType == ReelType.Left && upper == "Suika") score += 35;
                if (reelType == ReelType.Center && center == "Suika") score += 35;
                if (reelType == ReelType.Right && lower == "Suika") score += 35;
            }

            // 同点なら押下位置に近い停止を選ぶ。
            score -= slip;
            if (score > bestScore)
            {
                bestScore = score;
                bestSlip = slip;
            }
        }

        if (bestSlip >= 0) return bestSlip;

        // 現在の配列・制約で候補なし（スイカ取りこぼし等）。
        // 不正な5コマ以上の引き込みは行わない。
        Debug.LogWarning(GetReelName() +
            "：成立役" + role + "の停止候補が4コマ以内にありません。" +
            "取りこぼし候補として0コマ停止します。");
        return 0;
    }

    // =========================================
    // ベル専用停止制御
    // 通常: 左上・中中・右下で1枚（右下がり）
    // AT: ファーストナビ別に停止形を選択。
    // L/C first 11枚=左上・中中・右上 / R first 11枚=左下・中中・右下。
    // 不正解1枚=左上・中中・右下（共通）。
    // 第1停止は共通目。第2停止以降は入力順に合わせて分岐。
    // =========================================
    private int FindChronicleBellSlip(
        int baseCenterIndex, SlotGameManager.RoleType role, int max)
    {
        bool atBell = role == SlotGameManager.RoleType.ATBell;
        ReelController.TargetRow row = ReelController.TargetRow.Upper;

        if (atBell)
        {
            // ゲームマネージャは停止入力を事前に記録している。
            if (!gameManager.TryGetChronicleATBellTargetRow(reelType, out row))
            {
                Debug.LogWarning("[クロニクル] ATベル停止目標未設定: " + GetReelName());
            }
        }
        else
        {
            if (reelType == ReelType.Center)
                row = ReelController.TargetRow.Center;
            else if (reelType == ReelType.Right)
                row = ReelController.TargetRow.Lower;
        }

        for (int slip = 0; slip <= max; slip++)
        {
            int middleIndex = GetIndexAfterSlip(baseCenterIndex, slip);
            int upperIndex = WrapIndex(middleIndex - 1);
            int lowerIndex = WrapIndex(middleIndex + 1);
            int targetIndex = row == ReelController.TargetRow.Upper ? upperIndex
                : row == ReelController.TargetRow.Lower ? lowerIndex : middleIndex;
            if (reelData[targetIndex] != "Bell") continue;

            // 通常ベルのみ左下段4位置固定。AT時は上下の分岐を優先。
            if (!atBell && reelType == ReelType.Left &&
                !IsChronicleNormalLower(lowerIndex)) continue;

            Debug.Log("[クロニクル][ベル停止] " + GetReelName()
                + " / 停止段=" + row
                + " / 入力=" + gameManager.PressedATBellOrder
                + " / スベリ=" + slip);
            return slip;
        }

        Debug.LogWarning("[クロニクル] 4コマ以内にベル停止候補なし: "
            + GetReelName() + " / 停止段=" + row);
        return 0;
    }

    private static bool IsChronicleStrongCherryRight(string symbol)
    {
        return symbol == "BAR" || symbol == "BlueV" || symbol == "PinkV";
    }

    private bool IsChronicleNormalLower(int lowerIndex)
    {
        return lowerIndex == 19 || lowerIndex == 14 ||
               lowerIndex == 9 || lowerIndex == 4;
    }

    private bool IsChronicleCherryLower(int centerIndex, int lowerIndex)
    {
        if (reelData[lowerIndex] == "Cherry") return true;
        // 下段5番(index=15)、中段6番(index=14)の2連青V。
        return lowerIndex == 15 && centerIndex == 14 &&
               reelData[lowerIndex] == "BlueV" &&
               reelData[centerIndex] == "BlueV";
    }

    // =========================================
    // 通常ハズレ安全位置検索
    // =========================================

    private int FindSafeLoseSlip(
        int baseCenterIndex
    )
    {
        for (
            int slip = 0;
            slip <= CurrentMaxSlipCount;
            slip++
        )
        {
            int centerIndex =
                GetIndexAfterSlip(
                    baseCenterIndex,
                    slip
                );


            int upperIndex =
                WrapIndex(
                    centerIndex - 1
                );


            int lowerIndex =
                WrapIndex(
                    centerIndex + 1
                );


            string upperSymbol =
                reelData[upperIndex];

            string centerSymbol =
                reelData[centerIndex];

            string lowerSymbol =
                reelData[lowerIndex];


            // =================================
            // 左リールはCherryを必ず避ける
            // =========================================

            if (
                reelType ==
                    ReelType.Left
                &&
                IsCherryVisibleAtCenterIndex(
                    centerIndex
                )
            )
            {
                continue;
            }


            // =================================
            // SlotGameManager側で
            // 他の停止済みリールと合わせて確認
            // =========================================

            if (
                gameManager != null
                &&
                !gameManager.IsSafeLoseStopCandidate(
                    reelType,
                    upperSymbol,
                    centerSymbol,
                    lowerSymbol
                )
            )
            {
                continue;
            }


            return slip;
        }


        return -1;
    }


    // =========================================
    // 通常ターゲット検索
    // =========================================

    private int FindTargetSlip(
        int baseCenterIndex
    )
    {
        for (
            int slip = 0;
            slip <= CurrentMaxSlipCount;
            slip++
        )
        {
            int centerIndex =
                GetIndexAfterSlip(
                    baseCenterIndex,
                    slip
                );


            if (
                targetRow ==
                TargetRow.Any
            )
            {
                int upperIndex =
                    WrapIndex(
                        centerIndex - 1
                    );


                int lowerIndex =
                    WrapIndex(
                        centerIndex + 1
                    );


                bool found =
                    reelData[upperIndex] ==
                        targetSymbol
                    ||
                    reelData[centerIndex] ==
                        targetSymbol
                    ||
                    reelData[lowerIndex] ==
                        targetSymbol;


                if (!found)
                {
                    continue;
                }
            }
            else
            {
                int targetIndex =
                    GetIndexForRow(
                        centerIndex,
                        targetRow
                    );


                if (
                    reelData[targetIndex] !=
                    targetSymbol
                )
                {
                    continue;
                }
            }


            if (
                reelType ==
                    ReelType.Left
                &&
                targetSymbol !=
                    "Cherry"
            )
            {
                if (
                    IsCherryVisibleAtCenterIndex(
                        centerIndex
                    )
                )
                {
                    continue;
                }
            }


            return slip;
        }


        return -1;
    }


    // =========================================
    // 段 → Index
    // =========================================

    private int GetIndexForRow(
        int centerIndex,
        TargetRow row
    )
    {
        switch (row)
        {
            case TargetRow.Upper:

                return
                    WrapIndex(
                        centerIndex - 1
                    );


            case TargetRow.Center:

                return centerIndex;


            case TargetRow.Lower:

                return
                    WrapIndex(
                        centerIndex + 1
                    );


            case TargetRow.Any:

                return centerIndex;
        }


        return centerIndex;
    }


    // =========================================
    // 停止距離設定
    // =========================================

    private void SetStopDistance(
        float distanceToBoundary,
        int slip
    )
    {
        stopRemainingDistance =
            distanceToBoundary
            +
            (
                slip *
                symbolHeight
            );


        isStopping =
            true;
    }


    // =========================================
    // Cherry引き込み
    // =========================================

    private int FindCherrySlip(
        int baseCenterIndex
    )
    {
        for (
            int slip = 0;
            slip <= CurrentMaxSlipCount;
            slip++
        )
        {
            int centerIndex =
                GetIndexAfterSlip(
                    baseCenterIndex,
                    slip
                );


            if (
                IsCherryVisibleAtCenterIndex(
                    centerIndex
                )
            )
            {
                return slip;
            }
        }


        return -1;
    }


    // =========================================
    // Cherry回避
    // =========================================

    private int FindNonCherrySlip(
        int baseCenterIndex
    )
    {
        for (
            int slip = 0;
            slip <= CurrentMaxSlipCount;
            slip++
        )
        {
            int centerIndex =
                GetIndexAfterSlip(
                    baseCenterIndex,
                    slip
                );


            if (
                !IsCherryVisibleAtCenterIndex(
                    centerIndex
                )
            )
            {
                return slip;
            }
        }


        return -1;
    }


    // =========================================
    // 指定位置でCherryが見えるか
    // =========================================

    private bool IsCherryVisibleAtCenterIndex(
        int centerIndex
    )
    {
        int upperIndex =
            WrapIndex(
                centerIndex - 1
            );


        int lowerIndex =
            WrapIndex(
                centerIndex + 1
            );


        return
            reelData[upperIndex] ==
                "Cherry"
            ||
            reelData[centerIndex] ==
                "Cherry"
            ||
            reelData[lowerIndex] ==
                "Cherry";
    }



    // 20→1のCSV順では、下段5番は0-based index=15。
    // 下段が5番のとき中段が6番になり、2連青V停止となる。
    // ゲームマネージャ側のCherry成立時だけ有効にする。
    private bool ShouldUseCherrySubstitute()
    {
        return reelType == ReelType.Left
            && gameManager != null
            && gameManager.CurrentRole == SlotGameManager.RoleType.Cherry
            && targetSymbol == "Cherry"
            && !useReachPattern;
    }

    private int FindCherrySubstituteSlip(int baseCenterIndex)
    {
        const int blueVFiveLowerIndex = 15;
        int limit = Mathf.Min(CurrentMaxSlipCount, 4);
        for (int slip = 0; slip <= limit; slip++)
        {
            int centerIndex = GetIndexAfterSlip(baseCenterIndex, slip);
            int lowerIndex = WrapIndex(centerIndex + 1);
            if (lowerIndex == blueVFiveLowerIndex
                && reelData[lowerIndex] == "BlueV"
                && reelData[centerIndex] == "BlueV")
            {
                return slip;
            }
        }
        return -1;
    }

    // 通常役のみ左下段を固定。レア役やAT専用演出は対象外。
    private bool ShouldLockLeftLowerPosition()
    {
        if (reelType != ReelType.Left || gameManager == null)
            return false;

        SlotGameManager.RoleType role = gameManager.CurrentRole;
        return role == SlotGameManager.RoleType.Lose
            || role == SlotGameManager.RoleType.Bell
            || role == SlotGameManager.RoleType.ATBell
            || role == SlotGameManager.RoleType.Replay;
    }

    // ReelMaster.csvが20→1順のため、0-based配列の下段indexは
    // 1番=19、6番=14、11番=9、16番=4。
    private int FindFixedLeftLowerSlip(int baseCenterIndex)
    {
        for (int slip = 0; slip <= maxSlipCount; slip++)
        {
            int centerIndex = GetIndexAfterSlip(baseCenterIndex, slip);
            int lowerIndex = WrapIndex(centerIndex + 1);
            if (lowerIndex == 19 || lowerIndex == 14 ||
                lowerIndex == 9 || lowerIndex == 4)
                return slip;
        }
        return -1;
    }

    // =========================================
    // スベリ後Index
    // =========================================

    private int GetIndexAfterSlip(
        int startIndex,
        int slip
    )
    {
        return
            WrapIndex(
                startIndex -
                slip
            );
    }
}