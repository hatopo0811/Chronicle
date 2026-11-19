using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Normal_GameManager : MonoBehaviour
{
    public static Normal_GameManager instance = null;

    [SerializeField] private TextMeshProUGUI gameCountText;
    
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI cycleText;

    
    
    // ゲーム数はまなとが管理しているファイルにもともとあった物を使っている
    // 現在のモード
    public eNormalMode Mode { get; private set; } = eNormalMode.Invalid;
    // 現在の周期
    public eCycleCount cycleCount { get; private set; } = eCycleCount.Invalid;

    // 優出モードに滞在しているかどうか
    public bool IsYusyutuMode { get; set; } = false;

    



    private void Awake()
    {

        Mode = eNormalMode.Morning;
        cycleCount = eCycleCount.First;



    }

    void Start()
    {
        instance = this;

        
        // 初期値
        modeText.text = "NowMode : " + Mode.ToString();
        cycleText.text = "NowCycle : " + cycleCount.ToString();

        IsYusyutuMode = false;
    }

    void Update()
    {
        // 仮実装　モード抽選
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectMode();
            modeText.text = "NowMode : " + Mode.ToString();
            cycleText.text = "NowCycle : " + cycleCount.ToString();

        }


    }

    /// <summary>
    /// AT終了時にモードを抽選
    /// </summary>
    /// <returns></returns>
    private eNormalMode SelectMode()
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
