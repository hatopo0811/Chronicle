using UnityEngine;

public class JudgeManeger : MonoBehaviour
{
    [SerializeField]
    private lotteryJudge _lotteryJudge;
    [SerializeField]
    private lotteryBattle _lotteryBattle;
    [SerializeField]
    private Judge _judge;

    public static JudgeManeger instance = null;

    private void Start()
    {
        instance = this;
    }


}
