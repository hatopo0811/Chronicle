
using System.Collections;
using UnityEngine;

public partial class SlotGameManager
{
    // =========================================
    // データカウンター
    // =========================================

    [Header("データカウンター")]
    [SerializeField]
    private SlotDataCounter dataCounter;

    [Header("前兆演出")]
    [SerializeField]
    private NightBattle nightBattle;


    // =========================================
    // ゲーム開始
    // =========================================

    private void StartGame()
    {
        if (GameStarted) return;

        int currentBet = betCount;

        if (!replayNextGame)
        {
            if (Credit < currentBet)
            {
                if (slotAudio != null)
                    slotAudio.PlayCreditShortageSound();

                AddMedals();
                return;
            }

            Credit -= currentBet;
            UpdateCreditDisplay();

            if (slotAudio != null)
                slotAudio.PlayBetSound(currentBet);
        }
        else
        {
            Debug.Log("リプレイ再遊技：BET不要");
            replayNextGame = false;
        }

        LastPayout = 0;
        UpdatePayDisplay();

        leftStopped = centerStopped = rightStopped = false;

        CurrentHitRole = HitRoleType.None;
        CurrentHitLine = PayLine.None;

        SetPseudoPlayModeForAllReels(false);

        if (dataCounter != null)
            dataCounter.AddGame();

        // =========================================
        // 小役抽選・ATベル補正
        // 既存処理を維持
        // =========================================

        RoleType drawnRole = DrawNormalRole();

        CurrentRole = NormalizeChronicleBellRole(drawnRole);

        if (drawnRole != CurrentRole)
        {
            Debug.Log(
                "[クロニクル] ベル役補正: " +
                drawnRole + " -> " + CurrentRole
            );
        }

        PrepareChronicleATBellGame();

        Debug.Log(
            "[クロニクル] モード=" +
            CurrentChronicleMode +
            " / 今回の成立役=" +
            GetRoleName(CurrentRole) +
            " / 内部ID=" +
            CurrentRole
        );

        if (DebugText != null)
            DebugText.text = GetRoleName(CurrentRole);

        SetReelTargets();

      

        StartAllReels();

       
    }


    // =========================================
    // メダル追加
    // =========================================

    private void AddMedals()
    {
        int oldCredit = Credit;

        Credit += PlusMedal;

        if (Credit > maxCredit)
        {
            Credit = maxCredit;
        }

        UpdateCreditDisplay();

        Debug.Log("★ メダル追加");

        Debug.Log(
            oldCredit +
            " → " +
            Credit +
            "枚"
        );

        if (slotAudio != null)
        {
            slotAudio.PlayMedalAddSound();
        }

        PanelON();
    }


    // =========================================
    // 3リール 疑似遊戯モード切替
    // =========================================

    private void SetPseudoPlayModeForAllReels(bool enabled)
    {
        if (leftReel != null)
        {
            leftReel.SetPseudoPlayMode(enabled);
        }

        if (centerReel != null)
        {
            centerReel.SetPseudoPlayMode(enabled);
        }

        if (rightReel != null)
        {
            rightReel.SetPseudoPlayMode(enabled);
        }
    }


    // =========================================
    // 3リール回転開始
    // =========================================

    private void StartAllReels()
    {
      

        if (Normal_GameManager.instance.IsYusyutuMode)
        {
            nightBattle.SelectEvent();

            if (
                nightBattle.IsFakeShutter ||
                nightBattle.IsShutter
            )
            {
                StartCoroutine(
                    nightBattle.CloseShutter()
                );
            }
        }

        // =========================================
        // ★変更：ウェイト開始
        // =========================================

        // 停止入力を禁止する
        canStopReels = false;

        // Update()側でウェイト中に
        // canStopReelsがtrueになるのを防ぐ
        spinStartTime = float.PositiveInfinity;

        // 待機中のSpace連打・モード切替を防ぐ
        GameStarted = true;

        Debug.Log(
            "[クロニクル] 回転前ウェイト開始：" +
            stopEnableDelay +
            "秒"
        );

        // ウェイト終了後に回転
        StartCoroutine(StartReelsAfterWait());
    }


    // =========================================
    // ★新規追加
    // ウェイト終了後のリール回転処理
    // =========================================

    private IEnumerator StartReelsAfterWait()
    {
        // =========================================
        // 指定時間待機
        // この間はリールを回転させない
        // =========================================

        yield return new WaitForSeconds(
            Mathf.Max(0f, stopEnableDelay)
        );

        // =========================================
        // 遊戯説明が表示されている場合は
        // 閉じるまで回転を開始しない
        // =========================================

        while (PlayGuidePanelManager.IsGuideOpen)
        {
            yield return null;
        }

        // =========================================
        // 既存の回転開始処理
        // =========================================

        if (slotAudio != null)
        {
            slotAudio.PlayLeverOnSound();
        }

        leftReel.StartSpin();
        centerReel.StartSpin();
        rightReel.StartSpin();

        // =========================================
        // ★変更：回転開始後に停止受付
        // =========================================

        spinStartTime = Time.time;

        canStopReels = true;

        Debug.Log(
            "[クロニクル] ウェイト終了・リール回転開始"
        );

        Debug.Log(
            "[クロニクル] 停止受付開始"
        );

        // =========================================
        // ★変更：回転後にナビ通知
        // =========================================

        NotifyChronicleNavigationAfterSpinStarted();
    }


    // =========================================
    // リール停止通知
    // =========================================

    public void NotifyReelStopped(
        ReelController.ReelType reelType
    )
    {
        switch (reelType)
        {
            case ReelController.ReelType.Left:

                leftStopped = true;

                break;


            case ReelController.ReelType.Center:

                centerStopped = true;


                if (
                    Normal_GameManager.instance.IsYusyutuMode &&
                    nightBattle.IsFakeShutter
                )
                {
                    StartCoroutine(
                        nightBattle.OpenShutter()
                    );
                }

                break;


            case ReelController.ReelType.Right:

                rightStopped = true;

                break;
        }

        bool allStopped =
            leftStopped &&
            centerStopped &&
            rightStopped;

        if (slotAudio != null)
        {
            slotAudio.PlayReelStopSound();
        }

        if (
            allStopped &&
            slotAudio != null
        )
        {
            slotAudio.PlayThirdStopSound();
        }

        CheckAllReelsStopped();
    }


    // =========================================
    // 全リール停止確認
    // =========================================

    private void CheckAllReelsStopped()
    {
        if (
            !leftStopped ||
            !centerStopped ||
            !rightStopped
        )
        {
            return;
        }

        if (
            Normal_GameManager.instance.IsYusyutuMode &&
            nightBattle.IsShutter
        )
        {
            nightBattle.UpdateShutterColor();

            StartCoroutine(
                nightBattle.OpenShutter()
            );
        }

        ShowStoppedSymbols();

        CheckHitRole();

        ProcessPayout();

        EndGame();
    }


    // =========================================
    // 停止図柄
    // =========================================

    private void ShowStoppedSymbols()
    {
        Debug.Log(
            "========== 停止図柄 =========="
        );

        Debug.Log(
            "上段：" +
            leftReel.GetUpperSymbol() +
            " | " +
            centerReel.GetUpperSymbol() +
            " | " +
            rightReel.GetUpperSymbol()
        );

        Debug.Log(
            "中段：" +
            leftReel.GetCenterSymbol() +
            " | " +
            centerReel.GetCenterSymbol() +
            " | " +
            rightReel.GetCenterSymbol()
        );

        Debug.Log(
            "下段：" +
            leftReel.GetLowerSymbol() +
            " | " +
            centerReel.GetLowerSymbol() +
            " | " +
            rightReel.GetLowerSymbol()
        );

        Debug.Log(
            "=============================="
        );
    }


    // =========================================
    // ゲーム終了
    // =========================================

    private void EndGame()
    {
        canStopReels = false;

        GameStarted = false;

        // PointManager.instance.NowPoint
        //     = PointManager.instance.AddPoint(
        //         PointManager.instance.NowPoint,
        //         CurrentRole
        //       );

        Debug.Log(
            "3リール完全停止！ゲーム終了"
        );
    }
}
