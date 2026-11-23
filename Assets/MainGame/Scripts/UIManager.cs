using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    [SerializeField] private GameObject normal_UICanvas;
    [SerializeField] private GameObject nightBattle_UICanvas;
    [SerializeField] private GameObject AT_UICanvas;
    [SerializeField] private GameObject CZ_UICanvas;


    // デバックテキスト
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI cycleText;
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private TextMeshProUGUI nightBattleGameCount;



    // あそび
    [SerializeField] private GameObject Enoki;




    [SerializeField] private NightBattle nightBattle;
    private Normal_GameManager normalInstance = null;
    private PointManager pointinstance = null;

    private void Awake()
    {
        instance = this;
        
        



    }

    private void Start()
    {
        normalInstance = Normal_GameManager.instance;
        pointinstance = PointManager.instance;
        modeText.text = "NowMode : " + normalInstance.Mode.ToString();
        cycleText.text = "NowCycle : " + normalInstance.cycleCount.ToString();
        gameStateText.text = "NowState : " + Normal_GameManager.instance.GameState.ToString();


    }



    void Update()
    {

        // 100ポイントを超えたら榎木を出す
        if ((int)eTargetPoint.Point100 < pointinstance.NowPoint)
        {
            Enoki.SetActive(true);
        }
        // エンターキーで決戦前夜に移行
        if (Enoki.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            ChangeUICanvas(normal_UICanvas, nightBattle_UICanvas);
            Enoki.SetActive(false);
        }
        // 前兆中は前兆のゲーム数表示
        if (normalInstance.IsNightBattle)
        {
            nightBattleGameCount.text = nightBattle.CurrentNightBattleGameCount + "G";
        }
        // ジャッジ演出に入ったら
        if (Normal_GameManager.instance.IsNightBattleJudge)
        {
            // ジャッジの残りゲーム数を表示
            nightBattleGameCount.text = nightBattle.nowJudgeGameCount.ToString();
        }
        // ジャッジ成功したら　仮：３ゲーム経ったら
        if (nightBattle.nowJudgeGameCount < 0)
        {
            // ATに移行
            Normal_GameManager.instance.GameState = eGameState.AT;
            // UI切り替え
            ChangeUICanvas(nightBattle_UICanvas,AT_UICanvas);
        }


        // 常に表示
        modeText.text = "NowMode : " + normalInstance.Mode.ToString();
        cycleText.text = "NowCycle : " + normalInstance.cycleCount.ToString();
        gameStateText.text = "NowState : " + normalInstance.GameState.ToString();

    }

    /// <summary>
    /// 通常、前兆、CZ、ATのUICanvasを切り替える
    /// </summary>
    /// <param name="beforeUICanvas"></param>
    /// <param name="afterUICanvas"></param>
    private void ChangeUICanvas(GameObject beforeUICanvas, GameObject afterUICanvas)
    {
        beforeUICanvas.SetActive(false);
        afterUICanvas.SetActive(true);
    }



}
