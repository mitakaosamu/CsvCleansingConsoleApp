using System.Text.RegularExpressions;

// 引数がひとつであるか確認する
if (args.Length != 1)
{
    Console.WriteLine("引数にフォルダパスを指定してください");
    return;
}

string folderPath = args[0]; // 引数からフォルダパスを取得

// フォルダが存在するか確認する
if (!Directory.Exists(folderPath))
{
    Console.WriteLine("指定されたフォルダが存在しません");
    return;
}

string[] csvFiles = Directory.GetFiles(folderPath, "*.csv"); // フォルダ内のCSVファイルを取得

foreach (string csvFile in csvFiles)
{
    // ファイルの内容を読み込む
    string allText = File.ReadAllText(csvFile);

    // ダブルクォーテーションで囲まれた項目を取得
    var matches = Regex.Matches(allText, "\"(.*?)\"");

    // matchesが空の場合は処理をスキップ
    if (matches.Count == 0)
    {
        continue;
    }

    // matchesをstring[]に変換
    string[] columns = matches.Select(m => m.Groups[1].Value).ToArray();

    // 全ての全角および半角スペースを除去する
    columns = columns.Select(c => c.Replace("　", "").Replace(" ", "")).ToArray();

    // 全角英数字を全て半角に変換する
    columns = columns.Select(c => Regex.Replace(c, "[０-９Ａ-Ｚａ-ｚ]", m => ((char)(m.Value[0] - '０' + '0')).ToString()))
        .ToArray();

    // ３番目のcolumnに含まれるカンマと円という漢字と￥マークを削除する
    columns[2] = columns[2].Replace(",", "").Replace("円", "").Replace("¥", "").Replace("￥", "");

    // ファイルを更新する
    using StreamWriter writer = new StreamWriter(csvFile);

    // columnsをダブルクォーテーションで囲んでカンマ区切りで書き込む
    writer.Write(string.Join(",", columns.Select(c => $"\"{c}\"")));
}

Console.WriteLine("処理完了");
