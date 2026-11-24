using TMPro;
using UnityEngine;
using UnityEngine.UI;
//ゲーム数を管理するクラス
public class Numberofgames : MonoBehaviour
{
    [SerializeField]lotteryJudge lotteryJudge;
    [SerializeField] Judge _judge;
    //ゲーム数の初期値を設定する変数
    public int _initialGameCount;
    //private int amount;
    int _gameCount=10;
    [SerializeField] public TextMeshProUGUI GameText;
    [SerializeField] TextMeshProUGUI GetGameText;
    public int GameNum;
    [SerializeField] public RawImage _endScene;
    private int _scenarioNum;   
    public bool _continuing = true;



    

    

    private void Update()
    {
        if (Normal_GameManager.instance.IsAT)
        {
            SetGameCount();
        }


        GameText.text = "Game" + _initialGameCount.ToString();
        //ゲーム数が999を超えないようにする処理
        if (_initialGameCount >= 999)
        {
            _initialGameCount = 999;
        }
        //ゲーム数を増加させる処理を行う
        else
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                GetGameCount(_gameCount);
            }
        }
    }
    public void SetGameCount()
    {
        //ゲーム数を初期値に設定する
        _initialGameCount = 5;      
    }
    /// <summary>
    /// 上乗せ抽選でゲーム数を増加させる処理を行う
    /// </summary>
    /// <param name="amount">ゲーム数の増加する数を指定する</param>
    public void GetGameCount(int amount)
    {
        //ゲーム数を足す処理を行う
        _initialGameCount += amount;
    }

    public void GetSetNum()
    {
        if (!Normal_GameManager.instance.IsAT) return;

        if (lotteryJudge == null) return;
        if (lotteryJudge.isScenario)
        {
            if (_initialGameCount >= 0)
            {
                if (_continuing)
                {
                    _initialGameCount--;
                }
            }
            if (_initialGameCount == 0)
            {
                _continuing = false;
                _judge.ATCount();

            }
            if (_initialGameCount == -1)
            {
                lotteryJudge.isScenario = false;
                GameText.gameObject.SetActive(false);
                _endScene.gameObject.SetActive(true);
                Debug.Log("通常時に戻る");

            }
            
        }

    }

   
}