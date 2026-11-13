using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Normal_GameManager : MonoBehaviour
{
    public static Normal_GameManager instance = null;

    [SerializeField] private TextMeshProUGUI gameCountText;
    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI cycleText;

    [SerializeField] private GameObject Test_UICanvas;

    [SerializeField] private SlotGameManager gameManager;

    // 現在のポイント
    public int NowPoint { get; set; } = 0;
    // 現在のゲーム数
    int nowGameCount;
    // 現在のモード
    public eNormalMode mode { get; private set; } = eNormalMode.Invalid;
    // 現在の周期
    public eCycleCount cycleCount { get; private set; } = eCycleCount.Invalid;

    public int _GameCountTEST_TEST_TEST_TEST_ { get; private set; }

    // シーンのロード中かどうか
    private bool isLoadScene;



    private void Awake()
    {

        mode = eNormalMode.Morning;
        cycleCount = eCycleCount.First;



    }

    void Start()
    {
        instance = this;
        nowGameCount = 0;
        NowPoint = 0;
        // 初期値
        modeText.text = "NowMode : " + mode.ToString();
        cycleText.text = "NowCycle : " + cycleCount.ToString();

        isLoadScene = false;

    }

    void Update()
    {

        pointText.text = "NowPoint : " + NowPoint;


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectMode();
            modeText.text = "NowMode : " + mode.ToString();
            cycleText.text = "NowCycle : " + cycleCount.ToString();

        }

        if (((int)eTargetPoint.Point100 < NowPoint) && !isLoadScene)
        {
            // 複数回ロードされるのを防ぐ
            isLoadScene = true;
            // 仮の連即演出成功画像を表示
            Test_UICanvas.SetActive(true);
            StartCoroutine(LoadSceneAsync());   
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
            mode = eNormalMode.A;
            return mode;
        }
        if (random >= 56 && random <= 85)
        {
            mode = eNormalMode.B;
            return mode;
        }
        if (random >= 86 && random <= 95)
        {
            mode = eNormalMode.Heaven;
            return mode;
        }
        if (random >= 96 && random <= 100)
        {
            mode = eNormalMode.SokuYusyutu;
            return mode;
        }

        return eNormalMode.Invalid;
    }


    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("ZentyouScene");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // リールが回り始めたら
            if (gameManager.GameStarted)
            {
                // シーン移行の許可を出す
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

    }
    
}
