using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Normal_GameManager : MonoBehaviour
{
    public static Normal_GameManager instance = null;

     
    // ゲームステート
    public eGameState GameState {  get; set; }



    // ゲーム数はまなとのHOHANAしているファイルにもともとあった物を使っている
    // 現在のモード
    public eNormalMode Mode { get; private set; } = eNormalMode.Invalid;
    // 現在の周期
    public eCycleCount cycleCount { get; private set; } = eCycleCount.Invalid;

    private void Awake()
    {
        instance = this;

        // モードと周期の初期設定
        Mode = eNormalMode.Morning;
        cycleCount = eCycleCount.First;
    }

    void Start()
    {
           
    }

    private void OnEnable()
    {
        GameState = eGameState.Normal;
    }
    

    void Update()
    {
        // 仮実装　モード抽選　本実装は、AT終了時ゲームで抽選
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectMode();
        }
    }

    /// <summary>
    /// AT終了時にモードを抽選
    /// </summary>
    /// <returns></returns>
    public eNormalMode SelectMode()
    {
        int random = Random.Range(GameConst.RANDOM_MIN, GameConst.RANDOM_MAX);

        if (random >= 0 && random <= 55)
        {
            Mode = eNormalMode.A;
            return Mode;
        }
        if (random >= 56 && random <= 85)
        {
            Mode = eNormalMode.B;
            return Mode;
        }
        if (random >= 86 && random <= 95)
        {
            Mode = eNormalMode.Heaven;
            return Mode;
        }
        if (random >= 96 && random <= 100)
        {
            Mode = eNormalMode.SokuYusyutu;
            return Mode;
        }

        return eNormalMode.Invalid;
    }




}
