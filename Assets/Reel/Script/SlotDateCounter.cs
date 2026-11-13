using TMPro;
using UnityEngine;

public class SlotDataCounter : MonoBehaviour
{
    // =========================================
    // 表示
    // =========================================

    [Header("データカウンター表示")]
    [SerializeField]
    private TMP_Text bigCountText;

    [SerializeField]
    private TMP_Text regCountText;

    [SerializeField]
    private TMP_Text bonusCountText;

    [SerializeField]
    private TMP_Text currentGameCountText;

    [SerializeField]
    private TMP_Text totalGameCountText;

    [SerializeField]
    private TMP_Text combinedText;


    // =========================================
    // カウント
    // =========================================

    private int currentGameCount = 0;

    private int totalGameCount = 0;

    private int bigBonusCount = 0;

    private int regBonusCount = 0;


    // =========================================
    // 外部参照用
    // =========================================

    public int CurrentGameCount
    {
        get
        {
            return currentGameCount;
        }
    }


    public int TotalGameCount
    {
        get
        {
            return totalGameCount;
        }
    }


    public int BigBonusCount
    {
        get
        {
            return bigBonusCount;
        }
    }


    public int RegBonusCount
    {
        get
        {
            return regBonusCount;
        }
    }


    public int TotalBonusCount
    {
        get
        {
            return
                bigBonusCount +
                regBonusCount;
        }
    }


    // =========================================
    // Start
    // =========================================

    private void Start()
    {
        ResetCounter();
    }


    // =========================================
    // 通常ゲーム +1
    // =========================================

    public void AddGame()
    {
        // 現在の回転数
        currentGameCount++;

        // 総回転数
        totalGameCount++;


        UpdateDisplay();


        Debug.Log(
            "現在回転数：" +
            currentGameCount +
            "G"
        );


        Debug.Log(
            "総回転数：" +
            totalGameCount +
            "G"
        );
    }


    // =========================================
    // BIG +1
    // =========================================

    public void AddBigBonus()
    {
        bigBonusCount++;


        // =====================================
        // ボーナス当選したので
        // 現在回転数を0へ
        // =========================================

        currentGameCount =
            0;


        UpdateDisplay();


        Debug.Log(
            "BIG回数：" +
            bigBonusCount
        );


        Debug.Log(
            "現在回転数リセット"
        );
    }


    // =========================================
    // REG +1
    // =========================================

    public void AddRegBonus()
    {
        regBonusCount++;


        // =====================================
        // ボーナス当選したので
        // 現在回転数を0へ
        // =========================================

        currentGameCount =
            0;


        UpdateDisplay();


        Debug.Log(
            "REG回数：" +
            regBonusCount
        );


        Debug.Log(
            "現在回転数リセット"
        );
    }


    // =========================================
    // カウンターリセット
    // =========================================

    public void ResetCounter()
    {
        currentGameCount =
            0;


        totalGameCount =
            0;


        bigBonusCount =
            0;


        regBonusCount =
            0;


        UpdateDisplay();
    }


    // =========================================
    // 表示更新
    // =========================================

    private void UpdateDisplay()
    {
        // =====================================
        // BIG
        // =========================================

        if (bigCountText != null)
        {
            bigCountText.text =
                bigBonusCount.ToString();
        }


        // =====================================
        // REG
        // =========================================

        if (regCountText != null)
        {
            regCountText.text =
                regBonusCount.ToString();
        }


        // =====================================
        // BONUS合計
        // =========================================

        if (bonusCountText != null)
        {
            bonusCountText.text =
                TotalBonusCount.ToString();
        }


        // =====================================
        // 現在の回転数
        // =========================================

        if (currentGameCountText != null)
        {
            currentGameCountText.text =
                "NowGame : " + currentGameCount;
        }


        // =====================================
        // 総回転数
        // =========================================

        if (totalGameCountText != null)
        {
            totalGameCountText.text =
                totalGameCount.ToString();
        }


        // =====================================
        // 合算
        // =========================================

        if (combinedText != null)
        {
            if (TotalBonusCount <= 0)
            {
                combinedText.text =
                    "-/---";
            }
            else
            {
                float combined =
                    (float)totalGameCount /
                    TotalBonusCount;


                combinedText.text =
                    "1/" +
                    combined.ToString(
                        "0.0"
                    );
            }
        }
    }
}