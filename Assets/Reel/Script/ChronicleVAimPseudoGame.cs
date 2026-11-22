using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 通常SlotGameManager / ReelControllerを変更せず動く、独立した疑似遊技。
// 3x3のUI Imageで20コマ配列を再現。実際の通常リールは動かさない。
public class ChronicleVAimPseudoGame : MonoBehaviour
{
    [Header("既存演出を再利用")]
    [SerializeField] private ChronicleVAimPresentation presentation;
    [SerializeField] private SlotGameManager slotGameManager;
    [SerializeField] private RawImage backgroundRawImage;
    [Header("疑似リール画面：通常リールと同じ位置に重ねる")]
    [SerializeField] private GameObject pseudoReelsRoot;
    [Tooltip("上段・中段・下段の順で3つ設定")]
    [SerializeField] private Image[] leftSymbols = new Image[3];
    [SerializeField] private Image[] centerSymbols = new Image[3];
    [SerializeField] private Image[] rightSymbols = new Image[3];

    [Header("図柄画像：通常リールと同じSpriteを設定")]
    [SerializeField] private Sprite bell;
    [SerializeField] private Sprite replay;
    [SerializeField] private Sprite blueV;
    [SerializeField] private Sprite bar;
    [SerializeField] private Sprite suika;
    [SerializeField] private Sprite cherry;
    [SerializeField] private Sprite lose;
    [SerializeField] private Sprite pinkV;

    [Header("カットイン／背景差分（いずれも任意）")]
    [SerializeField] private Texture blueBackground;
    [SerializeField] private Texture redBackground;
    [SerializeField] private Texture rainbowBackground;
    [SerializeField] private Texture premiumBackground;
    [Range(0, 100)][SerializeField] private float successRedUpgradePercent = 50f;
    [Range(0, 100)][SerializeField] private float fakeRedUpgradePercent = 5f;
    [SerializeField] private Image aimTextImage;
    [SerializeField] private Sprite blueAimText;
    [SerializeField] private Sprite redAimText;
    [SerializeField] private Sprite premiumAimText;

    [Header("演出速度・入力")]
    [Min(0.02f)][SerializeField] private float reelFrameSeconds = 0.075f;
    [Min(0.02f)][SerializeField] private float stopStepSeconds = 0.055f;
    [Min(0f)][SerializeField] private float resultHoldSeconds = 2f;
    [Tooltip("疑似遊技中は通常GameManagerのキー入力を止める。状態は終了後に戻す")]
    [SerializeField] private bool suspendMainGameInput = true;

    // AT管理チームから購読する。実際の報酬付与はここでは行わない。
    public event Action<ChronicleVAimPresentation.VAimOutcome, string> PseudoVAimCompleted;
    public bool IsPlaying { get; private set; }
    public string PressOrder { get; private set; } = "";

    private readonly Image[][] symbolSlots = new Image[3][];
    private readonly int[] centerIndices = new int[3];
    private readonly bool[] spinning = new bool[3];
    private ChronicleVAimPresentation.VAimOutcome outcome;
    private bool stopAnimating;
    private int stopCount;
    private float frameTimer;
    private Texture initialBackground;
    private bool wasManagerEnabled;
    private Coroutine stopCoroutine;
    private Coroutine finishCoroutine;

    // 配列は元ReelMaster.csvと同じ20→1の並び。
    private static readonly string[][] Strips =
    {
        new[] {"Cherry","Suika","Bell","Replay","PinkV","Cherry","Suika","Bell","Replay","BAR","Cherry","Suika","Bell","Replay","BlueV","BlueV","Suika","Bell","Replay","Lose"},
        new[] {"Lose","PinkV","Replay","Cherry","Bell","Lose","Suika","Replay","Cherry","Bell","BAR","Suika","Replay","Cherry","Bell","Lose","BlueV","Replay","Cherry","Bell"},
        new[] {"Lose","Bell","Replay","Suika","Cherry","PinkV","Bell","Replay","Suika","Cherry","BAR","Bell","Replay","Suika","Cherry","BlueV","BlueV","Bell","Replay","Cherry"}
    };

    private void Awake()
    {
        symbolSlots[0] = leftSymbols;
        symbolSlots[1] = centerSymbols;
        symbolSlots[2] = rightSymbols;
        if (pseudoReelsRoot != null) pseudoReelsRoot.SetActive(false);
    }

    private void Update()
    {
        if (!IsPlaying) return;
        if (Keyboard.current != null && !stopAnimating && stopCount < 3)
        {
            // 右第一停止固定。2停止目は左・中の好きな方。
            if (Keyboard.current.dKey.wasPressedThisFrame) RequestStop(2, 'R');
            else if (Keyboard.current.aKey.wasPressedThisFrame) RequestStop(0, 'L');
            else if (Keyboard.current.sKey.wasPressedThisFrame) RequestStop(1, 'C');
        }

        frameTimer += Time.deltaTime;
        if (frameTimer >= Mathf.Max(0.02f, reelFrameSeconds))
        {
            frameTimer = 0f;
            for (int r = 0; r < 3; r++)
            {
                if (!spinning[r]) continue;
                centerIndices[r] = Wrap(centerIndices[r] - 1);
                RenderReel(r);
            }
        }
    }

    [ContextMenu("テスト：疑似遊技を開始")]
    private void DebugBegin()
    {
        if (Application.isPlaying) BeginPseudoVAim();
        else Debug.LogWarning("[V疑似遊技] Playモードで実行してください", this);
    }

    [ContextMenu("テスト：疑似遊技を終了")]
    private void DebugEnd() { EndPseudoVAim(); }

    public void BeginPseudoVAim()
    {
        if (IsPlaying) return;
        if (presentation == null || pseudoReelsRoot == null || !ValidateSlots())
        {
            Debug.LogError("[V疑似遊技] 演出参照／疑似リール9画像が未設定です", this);
            return;
        }
        if (slotGameManager != null && slotGameManager.GameStarted)
        {
            Debug.LogWarning("[V疑似遊技] 通常ゲーム中は開始できません。3リール停止後に実行してください", this);
            return;
        }

        presentation.BeginVAim();
        if (!presentation.IsPlaying) return;
        outcome = presentation.CurrentOutcome;
        PressOrder = "";
        stopCount = 0;
        stopAnimating = false;
        frameTimer = 0f;
        initialBackground = backgroundRawImage != null ? backgroundRawImage.texture : null;
        SetBackground(blueBackground);
        SetText(blueAimText);

        pseudoReelsRoot.SetActive(true);
        for (int r = 0; r < 3; r++)
        {
            spinning[r] = true;
            centerIndices[r] = UnityEngine.Random.Range(0, 20);
            RenderReel(r);
        }
        if (slotGameManager != null && suspendMainGameInput)
        {
            wasManagerEnabled = slotGameManager.enabled;
            slotGameManager.enabled = false;
        }
        IsPlaying = true;
        Debug.Log("[V疑似遊技] 開始 / 右第一停止 / D→S→A または D→A→S / 内部=" + outcome);
    }

    private void RequestStop(int reel, char key)
    {
        if (!spinning[reel]) return;
        if (stopCount == 0 && reel != 2)
        {
            Debug.Log("[V疑似遊技] 第一停止は右(D)限定です");
            return;
        }
        stopAnimating = true;
        spinning[reel] = false;
        PressOrder += key;
        stopCoroutine = StartCoroutine(StopReelAnimation(reel));
    }

    private IEnumerator StopReelAnimation(int reel)
    {
        int goal = GetGoalCenterIndex(reel);
        // ピンクプレミア：右停止時に青V(6番)を一度見せてからピンクV(15番)まで滑る。
        // 疑似遊技専用の長いスベリ演出で、通常の4コマ制限とは無関係。
        if (outcome == ChronicleVAimPresentation.VAimOutcome.PinkV && reel == 2)
        {
            centerIndices[reel] = 14; // 6番BlueV
            RenderReel(reel);
            yield return new WaitForSeconds(0.22f);
        }
        int steps = (centerIndices[reel] - goal + 20) % 20;
        if (steps < 4) steps += 20; // 一定の停止アニメーションを確保
        for (int n = 0; n < steps; n++)
        {
            centerIndices[reel] = Wrap(centerIndices[reel] - 1);
            RenderReel(reel);
            yield return new WaitForSeconds(Mathf.Max(0.02f, stopStepSeconds));
        }
        centerIndices[reel] = goal;
        RenderReel(reel);
        stopCount++;
        stopAnimating = false;
        Debug.Log("[V疑似遊技] 停止" + stopCount + " / " + PressOrder);

        if (stopCount == 1)
        {
            if (outcome == ChronicleVAimPresentation.VAimOutcome.PinkV)
            {
                SetBackground(premiumBackground);
                SetText(premiumAimText);
            }
            else
            {
                float chance = outcome == ChronicleVAimPresentation.VAimOutcome.Fake
                    ? fakeRedUpgradePercent : successRedUpgradePercent;
                if (UnityEngine.Random.value * 100f < chance)
                {
                    SetBackground(redBackground);
                    SetText(redAimText);
                }
            }
        }
        if (stopCount == 3)
        {
            if (outcome == ChronicleVAimPresentation.VAimOutcome.DoubleV && PressOrder == "RLC")
                SetBackground(rainbowBackground);
            else if (outcome == ChronicleVAimPresentation.VAimOutcome.PinkV)
                SetBackground(premiumBackground);
            Debug.Log("[V疑似遊技] 結果=" + outcome + " / 押し順=" + PressOrder +
                " / キャラ=" + presentation.CurrentCharacterId);
            PseudoVAimCompleted?.Invoke(outcome, PressOrder);
            finishCoroutine = StartCoroutine(FinishAfterHold());
        }
        stopCoroutine = null;
    }

    private IEnumerator FinishAfterHold()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, resultHoldSeconds));
        EndPseudoVAim();
    }

    public void EndPseudoVAim()
    {
        if (stopCoroutine != null) { StopCoroutine(stopCoroutine); stopCoroutine = null; }
        if (finishCoroutine != null) { StopCoroutine(finishCoroutine); finishCoroutine = null; }
        bool wasPlaying = IsPlaying;
        IsPlaying = false;
        stopAnimating = false;
        if (pseudoReelsRoot != null) pseudoReelsRoot.SetActive(false);
        if (presentation != null) presentation.EndVAim();
        if (backgroundRawImage != null) backgroundRawImage.texture = initialBackground;
        if (slotGameManager != null && suspendMainGameInput && wasPlaying)
            slotGameManager.enabled = wasManagerEnabled;
    }

    private void OnDisable()
    {
        if (IsPlaying) EndPseudoVAim();
    }

    private int GetGoalCenterIndex(int reel)
    {
        switch (outcome)
        {
            case ChronicleVAimPresentation.VAimOutcome.Fake:
                // 左右中段青V、中リール中段Replayで揃わない
                return reel == 1 ? 2 : 14;
            case ChronicleVAimPresentation.VAimOutcome.SingleV:
                // 中段青V揃い。左右2連Vが上下にずれて見える
                return reel == 0 ? 15 : reel == 1 ? 16 : 14;
            case ChronicleVAimPresentation.VAimOutcome.DoubleV:
                // 両側に2連青V + 中央に青V（疑似遊技用のダブルV見せ方）
                return reel == 1 ? 16 : 14;
            case ChronicleVAimPresentation.VAimOutcome.PinkV:
                // 左16番、中19番、右15番のピンクVを中段へ
                return reel == 0 ? 4 : reel == 1 ? 1 : 5;
        }
        return 0;
    }

    private bool ValidateSlots()
    {
        for (int r = 0; r < 3; r++)
        {
            if (symbolSlots[r] == null || symbolSlots[r].Length != 3) return false;
            for (int row = 0; row < 3; row++)
                if (symbolSlots[r][row] == null) return false;
        }
        return true;
    }

    private void RenderReel(int reel)
    {
        int middle = centerIndices[reel];
        string[] strip = Strips[reel];
        SetSprite(symbolSlots[reel][0], strip[Wrap(middle - 1)]);
        SetSprite(symbolSlots[reel][1], strip[middle]);
        SetSprite(symbolSlots[reel][2], strip[Wrap(middle + 1)]);
    }

    private void SetSprite(Image img, string symbol)
    {
        switch (symbol)
        {
            case "Bell": img.sprite = bell; break;
            case "Replay": img.sprite = replay; break;
            case "BlueV": img.sprite = blueV; break;
            case "BAR": img.sprite = bar; break;
            case "Suika": img.sprite = suika; break;
            case "Cherry": img.sprite = cherry; break;
            case "Lose": img.sprite = lose; break;
            case "PinkV": img.sprite = pinkV; break;
        }
        img.enabled = img.sprite != null;
    }

    private void SetBackground(Texture texture)
    {
        if (backgroundRawImage != null && texture != null)
            backgroundRawImage.texture = texture;
    }
    private void SetText(Sprite sprite)
    {
        if (aimTextImage != null && sprite != null) aimTextImage.sprite = sprite;
    }
    private static int Wrap(int index) { return (index % 20 + 20) % 20; }
}
