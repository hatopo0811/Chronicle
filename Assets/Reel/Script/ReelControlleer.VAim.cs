using UnityEngine;

// V‹K’Ç‰Á‚Ì‚İBŠù‘¶ReelController.cs / .Stop.cs‚Í•ÏX‚µ‚Ü‚¹‚ñB
public partial class ReelController
{
    private bool vAimOwnsReel;
    private SlotGameManager vAimSavedManager;

    public bool IsVAimOwned { get { return vAimOwnsReel; } }

    // Šù‘¶UpdateReel()‚ª’ÊíƒQ[ƒ€‚Ö’â~’Ê’m‚·‚é‚Ì‚ğ”ğ‚¯‚é‚½‚ßA
    // ‹^——V‹Z’†‚¾‚¯“à•”‚ÌgameManagerQÆ‚ğ‘Ş”ğ‚·‚éB
    public bool BeginVAimReelControl()
    {
        if (vAimOwnsReel) return true;
        if (isSpinning || isStopping)
        {
            Debug.LogWarning("[V‹^——V‹Z] ‰ñ“]E’â~’†‚ÌƒŠ[ƒ‹‚ÍØ‚è‚ç‚ê‚Ü‚¹‚ñ: " + name);
            return false;
        }
        if (reelData == null || reelData.Count != 20 || symbolHeight <= 0f || reelLength <= 0f)
        {
            Debug.LogError("[V‹^——V‹Z] ƒŠ[ƒ‹‰Šú‰»–¢Š®—¹: " + name);
            return false;
        }
        vAimSavedManager = gameManager;
        gameManager = null;
        vAimOwnsReel = true;
        SetPseudoPlayMode(true);
        return true;
    }

    public void EndVAimReelControl()
    {
        if (!vAimOwnsReel) return;
        // ’â~Š®—¹‘O‚É‚ÍŒ³‚ÌManager‚É–ß‚³‚È‚¢‚±‚ÆB
        if (isSpinning || isStopping)
        {
            Debug.LogWarning("[V‹^——V‹Z] ƒŠ[ƒ‹‰ñ“]’†‚Ì‚½‚ß§Œä‚ğ•Ô‚¹‚Ü‚¹‚ñ: " + name);
            return;
        }
        SetPseudoPlayMode(false);
        gameManager = vAimSavedManager;
        vAimSavedManager = null;
        vAimOwnsReel = false;
    }

    // CSV‚ğ20¨1‡‚É“Ç‚ñ‚¾0-based center index‚Ö’â~‚·‚éB
    // ’ÊíƒQ[ƒ€‚ÌFindChronicleSlip()‚ğ’Ê‚³‚È‚¢‚Ì‚ÅÅ‘å4ƒRƒ}§ŒÀ‚ğó‚¯‚È‚¢B
    public bool StopVAimAtCenterIndex(int targetCenterIndex, int extraRevolutions = 0)
    {
        if (!vAimOwnsReel || !isSpinning || isStopping ||
            reelData == null || reelData.Count != 20)
            return false;
        if (targetCenterIndex < 0 || targetCenterIndex >= 20)
            return false;

        float remainder = Mathf.Repeat(reelOffset, symbolHeight);
        float toBoundary = remainder < 0.001f ? 0f : symbolHeight - remainder;
        float boundary = Mathf.Repeat(reelOffset + toBoundary, reelLength);
        int baseCenterIndex = GetCenterIndexFromOffset(boundary);

        // GetIndexAfterSlip = baseIndex - slip (mod 20)
        int slip = (baseCenterIndex - targetCenterIndex + 20) % 20;
        slip += Mathf.Clamp(extraRevolutions, 0, 2) * 20;
        SetStopDistance(toBoundary, slip);
        Debug.Log("[V‹^——V‹Z] " + name + " / ’†’iIndex=" + targetCenterIndex
            + " / ŠŠ‚è=" + slip + "ƒRƒ}");
        return true;
    }

    public bool IsVAimCenterTriple(string upper, string middle, string lower)
    {
        return GetUpperSymbol() == upper &&
               GetCenterSymbol() == middle &&
               GetLowerSymbol() == lower;
    }
}
