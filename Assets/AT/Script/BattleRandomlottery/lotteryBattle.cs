using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;

//レア役での上乗せ抽選.Battle抽選を行うクラス
public class lotteryBattle : MonoBehaviour
{

  

    //Random(1－20)の乱数を入れる変数
    //５パーセント刻み
    private int RandomNumMin = 1;
    private int RandomNumMax = 21;
    //一応の初期化
    private int _resetNum = 0;
    
    //弱チェ
    private int _cherryWeak;
    //強チェ
    private int _cherryStrong;

    //Randomによる抽選の結果を入れる変数
    private int _judgeExtraboost;
    private int _judgeHighstage;

    //弱チェの上乗せ抽選
    private int _judgeExtraboostWeakNum = 5;
    //強チェの上乗せ抽選
    private int _judgeExtraboostStrongNum = 18;

    //弱チェのBattle抽選
    private int _judgeBattleWeakNum = 6;
    //強チェのBattle抽選
    private int _judgeBattleStrongNum = 19;

    //スイカの高確率抽選
    private int _judgeHighstageNum = 7;

    private void Update()
    {
        //弱チェリーの上乗せ抽選、Battle抽選を行う
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetJudgeExtraboostWeak();
            SetJudgeBattleWeak();
        }

        //強チェリーの上乗せ抽選、Battle抽選を行う
        if(Input.GetKeyDown(KeyCode.B))
        {
            SetJudgeExtraboostStrong();
            SetJudgeBattleStrong();
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


        Debug.Log("SetJudgeExtraboostWeak: 乱数は" + _judgeExtraboost);

        if (_judgeExtraboost < _judgeExtraboostWeakNum)
        {
            GetCherryExtraboostWeak();
        }
        else if(_judgeExtraboost >= _judgeExtraboostWeakNum)
        {
            Debug.Log("SetJudgeExtraboostWeak: はずれ");
        }

        _judgeExtraboost = _resetNum;

    }


    //チェリーの上乗せ何Gかの抽選
    private void GetCherryExtraboostWeak()
    {

        int _cherryWeak = Random.Range(RandomNumMin, RandomNumMax);
        if(_cherryWeak <=14)
        {
            Debug.Log("SetJudgeExtraboostWeak: 10G上乗せ");
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
        Debug.Log("SetJudgeExtraboostStrong: 乱数は" + _judgeExtraboost);
        if (_judgeExtraboost < _judgeExtraboostStrongNum)
        {
            GetCherryExtraboostStrong();
        }
        else if (_judgeExtraboost >= _judgeExtraboostStrongNum)
        {
            Debug.Log("SetJudgeExtraboostStrong: はずれ");
        }
        _judgeExtraboost = _resetNum;
    }


    //チェリーの上乗せ何Gかの抽選
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

    private void SetJudgeBattleStrong()
    {
        //_judgeBattleにRandomNumを代入して、上乗せ抽選を行う
        int _judgeBattle = Random.Range(RandomNumMin, RandomNumMax);
        Debug.Log("SetJudgeBattleStrong: 乱数は" + _judgeBattle);
        if (_judgeBattle < _judgeBattleStrongNum)
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
            Debug.Log("SetJudgeHighstage: 高確率");
        }
        else if (_judgeHighstage >= _judgeHighstageNum)
        {
            Debug.Log("SetJudgeHighstage: はずれ");
        }
        _judgeHighstage = _resetNum;
    }
}
