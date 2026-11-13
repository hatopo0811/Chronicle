using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;

//レア役での上乗せ抽選.Battle抽選を行うクラス
public class lotteryBattle : MonoBehaviour
{

  Numberofgames _numberOfGames;

    //Random(1－20)の乱数を入れる変数
    //５パーセント刻み
    private int RandomNumMin = 1;
    private int RandomNumMax = 21;
    //一応の初期化
    private int _resetNum = 0;

    //高確率かどうかの判定を行うフラグ
    private bool _Highstage = false;

    //弱チェ
    private int _cherryWeak;
    //強チェ
    private int _cherryStrong;

    //Randomによる抽選の結果を入れる変数
    private int _judgeExtraboost;
    private int _judgeHighstage;

    //弱チェの上乗せ抽選
    private int _judgeExtraboostWeakNum = 5;
    private int _judgeExtraWeakHighstage = 10;
    //強チェの上乗せ抽選
    private int _judgeExtraboostStrongNum = 18;
    private int _judgeExtraStrongHighstage = 20;

    //弱チェのBattle抽選
    private int _judgeBattleWeakNum = 6;
    //強チェのBattle抽選
    private int _judgeBattleStrongNum = 19;
    private int _judgeBattleStrongHighstage = 20;

    //スイカの高確率抽選
    private int _judgeHighstageNum = 7;

    private void Update()
    {
        //弱チェリーの上乗せ抽選、Battle抽選を行う
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetJudgeExtraboostWeak();
           
        }

        //強チェリーの上乗せ抽選、Battle抽選を行う
        if(Input.GetKeyDown(KeyCode.B))
        {
            SetJudgeExtraboostStrong();
        }
        //スイカの高確率抽選を行う
        if(Input.GetKeyDown(KeyCode.S))
        {
            SetJudgeHighstage();
        }
    }

    //弱チェ上乗せするかの抽選
    private void SetJudgeExtraboostWeak()
    {
        //_judgeExtraboostにRandomNumを代入して、上乗せ抽選を行う
        int _judgeExtraboost = Random.Range(RandomNumMin,RandomNumMax);
        int _judgeEXtra;

        Debug.Log("SetJudgeExtraboostWeak: 乱数は" + _judgeExtraboost);

        if (!_Highstage)
        {
            _judgeEXtra = _judgeExtraboostWeakNum;
        }
        else
        {
            _judgeEXtra = _judgeExtraWeakHighstage;
        }

        if (_judgeExtraboost < _judgeEXtra)
        {
            GetCherryExtraboostWeak();
        }
        else if(_judgeExtraboost >= _judgeExtraboostWeakNum)
        {
            Debug.Log("SetJudgeExtraboostWeak: はずれ");
        }
        SetJudgeBattleWeak();
        _judgeExtraboost = _resetNum;

    }


    //弱チェリーの上乗せ何Gかの抽選
    private void GetCherryExtraboostWeak()
    {

        int _cherryWeak = Random.Range(RandomNumMin, RandomNumMax);
        if(_cherryWeak <=14)
        {
            Debug.Log("10G");
            //_numberOfGames.GetGameCount();
        }
        else if(_cherryWeak >= 15 && _cherryWeak <= 18)
        {
            Debug.Log("SetJudgeExtraboostWeak: 20G上乗せ");
        }
        else
        {
            Debug.Log("SetJudgeExtraboostWeak: 30G上乗せ");
        }
        _cherryWeak = _resetNum;
    }

    //弱チェのBattle抽選
    private void SetJudgeBattleWeak()
    {
        //_judgeBattleにRandomNumを代入して、上乗せ抽選を行う
        int _judgeBattle = Random.Range(RandomNumMin, RandomNumMax);
        Debug.Log("SetJudgeBattleWeak: 乱数は" + _judgeBattle);
        if (_judgeBattle < _judgeBattleWeakNum)
        {
            Debug.Log("SetJudgeBattleWeak: 当たり");
    
        }
        else if (_judgeBattle >= _judgeBattleWeakNum)
        {
            Debug.Log("SetJudgeBattleWeak: はずれ");
            //Battle抽選クラスをここに入れる
        }
        _judgeBattle = _resetNum;
    }

    //強チェ上乗せするかの抽選
    private void SetJudgeExtraboostStrong()
    {
        //_judgeExtraboostにRandomNumを代入して、上乗せ抽選を行う
        int _judgeExtraboost = Random.Range(RandomNumMin, RandomNumMax);
        int _judgeEXtra;
        Debug.Log("SetJudgeExtraboostStrong: 乱数は" + _judgeExtraboost);

        if (!_Highstage)
        {
            _judgeEXtra = _judgeExtraboostStrongNum;
        }
        else
        {
            _judgeEXtra = _judgeExtraStrongHighstage;
            Debug.Log("SetJudgeExtraboostStrong: 高確率状態なので、上乗せ抽選の確率が上がる");
        }

        if (_judgeExtraboost < _judgeExtraboostStrongNum)
        {
            GetCherryExtraboostStrong();
        }
        else if (_judgeExtraboost >= _judgeExtraboostStrongNum)
        {
            Debug.Log("SetJudgeExtraboostStrong: はずれ");
        }
        SetJudgeBattleStrong();

        _judgeExtraboost = _resetNum;
    }


    //強チェの上乗せ何Gかの抽選
    private void GetCherryExtraboostStrong()
    {
        int _cherryStrong = Random.Range(RandomNumMin, RandomNumMax);
        if (_cherryStrong <= 12)
        {
            Debug.Log("SetJudgeExtraboostStrong: 20G上乗せ");
        }
        else if (_cherryStrong >= 13 && _cherryStrong <= 18)
        {
            Debug.Log("SetJudgeExtraboostStrong: 50G上乗せ");
        }
        else
        {
            Debug.Log("SetJudgeExtraboostStrong: 100G上乗せ");
        }
        _cherryStrong = _resetNum;
    }

    //強チェのBattle抽選
    private void SetJudgeBattleStrong()
    {
        //_judgeBattleにRandomNumを代入して、上乗せ抽選を行う
        int _judgeBattle = Random.Range(RandomNumMin, RandomNumMax);
        int _judgeBattleSto;
        Debug.Log("SetJudgeBattleStrong: 乱数は" + _judgeBattle);
        if (!_Highstage)
        {
            _judgeBattleSto = _judgeBattleStrongNum;
        }
        else
        {
            _judgeBattleSto = _judgeBattleStrongHighstage;
        }

        if (_judgeBattle < _judgeBattleSto)
        {
            Debug.Log("SetJudgeBattleStrong: 当たり");

        }
        else if (_judgeBattle >= _judgeBattleStrongNum)
        {
            Debug.Log("SetJudgeBattleStrong: はずれ");
            //Battle抽選クラスをここに入れる
        }
        _judgeBattle = _resetNum;
    }

    //スイカの高確率抽選
    private void SetJudgeHighstage()
    {
        //_judgeExtraboostにRandomNumを代入して、上乗せ抽選を行う
        int _judgeHighstage = Random.Range(RandomNumMin, RandomNumMax);
        Debug.Log("SetJudgeHighstage: 乱数は" + _judgeHighstage);
        if (_judgeHighstage < _judgeHighstageNum)
        {
           _Highstage = true;
            Debug.Log("SetJudgeHighstage: 高確率状態に突入");
        }
        else if (_judgeHighstage >= _judgeHighstageNum)
        {
            _Highstage = false;
            Debug.Log("SetJudgeHighstage: 高確率状態に突入せず");
        }
        _judgeHighstage = _resetNum;
    }


    

}
