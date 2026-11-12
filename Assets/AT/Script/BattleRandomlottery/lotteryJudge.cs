using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//シナリオ抽選を行うクラス
public class lotteryJudge : MonoBehaviour
{
    //シナリオの番号とシナリオ名を入れる辞書
    Dictionary<int, string> judgeInfo;

    //シナリオの確率を入れる辞書
    Dictionary<int, float> jugdeDict;

    Dictionary<int, string> lotteryjudge;

    int _judgeCherryNum = 5;





    int hoge;



    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            
                aaa();
            SetJudge();
        }
    }

    void aaa()
    {

        hoge++;

        GetCollor();


    }

    void GetCollor()
    {

        switch (hoge)
        {

            case 1:
                 InitalizeDictsBlue();
                break;
            case 2:
                InitalizeDictsYellow();
                break;
            case 3:
                InitalizeDictsGreen();
                break;
            case 4:
                InitalizeDictsRed();
                break;
            case 5:
                InitalizeDictsRainbow();
                break;
        }
        GetJudgeEX();
    }

    void GetjudgeEX() {


       



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

   
         private void SetJudge()
    {

        //_judgeExtraboostにRandomNumを代入して、上乗せ抽選を行う
        int _judgeExtraboost = Random.Range(GameConst.RANDOM_MIN, GameConst.RANDOM_MAX);


        Debug.Log("SetJudgeExtraboostWeak: 乱数は" + _judgeExtraboost);

        if (_judgeExtraboost < _judgeCherryNum)
        {
            GetJudgeEX();
        }
        else if (_judgeExtraboost >= _judgeCherryNum)
        {
            Debug.Log("SetJudgeExtraboostWeak: はずれ");
        }


    }


    //チェリーの上乗せ何Gかの抽選
    private void GetJudgeEX()
    {

        int _cherryWeak = Random.Range(GameConst.RANDOM_MIN, GameConst.RANDOM_MAX);
        if (_cherryWeak <= 14)
        {
            Debug.Log("10G");
            //_numberOfGames.GetGameCount();
        }
        else if (_cherryWeak >= 15 && _cherryWeak <= 18)
        {
            Debug.Log("SetJudgeExtraboostWeak: 20G上乗せ");
        }
        else
        {
            Debug.Log("SetJudgeExtraboostWeak: 30G上乗せ");
        }
    }


}
