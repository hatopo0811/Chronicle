using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
//ゲーム数を管理するクラス
public class Numberofgames : MonoBehaviour
{
    //ゲーム数の初期値を設定する変数
    public int _initialGameCount;
    //private int amount;

    private void Start()
    {
        //ゲーム数を初期値に設定する
        SetGameCount();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetGameCount(10);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("現在のゲーム数は" + _initialGameCount);
        }

    }

    private void SetGameCount()
    {
        //ゲーム数を初期値に設定する
        _initialGameCount = 30;

    }

    public void GetGameCount(int amount)
    {
        //ゲーム数を足す処理を行う
        _initialGameCount += amount;
    }
}
   

