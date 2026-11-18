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
    Dictionary<int, bool> judgebool;
    private int _scenarioNum;
    [SerializeField] private Image[] _scenarioImages = new Image[7];
    [SerializeField] private int[] DS = new int[7];
    private Image _currentScenarioImage;
    int SManagement;
    public bool isScenario = true;
    void Start()
    {
        isScenario = true;
        foreach (Image image in _scenarioImages) image.gameObject.SetActive(false);
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

    private int RomdomDicts()
    {

        int num = 0;

        num = Random.Range(0,101);

        return num;
    }

    void InitalizeDictsBlue()
    {
        int _Blue = RomdomDicts();

        if (_Blue <= 20)
        {
            Debug.Log("青成功");
        }
        else
        {
            Debug.Log("青失敗");
            isScenario = false;
        }
        _currentScenarioImage.color = Color.blue;
    }
    void InitalizeDictsYellow()
    {
        int _Yellow = RomdomDicts();
        if (_Yellow <= 50)
        {
            Debug.Log("黄成功");
        }
        else
        {
            Debug.Log("黄失敗");
            isScenario = false;
        }
        _currentScenarioImage.color = Color.yellow;
    }
    void InitalizeDictsGreen()
    {

        int _Green = RomdomDicts();
        if (_Green <= 66)
        {
            Debug.Log("緑成功");
        }
        else
        {
            Debug.Log("緑失敗");
            isScenario = false;
        }
        _currentScenarioImage.color = Color.green;

    }
    void InitalizeDictsRed()
    {
        int _Yellow = RomdomDicts();
        if (_Yellow <= 80)
        {
            Debug.Log("赤成功");

        }
        else
        {
            Debug.Log("赤失敗");
            isScenario = false;
        }
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
