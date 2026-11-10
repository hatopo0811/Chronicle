using System.Collections.Generic;
using UnityEngine;

public class lotteryJudge : MonoBehaviour
{
    Dictionary<int, string> judgeInfo;


    Dictionary<int, float> jugdeDict;


    private void Update()
    {
       
            GetJudge();

    }

    void GetJudge()
    {
        InitalizeDicts();

        int judge = Chooze();

        if (judge != 0)
        {
            string Judge = judgeInfo[judge];

            Debug.Log("よいしょ" + Judge);


        }
        else
        {
            Debug.Log("あんにょーん");
        }


    }


    void InitalizeDicts()
    {
        //ボタンを押すと出る確率を変える
        if (Input.GetKeyDown(KeyCode.B))
        {
            InitalizeDictsBlue();
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            InitalizeDictsYellow();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            InitalizeDictsGreen();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            InitalizeDictsRed();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            InitalizeDictsRainbow();
        }
    }


    void InitalizeDictsBlue()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "成功");
        judgeInfo.Add(2, "残念");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 0.25f);
        jugdeDict.Add(2, 0.75f);
       
    }

    void InitalizeDictsYellow()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "成功");
        judgeInfo.Add(2, "残念");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 0.5f);
        jugdeDict.Add(2, 0.5f);

    }

    void InitalizeDictsGreen()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "成功");
        judgeInfo.Add(2, "残念");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 0.66f);
        jugdeDict.Add(2, 0.34f);

    }


    void InitalizeDictsRed()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "成功");
        judgeInfo.Add(2, "残念");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 0.8f);
        jugdeDict.Add(2, 0.2f);

    }

    void InitalizeDictsRainbow()
    {
        judgeInfo = new Dictionary<int, string>();
        judgeInfo.Add(1, "成功");
        jugdeDict = new Dictionary<int, float>();
        jugdeDict.Add(1, 1.0f);

    }

    int Chooze()
    {
        float total = 0;
        foreach (KeyValuePair<int, float> elem in jugdeDict)
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
