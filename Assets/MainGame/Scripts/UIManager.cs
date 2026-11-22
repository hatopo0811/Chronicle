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



    // 
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

        modeText.text = "NowMode : " + normalInstance.Mode.ToString();
        cycleText.text = "NowCycle : " + normalInstance.cycleCount.ToString();
        gameStateText.text = "NowState : " + normalInstance.GameState.ToString();
        if (normalInstance.GameState == eGameState.NightBattle)
        {
            nightBattleGameCount.text = nightBattle.CurrentNightBattleGameCount + "G";
        }


    }


    public void ChangeUICanvas(GameObject beforeUICanvas, GameObject afterUICanvas)
    {
        beforeUICanvas.SetActive(false);
        afterUICanvas.SetActive(true);
    }



}
