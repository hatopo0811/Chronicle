using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class NightBattle : MonoBehaviour
{
    

    // カンバス
    [SerializeField] private GameObject imageCanvas;
    
    
    // シャッターオブジェクト
    [SerializeField] private GameObject leftShutter;
    [SerializeField] private GameObject rightShutter;
    [SerializeField] private RectTransform leftShutterTransform;
    [SerializeField] private RectTransform rightShutterTransform;
    

    // テキスト
    [SerializeField] private TextMeshProUGUI MaxEventColor;
    [SerializeField] private TextMeshProUGUI NowEventColor;
    [SerializeField] private TextMeshProUGUI nowEvent;

    [SerializeField] private Image testBG;
    


    /// <summary>
    /// 演出の色
    /// </summary>
    private enum eEventColor
    {
        Invalid = -1,
        White,
        Blue,
        Yellow,
        Red,
        Rainbow,
    }
    private eEventColor eventColorMax = eEventColor.Invalid;
    private eEventColor CurrentEventColor = eEventColor.Invalid;

    
    public eEventType CurrentEventType {  get; private set; } = eEventType.Invalid;



    // フェイクシャッターだったらtrue
    public bool IsFakeShutter { get { return CurrentEventType == eEventType.FakeShutter; } }
    // 通常シャッターだったらtrue
    public bool IsShutter { get { return CurrentEventType == eEventType.Shutter; } }
    // 前兆の経過ゲーム数
    public int CurrentNightBattleGameCount {  get; private set; }
    // 前兆最大ゲーム数
    private const int MaxBattleGame = 15;
    // シャッターがしまっているかどうか
    private bool isClose = false;
    // 色が上がるシャッターと上がらないシャッターを切り変える
    public bool isFakeShutter = false;
    // ジャッジのゲーム数
    private const int NightBattleGameNum = 3;
    // 
    public int nowJudgeGameCount = 0;


    private void OnEnable()
    {
        Normal_GameManager.instance.GameState = eGameState.NightBattle;
        StartCoroutine(HideImage());
    }


    void Start()
    {

        // 突破率を突入時に決定
        eventColorMax = SelectEventColor();
        CurrentEventColor = eEventColor.White;
        testBG.color = Color.white;
        Debug.Log(eventColorMax.ToString());
        MaxEventColor.text = "EventColorMax : " + eventColorMax.ToString();
        NowEventColor.text = "NowEventColor : " + CurrentEventColor.ToString();


        // シャッターの初期位置開いている状態
        leftShutterTransform.anchoredPosition
            = new Vector2(GameConst.OPEN_LEFT_SHUTTER_POS_X, 0);
        rightShutterTransform.anchoredPosition
            = new Vector2(GameConst.OPEN_RIGHT_SHUTTER_POS_X, 0);

    }
    

    void Update()
    {
        
        // デバック用 : シャッター演出
        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartCoroutine(CloseShutter());
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            StartCoroutine(OpenShutter());
        }

        //前兆が始まって１５ゲーム経ったら
        if (CurrentNightBattleGameCount == MaxBattleGame)
        {
            // ジャッジ突入演出


        }


        // 背景色アップ演出 順番：白→青→黄色→赤→虹 期待度：青＜黄色＜白＝赤＜虹



        // ペラアタック演出(α版) → 可(本前兆濃厚)→良(子役・レア役示唆)→優(強レア役)→超(本前兆期待度UP)


        // AT突入ジャッジ


    }

    /// <summary>
    /// 決戦前夜中のゲーム数を加算する
    /// </summary>
    public void PlusOne_NightBattleGameCount()
    {
        if (!Normal_GameManager.instance.IsNightBattle) return;
        CurrentNightBattleGameCount++;
    }

    /// <summary>
    /// 決戦前夜突入時に突破率を決める
    /// </summary>
    /// <returns></returns>
    private eEventColor SelectEventColor()
    {

        //　★
        if (CurrentNightBattleGameCount == MaxBattleGame) ;

        int eventColor = Random.Range(GameConst.RANDOM_MIN, GameConst.RANDOM_MAX);

        if (eventColor < 3)
        {
            return eventColorMax = eEventColor.White;
        }
        else if (eventColor < 41)
        {
            return eventColorMax = eEventColor.Blue;
        }
        else if (eventColor < 91)
        {
            return eventColorMax = eEventColor.Yellow;
        }
        else if (eventColor <98)
        {
            return eventColorMax = eEventColor.Red;
        }
        else
        {
            return eventColorMax = eEventColor.Rainbow;
        }

    }

    // judgeゲーム数の減算　
    public void SubtractJudgeGame()
    {

    }

    /// <summary>
    /// どの演出を出すかレバーONで抽選、その関数を呼び出す
    /// </summary>
    public void SelectEvent()
    {
        // 抽選
        CurrentEventType = (eEventType)Random.Range(0, (int)eEventType.EventMax);

        // 呼び出し
        switch(CurrentEventType)
        {
            case eEventType.None:           // 演出無し
                CurrentEventType = eEventType.None;
                nowEvent.text = "NowEvent : " + eEventType.None.ToString();
                break;
            case eEventType.Shutter:        // シャッター演出
                CurrentEventType = eEventType.Shutter;
                nowEvent.text = "NowEvent : " + eEventType.Shutter.ToString();
                CloseShutter();
                if (eventColorMax != CurrentEventColor)
                {
                    CurrentEventColor++;
                    NowEventColor.text = "NowEventColor : " + CurrentEventColor.ToString();
                }
                
                break;
            case eEventType.FakeShutter:    // フェイクシャッター演出
                CurrentEventType = eEventType.FakeShutter;
                nowEvent.text = "NowEvent : " + eEventType.FakeShutter.ToString();
                CloseShutter();
                break;

            default:
                break;
        }

    }

    /// <summary>
    /// シャッター演出の時色を1段階上げる
    /// 最終的には画像を切り替えることができるようにする。
    /// </summary>
    public void UpdateShutterColor()
    {
        

        switch (CurrentEventColor)
        {
            case eEventColor.White:
                testBG.color = Color.white;
                break;

            case eEventColor.Blue:
                testBG.color = Color.blue;
                break;

            case eEventColor.Yellow:
                testBG.color = Color.yellow;
                break;

            case eEventColor.Red:
                testBG.color = Color.red;
                break;

            case eEventColor.Rainbow:
                // 虹色用の演出を再生
                break;
        }
    }

    /// <summary>
    /// シャッター開く演出
    /// </summary>
    /// <returns></returns>
    public IEnumerator OpenShutter()
    {
        if (!Normal_GameManager.instance.IsNightBattle) yield break;

            // 開いた状態はX座標±530
            // 閉まっているシャッターを画面外(左右)に移動
            float t = 0.0f;
        float duration = 0.1f;

        while (t < 1.0f)
        {
            isClose = false;
            t += Time.deltaTime / duration;

            t = Mathf.Clamp01(t);

            // スロット画面外に表示されているものを画面内に移動させる
            leftShutterTransform.anchoredPosition =
                new Vector2(Mathf.Lerp(
                    GameConst.CLOSE_LEFT_SHUTTER_POS_X,
                    GameConst.OPEN_LEFT_SHUTTER_POS_X, t), 0);

            rightShutterTransform.anchoredPosition =
                new Vector2(Mathf.Lerp(
                GameConst.CLOSE_RIGHT_SHUTTER_POS_X,
                GameConst.OPEN_RIGHT_SHUTTER_POS_X, t), 0);


            yield return null;
        }
        UpdateShutterColor();

    }

    /// <summary>
    /// シャッター閉まる演出
    /// </summary>
    /// <returns></returns>
    public IEnumerator CloseShutter()
    {
        if (!Normal_GameManager.instance.IsNightBattle) yield break;

            // 閉まった状態は右X座標178左-200

            float t = 0.0f;
        float duration = 0.1f;

        while (t < 1.0f)
        {
            isClose = true;

            t += Time.deltaTime / duration;

            t = Mathf.Clamp01(t);

            // スロット画面外に表示されているものを画面内に移動させる
            leftShutterTransform.anchoredPosition =
                new Vector2(Mathf.Lerp(
                GameConst.OPEN_LEFT_SHUTTER_POS_X,
                GameConst.CLOSE_LEFT_SHUTTER_POS_X, t), 0);

            rightShutterTransform.anchoredPosition = 
                new Vector2(Mathf.Lerp(
                GameConst.OPEN_RIGHT_SHUTTER_POS_X,
                GameConst.CLOSE_RIGHT_SHUTTER_POS_X, t), 0);
            
            
            yield return null;
        }


    }

    /// <summary>
    /// 突入時の画像を消す
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideImage()
    {
        yield return new WaitForSeconds(3f);

        imageCanvas.SetActive(false);
    }

    




}
