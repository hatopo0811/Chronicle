using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;

public class lotteryBattle : MonoBehaviour
{


    Dictionary<int, string> judgeInfo;


    Dictionary<int, float> jugdeDict;

    private int judge;

    private void Start()
    {
        for (int i = 0; i < 1; i++)
        {
      SetJudge();
        }


    }

    private void SetJudge()
    {

        int judge = Random.Range(1, 10);


        Debug.Log("SetJudge: —”‚Í" + judge);

        if (judge > 1&&judge < 7)
        {
            Debug.Log("“–‚½‚è");
        }
        else if(judge > 7 && judge < 10)
        {
            Debug.Log("‚Í‚¸‚ê");
        }

    }
  





    
}
