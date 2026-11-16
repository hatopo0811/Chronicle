using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//シナリオ抽選を行うクラス
public class lotteryJudge : MonoBehaviour
{

    

    //シナリオの番号とシナリオ名を入れる辞書
    Dictionary<int, string> judgeInfo;

    //シナリオの確率を入れる辞書
    Dictionary<int, float> jugdeDict;

    Dictionary<int, string> lotteryjudge;

    int _judgeCherryNum = 5;

    [SerializeField] private Image _scenarioImage1;
    [SerializeField] private Image _scenarioImage2;
    [SerializeField] private Image _scenarioImage3;
    [SerializeField] private Image _scenarioImage4;
    [SerializeField] private Image _scenarioImage5;
    [SerializeField] private Image _scenarioImage6;
    [SerializeField] private Image _scenarioImage7;

    private Image _currentScenarioImage;

    int SManagement;



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _currentScenarioImage = _scenarioImage1;
            GetCollor(SManagement = 1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            _currentScenarioImage = _scenarioImage2;
            GetCollor(SManagement = 2);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            _currentScenarioImage = _scenarioImage3;
            GetCollor(SManagement = 3);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            _currentScenarioImage = _scenarioImage4;
            GetCollor(SManagement = 4);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            _currentScenarioImage = _scenarioImage5;
            GetCollor(SManagement = 5);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            _currentScenarioImage = _scenarioImage6;
            GetCollor(SManagement = 1);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            _currentScenarioImage = _scenarioImage7;
            GetCollor(SManagement = 2);
        }

    }

    void aaa()
    {

        SManagement++;

        GetCollor(SManagement);


    }

    void GetCollor(int SManagement)
    {

        switch (SManagement)
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
        GetjudgeEX();
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

        _currentScenarioImage.color = Color.blue;
    }

    void InitalizeDictsYellow()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "黄成功");
        judgeInfo.Add(2, "黄残念");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 0.5f);
        jugdeDict.Add(2, 0.5f);

        _currentScenarioImage.color = Color.yellow;
    }

    void InitalizeDictsGreen()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "緑成功");
        judgeInfo.Add(2, "緑残念");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 0.66f);
        jugdeDict.Add(2, 0.34f);

        _currentScenarioImage.color = Color.green;
    }


    void InitalizeDictsRed()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "赤成功");
        judgeInfo.Add(2, "赤残念");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 0.8f);
        jugdeDict.Add(2, 0.2f);

        _currentScenarioImage.color = Color.red;
    }

    void InitalizeDictsRainbow()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "虹成功");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 1.0f);
        _currentScenarioImage.color = Color.magenta;

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
            GetjudgeEX();
        }
        else if (_judgeExtraboost >= _judgeCherryNum)
        {
            Debug.Log("SetJudgeExtraboostWeak: はずれ");
        }


    }


    //チェリーの上乗せ何Gかの抽選
    //private void GetJudgeEX()
    //{

    //    int _cherryWeak = Random.Range(GameConst.RANDOM_MIN, GameConst.RANDOM_MAX);
    //    if (_cherryWeak <= 14)
    //    {
    //        Debug.Log("10G");
    //        //_numberOfGames.GetGameCount();
    //    }
    //    else if (_cherryWeak >= 15 && _cherryWeak <= 18)
    //    {
    //        Debug.Log("SetJudgeExtraboostWeak: 20G上乗せ");
    //    }
    //    else
    //    {
    //        Debug.Log("SetJudgeExtraboostWeak: 30G上乗せ");
    //    }
    //}


}
