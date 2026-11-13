using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
//ゲーム数を管理するクラス
public class Numberofgames : MonoBehaviour
{
    //ゲーム数の初期値を設定する変数
    public int _initialGameCount;
    //private int amount;
    int _gameCount=10;
    [SerializeField] TextMeshProUGUI GameText;
    [SerializeField] TextMeshProUGUI GetGameText;

    private void Start()
    {
        //ゲーム数を初期値に設定する
        SetGameCount();
    }

    private void Update()
    {
        //ゲーム数を減らす処理を行う
        if (Input.GetKeyDown(KeyCode.A))
        {
            _initialGameCount--;
        }


            GameText.text = "ゲーム数：" + _initialGameCount.ToString();

        //ゲーム数が999を超えないようにする処理
        if (_initialGameCount >= 999)
        {
            _initialGameCount = 999;
        }
        //ゲーム数を増加させる処理を行う
        else
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                GetGameCount(_gameCount);
            }
        }
    }

    private void SetGameCount()
    {
        //ゲーム数を初期値に設定する
        _initialGameCount = 30;

    }
    /// <summary>
    /// 上乗せ抽選でゲーム数を増加させる処理を行う
    /// </summary>
    /// <param name="amount">ゲーム数の増加する数を指定する</param>
    public void GetGameCount(int amount)
    {
        //ゲーム数を足す処理を行う

        _initialGameCount += amount;
            


    }
}
   

