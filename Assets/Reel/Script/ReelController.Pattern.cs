using UnityEngine;

public partial class ReelController
{
    // =========================================
    // 通常停止ターゲット
    // =========================================

    private string targetSymbol = null;

    private TargetRow targetRow = TargetRow.Center;


    // =========================================
    // マスターパターン停止制御
    // =========================================

    private bool useReachPattern = false;

    private string reachUpper = "*";
    private string reachCenter = "*";
    private string reachLower = "*";


    // =========================================
    // ★ Lose安全停止
    // =========================================

    // trueの場合、
    // Loseマスターが作れなかった時や
    // Loseマスター自体が無い時に、
    // 0～4コマから安全なハズレ位置を探す。
    private bool useLoseSafetyFallback = false;


    // =========================================
    // 通常ターゲット設定
    // =========================================

    public void SetTargetSymbol(
        string symbolName
    )
    {
        SetTargetSymbol(
            symbolName,
            TargetRow.Center
        );
    }


    public void SetTargetSymbol(
        string symbolName,
        TargetRow row
    )
    {
        useReachPattern = false;
        useLoseSafetyFallback = false;

        targetSymbol = symbolName;
        targetRow = row;


        if (
            string.IsNullOrEmpty(
                targetSymbol
            )
        )
        {
            Debug.Log(
                GetReelName() +
                "：引き込みなし"
            );
        }
    }


    // =========================================
    // Reach系マスターパターン
    // =========================================

    public void SetReachPattern(
        string upper,
        string center,
        string lower
    )
    {
        SetMasterPatternValues(
            upper,
            center,
            lower
        );

        useReachPattern = true;
        useLoseSafetyFallback = false;

        targetSymbol = null;
    }


    // =========================================
    // ★ Loseマスターパターン
    //
    // 指定したLose目を作れなかった場合、
    // 自動安全停止へ切り替える。
    // =========================================

    public void SetLosePattern(
        string upper,
        string center,
        string lower
    )
    {
        SetMasterPatternValues(
            upper,
            center,
            lower
        );

        useReachPattern = true;
        useLoseSafetyFallback = true;

        targetSymbol = null;
    }


    // =========================================
    // ★ Loseマスターが無い場合
    // 自動安全停止だけを使う
    // =========================================

    public void SetAutoSafeLoseMode()
    {
        useReachPattern = false;
        useLoseSafetyFallback = true;

        targetSymbol = null;
        targetRow = TargetRow.Center;
    }


    // =========================================
    // マスター3コマ保存
    // =========================================

    private void SetMasterPatternValues(
        string upper,
        string center,
        string lower
    )
    {
        reachUpper =
            string.IsNullOrWhiteSpace(
                upper
            )
            ? "*"
            : upper;


        reachCenter =
            string.IsNullOrWhiteSpace(
                center
            )
            ? "*"
            : center;


        reachLower =
            string.IsNullOrWhiteSpace(
                lower
            )
            ? "*"
            : lower;
    }


    // =========================================
    // マスターパターン停止位置検索
    // =========================================

    private int FindReachPatternSlip(
        int baseCenterIndex
    )
    {
        for (
            int slip = 0;
            slip <= maxSlipCount;
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


            if (
                !MatchesReachSymbol(
                    reachUpper,
                    upperSymbol
                )
            )
            {
                continue;
            }


            if (
                !MatchesReachSymbol(
                    reachCenter,
                    centerSymbol
                )
            )
            {
                continue;
            }


            if (
                !MatchesReachSymbol(
                    reachLower,
                    lowerSymbol
                )
            )
            {
                continue;
            }


            // =================================
            // 左リールCherry誤入賞防止
            // =========================================

            if (
                reelType ==
                    ReelType.Left
                &&
                !ReachPatternContainsCherry()
                &&
                IsCherryVisibleAtCenterIndex(
                    centerIndex
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
    // マスター図柄一致判定
    // =========================================

    private bool MatchesReachSymbol(
        string patternSymbol,
        string actualSymbol
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                patternSymbol
            )
            ||
            patternSymbol == "*"
        )
        {
            return true;
        }


        return
            patternSymbol ==
            actualSymbol;
    }


    // =========================================
    // マスターにCherry指定があるか
    // =========================================

    private bool ReachPatternContainsCherry()
    {
        return
            reachUpper == "Cherry"
            ||
            reachCenter == "Cherry"
            ||
            reachLower == "Cherry";
    }


    // =========================================
    // 段名
    // =========================================

    private string GetRowName(
        TargetRow row
    )
    {
        switch (row)
        {
            case TargetRow.Any:
                return "指定なし";

            case TargetRow.Upper:
                return "上段";

            case TargetRow.Center:
                return "中段";

            case TargetRow.Lower:
                return "下段";
        }


        return "不明";
    }
}
