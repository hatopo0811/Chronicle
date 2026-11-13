using UnityEngine;
using static SlotGameManager;

public class PointManager : MonoBehaviour 
{

    public static PointManager instance = null;

    public RoleType RoleType { get; private set; } = RoleType.Lose;

    public eTargetPoint TargetPoint { get; private set; } = eTargetPoint.Invalid;

    
    void Start()
    {
        instance = this;
        
    }


    /// <summary>
    /// ポイントを加算する
    /// </summary>
    /// <param name="point">現在の獲得済みポイント</param>
    /// <returns>NowPoint + 加算ポイントを返す</returns>
    public int AddPoint(int point, RoleType type)
    {

        // 子役ごとに加算するポイントを切り替える
        switch (type)
        {
            case RoleType.Lose:
                return point + 1;
                
            case RoleType.Bell:
                return point;

            case RoleType.Replay:
                return point + 5;

            case RoleType.Suika:
                return point + 10;

            case RoleType.Cherry:
                return point + 10;

            case RoleType.StrongCherry:
                return point + 20;

            case RoleType.ATBell:
                break;
        }
        return 0;
    }

    
    /// <summary>
    /// ATから通常時に戻ったときや、規定ポイントに達して前兆を踏んだ時にポイントをリセットする
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public int ResetPoint(int nowPoint)
    {
        return nowPoint = 0;
    }

    /// <summary>
    /// 規定ポイントに達して前兆に入ったときに超過しているポイントを次の周期に繰越す用
    /// </summary>
    /// <param name="targetPoint">規定ポイント</param>
    /// <param name="nowPoint">現在の獲得ポイント</param>
    /// <returns>規定ポイントからの超過ポイント分</returns>
    public int CarryoverOverflowPoint(eTargetPoint targetPoint, int nowPoint)
    {
        int overflowPoint;
        return overflowPoint = nowPoint - (int)targetPoint;
    }




}
