using UnityEngine;

public class PointManager : MonoBehaviour
{

    public static PointManager instance = null;

    public eRoleTypeTest RoleType { get; private set; } = eRoleTypeTest.Invalid;

    public eTargetPoint TargetPoint { get; private set; } = eTargetPoint.Invalid;

    public int NowPoint{ get; private set; }

    // 仮===============================================================

    public int _GameCountTEST_TEST_TEST_TEST_ {  get; private set; }

    public bool _TEST_ReelStart_TEST_ { get; private set; }
    public bool _TEST_ReelStop_TEST_ { get; private set; }


    // 仮===============================================================





    void Start()
    {
        instance = this;
        

    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _TEST_ReelStart_TEST_ = true;
            _TEST_ReelStop_TEST_ = false;

        }

        if (Input.GetKeyDown(KeyCode.D) && _TEST_ReelStart_TEST_)
        {
            _TEST_ReelStart_TEST_ = false;
            _TEST_ReelStop_TEST_ = true;

            int random = Random.Range(0, 6);
            RoleType = (eRoleTypeTest)random;
            // 子役抽選結果表示
            Debug.Log(RoleType.ToString());
            NowPoint = AddPoint(NowPoint, RoleType);
            // ★========================================リールができてG数が管理できるようになるまで使う。出来たら絶対に消す
            _GameCountTEST_TEST_TEST_TEST_++;
            // ★========================================
            
        }

    }

    /// <summary>
    /// ポイントを加算する
    /// </summary>
    /// <param name="point">現在の獲得済みポイント</param>
    /// <returns>NowPoint + 加算ポイントを返す</returns>
    public int AddPoint(int point, eRoleTypeTest type)
    {
        // 子役ごとに加算するポイントを切り替える
        // ★この列挙体はOHANAの物なのでリールが完成したら変える
        switch (type)
        {
            case eRoleTypeTest.hazure:
                // 加算されたポイントを返す
                return point += 1;
                
            case eRoleTypeTest.Replay:
                // 加算されたポイントを返す
                return point += 5;
                
            case eRoleTypeTest.Bell:
                // 加算されたポイントを返す
                return point;
            
            case eRoleTypeTest.Watermelon:
                // 加算されたポイントを返す
                return point += 10;
            
            case eRoleTypeTest.Cherry:
                // 加算されたポイントを返す
                return point += 10;
            
            case eRoleTypeTest.HiCherry:
                // 加算されたポイントを返す
                return point += 20;
                
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
