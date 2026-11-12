using UnityEngine;

public partial class SlotGameManager
{
    private void SetReelTargets()
    {
        // 旧BIG/REG/7揃いの目標は使わない。
        // 実際の停止選択は ReelController.Stop.cs と
        // SlotGameManager.Chronicle.cs の 6役用判定で行う。
        leftReel.SetTargetSymbol(null);
        centerReel.SetTargetSymbol(null);
        rightReel.SetTargetSymbol(null);
    }
}
