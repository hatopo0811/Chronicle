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


    [SerializeField] private GameObject Story;

    public RoleType RoleType { get; private set; } = RoleType.Lose;

    public eTargetPoint TargetPoint { get; private set; } = eTargetPoint.Invalid;

    // 現在のポイント
    public int NowPoint { get; set; } = 0;



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
    public int AddPoint(RoleType type)
    {
        // 通常時じゃなかったら終了 
        if (Normal_GameManager.instance.GameState != eGameState.Normal) return 0;

        // 子役ごとに加算するポイントを切り替える
        switch (type)
        {
            case RoleType.Lose:
                return 1;
                
            case RoleType.Bell:
                return 0;

            case RoleType.Replay:
                return 5;

            case RoleType.Suika:
                return 10;

            case RoleType.Cherry:
                return 10;

            case RoleType.StrongCherry:
                // ★優出モードに行くのがめんどくさいため20→200に変更デバックのみ
                return 200;

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
