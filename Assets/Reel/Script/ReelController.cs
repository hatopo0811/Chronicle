using UnityEngine;
using System.Collections.Generic;

public partial class ReelController : MonoBehaviour
{
    // =========================================
    // リール種類
    // =========================================

    public enum ReelType
    {
        Left,
        Center,
        Right
    }


    // =========================================
    // 停止させる段
    // =========================================

    public enum TargetRow
    {
        Any,
        Upper,
        Center,
        Lower
    }


    // =========================================
    // Inspector
    // =========================================

    [Header("このリールの種類")]
    [SerializeField]
    private ReelType reelType =
        ReelType.Left;


    [Header("ゲーム管理")]
    [SerializeField]
    private SlotGameManager gameManager;


    [Header("回転設定")]
    [SerializeField]
    private float spinSpeed =
        2000f;


    // =========================================
    // リール状態
    // =========================================

    public bool isSpinning = false;

    public bool isStopping = false;

    private float reelOffset =
        0f;

    private float stopRemainingDistance =
        0f;

    private float reelLength;



    // =========================================
    // Start
    // =========================================

    private void Start()
    {
        SetReelData();

        if (
            reelData == null ||
            reelData.Count == 0
        )
        {
            return;
        }


        reelLength =
            reelData.Count *
            symbolHeight;


        CreateReel();

        UpdateSymbolPositions();
    }


    // =========================================
    // Update
    // =========================================

    private void Update()
    {
        if (isSpinning)
        {
            UpdateReel();
        }
    }


    // =========================================
    // リール回転処理
    // =========================================

    private void UpdateReel()
    {
        float moveDistance =
            spinSpeed *
            Time.deltaTime;


        // =====================================
        // 停止中
        // =========================================

        if (isStopping)
        {
            moveDistance =
                Mathf.Min(
                    moveDistance,
                    stopRemainingDistance
                );


            stopRemainingDistance -=
                moveDistance;
        }


        // =====================================
        // リール移動
        // =========================================

        reelOffset +=
            moveDistance;


        reelOffset =
            Mathf.Repeat(
                reelOffset,
                reelLength
            );


        UpdateSymbolPositions();


        // =====================================
        // 完全停止
        // =========================================

        if (
            isStopping &&
            stopRemainingDistance <= 0.001f
        )
        {
            // =================================
            // 図柄境界へ完全に合わせる
            // =================================

            reelOffset =
                Mathf.Round(
                    reelOffset /
                    symbolHeight
                ) *
                symbolHeight;


            reelOffset =
                Mathf.Repeat(
                    reelOffset,
                    reelLength
                );


            UpdateSymbolPositions();


            isSpinning =
                false;


            isStopping =
                false;


            stopRemainingDistance =
                0f;


            // =================================
            // デバッグ
            // =================================

            Debug.Log(
                "上：" +
                GetUpperSymbol() +
                " / 中：" +
                GetCenterSymbol() +
                " / 下：" +
                GetLowerSymbol()
            );


            // =================================
            // Managerへ停止通知
            // =================================

            if (gameManager != null)
            {
                gameManager.NotifyReelStopped(
                    reelType
                );
            }
        }
    }


    // =========================================
    // リール回転開始
    // =========================================

    public void StartSpin()
    {
        // =====================================
        // ★ 遊戯説明表示中は回転禁止
        // =========================================

        if (PlayGuidePanelManager.IsGuideOpen)
        {
            Debug.Log(
                GetReelName() +
                "：遊戯説明中なので回転禁止"
            );

            return;
        }


        // =====================================
        // すでに回転中
        // =========================================

        if (isSpinning)
        {
            return;
        }


        // =====================================
        // 回転開始
        // =========================================

        isSpinning =
            true;


        isStopping =
            false;


        stopRemainingDistance =
            0f;


        Debug.Log(
            GetReelName() +
            " 回転開始"
        );
    }


    // =========================================
    // リール名
    // =========================================

    private string GetReelName()
    {
        switch (reelType)
        {
            case ReelType.Left:

                return
                    "左リール";


            case ReelType.Center:

                return
                    "中リール";


            case ReelType.Right:

                return
                    "右リール";
        }


        return
            "リール";
    }
}