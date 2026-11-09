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

    public void StopSpin()
    {
        if (!isSpinning)
        {
            return;
        }


        if (isStopping)
        {
            return;
        }


        // =====================================
        // 次の図柄境界までの距離
        // =========================================

        float remainder =
            Mathf.Repeat(
                reelOffset,
                symbolHeight
            );


        float distanceToBoundary;


        if (
            remainder <
            0.001f
        )
        {
            distanceToBoundary = 0f;
        }
        else
        {
            distanceToBoundary =
                symbolHeight -
                remainder;
        }


        float boundaryOffset =
            reelOffset +
            distanceToBoundary;


        boundaryOffset =
            Mathf.Repeat(
                boundaryOffset,
                reelLength
            );


        int baseCenterIndex =
            GetCenterIndexFromOffset(
                boundaryOffset
            );


        // =====================================
        // ① マスターパターン停止制御
        // =========================================

        if (useReachPattern)
        {
            int masterSlip =
                FindReachPatternSlip(
                    baseCenterIndex
                );


            if (masterSlip >= 0)
            {
                SetStopDistance(
                    distanceToBoundary,
                    masterSlip
                );


                Debug.Log(
                    GetReelName() +
                    " マスター出目位置へ停止 → " +
                    masterSlip +
                    "コマ滑り"
                );


                return;
            }


            // =================================
            // Loseマスター失敗時
            // 自動安全停止へフォールバック
            // =========================================

            if (useLoseSafetyFallback)
            {
                int safeSlip =
                    FindSafeLoseSlip(
                        baseCenterIndex
                    );


                if (safeSlip >= 0)
                {
                    SetStopDistance(
                        distanceToBoundary,
                        safeSlip
                    );


                    Debug.Log(
                        GetReelName() +
                        "：Loseマスターを作れないため" +
                        "安全ハズレ位置へ停止 → " +
                        safeSlip +
                        "コマ滑り"
                    );


                    return;
                }
            }


            SetStopDistance(
                distanceToBoundary,
                0
            );


            Debug.LogWarning(
                GetReelName() +
                "：マスター出目条件を" +
                CurrentMaxSlipCount +
                "コマ以内で作れず、" +
                "安全候補も見つかりませんでした。"
            );


            return;
        }


        // =====================================
        // ② Lose自動安全停止のみ
        // =========================================

        if (useLoseSafetyFallback)
        {
            int safeSlip =
                FindSafeLoseSlip(
                    baseCenterIndex
                );


            if (safeSlip >= 0)
            {
                SetStopDistance(
                    distanceToBoundary,
                    safeSlip
                );


                Debug.Log(
                    GetReelName() +
                    "：安全ハズレ位置へ停止 → " +
                    safeSlip +
                    "コマ滑り"
                );


                return;
            }


            SetStopDistance(
                distanceToBoundary,
                0
            );


            Debug.LogWarning(
                GetReelName() +
                "：0～" +
                CurrentMaxSlipCount +
                "コマに安全なハズレ候補がありません。"
            );


            return;
        }


        // =====================================
        // ③ Cherry成立
        // =========================================

        if (
            reelType ==
                ReelType.Left
            &&
            targetSymbol ==
                "Cherry"
        )
        {
            int cherrySlip =
                FindCherrySlip(
                    baseCenterIndex
                );


            if (cherrySlip >= 0)
            {
                SetStopDistance(
                    distanceToBoundary,
                    cherrySlip
                );


                Debug.Log(
                    "左リール Cherry引き込み → " +
                    cherrySlip +
                    "コマ滑り"
                );


                return;
            }


            SetStopDistance(
                distanceToBoundary,
                0
            );


            Debug.Log(
                "左リール Cherryを" +
                CurrentMaxSlipCount +
                "コマ以内で引き込めません"
            );


            return;
        }


        // =====================================
        // ④ ターゲットなし
        // =========================================

        if (
            string.IsNullOrEmpty(
                targetSymbol
            )
        )
        {
            if (
                reelType ==
                ReelType.Left
            )
            {
                int safeSlip =
                    FindNonCherrySlip(
                        baseCenterIndex
                    );


                if (safeSlip >= 0)
                {
                    SetStopDistance(
                        distanceToBoundary,
                        safeSlip
                    );


                    return;
                }
            }


            SetStopDistance(
                distanceToBoundary,
                0
            );


            return;
        }


        // =====================================
        // ⑤ 通常ターゲット
        // =========================================

        int selectedSlip =
            FindTargetSlip(
                baseCenterIndex
            );


        if (selectedSlip >= 0)
        {
            SetStopDistance(
                distanceToBoundary,
                selectedSlip
            );


            Debug.Log(
                GetReelName() +
                " " +
                targetSymbol +
                "を" +
                GetRowName(
                    targetRow
                ) +
                "へ引き込み → " +
                selectedSlip +
                "コマ滑り"
            );


            return;
        }


        // =====================================
        // ⑥ ターゲット引き込み失敗
        // =========================================

        if (
            reelType ==
            ReelType.Left
        )
        {
            int safeSlip =
                FindNonCherrySlip(
                    baseCenterIndex
                );


            if (safeSlip >= 0)
            {
                SetStopDistance(
                    distanceToBoundary,
                    safeSlip
                );


                return;
            }
        }


        SetStopDistance(
            distanceToBoundary,
            0
        );
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