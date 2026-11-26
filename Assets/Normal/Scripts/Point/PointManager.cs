using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SlotGameManager;
using UnityEngine.UIElements;
using System.Collections;

public class PointManager : MonoBehaviour 
{

    public static PointManager instance = null;

    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private SlotGameManager gameManager;


    public RoleType RoleType { get; private set; } = RoleType.Lose;

    public eTargetPoint TargetPoint { get; private set; } = eTargetPoint.Invalid;

    // 現在のポイント
    public int NowPoint { get; set; } = 0;

    public int CarryoverPoint { get; private set; }


    void Start()
    {
        instance = this;

        NowPoint = 0;


    }

    private void Update()
    {
        pointText.text = NowPoint.ToString();


    }

    /// <summary>
    /// 規定ポイントに到達したときに走る前兆を開始する
    /// </summary>
    private void StartSigns()
    {
      
        

    }


    /// <summary>
    /// ポイントを加算する
    /// </summary>
    /// <param name="point">現在の獲得済みポイント</param>
    /// <returns>NowPoint + 加算ポイントを返す</returns>
    public void AddPoint( RoleType type)
    {
        // 通常時じゃなかったら終了 
        if (!Normal_GameManager.instance.IsNormal) return;


        // 子役ごとに加算するポイントを切り替える
        switch (type)
        {
            // １ポイント加算
            case RoleType.Lose:
                NowPoint += 1;
                break;
                break;
            // ポイント加算なし
            case RoleType.Bell:
                 NowPoint += 0;
                break;
            // ５ポイント加算
            case RoleType.Replay:
                NowPoint += 5;
                break;
            // １０ポイント加算
            case RoleType.Suika:
                NowPoint += 10;
                break;
            // ２０ポイント加算
            case RoleType.Cherry:
                NowPoint += 10;
                break;
            // ２０ポイント加算
            case RoleType.StrongCherry:
                NowPoint += 100;
                break;
            // ポイント加算なし
            case RoleType.ATBell:
                break;
        }
        
    }

    
    /// <summary>
    /// ATから通常時に戻ったときや、規定ポイントに達して前兆を踏んだ時にポイントをリセットする
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public void ResetPoint()
    {
        NowPoint = 0;
    }

    /// <summary>
    /// 規定ポイントに達して前兆に入ったときに超過しているポイントを次の周期に繰越す用
    /// </summary>
    /// <param name="targetPoint">規定ポイント</param>
    /// <param name="nowPoint">現在の獲得ポイント</param>
    /// <returns>規定ポイントからの超過ポイント分</returns>
    public void CarryoverOverflowPoint(eTargetPoint targetPoint, int nowPoint)
    {
        int overflowPoint;
        CarryoverPoint = overflowPoint = nowPoint - (int)targetPoint;
        
    }




}
