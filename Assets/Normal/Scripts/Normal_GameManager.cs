using Unity.VisualScripting;
using UnityEngine;

public class Normal_GameManager : MonoBehaviour
{
    // 現在のモード
    public GameEnum.eNormalMode mode {  get; private set; } = GameEnum.eNormalMode.Invalid;
    // 現在のポイント
    int nowPoint;
    // 現在の周期
    public GameEnum.eCycleCount cycleCount { get; private set; } = GameEnum.eCycleCount.Invalid;


    private void Awake()
    {

        mode = GameEnum.eNormalMode.Morning;
        cycleCount = GameEnum.eCycleCount.First;



    }

    void Start()
    {
        nowPoint = 0;
        

    }

    void Update()
    {

        nowPoint = PointManager.instance.NowPoint;


        

    }

    /// <summary>
    /// AT終了時にモードを抽選
    /// </summary>
    /// <returns></returns>
    private GameEnum.eNormalMode SelectMode()
    {
        int random = Random.Range(0, 101);

        if (random >= 0 && random <= 55 )
        {
            mode = GameEnum.eNormalMode.A;
            return mode;
        }
        if (random >= 56 && random <= 85)
        {
            mode = GameEnum.eNormalMode.B;
            return mode;
        }
        if (random >= 86 && random <= 95)
        {
            mode = GameEnum.eNormalMode.Heaven;
            return mode;
        }
        if (random >= 96 && random <= 100)
        {
            mode = GameEnum.eNormalMode.SokuYusyutu;
            return mode;
        }

        return GameEnum.eNormalMode.Invalid;
    }


}
