using UnityEngine;

public class PointManager : MonoBehaviour
{

    public static PointManager instance = null;

    
    

    void Start()
    {
        instance = this;

        
        

    }

    void Update()
    {
            
    }

    /// <summary>
    /// ポイントを加算する
    /// </summary>
    /// <param name="point">現在の獲得済みポイント</param>
    /// <returns>point + 加算ポイントを返す</returns>
    public int AddPoint(int point, SlotGameManager.RoleType type)
    {
        // 子役ごとに加算するポイントを切り替える
        // ★この列挙体はOHANAの物なのでリールが完成したら変える
        switch (type)
        {
            case SlotGameManager.RoleType.Lose:
                // 加算されたポイントを返す
                return point += 1;
                
            case SlotGameManager.RoleType.Replay:
                // 加算されたポイントを返す
                return point += 5;
                
            case SlotGameManager.RoleType.Grape:
                // 加算されたポイントを返す
                return point;
                
            case SlotGameManager.RoleType.Bell:
                // 加算されたポイントを返す
                return point;
                
            case SlotGameManager.RoleType.Cherry:
                // 加算されたポイントを返す
                return point += 10;
                
            case SlotGameManager.RoleType.Hibiscus:
                // 加算されたポイントを返す
                return point += 10;
                
        }
        return 0;
    }

    // ★
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
    /// 規定ポイントに達して前兆に入ったときに超過しているポイントを次の周期に繰越す
    /// </summary>
    /// <param name="targetPoint">規定ポイント</param>
    /// <param name="nowPoint">現在の獲得ポイント</param>
    /// <returns></returns>
    public int CarryoverOverflowPoint(GameEnum.eTargetPoint targetPoint, int nowPoint)
    {
        int dif = -1;
        switch (targetPoint)
        {
            case GameEnum.eTargetPoint.Point100:
                dif = nowPoint - (int)targetPoint;
                break;
            case GameEnum.eTargetPoint.Point200:
                dif = nowPoint - (int)targetPoint;
                break;
            case GameEnum.eTargetPoint.Point300:
                dif = nowPoint - (int)targetPoint;
                break;
            case GameEnum.eTargetPoint.Point400:
                dif = nowPoint - (int)targetPoint;
                break;
            case GameEnum.eTargetPoint.Point500:
                dif = nowPoint - (int)targetPoint;
                break;
            case GameEnum.eTargetPoint.Point600:
                dif = nowPoint - (int)targetPoint;
                break;
        }

        return dif;
    }




}
