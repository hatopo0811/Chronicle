
using System;
using System.Collections.Generic;
using UnityEngine;

public class HitPatternMasterLoader : MonoBehaviour
{
    // =========================================
    // InspectorからCSVを設定
    // =========================================

    [Header("停止形マスターCSV")]
    [SerializeField]
    private TextAsset hitPatternCsv;

    // =========================================
    // 停止形データ
    // =========================================

    [Serializable]
    public class HitPatternData
    {
        public string ID;
        public string Name;
        public string Result;
        public int Priority;

        // Left
        public string LU;
        public string LC;
        public string LL;

        // Center
        public string CU;
        public string CC;
        public string CL;

        // Right
        public string RU;
        public string RC;
        public string RL;

        public bool IsMatch(
            string lu, string lc, string ll,
            string cu, string cc, string cl,
            string ru, string rc, string rl)
        {
            return
                MatchSymbol(LU, lu) &&
                MatchSymbol(LC, lc) &&
                MatchSymbol(LL, ll) &&
                MatchSymbol(CU, cu) &&
                MatchSymbol(CC, cc) &&
                MatchSymbol(CL, cl) &&
                MatchSymbol(RU, ru) &&
                MatchSymbol(RC, rc) &&
                MatchSymbol(RL, rl);
        }

        private bool MatchSymbol(
            string masterSymbol,
            string actualSymbol)
        {
            if (string.IsNullOrWhiteSpace(masterSymbol) ||
                masterSymbol == "*")
            {
                return true;
            }

            return string.Equals(
                masterSymbol,
                actualSymbol,
                StringComparison.OrdinalIgnoreCase);
        }
    }

    // =========================================
    // 読み込み済みデータ
    // =========================================

    public static List<HitPatternData> Patterns =
        new List<HitPatternData>();

    // =========================================
    // 初期化
    // =========================================

    private void Awake()
    {
        LoadHitPatternMaster();
    }

    // =========================================
    // CSV読み込み
    // =========================================

    private void LoadHitPatternMaster()
    {
        Patterns.Clear();

        if (hitPatternCsv == null)
        {
            Debug.LogError(
                "HitPatternMasterLoader: " +
                "CSVがInspectorに設定されていません。",
                this);

            return;
        }

        string[] lines = hitPatternCsv.text.Split(
            new[] { "\r\n", "\n" },
            StringSplitOptions.None);

        int loadedCount = 0;
        int skippedCount = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            List<string> columns = ParseCsvLine(line);

            if (columns.Count < 13)
            {
                Debug.LogWarning(
                    "HitPatternMaster: " +
                    (i + 1) + "行目は列数不足のためスキップ");

                skippedCount++;
                continue;
            }

            HitPatternData data = new HitPatternData();

            data.ID = CleanText(columns[0]);
            data.Name = CleanText(columns[1]);
            data.Result = CleanText(columns[2]);

            int priority;

            if (!int.TryParse(
                CleanText(columns[3]),
                out priority))
            {
                priority = 0;
            }

            data.Priority = priority;

            data.LU = CleanText(columns[4]);
            data.LC = CleanText(columns[5]);
            data.LL = CleanText(columns[6]);

            data.CU = CleanText(columns[7]);
            data.CC = CleanText(columns[8]);
            data.CL = CleanText(columns[9]);

            data.RU = CleanText(columns[10]);
            data.RC = CleanText(columns[11]);
            data.RL = CleanText(columns[12]);

            Patterns.Add(data);
            loadedCount++;
        }

        // 優先度が高い順に並べる
        Patterns.Sort(
            (a, b) => b.Priority.CompareTo(a.Priority));

        Debug.Log(
            "HitPatternMaster 読み込み成功！ " +
            "登録数: " + loadedCount +
            " / スキップ: " + skippedCount);
    }

    // =========================================
    // 一致する出目をすべて取得
    // =========================================

    public static List<HitPatternData> FindAllPatterns(
        string lu, string lc, string ll,
        string cu, string cc, string cl,
        string ru, string rc, string rl)
    {
        List<HitPatternData> matches =
            new List<HitPatternData>();

        foreach (HitPatternData pattern in Patterns)
        {
            if (pattern.IsMatch(
                lu, lc, ll,
                cu, cc, cl,
                ru, rc, rl))
            {
                matches.Add(pattern);
            }
        }

        return matches;
    }

    // =========================================
    // Result別に一覧取得
    // =========================================

    public static List<HitPatternData> GetPatternsByResult(
        string result)
    {
        List<HitPatternData> resultPatterns =
            new List<HitPatternData>();

        foreach (HitPatternData pattern in Patterns)
        {
            if (string.Equals(
                pattern.Result,
                result,
                StringComparison.OrdinalIgnoreCase))
            {
                resultPatterns.Add(pattern);
            }
        }

        return resultPatterns;
    }

    // =========================================
    // Result別にランダム取得
    // =========================================

    public static HitPatternData GetRandomPatternByResult(
        string result)
    {
        List<HitPatternData> candidates =
            GetPatternsByResult(result);

        if (candidates.Count == 0)
        {
            return null;
        }

        int index = UnityEngine.Random.Range(
            0,
            candidates.Count);

        return candidates[index];
    }

    // =========================================
    // 指定Resultの一致パターン取得
    // =========================================

    public static HitPatternData FindPatternByResult(
        string result,
        string lu, string lc, string ll,
        string cu, string cc, string cl,
        string ru, string rc, string rl)
    {
        foreach (HitPatternData pattern in Patterns)
        {
            if (!string.Equals(
                pattern.Result,
                result,
                StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (pattern.IsMatch(
                lu, lc, ll,
                cu, cc, cl,
                ru, rc, rl))
            {
                return pattern;
            }
        }

        return null;
    }

    // =========================================
    // CSVの1行を解析
    // =========================================

    private List<string> ParseCsvLine(string line)
    {
        List<string> result = new List<string>();

        bool insideQuotes = false;
        System.Text.StringBuilder current =
            new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (insideQuotes &&
                    i + 1 < line.Length &&
                    line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }
            }
            else if (c == ',' && !insideQuotes)
            {
                result.Add(CleanText(current.ToString()));
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(CleanText(current.ToString()));

        return result;
    }

    // =========================================
    // 文字列の整形
    // =========================================

    private string CleanText(string value)
    {
        if (value == null)
        {
            return "";
        }

        return value
            .Trim()
            .Trim('\uFEFF')
            .Trim('"')
            .Trim();
    }
}
