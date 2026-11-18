using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

//シナリオ抽選を行うクラス
public class lotteryJudge : MonoBehaviour
{
    [SerializeField] Numberofgames numberofgames;
    private int _scenarioNum;
    [SerializeField] private Image[] _scenarioImages = new Image[7];
    [SerializeField] private int[] DS = new int[7];
    [SerializeField] private int[] OZ = new int[7];
    [SerializeField] private int[] OG = new int[7];
    [SerializeField] AudioSource _audioSource;
    private int[] hoge = new int[7];
    private Image _currentScenarioImage;
    int SManagement;
    public bool isScenario = true;
    private enum  Judge
    {
     _blueJudge = 20,
    _yellowJudge = 50,
    _greenJudge = 66,
    _redJudge = 80,
     _magentaJudge = 100,
    }
void Start()
    {
        isScenario = true;
        Scenariotable();
    }
    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.Z))
        {
            foreach (Image image in _scenarioImages) image.gameObject.SetActive(false);
            _currentScenarioImage = _scenarioImages[0];
            _scenarioImages[0].gameObject.SetActive(true);
            GetCollor(SManagement = hoge[_scenarioNum]);
        }
    }
    void GetCollor(int SManagement)
    {
        switch (SManagement)
        {
            case 1:InitalizeDictsJudge((int)Judge._blueJudge);_currentScenarioImage.color = Color.blue;break;
            case 2:InitalizeDictsJudge((int)Judge._yellowJudge);_currentScenarioImage.color = Color.yellow;break;
            case 3:InitalizeDictsJudge((int)Judge._greenJudge);_currentScenarioImage.color = Color.green;break;
            case 4:InitalizeDictsJudge((int)Judge._redJudge);_currentScenarioImage.color = Color.red;break;
            case 5:InitalizeDictsJudge((int)Judge._magentaJudge);_currentScenarioImage.color = Color.magenta;break;
        }  
    }
    private int RomdomDicts()
    {
        int num = 0;
        num = Random.Range(0,101);
        return num;
    }

    void InitalizeDictsJudge(int _DictsJudge)
    {
        int _Judge = RomdomDicts();

        if (_Judge <= _DictsJudge)
        {
            Debug.Log("成功");
        }
        else
        {
            Debug.Log("失敗");
            isScenario = false;
        }   
    }
    public void NextScenario()
    {
        _scenarioNum++;
        _currentScenarioImage = _scenarioImages[_scenarioNum];
        _scenarioImages[_scenarioNum].gameObject.SetActive(true);
        GetCollor(SManagement = hoge[_scenarioNum]);
        if (_scenarioNum >= _scenarioImages.Length)
        {
            _scenarioNum = _scenarioImages.Length - 1;
            return;
        }
        _scenarioImages[_scenarioNum].gameObject.SetActive(true);
    }
    void Scenariotable()
    {
        int _table = Random.Range(0,3);

        switch (_table)
        {
            case 0:hoge = DS;Debug.Log("DS");break;
            case 1:hoge = OZ;Debug.Log("OZ");break;
            case 2:hoge = OG; Debug.Log("OG"); break;
            default:Debug.Log("tableがエラー");return;
        }
    }
}