using UnityEngine;


public class GameEnum : MonoBehaviour 
{

    /// <summary>
    /// モード
    /// A、B、天国
    /// </summary>
    public enum eNormalMode
    {
        Invalid = -1,
        A,          // 55%
        B,          // 30%
        Heaven,     // 10%
        SokuYusyutu,// 5%
        // ↓は朝一や上位AT後のみ出現するモードなので通常AT後などの抽選には含まれない
        Morning,
        AfterHiAT,
    }

    /// <summary>
    /// 周期数
    /// </summary>
    public enum eCycleCount
    {
        Invalid = -1,
        First,
        Second,
        Third,
        Fourth,
        Fifth,
        Six,
        Max,
    }

    /// <summary>
    /// 規定ポイント
    /// </summary>
    public enum eTargetPoint
    {
        Invalid = -1,
        Point100 = 100,
        Point200 = 200,
        Point300 = 300,
        Point400 = 400,
        Point500 = 500,
        Point600 = 600,
    }



    /// <summary>
    /// ライバルモード
    /// </summary>
    public enum eRivalMode
    {
        Invalid = -1,
        Default,
        Hamaoka,
        Enoki,
        Aosima,

    }

    /// <summary>
    /// 子役の種類
    /// </summary>
    public enum eRoleTypeTest
    {
        Invalid = -1,
        hazure,
        Replay,
        Bell,
        Watermelon,
        Cherry,
        HiCherry,
    }

    


}