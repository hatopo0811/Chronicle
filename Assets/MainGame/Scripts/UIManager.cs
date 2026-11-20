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

    private Normal_GameManager normalInstance = null;
    private PointManager pointinstance = null;

    private void Awake()
    {
        instance = this;

        
    }

    void Start()
    {
        
        normalInstance = Normal_GameManager.instance;
        pointinstance = PointManager.instance;
        modeText.text = "NowMode : " + normalInstance.Mode.ToString();
        cycleText.text = "NowCycle : " + normalInstance.cycleCount.ToString();


    }

    

    void Update()
    {
        // デバック用　100ポイントを超えたら強制的に決戦前夜に移行
        if ((int)eTargetPoint.Point100 < pointinstance.NowPoint && Input.GetKeyDown(KeyCode.Return))
        {
            ChangeUICanvas(normal_UICanvas, nightBattle_UICanvas);

        }



    }


    public void ChangeUICanvas(GameObject beforeUICanvas, GameObject afterUICanvas)
    {
        beforeUICanvas.SetActive(false);
        afterUICanvas.SetActive(true);
    }



}
