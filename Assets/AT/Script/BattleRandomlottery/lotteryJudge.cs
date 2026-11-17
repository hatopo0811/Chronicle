using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//シナリオ抽選を行うクラス
public class lotteryJudge : MonoBehaviour
{
    Numberofgames numberofgames;
    //シナリオの番号とシナリオ名を入れる辞書
    Dictionary<int, string> judgeInfo;
    //シナリオの確率を入れる辞書
    Dictionary<int, float> jugdeDict;
    private int _scenarioNum = 0;
    [SerializeField] private Image[] _scenarioImages = new Image[7];
    [SerializeField] private int[] DS = new int[7];
    private Image _currentScenarioImage;
    int SManagement;
    void Start()
    {
        foreach (Image image in _scenarioImages)image.gameObject.SetActive(false);
        _currentScenarioImage = _scenarioImages[0];
        _scenarioImages[0].gameObject.SetActive(true);
        GetCollor(SManagement = DS[_scenarioNum]);
    }
    void Update()
    {
        //シナリオ抽選の処理を行う 
        if (Input.GetKeyDown(KeyCode.Z))
        {
            NextScenario();
        }   
    }
    void GetCollor(int SManagement)
    {
        switch (SManagement)
        {
            case 1:InitalizeDictsBlue();break;
            case 2:InitalizeDictsYellow();break;
            case 3:InitalizeDictsGreen();break;
            case 4:InitalizeDictsRed();break;
            case 5:InitalizeDictsRainbow();break;
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
            if (randomPoint < elem.Value)return elem.Key;
            else randomPoint -= elem.Value;
        }
        return 0;
    }
    public void NextScenario()
    {
        _scenarioNum++;
        _currentScenarioImage = _scenarioImages[_scenarioNum];
        _scenarioImages[_scenarioNum].gameObject.SetActive(true);
        GetCollor(SManagement = DS[_scenarioNum]);
        if (_scenarioNum >= _scenarioImages.Length)
        {   
            _scenarioNum = _scenarioImages.Length - 1;
            return;
        }
        _scenarioImages[_scenarioNum].gameObject.SetActive(true);
    }
}
