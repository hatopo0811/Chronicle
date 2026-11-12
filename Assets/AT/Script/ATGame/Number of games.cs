using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
//ゲーム数を管理するクラス
public class Numberofgames : MonoBehaviour
{
   public int _initialGameCount = 30;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("現在のゲーム数は" + _initialGameCount + "です");
        }


    }

    

}
