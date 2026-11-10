using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class lotteryJudge : MonoBehaviour
{
    //シナリオの番号とシナリオ名を入れる辞書
    Dictionary<int, string> judgeInfo;

    //シナリオの確率を入れる辞書
    Dictionary<int, float> jugdeDict;

    Dictionary<int, string> lotteryjudge;

    int LotteryCount = 7;


    bool getScenario = false;


    void Update()
    {

        //一回だけ呼び出す
        if (!getScenario)
        {

            getScenario = true;
            Debug.Log("シナリオを取得します");
            GetJudge();
        }

    }

    void GetJudge() {


        //ボタンを押すと出る確率を変える
        for (int i = 0; i < LotteryCount; i++)
        {
            InitalizeDictsBlue();
            InitalizeDictsYellow();
            InitalizeDictsGreen();
            InitalizeDictsRed();
            InitalizeDictsRainbow();
        }



        if (judgeInfo == null || jugdeDict == null) return;

        int judge = Chooze();

        if (judge != 0)
        {
            string Judge = judgeInfo[judge];

            Debug.Log("シナリオ" + Judge);


        }
        else
        {
            Debug.Log("シナリオなし");
        }
    }


    void InitalizeDictsBlue()
    {
        
            judgeInfo = new Dictionary<int, string>();
            judgeInfo.Add(1, "青成功");
            judgeInfo.Add(2, "青残念");
            jugdeDict = new Dictionary<int, float>();
            jugdeDict.Add(1, 0.25f);
            jugdeDict.Add(2, 0.75f);
        
    }

    void InitalizeDictsYellow()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "黄成功");
        judgeInfo.Add(2, "黄残念");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 0.5f);
        jugdeDict.Add(2, 0.5f);

    }

    void InitalizeDictsGreen()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "緑成功");
        judgeInfo.Add(2, "緑残念");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 0.66f);
        jugdeDict.Add(2, 0.34f);

    }


    void InitalizeDictsRed()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "赤成功");
        judgeInfo.Add(2, "赤残念");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 0.8f);
        jugdeDict.Add(2, 0.2f);

    }

    void InitalizeDictsRainbow()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "虹成功");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 1.0f);

    }

    int Chooze()
    {
        //確率の合計を計算
        float total = 0;
        //確率の合計を計算
        foreach (KeyValuePair<int, float> elem in jugdeDict)
        {
            total += elem.Value;
        }
        //ランダムな値を生成
        float randomPoint = Random.value * total;
        //確率の合計をもとにシナリオを選択
        foreach (KeyValuePair<int, float> elem in jugdeDict)
        {
            if (randomPoint < elem.Value)
            {
                return elem.Key;
            }
            else
            {
                randomPoint -= elem.Value;
            }
        }
        return 0;
    }


}
