using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Judge : MonoBehaviour
{
    [SerializeField] Numberofgames _numberofgames;
    [SerializeField] lotteryJudge _lotteryJudge;
    [SerializeField] private int JudgeCount;
    [SerializeField] private TextMeshProUGUI JudgeText;
    //[SerializeField] private Image[] _scenarioImages = new Image[8];
    //private Image _currentScenarioImage;
    private readonly int _scenarioNum;
    private int _judgeCountImageNum;
    public bool _setCount = true;
    private void Update()
    {
        JudgeText.text = "JudgeCount" + JudgeCount.ToString();
    }
    public void ATCount()
    {
        
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    ContinuingJudge();
        //}
    }
    void SetCount()
    {
        JudgeCount = 10;
    }
    private void ContinuingJudge()
    {
        int Continuing = RomdomGames();
        if (JudgeCount == 1)
        {
            _lotteryJudge.isScenario = true;
            Debug.Log("ê¨å˜");
        }
        else
        {
            if (Continuing <= 5)
            {
                _lotteryJudge.isScenario = true;
                Debug.Log("ê¨å˜");
            }
            else
            {
                Debug.Log("é∏îs");
                return;
            }
        }
    }
    //public void NextJudge()
    //{
    //    _judgeCountImageNum--;
    //    if (_currentScenarioImage == null) return;
    //    _currentScenarioImage = _scenarioImages[_judgeCountImageNum];
    //    _scenarioImages[_judgeCountImageNum].gameObject.SetActive(false);
    //    //_numberofgames.SetGameCount();
    //    //GetCollor(SManagement = hoge[_judgeCountImageNum]);
    //    if (_judgeCountImageNum >= _scenarioImages.Length)
    //    {
    //        _judgeCountImageNum = _scenarioImages.Length - 1;
    //        return;
    //    }
    //    _scenarioImages[_judgeCountImageNum].gameObject.SetActive(false);
    //}
    private int RomdomGames()
    {
        int num = 0;
        num = Random.Range(0, 101);
        return num;
    }

    public void GetJudgeRole()
    {
        if (!_numberofgames._continuing)
        {
            if (_setCount)
            {
                _setCount = false;
                SetCount();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                JudgeCount--;
                //NextJudge();
            }
            if (JudgeCount <= 0)
            {
                _lotteryJudge.GetCollorScenario(_lotteryJudge.SManagement = _lotteryJudge.hoge[_scenarioNum]);
                _setCount = true;
                JudgeCount = 0;
            }
        }
    }
}
