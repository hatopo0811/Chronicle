using UnityEngine;

public partial class SlotGameManager
{
    private void SetReelTargets()
    {
        // 6役共通で、0～4コマ以内の停止候補を後段で評価する。
        // SetTargetSymbolは旧制御の状態リセットにも使う。
        leftReel.SetTargetSymbol(null);
        centerReel.SetTargetSymbol(null);
        rightReel.SetTargetSymbol(null);
    }

    private void ApplyMasterPattern(
        HitPatternMasterLoader.HitPatternData
            pattern
    )
    {
        bool isLose =
            string.Equals(
                pattern.Result,
                "Lose",
                System.StringComparison.OrdinalIgnoreCase
            );


        if (isLose)
        {
            // Loseマスターの場合は、
            // マスター目を作れなかった時に
            // 自動安全停止へフォールバックする。
            leftReel.SetLosePattern(
                pattern.LU,
                pattern.LC,
                pattern.LL
            );


            centerReel.SetLosePattern(
                pattern.CU,
                pattern.CC,
                pattern.CL
            );


            rightReel.SetLosePattern(
                pattern.RU,
                pattern.RC,
                pattern.RL
            );
        }
        else
        {
            leftReel.SetReachPattern(
                pattern.LU,
                pattern.LC,
                pattern.LL
            );


            centerReel.SetReachPattern(
                pattern.CU,
                pattern.CC,
                pattern.CL
            );


            rightReel.SetReachPattern(
                pattern.RU,
                pattern.RC,
                pattern.RL
            );
        }


        Debug.Log(
            "マスター停止制御：" +
            pattern.ID +
            " / " +
            pattern.Name
        );
    }


    private PayLine SelectTargetPayLine()
    {
        int random =
            UnityEngine.Random.Range(
                0,
                5
            );


        switch (random)
        {
            case 0:
                return PayLine.Upper;

            case 1:
                return PayLine.Center;

            case 2:
                return PayLine.Lower;

            case 3:
                return PayLine.DiagonalDown;

            case 4:
                return PayLine.DiagonalUp;
        }


        return PayLine.Center;
    }

    private void SetThreeReelTarget(
        string leftSymbol,
        string centerSymbol,
        string rightSymbol,
        PayLine line
    )
    {
        ReelController.TargetRow leftRow;
        ReelController.TargetRow centerRow;
        ReelController.TargetRow rightRow;


        GetRowsForPayLine(
            line,
            out leftRow,
            out centerRow,
            out rightRow
        );


        leftReel.SetTargetSymbol(
            leftSymbol,
            leftRow
        );


        centerReel.SetTargetSymbol(
            centerSymbol,
            centerRow
        );


        rightReel.SetTargetSymbol(
            rightSymbol,
            rightRow
        );
    }

    private void GetRowsForPayLine(
        PayLine line,
        out ReelController.TargetRow leftRow,
        out ReelController.TargetRow centerRow,
        out ReelController.TargetRow rightRow
    )
    {
        leftRow =
            ReelController.TargetRow.Center;


        centerRow =
            ReelController.TargetRow.Center;


        rightRow =
            ReelController.TargetRow.Center;


        switch (line)
        {
            case PayLine.Upper:

                leftRow =
                    ReelController.TargetRow.Upper;

                centerRow =
                    ReelController.TargetRow.Upper;

                rightRow =
                    ReelController.TargetRow.Upper;

                break;


            case PayLine.Center:

                break;


            case PayLine.Lower:

                leftRow =
                    ReelController.TargetRow.Lower;

                centerRow =
                    ReelController.TargetRow.Lower;

                rightRow =
                    ReelController.TargetRow.Lower;

                break;


            case PayLine.DiagonalDown:

                leftRow =
                    ReelController.TargetRow.Upper;

                centerRow =
                    ReelController.TargetRow.Center;

                rightRow =
                    ReelController.TargetRow.Lower;

                break;


            case PayLine.DiagonalUp:

                leftRow =
                    ReelController.TargetRow.Lower;

                centerRow =
                    ReelController.TargetRow.Center;

                rightRow =
                    ReelController.TargetRow.Upper;

                break;
        }
    }
}
