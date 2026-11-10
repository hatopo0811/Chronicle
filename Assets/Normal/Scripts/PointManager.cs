using UnityEngine;

public class PointManager : MonoBehaviour
{

    public static PointManager instance = null;

    GameEnum.eRoleTypeTest roleType = GameEnum.eRoleTypeTest.hazure;

    int point = 0;

    void Start()
    {
        instance = this;
        

    }

    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.Return))
        {
            int random = Random.Range(0, 7);
            roleType = (GameEnum.eRoleTypeTest)random;
            Debug.Log(roleType.ToString());
            point = AddPoint(point, roleType);

            Debug.Log("現在のポイント：" + point);
        }
        
        



    }

    /// <summary>
    /// ポイントを加算する
    /// </summary>
    /// <param name="point">現在の獲得済みポイント</param>
    /// <returns>point + 加算ポイントを返す</returns>
    public int AddPoint(int point, GameEnum.eRoleTypeTest type)
    {
        // 子役ごとに加算するポイントを切り替える
        // ★この列挙体はOHANAの物なのでリールが完成したら変える
        switch (type)
        {
            case GameEnum.eRoleTypeTest.hazure:
                // 加算されたポイントを返す
                return point += 1;
                
            case GameEnum.eRoleTypeTest.Replay:
                // 加算されたポイントを返す
                return point += 5;
                
            case GameEnum.eRoleTypeTest.Bell:
                // 加算されたポイントを返す
                return point;
            
            case GameEnum.eRoleTypeTest.Watermelon:
                // 加算されたポイントを返す
                return point += 10;
            
            case GameEnum.eRoleTypeTest.Cherry:
                // 加算されたポイントを返す
                return point += 10;
            
            case GameEnum.eRoleTypeTest.HiCherry:
                // 加算されたポイントを返す
                return point += 20;
                
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
    /// 規定ポイントに達して前兆に入ったときに超過しているポイントを次の周期に繰越す用
    /// </summary>
    /// <param name="targetPoint">規定ポイント</param>
    /// <param name="nowPoint">現在の獲得ポイント</param>
    /// <returns>規定ポイントからの超過ポイント分</returns>
    public int CarryoverOverflowPoint(GameEnum.eTargetPoint targetPoint, int nowPoint)
    {
        int overflowPoint;
        return overflowPoint = nowPoint - (int)targetPoint;
    }




}
