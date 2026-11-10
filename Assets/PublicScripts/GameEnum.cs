using UnityEngine;


public class GameEnum : MonoBehaviour 
{
    /// <summary>
    /// ‹K’èƒ|ƒCƒ“ƒg
    /// </summary>
    public enum eTargetPoint
    {
        Invalid = -1,
        Point100,
        Point200,
        Point300,
        Point400,
        Point500,
        Point600,
    }


    public enum eRivalMode
    {
        Invalid = -1,
        Default,
        Hamaoka,
        Enoki,
        Aosima,

    }

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