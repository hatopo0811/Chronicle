using UnityEngine;

public partial class SlotGameManager
{
    // =========================================
    // データカウンター
    // =========================================

    [Header("データカウンター")]
    [SerializeField]
    private SlotDataCounter dataCounter;


    // =========================================
    // ゲーム開始
    // =========================================

    private void StartGame()
    {
        if (gameStarted) return;
        int currentBet = betCount;
        if (!replayNextGame)
        {
            if (Credit < currentBet)
            {
                if (slotAudio != null) slotAudio.PlayCreditShortageSound();
                AddMedals();
                return;
            }
            Credit -= currentBet;
            UpdateCreditDisplay();
            if (slotAudio != null) slotAudio.PlayBetSound(currentBet);
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
        if (dataCounter != null) dataCounter.AddGame();

        // Tキー/チームAPIでATに切り替えた場合も、
        // 新しいゲームの開始時点でBellとATBellを確実に同期させる。
        RoleType drawnRole = DrawNormalRole();
        CurrentRole = NormalizeChronicleBellRole(drawnRole);
        if (drawnRole != CurrentRole)
        {
            Debug.Log("[クロニクル] ベル役補正: " + drawnRole + " -> " + CurrentRole);
        }
        PrepareChronicleATBellGame();
        Debug.Log("[クロニクル] モード=" + CurrentChronicleMode
            + " / 今回の成立役=" + GetRoleName(CurrentRole)
            + " / 内部ID=" + CurrentRole);
        if (DebugText != null) DebugText.text = GetRoleName(CurrentRole);
        SetReelTargets();
        StartAllReels();
        // 回転開始後、ATベルなら毎ゲーム新しい第1停止ナビを通知。
        NotifyChronicleNavigationAfterSpinStarted();
    }

    // =========================================
    // メダル追加
    // =========================================

    private void AddMedals()
    {
        int oldCredit =
            Credit;


        Credit +=
            PlusMedal;


        if (
            Credit >
            maxCredit
        )
        {
            Credit =
                maxCredit;
        }


        UpdateCreditDisplay();


        Debug.Log(
            "★ メダル追加"
        );


        Debug.Log(
            oldCredit +
            " → " +
            Credit +
            "枚"
        );


        if (slotAudio != null)
        {
            slotAudio
                .PlayMedalAddSound();
        }


        PanelON();
    }


    // =========================================
    // 3リール 疑似遊戯モード切替
    // =========================================

    private void SetPseudoPlayModeForAllReels(
        bool enabled
    )
    {
        if (leftReel != null)
        {
            leftReel.SetPseudoPlayMode(
                enabled
            );
        }


        if (centerReel != null)
        {
            centerReel.SetPseudoPlayMode(
                enabled
            );
        }


        if (rightReel != null)
        {
            rightReel.SetPseudoPlayMode(
                enabled
            );
        }
    }


    // =========================================
    // 3リール回転開始
    // =========================================

    private void StartAllReels()
    {
        canStopReels =
            false;


        spinStartTime =
            Time.time;


        if (slotAudio != null)
        {
            slotAudio
                .PlayLeverOnSound();
        }


        leftReel.StartSpin();


        centerReel.StartSpin();


        rightReel.StartSpin();


        gameStarted =
            true;


        Debug.Log(
            "リール回転開始"
        );


        Debug.Log(
            "停止受付まで：" +
            stopEnableDelay +
            "秒"
        );
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

                leftStopped =
                    true;

                break;


            case ReelController.ReelType.Center:

                centerStopped =
                    true;

                break;


            case ReelController.ReelType.Right:

                rightStopped =
                    true;

                break;
        }


        bool allStopped =
            leftStopped &&
            centerStopped &&
            rightStopped;


        if (slotAudio != null)
        {
            slotAudio
                .PlayReelStopSound();
        }


        if (
            allStopped &&
            slotAudio != null
        )
        {
            slotAudio
                .PlayThirdStopSound();
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
        canStopReels =
            false;


        gameStarted =
            false;


        Debug.Log(
            "3リール完全停止！ゲーム終了"
        );
    }
}