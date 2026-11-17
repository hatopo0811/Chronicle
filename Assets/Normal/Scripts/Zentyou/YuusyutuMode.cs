using System.Collections;
using TMPro;
using UnityEngine;



public class YuusyutuMode : MonoBehaviour
{
    [SerializeField] private GameObject imageCanvas;
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
    private enum eYusyutuEvent
    {
        Invalid = -1,
        Shutter,
    }


    private void Awake()
    {
        imageCanvas.SetActive(true);
        StartCoroutine(HideImage());
    }

    void Start()
    {

        eventColorMax = SelectEventColor();
        Debug.Log(eventColorMax.ToString());
        nowEventColor.text = eventColorMax.ToString();

        
        

    }

    void Update()
    {
        
        // シャッター演出


        // 背景色アップ演出 順番：白→青→黄色→赤→虹 期待度：青＜黄色＜白＝赤＜虹



        // ペラアタック演出(α版) → 可(本前兆濃厚)→良(子役・レア役示唆)→優(強レア役)→超(本前兆期待度UP)


        // AT突入ジャッジ


    }

    private eEventColor SelectEventColor()
    {
        int eventColor = Random.Range(0, 101);

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

    private void SelectEvent()
    {
        int rand = Random.Range(0, 0);



    }

    private void ShatterEvent()
    {

    }


    private void OpenShutter()
    {


    }

    private void CloseShutter()
    {


    }





    private IEnumerator HideImage()
    {
        yield return new WaitForSeconds(3f);

        imageCanvas.SetActive(false);
    }






}
