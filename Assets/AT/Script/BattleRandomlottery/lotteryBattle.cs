using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;

public class lotteryBattle : MonoBehaviour
{


    Dictionary<int, string> judgeInfo;


    Dictionary<int, float> jugdeDict;


    private void Start()
    {
        for(int i = 0; i < 10; i++)
        {
        GetJudge();

        }

    }

    void GetJudge()
    {
        InitalizeDicts();

        int judge = Chooze();

        if(judge != 0)
        {
            string Judge = judgeInfo[judge];

            Debug.Log("‚æ‚¢‚µ‚å" + Judge);


        }
        else
        {
            Debug.Log("‚ ‚ñ‚É‚å[‚ñ");
        }


    }





    void InitalizeDicts()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "Žc”O");
        judgeInfo.Add(2, "‚Ô‚Á‚¿‚¬‚è");
        //itemInfo.Add(3, "item3");
        //itemInfo.Add(4, "item4");
        //itemInfo.Add(5, "item5");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 0.9f);
        jugdeDict.Add(2, 0.1f);
        //ItemDropDict.Add(3, 0.1f);
        //ItemDropDict.Add(4, 0.05f);
        //ItemDropDict.Add(5, 0.05f);
    }

    int Chooze()
    {
        float total = 0;
        foreach (KeyValuePair<int,float>elem in jugdeDict)
        {
            total += elem.Value;
        }
        float randomPoint = Random.value * total;
        foreach (KeyValuePair<int, float> elem in jugdeDict)
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
