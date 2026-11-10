
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 忍者スロット用リール配列読み込み
///
/// InspectorからReelMaster.csvを登録する方式。
/// Resourcesフォルダは不要。
///
/// CSV形式:
/// index,Left,Center,Right
///
/// 20番から1番の順番で読み込む。
/// </summary>
[DefaultExecutionOrder(-100)]
public class ReelMasterLoader : MonoBehaviour
{
    // =========================================
    // Inspector設定
    // =========================================

    [Header("リール配列CSV")]
    [SerializeField]
    private TextAsset reelMasterCsv;

    // =========================================
    // リール配列
    // =========================================

    public static List<string> LeftReel { get; private set; }
    public static List<string> CenterReel { get; private set; }
    public static List<string> RightReel { get; private set; }

    // =========================================
    // 登録済み図柄
    // =========================================

    private static readonly HashSet<string> ValidSymbols =
        new HashSet<string>
        {
            "Bell",
            "Replay",
            "BlueV",
            "BAR",
            "Suika",
            "Cherry",
            "Lose",
            "PinkV"
        };

    // =========================================
    // Awake
    // =========================================

    private void Awake()
    {
        LoadReelMaster();
    }

    // =========================================
    // 外部からの読み込み確認
    // =========================================

    public static void EnsureLoaded()
    {
        if (
            LeftReel != null &&
            CenterReel != null &&
            RightReel != null &&
            LeftReel.Count > 0 &&
            CenterReel.Count > 0 &&
            RightReel.Count > 0
        )
        {
            return;
        }

        Debug.LogError(
            "リール配列が読み込まれていません。" +
            "ReelMasterLoaderのInspectorにCSVを設定してください。"
        );
    }

    // =========================================
    // CSV読み込み
    // =========================================

    private void LoadReelMaster()
    {
        LeftReel = new List<string>();
        CenterReel = new List<string>();
        RightReel = new List<string>();

        // CSV未設定
        if (reelMasterCsv == null)
        {
            Debug.LogError(
                "ReelMaster.csvが設定されていません！" +
                "ReelMasterLoaderのInspectorから登録してください。"
            );

            return;
        }

        // CSVを改行で分割
        string[] lines = reelMasterCsv.text.Split(
            new[] { "\r\n", "\n" },
            StringSplitOptions.RemoveEmptyEntries
        );

        // =====================================
        // 各行を読み込む
        // =====================================

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i]
                .Trim()
                .Trim('\uFEFF');

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // CSVの行全体が引用符で
            // 囲まれている場合に対応
            if (
                line.Length >= 2 &&
                line[0] == '"' &&
                line[line.Length - 1] == '"'
            )
            {
                line = line.Substring(
                    1,
                    line.Length - 2
                );
            }

            string[] columns = line.Split(',');

            // 列数チェック
            if (columns.Length < 4)
            {
                Debug.LogWarning(
                    "ReelMaster.csvの列数が不足しています。" +
                    " 行：" + (i + 1)
                );

                continue;
            }

            // =================================
            // 図柄名取得
            // =================================

            string left = CleanText(columns[1]);
            string center = CleanText(columns[2]);
            string right = CleanText(columns[3]);

            // =================================
            // 図柄名チェック
            // =================================

            if (
                !ValidSymbols.Contains(left) ||
                !ValidSymbols.Contains(center) ||
                !ValidSymbols.Contains(right)
            )
            {
                Debug.LogError(
                    "未登録の図柄名があります。" +
                    " 行：" + (i + 1) +
                    " / 左：" + left +
                    " / 中：" + center +
                    " / 右：" + right
                );
            }

            // =================================
            // リール配列に追加
            // =================================

            LeftReel.Add(left);
            CenterReel.Add(center);
            RightReel.Add(right);
        }

        // =====================================
        // 読み込み結果確認
        // =====================================

        if (LeftReel.Count == 0)
        {
            Debug.LogError(
                "リール配列を読み込めませんでした。"
            );

            return;
        }

        // 20コマ確認
        if (
            LeftReel.Count != 20 ||
            CenterReel.Count != 20 ||
            RightReel.Count != 20
        )
        {
            Debug.LogWarning(
                "リール配列は20コマ想定です。" +
                " 左：" + LeftReel.Count +
                " 中：" + CenterReel.Count +
                " 右：" + RightReel.Count
            );
        }

        // =====================================
        // 読み込み成功
        // =====================================

        Debug.Log(
            "忍者リール配列 読み込み完了！" +
            " 左：" + LeftReel.Count +
            " / 中：" + CenterReel.Count +
            " / 右：" + RightReel.Count
        );
    }

    // =========================================
    // 図柄名の整形
    // =========================================

    private static string CleanText(string text)
    {
        return text
            .Trim()
            .Trim('"')
            .Trim()
            .Trim('\uFEFF');
    }
}
