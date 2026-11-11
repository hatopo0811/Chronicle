using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Numberofgames : MonoBehaviour
{
    Dictionary<int, string> _gameInfo;

    Dictionary<int, float> _gameDict;

    private int numberOfGames = 0;
    //一回だけ呼び出すための
    private int gameCount = 0;

    //高確率状態かどうかを判定するフラグ
    private bool isUpstage = false;

    private int _gameNumber;
    //初期ゲーム数
    private int _initialGameCount = 30;

    private int hoge;


    private void Update()
    {

        if(gameCount==0)
        {
        GetGameCount();

        }


        Debug.Log("現在のゲーム数は" + _initialGameCount + "です");


    }

    private void GetGameCount()
    {
        switch(hoge)
        {
            case 1:
                _initialGameCount += 10;
                break;
            case 2:
                _initialGameCount += 20;
                break;
            case 3:
                _initialGameCount += 30;
                break;

        }

        hoge = 0;
        GetLog();

    }


    private void Getgames()
    {

        if (Input.GetKeyUp(KeyCode.Space))
        {

            Debug.Log("チェリーを取得しました");

            _gameInfo = new Dictionary<int, string>();
            _gameInfo.Add(1, "チェリー1");
            _gameInfo.Add(2, "チェリー2");
            _gameInfo.Add(3, "チェリー3");

            _gameDict = new Dictionary<int, float>();
            _gameDict.Add(1, 0.2f);
            _gameDict.Add(2, 0.4f);
            _gameDict.Add(3, 0.4f);

            gameCount++;

        }



        if (Input.GetKeyUp(KeyCode.S))
        {

            Debug.Log("スイカを取得しました");

            _gameInfo = new Dictionary<int, string>();
            _gameInfo.Add(1, "高確率");
            _gameInfo.Add(2, "ゆーあーげい");

            _gameDict = new Dictionary<int, float>();
            _gameDict.Add(1, 0.3f);
            _gameDict.Add(2, 0.7f);

            gameCount++;

        }

        if (Input.GetKeyUp(KeyCode.T))
        {

            Debug.Log("強チェリーのBattle抽選");

            _gameInfo = new Dictionary<int, string>();
            _gameInfo.Add(1, "Battle");
            _gameInfo.Add(2, "ハズレ");

            _gameDict = new Dictionary<int, float>();
            _gameDict.Add(1, 0.95f);
            _gameDict.Add(2, 0.05f);

            gameCount++;

        }
        if (Input.GetKeyUp(KeyCode.T))
        {

            Debug.Log("強チェリーの上乗せ抽選");

            _gameInfo = new Dictionary<int, string>();
            _gameInfo.Add(1, "上乗せ");
            _gameInfo.Add(2, "ハズレ");

            _gameDict = new Dictionary<int, float>();
            _gameDict.Add(1, 0.90f);
            _gameDict.Add(2, 0.10f);

            gameCount++;

        }

        if (isUpstage)
        {
            Debug.Log("ステージを高確率に変更しました");

            if (Input.GetKeyUp(KeyCode.Space))
            {

                Debug.Log("チェリーを取得しました");

                _gameInfo = new Dictionary<int, string>();
                _gameInfo.Add(1, "10");
                _gameInfo.Add(2, "20");
                _gameInfo.Add(3, "Battle");

                _gameDict = new Dictionary<int, float>();
                _gameDict.Add(1, 0.1f);
                _gameDict.Add(2, 0.3f);
                _gameDict.Add(3, 0.6f);

                gameCount++;

            }

            if (Input.GetKeyUp(KeyCode.S))
            {

                Debug.Log("スイカを取得しました");

                _gameInfo = new Dictionary<int, string>();
                _gameInfo.Add(1, "Battle");
                _gameInfo.Add(2, "ゆーあーげい");

                _gameDict = new Dictionary<int, float>();
                _gameDict.Add(1, 0.25f);
                _gameDict.Add(2, 0.75f);

                gameCount++;

            }




        }


    }
    void GetLog()
    {


        Getgames();

        int judge = Chooze();
        
        if (judge != 0)
        {
            string Judge = _gameInfo[judge];

            Debug.Log("judge" + Judge);


        }
        else
        {
            Debug.Log("あんにょーん");

        }


    }

    int Chooze()
    {

        float total = 0;
        if (_gameDict == null) return 0;

        foreach (KeyValuePair<int, float> elem in _gameDict)
        {
            total += elem.Value;
        }
        float randomPoint = Random.value * total;

        foreach (KeyValuePair<int, float> elem in _gameDict)
        {

            if (randomPoint < elem.Value)
            {
                return elem.Key;
            }
            else
            {
                randomPoint -= elem.Value;
            }
        }
        return 0;
    }






}
