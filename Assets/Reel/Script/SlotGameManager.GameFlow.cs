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
        if (gameStarted)
        {
            Debug.Log(
                "すでにゲーム中です。"
            );

            return;
        }


        LastPayout =
            0;


        UpdatePayDisplay();


        selectedReachPattern =
            null;


        selectedLosePattern =
            null;


        currentHitPattern =
            null;


        currentReachPattern =
            null;


        currentLosePattern =
            null;


        // =====================================
        // 疑似遊戯状態をいったん解除
        // =========================================

        SetPseudoPlayModeForAllReels(
            false
        );


        // =====================================
        // 停止状態初期化
        // =========================================

        leftStopped =
            false;


        centerStopped =
            false;


        rightStopped =
            false;


        CurrentHitRole =
            HitRoleType.None;


        CurrentHitLine =
            PayLine.None;


        // =====================================
        // ボーナス確定後だけ疑似遊戯
        // =========================================

        if (ShouldStartBonusPseudoPlay())
        {
            SetPseudoPlayModeForAllReels(
                true
            );


            CurrentRole =
                PendingBonus ==
                BonusType.Big
                ? RoleType.BigBonus
                : RoleType.RegBonus;


            targetPayLine =
                SelectTargetPayLine();


            Debug.Log(
                "★ ボーナス確定後の疑似遊戯開始：" +
                GetBonusName(
                    PendingBonus
                )
            );


            if (DebugText != null)
            {
                DebugText.text =
                    PendingBonus ==
                        BonusType.Big
                    ? "疑似遊戯 BIG BONUS"
                    : "疑似遊戯 REG BONUS";
            }


            SetReelTargets();


            StartAllReels();


            return;
        }


        // =====================================
        // BET枚数
        // =========================================

        int currentBet =
            ActiveBonus !=
            BonusType.None
            ? bonusBetCount
            : betCount;


        // =====================================
        // Replay
        // =========================================

        if (
            replayNextGame &&
            ActiveBonus ==
            BonusType.None
        )
        {
            replayNextGame =
                false;


            Debug.Log(
                "Replay再遊技 → BET不要"
            );
        }
        else
        {
            // =================================
            // CREDIT不足
            // =========================================

            if (
                Credit <
                currentBet
            )
            {
                Debug.Log(
                    "CREDIT不足！"
                );


                Debug.Log(
                    "現在：" +
                    Credit +
                    "枚 / 必要：" +
                    currentBet +
                    "枚"
                );


                if (slotAudio != null)
                {
                    slotAudio
                        .PlayCreditShortageSound();
                }


                AddMedals();


                return;
            }


            // =================================
            // BET
            // =========================================

            Credit -=
                currentBet;


            UpdateCreditDisplay();


            if (slotAudio != null)
            {
                slotAudio
                    .PlayBetSound(
                        currentBet
                    );
            }


            Debug.Log(
                currentBet +
                "BET"
            );


            Debug.Log(
                "現在CREDIT：" +
                Credit
            );
        }


        // =====================================
        // ★ 通常ゲーム数加算
        //
        // BONUS中は加算しない
        // 疑似遊戯は上でreturn済み
        // Replayは1Gとして加算
        // =========================================

        if (
            ActiveBonus ==
            BonusType.None
        )
        {
            if (dataCounter != null)
            {
                dataCounter.AddGame();
            }
        }


        // =====================================
        // BONUS消化中
        // =========================================

        if (
            ActiveBonus !=
            BonusType.None
        )
        {
            CurrentRole =
                RoleType.Grape;


            targetPayLine =
                SelectTargetPayLine();


            SetThreeReelTarget(
                "Grape",
                "Grape",
                "Grape",
                targetPayLine
            );


            UpdateBonusDebugText();


            StartAllReels();


            return;
        }


        // =====================================
        // 通常時
        // =========================================

        if (
            PendingBonus ==
            BonusType.None
        )
        {
            CurrentRole =
                DrawNormalRole();


            // =================================
            // BIG内部当選
            // =========================================

            if (
                CurrentRole ==
                RoleType.BigBonus
            )
            {
                PendingBonus =
                    BonusType.Big;


                reachShownForCurrentBonus =
                    false;


                bonusLampNotified =
                    false;


                Debug.Log(
                    "★ BIG BONUS内部当選！"
                );


                SelectReachPatternForThisGame();
            }


            // =================================
            // REG内部当選
            // =========================================

            else if (
                CurrentRole ==
                RoleType.RegBonus
            )
            {
                PendingBonus =
                    BonusType.Reg;


                reachShownForCurrentBonus =
                    false;


                bonusLampNotified =
                    false;


                Debug.Log(
                    "★ REG BONUS内部当選！"
                );


                SelectReachPatternForThisGame();
            }


            // =================================
            // Lose
            // =========================================

            else if (
                CurrentRole ==
                RoleType.Lose
            )
            {
                SelectLosePatternForThisGame();
            }
        }


        // =====================================
        // BONUS持ち越し中
        // =========================================

        else
        {
            CurrentRole =
                DrawRoleDuringBonusPending();


            Debug.Log(
                "★ ボーナスフラグ持ち越し中"
            );


            if (
                CurrentRole ==
                    RoleType.Lose
                &&
                !reachShownForCurrentBonus
            )
            {
                SelectReachPatternForThisGame();
            }
        }


        // =====================================
        // 狙いライン
        // =========================================

        targetPayLine =
            SelectTargetPayLine();


        Debug.Log(
            "========================"
        );


        Debug.Log(
            "今回の成立役：" +
            GetRoleName(
                CurrentRole
            )
        );


        Debug.Log(
            "保持中ボーナス：" +
            GetBonusName(
                PendingBonus
            )
        );


        if (
            selectedLosePattern !=
            null
        )
        {
            Debug.Log(
                "今回のハズレ目：" +
                selectedLosePattern.ID +
                " / " +
                selectedLosePattern.Name
            );
        }


        if (
            selectedReachPattern !=
            null
        )
        {
            Debug.Log(
                "今回のリーチ目：" +
                selectedReachPattern.ID +
                " / " +
                selectedReachPattern.Name
            );
        }


        Debug.Log(
            "========================"
        );


        if (DebugText != null)
        {
            DebugText.text =
                GetRoleName(
                    CurrentRole
                );
        }


        SetReelTargets();


        StartAllReels();
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
    // ボーナス確定後の疑似遊戯判定
    // =========================================

    private bool ShouldStartBonusPseudoPlay()
    {
        return
            ActiveBonus ==
                BonusType.None
            &&
            PendingBonus !=
                BonusType.None
            &&
            bonusLampNotified;
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


        bool wasInBonus =
            ActiveBonus !=
            BonusType.None;


        ShowStoppedSymbols();


        CheckHitRole();


        CheckBonusLampNotification();


        ProcessPayout();


        if (!wasInBonus)
        {
            CheckBonusResult();
        }


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


        selectedReachPattern =
            null;


        selectedLosePattern =
            null;


        Debug.Log(
            "3リール完全停止！ゲーム終了"
        );
    }
}