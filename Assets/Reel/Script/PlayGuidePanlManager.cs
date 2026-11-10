using UnityEngine;

public class PlayGuidePanelManager : MonoBehaviour
{
    // =========================================
    // —V‹Yà–¾Panel
    // =========================================

    [Header("—V‹Yà–¾Panel")]
    [SerializeField]
    private GameObject playGuidePanel;


    // =========================================
    // Œ»İŠJ‚¢‚Ä‚¢‚é‚©
    // =========================================

    public static bool IsGuideOpen
    {
        get;
        private set;
    }


    // =========================================
    // Start
    // =========================================

    private void Start()
    {
        ClosePanel();
    }


    // =========================================
    // Panel‚ğŠJ‚­
    // =========================================

    public void OpenPanel()
    {
        if (playGuidePanel == null)
        {
            Debug.LogWarning(
                "—V‹Yà–¾Panel‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ"
            );

            return;
        }


        playGuidePanel.SetActive(
            true
        );


        IsGuideOpen =
            true;


        Debug.Log(
            "—V‹Yà–¾ OPEN / IsGuideOpen = " +
            IsGuideOpen
        );
    }


    // =========================================
    // Panel‚ğ•Â‚¶‚é
    // =========================================

    public void ClosePanel()
    {
        if (playGuidePanel == null)
        {
            Debug.LogWarning(
                "—V‹Yà–¾Panel‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ"
            );

            return;
        }


        playGuidePanel.SetActive(
            false
        );


        IsGuideOpen =
            false;


        Debug.Log(
            "—V‹Yà–¾ CLOSE / IsGuideOpen = " +
            IsGuideOpen
        );
    }
}