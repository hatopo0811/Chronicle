using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ATManager
{
    private static ATManager _instance;
    [SerializeField] public TextMeshProUGUI GameText;
    [SerializeField] TextMeshProUGUI GetGameText;
    [SerializeField] public RawImage _endScene;
    public static ATManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ATManager();
            }
            return _instance;
        }
    }
    private ATManager() { }
    public eATMode ATMode{ get; set; }
    public bool IsDigestion { get { return ATMode == eATMode.Digestion; } }
    public bool IsBattle { get {  return ATMode == eATMode.Battle; } }
    public bool IsSpecialized { get {  return ATMode == eATMode.Specialized; } }


    private int _gameCount = 0;
    public int GetGameCount()
    {
        return _gameCount;
        
    }

    public void Normalback()
    {
        lotteryJudge.instance.isScenario = false;
        GameText.gameObject.SetActive(false);
        _endScene.gameObject.SetActive(true);
        Debug.Log("í èÌéûÇ…ñﬂÇÈ");
    }

}
