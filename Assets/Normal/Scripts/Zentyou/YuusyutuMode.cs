using System.Collections;
using TMPro;
using UnityEngine;



public class YuusyutuMode : MonoBehaviour
{

    // カンバス
    [SerializeField] private GameObject imageCanvas;
    [SerializeField] private GameObject ShutterCanvas;
    
    // シャッターオブジェクト
    [SerializeField] private GameObject leftShutter;
    [SerializeField] private GameObject rightShutter;
    [SerializeField] private RectTransform leftShutterTransform;
    [SerializeField] private RectTransform rightShutterTransform;
    

    // テキスト
    [SerializeField] private TextMeshProUGUI nowEventColor;
    

    /// <summary>
    /// 演出の色
    /// </summary>
    public enum eEventColor
    {
        Invalid = -1,
        White,
        Brue,
        Yellow,
        Red,
        Reinbow,
    }

    private eEventColor eventColorMax = eEventColor.Invalid;
    /// <summary>
    /// 前兆中の演出の種類
    /// </summary>
    private enum eEventType
    {
        Invalid = -1,
        Shutter,


        EventMax,
    }

    private eEventType CurrentEventType = eEventType.Invalid;


    private void Awake()
    {
        imageCanvas.SetActive(true);
        StartCoroutine(HideImage());
    }

    void Start()
    {
        // 突破率を突入時に決定
        eventColorMax = SelectEventColor();
        Debug.Log(eventColorMax.ToString());
        nowEventColor.text = eventColorMax.ToString();


        // シャッターの初期位置開いている状態
        leftShutterTransform.anchoredPosition
            = new Vector2(GameConst.OPEN_LEFT_SHUTTER_POS_X, 0);
        rightShutterTransform.anchoredPosition
            = new Vector2(GameConst.OPEN_RIGHT_SHUTTER_POS_X, 0);


    }

    void Update()
    {
        SelectEvent();
        // シャッター演出

        if (Input.GetKeyDown(KeyCode.F1))
        {
            CloseShutter();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            OpenShutter();
        }

        // 背景色アップ演出 順番：白→青→黄色→赤→虹 期待度：青＜黄色＜白＝赤＜虹



        // ペラアタック演出(α版) → 可(本前兆濃厚)→良(子役・レア役示唆)→優(強レア役)→超(本前兆期待度UP)


        // AT突入ジャッジ


    }

    private eEventColor SelectEventColor()
    {
        int eventColor = Random.Range(GameConst.RANDOM_MIN, GameConst.RANDOM_MAX);

        if (eventColor < 3)
        {
            return eventColorMax = eEventColor.White;
        }
        else if (eventColor < 41)
        {
            return eventColorMax = eEventColor.Brue;
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
            return eventColorMax = eEventColor.Reinbow;
        }

    }


    /// <summary>
    /// どの演出を出すか抽選、その関数を呼び出す
    /// </summary>
    private void SelectEvent()
    {
        // 抽選
        CurrentEventType = (eEventType)Random.Range(0, (int)eEventType.EventMax);

        // 呼び出し
        switch(CurrentEventType)
        {
            case eEventType.Shutter:    // シャッター演出

                break;

            default:
                break;
        }

    }

    private void ShatterEvent()
    {

    }

    private void OpenShutter()
    {
        // 開いた状態はX座標±530
        // 閉まっているシャッターを画面外(左右)に移動
        float t = 0.0f;

        // スロット画面外に表示されているものを画面内に移動させる
        leftShutterTransform.anchoredPosition
            = new Vector2(GameConst.OPEN_LEFT_SHUTTER_POS_X, 0);


        while (true)
        {
            t += 0.1f * Time.deltaTime;

            rightShutterTransform.anchoredPosition
            = new Vector2(Mathf.Lerp(
                GameConst.CLOSE_RIGHT_SHUTTER_POS_X,
                GameConst.OPEN_RIGHT_SHUTTER_POS_X, t)
            , 0);

            if (t > 1.0f) break;

        }
        
    }

    private void CloseShutter()
    {
        // 閉まった状態は右X座標178左-200

        float t = 0.0f;

        // スロット画面外に表示されているものを画面内に移動させる
        leftShutterTransform.anchoredPosition
            = new Vector2(GameConst.CLOSE_LEFT_SHUTTER_POS_X, 0);
        

        while (true)
        {
            t *= 0.1f;
            
            rightShutterTransform.anchoredPosition
            = new Vector2(Mathf.Lerp(
                GameConst.OPEN_RIGHT_SHUTTER_POS_X,
                GameConst.CLOSE_RIGHT_SHUTTER_POS_X, t)
            , 0);
            
            if (t > 1.0f) break; 
            
        }


    }





    private IEnumerator HideImage()
    {
        yield return new WaitForSeconds(3f);

        imageCanvas.SetActive(false);
    }






}
