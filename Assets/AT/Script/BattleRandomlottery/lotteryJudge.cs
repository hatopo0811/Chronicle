using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//シナリオ抽選を行うクラス
//継続したかどうかのクラス
public class lotteryJudge : MonoBehaviour
{
    [SerializeField] Numberofgames numberofgames;
    public int _scenarioNum;
    [SerializeField] private Image[] _scenarioImages = new Image[7];
    [SerializeField] private int[] DS = new int[7];
    [SerializeField] private int[] OZ = new int[7];
    [SerializeField] private int[] OG = new int[7];
    [SerializeField] private Image _image;
    public int[] hoge = new int[7];
    private Image _currentScenarioImage;
    public int SManagement;
    public bool isScenario = false;
    private int normalCount = 2;
    private enum  Judge
    {
     _blueJudge = 20,
    _yellowJudge = 50,
    _greenJudge = 66,
    _redJudge = 80,
     _magentaJudge = 100,
    }


    private void Update()
    {

        if (Normal_GameManager.instance.IsAT&&!isScenario)
        {
            isScenario = true;
            Scenariotable();
            StartScenario();

        }

    }

    public void GetCollorScenario(int SManagement)
    {
        switch (SManagement)
        {
            case 1:InitalizeDictsJudge((int)Judge._blueJudge);break;
            case 2:InitalizeDictsJudge((int)Judge._yellowJudge);break;
            case 3:InitalizeDictsJudge((int)Judge._greenJudge);break;
            case 4:InitalizeDictsJudge((int)Judge._redJudge);break;
            case 5:InitalizeDictsJudge((int)Judge._magentaJudge);break;
        }  
    }

    private void StartScenario()
    {
        foreach (Image image in _scenarioImages) image.gameObject.SetActive(false);
        _currentScenarioImage = _scenarioImages[0];
        _scenarioImages[0].gameObject.SetActive(true);
        GetCollor(SManagement = hoge[_scenarioNum]);
    } 

    public void GetCollor(int SManagement)
    {
        switch (SManagement)
        {
            case 1:_currentScenarioImage.color = Color.blue; break;
            case 2:_currentScenarioImage.color = Color.yellow; break;
            case 3:_currentScenarioImage.color = Color.green; break;
            case 4:_currentScenarioImage.color = Color.red; break;
            case 5:_currentScenarioImage.color = Color.magenta; break;
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
            numberofgames._continuing = true;
            NextScenario();
        }
        else
        {
            Debug.Log("失敗");
            isScenario = false;
            numberofgames.GameText.gameObject.SetActive(false);
            numberofgames._endScene.gameObject.SetActive(true);
            Debug.Log("通常時に戻る");
        }
    }
    public void NextScenario()
    {
        _scenarioNum++;
        if (_currentScenarioImage == null) return;
        _currentScenarioImage = _scenarioImages[_scenarioNum];
        _scenarioImages[_scenarioNum].gameObject.SetActive(true);
        numberofgames.SetGameCount();
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

    public void Getflag()
    {
        if (!Normal_GameManager.instance.IsAT) return;
        if (!isScenario)
        {
            if (Input.GetKeyDown(KeyCode.Z))
                
            {
                normalCount--;

            }
            if (normalCount == 0)
            {
                _image.gameObject.SetActive(true);


            }



        }

    }

}