using UnityEngine;
using System.Collections.Generic;

public class ReachMasterLoader : MonoBehaviour
{
    // =========================================
    // リーチ目1個分のデータ
    // =========================================

    [System.Serializable]
    public class ReachPattern
    {
        public int id;

        public string name;

        // 左リール
        public string LU;
        public string LC;
        public string LL;

        // 中リール
        public string CU;
        public string CC;
        public string CL;

        // 右リール
        public string RU;
        public string RC;
        public string RL;

        // BIG / REG
        public string bonus;
    }


    // =========================================
    // 読み込んだリーチ目一覧
    // =========================================

    public static List<ReachPattern> ReachPatterns
    { get; private set; }


    // =========================================
    // Awake
    // =========================================

    private void Awake()
    {
        LoadReachMaster();
    }


    // =========================================
    // CSV読み込み
    // =========================================

    private void LoadReachMaster()
    {
        ReachPatterns = new List<ReachPattern>();


        TextAsset csvFile = Resources.Load<TextAsset>( "MasterData/ReachMasterLoader" );


        if (csvFile == null)
        {
            Debug.LogError("ReachMaster.csvがない。");
            return;
        }


        string[] lines = csvFile.text.Split( new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);


        // 0行目は見出しなので1から
        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');


            // id～bonusまで12列
            if (columns.Length < 12)
            {
                Debug.LogWarning( "ReachMasterの列数がおかしい" +lines[i]);
                continue;
            }


            ReachPattern pattern = new ReachPattern();


            // =================================
            // ID
            // =================================

            int parsedId;

            if (int.TryParse( CleanText(columns[0]), out parsedId))
            {
                pattern.id = parsedId;
            }
            else
            {
                Debug.LogWarning("リーチ目IDがおかしい"+ columns[0]);

                continue;
            }


            // =================================
            // 図柄パターンの名前
            // =================================
            pattern.name = CleanText(columns[1]);

            // =================================
            // 左リール
            // =================================
            pattern.LU =ConvertSymbolName( CleanText(columns[2]));
            pattern.LC =ConvertSymbolName(CleanText(columns[3]));
            pattern.LL = ConvertSymbolName( CleanText(columns[4]));


            // =================================
            // 中リール
            // =================================
            pattern.CU = ConvertSymbolName( CleanText(columns[5]));
            pattern.CC = ConvertSymbolName(CleanText(columns[6]));
            pattern.CL = ConvertSymbolName( CleanText(columns[7]) );

            // =================================
            // 右リール
            // =================================
            pattern.RU =ConvertSymbolName( CleanText(columns[8]));
            pattern.RC =ConvertSymbolName(CleanText(columns[9]));
            pattern.RL =ConvertSymbolName( CleanText(columns[10]));


            // =================================
            // ボーナス
            // =================================

            pattern.bonus =CleanText(columns[11]);

            ReachPatterns.Add( pattern);

            Debug.Log("リーチ目読み込み：" +pattern.id + " / " + pattern.name + " / BONUS：" + pattern.bonus );
        }


        Debug.Log( "ReachMaster読み込み " +ReachPatterns.Count + "個");
    }

    // =========================================
    // CSV文字整理
    // =========================================

    private string CleanText( string text)
    {
        return text
            .Trim()
            .Trim('"')
            .Trim();
    }


    // =========================================
    // 図柄名変換
    // =========================================

    private string ConvertSymbolName( string symbolName)
    {
        // * はなんでも枠
        if (symbolName == "*")
        {
            return "*";
        }


        switch (symbolName)
        {
            case "Seven":
                return "Seven";

            case "7":
                return "Seven";

            case "BAR":
                return "BAR";

            case "Grape":
                return "Grape";

            case "Bell":
                return "Bell";

            case "Cherry":
                return "Cherry";

            case "Replay":
                return "Replay";

            case "Hibiscus":
                return "Hibiscus";

            default:

                Debug.LogWarning( "ReachMasterに登録されていない" + symbolName );

                return symbolName;
        }
    }
}