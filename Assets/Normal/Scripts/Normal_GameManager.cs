using UnityEngine;

public class Normal_GameManager : MonoBehaviour
{
    // 現在のモード
    GameEnum.eNormalMode mode
    // 現在のポイント
    int nowPoint;
    // 現在の周期


    void Start()
    {
        
    }

    void Update()
    {
        nowPoint = PointManager.instance.NowPoint;


        

    }
}
