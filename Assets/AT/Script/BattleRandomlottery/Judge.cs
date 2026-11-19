using TMPro;
using UnityEngine;
public class Judge : MonoBehaviour
{
    [SerializeField] Numberofgames _numberofgames;
    [SerializeField] lotteryJudge _lotteryJudge;
    [SerializeField] private int JudgeCount;
    [SerializeField] private TextMeshProUGUI JudgeText;
    private readonly int _scenarioNum;
    public bool _setCount = true;
    private void Update()
    {
        JudgeText.text = "JudgeCount" + JudgeCount.ToString();
    }
    public void ATCount()
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
            }
            if (JudgeCount <= 0)
            {
                _lotteryJudge.GetCollorScenario(_lotteryJudge.SManagement = _lotteryJudge.hoge[_scenarioNum]);
                _setCount = true;
                JudgeCount = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ContinuingJudge();
        }
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
            Debug.Log("¬Œ÷");
        }
        else
        {
            if (Continuing <= 5)
            {
                _lotteryJudge.isScenario = true;
                Debug.Log("¬Œ÷");
            }
            else
            {
                Debug.Log("Ž¸”s");
                return;
            }
        }
    }
    private int RomdomGames()
    {
        int num = 0;
        num = Random.Range(0, 101);
        return num;
    }
}
