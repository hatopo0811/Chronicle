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

    // シーンのロード中かどうか
    private bool isLoadScene;


    void Start()
    {
        instance = this;

        NowPoint = 0;

        isLoadScene = false;

    }

    private void Update()
    {
        pointText.text = NowPoint.ToString();


        
        if (((int)eTargetPoint.Point100 < NowPoint) && !isLoadScene)
        {
            // 複数回ロードされるのを防ぐ
            isLoadScene = true;
            // 仮の連即演出成功画像を表示
            Story.SetActive(true);
            StartCoroutine(LoadSceneAsync());

            // ポイントをリセットする
            PointManager.instance.ResetPoint(NowPoint);
            // 前兆中に獲得したポイントは次周期に持ち越される
            // ATに入った場合はゼロポイントからスタート　
            PointManager.instance.CarryoverOverflowPoint(eTargetPoint.Point100, NowPoint);




        }

    }

    /// <summary>
    /// シーン切り替えのかくつきをなくす
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("ZentyouScene");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // リールが回り始めたら
            if (gameManager.GameStarted)
            {
                // シーン移行の許可を出す
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

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
                // ★優出モードに行くのがめんどくさいため20→200に変更デバックのみ
                return point + 200;

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
