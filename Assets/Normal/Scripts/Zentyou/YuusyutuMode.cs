using System.Collections;
using UnityEngine;



public class YuusyutuMode : MonoBehaviour
{
    [SerializeField] private GameObject imageCanvas;

    private void Awake()
    {
        
    }


    void Start()
    {

        
    }

    void Update()
    {
        // 優出モードに入る

        // シャッター演出

        // 背景色アップ演出 順番：白→青→黄色→赤→虹 期待度：青＜黄色＜白＝赤＜虹

        // 本前兆期待度示唆演出 可→良→優→超

        // AT突入ジャッジ


    }

    private IEnumerator HideImage()
    {
        yield return new WaitForSeconds(3f);

        imageCanvas.SetActive(false);
    }






}
