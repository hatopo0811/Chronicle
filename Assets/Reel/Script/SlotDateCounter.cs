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
    // カウント(外部参照可能)
    // =========================================

    // 現在のゲーム数
    public int currentGameCount { get; private set; }
    // 総ゲーム数
    public int totalGameCount { get; private set; }
    // BB回数
    public int bigBonusCount { get; private set; }
    // REG回数
    public int regBonusCount { get; private set; }
    // ボーナス（REG＋BB）の合計回数
    public int TotalBonusCount
    { get { return bigBonusCount + regBonusCount; } }


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
            currentGameCountText.text = currentGameCount.ToString() + "G";
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