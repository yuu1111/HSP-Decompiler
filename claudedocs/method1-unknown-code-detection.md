# 手法1: 未知命令検出機能による辞書更新

## 概要

このデコンパイラ自体に未知の命令コードを自動検出・記録する機能を追加し、新バージョンのHSPでコンパイルされたファイルから新規命令を抽出する方法です。

## メリット

- **自動化**: 手動での比較作業が不要
- **網羅性**: 実際に使用された命令のみを抽出
- **実践的**: 逆コンパイル過程で自然に情報が得られる
- **効率的**: 複数のファイルから一度に情報収集可能

## デメリット

- **実装が必要**: コードの追加・修正が必要
- **不完全**: 実際に使われていない命令は検出できない
- **手動作業が残る**: 命令名の特定は別途必要

## 実装設計

### 1. ログ出力機能の追加

`Hsp3Dictionary.cs`に未知命令のログ機能を追加します。

```csharp
namespace KttK.HspDecompiler.Ax3ToAs
{
    class Hsp3Dictionary
    {
        // 追加: ログ出力用
        private static StreamWriter unknownCodesLog = null;
        private static HashSet<string> loggedCodes = new HashSet<string>();

        /// <summary>
        /// 未知命令のログ記録を有効化
        /// </summary>
        internal static void EnableUnknownCodeLogging(string logPath)
        {
            if (unknownCodesLog != null)
                return;

            unknownCodesLog = new StreamWriter(logPath, false, Encoding.UTF8);
            unknownCodesLog.WriteLine("# Unknown codes detected during decompilation");
            unknownCodesLog.WriteLine("# Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            unknownCodesLog.WriteLine("#");
            unknownCodesLog.WriteLine("#TypeCode,ValueCode,Name,Type,ExtraFlag");
            unknownCodesLog.WriteLine("$Code");
        }

        /// <summary>
        /// ログ記録を終了
        /// </summary>
        internal static void DisableUnknownCodeLogging()
        {
            if (unknownCodesLog != null)
            {
                unknownCodesLog.WriteLine("$End");
                unknownCodesLog.Flush();
                unknownCodesLog.Close();
                unknownCodesLog = null;
                loggedCodes.Clear();
            }
        }

        /// <summary>
        /// 未知命令をログに記録
        /// </summary>
        private void LogUnknownCode(HspDictionaryKey key)
        {
            if (unknownCodesLog == null)
                return;

            string codeKey = $"0x{key.Type:X02}_0x{key.Value:X04}";

            // 重複記録を防ぐ
            if (loggedCodes.Contains(codeKey))
                return;

            loggedCodes.Add(codeKey);
            unknownCodesLog.WriteLine(
                $"0x{key.Type:X02},0x{key.Value:X04},UNKNOWN_{key.Type:X02}_{key.Value:X04},?,? # TODO: 命令名を調査");
            unknownCodesLog.Flush();
        }

        internal bool CodeLookUp(HspDictionaryKey key, out HspDictionaryValue value)
        {
            // 既存の辞書検索
            if (codeDictionary.TryGetValue(key, out value))
                return true;

            HspDictionaryKey newkey = new HspDictionaryKey(key);
            newkey.Value = -1;
            newkey.AllValue = true;
            if (codeDictionary.TryGetValue(newkey, out value))
                return true;

            if ((key.Type == 0x11) && (key.Value >= 0x1000)) //ComFunction
            {
                value.Name = "comfunc";
                value.Type = HspCodeType.ComFunction;
                value.Extra = HspCodeExtraFlags.NONE;
                return true;
            }

            if (key.Type >= 0x12) //PlugInFunction
            {
                value.Name = "pluginFuction";
                value.OparatorPriority = key.Type - 0x12;
                value.Type = HspCodeType.PlugInFunction;
                value.Extra = HspCodeExtraFlags.NONE;
                return true;
            }

            // 追加: 見つからなかった命令をログ記録
            LogUnknownCode(key);

            return false;
        }
    }
}
```

### 2. コマンドライン引数の追加

`Program.cs`にログ機能を有効化するオプションを追加します。

```csharp
internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        HspConsole.Initialize();

        // コマンドライン引数の解析
        bool logUnknownCodes = false;
        string unknownCodesLogPath = "unknown_codes.csv";

        if (args != null && args.Length > 0)
        {
            foreach (string arg in args)
            {
                if (arg == "--log-unknown" || arg == "-u")
                {
                    logUnknownCodes = true;
                }
                else if (arg.StartsWith("--log-unknown="))
                {
                    logUnknownCodes = true;
                    unknownCodesLogPath = arg.Substring("--log-unknown=".Length);
                }
            }
        }

        if (!HspDecoder.Initialize())
        {
            HspConsole.Close();
            return;
        }

        // 未知命令ログを有効化
        if (logUnknownCodes)
        {
            Hsp3Dictionary.EnableUnknownCodeLogging(unknownCodesLogPath);
            HspConsole.WriteLog("Unknown code logging enabled: " + unknownCodesLogPath);
        }

        // 既存のコード...

        // アプリケーション終了時にログをクローズ
        try
        {
            Application.Run(new deHspDialog());
        }
        finally
        {
            if (logUnknownCodes)
            {
                Hsp3Dictionary.DisableUnknownCodeLogging();
            }
            HspConsole.Close();
        }
    }
}
```

### 3. GUI版でのオプション追加（オプション）

`deHspDialog.cs`にチェックボックスを追加して、GUI上からも有効化できるようにします。

```csharp
private CheckBox chkLogUnknownCodes;

private void InitializeComponent()
{
    // 既存のコード...

    this.chkLogUnknownCodes = new CheckBox();
    this.chkLogUnknownCodes.Text = "未知命令をログに記録";
    this.chkLogUnknownCodes.Location = new Point(10, 250);
    this.chkLogUnknownCodes.Size = new Size(200, 20);
    this.Controls.Add(this.chkLogUnknownCodes);
}

private void btnDecompile_Click(object sender, EventArgs e)
{
    if (chkLogUnknownCodes.Checked)
    {
        string logPath = Path.Combine(Path.GetDirectoryName(inputFilePath),
            "unknown_codes_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
        Hsp3Dictionary.EnableUnknownCodeLogging(logPath);
    }

    // 既存の逆コンパイル処理...

    if (chkLogUnknownCodes.Checked)
    {
        Hsp3Dictionary.DisableUnknownCodeLogging();
        MessageBox.Show("未知命令ログを出力しました: " + logPath);
    }
}
```

## 使用方法

### ステップ1: 新バージョンの.axファイルを用意

HSP 3.6でコンパイルされた.axファイルを用意します。

```hsp
; test_new_version.hsp
mes "HSP 3.6 test"
; 新しい命令を使用
newcommand 10, 20
stop
```

### ステップ2: ログモードで実行

コマンドラインから実行:

```bash
HSPdecom.exe --log-unknown test_new_version.ax
```

または、カスタムログパスを指定:

```bash
HSPdecom.exe --log-unknown=c:\logs\unknown_v36.csv test_new_version.ax
```

### ステップ3: 生成されたログを確認

`unknown_codes.csv`の内容:

```csv
# Unknown codes detected during decompilation
# Generated: 2025-11-07 12:34:56
#
#TypeCode,ValueCode,Name,Type,ExtraFlag
$Code
0x08,0x0050,UNKNOWN_08_0050,?,? # TODO: 命令名を調査
0x09,0x0100,UNKNOWN_09_0100,?,? # TODO: 命令名を調査
0x09,0x0101,UNKNOWN_09_0101,?,? # TODO: 命令名を調査
$End
```

### ステップ4: 命令名の調査

検出されたコードに対応する命令名を調べます:

**方法A: HSPヘルプを確認**
- HSPのドキュメントで該当するコードを探す

**方法B: テストスクリプトで確認**
```hsp
; 命令を1つずつテスト
newcmd1 10, 20  ; これが 0x08,0x0050 か？
stop
```

**方法C: HSPの公式フォーラム・コミュニティで質問**

### ステップ5: 命令名を記入

```csv
0x08,0x0050,newcmd1,HspFunction,
0x09,0x0100,newcmd2,HspFunction,
0x09,0x0101,newcmd3,HspFunction,Priority_2
```

### ステップ6: 既存のDictionary.csvにマージ

手動でマージするか、マージツールを使用します（手法5参照）。

```csv
# Dictionary.csv に追加
$Code
# ... 既存のエントリ ...
0x08,0x0050,newcmd1,HspFunction,
0x09,0x0100,newcmd2,HspFunction,
0x09,0x0101,newcmd3,HspFunction,Priority_2
$End
```

## 応用: バッチ処理

複数のファイルを一度に処理するバッチスクリプト:

```powershell
# analyze_all.ps1
$axFiles = Get-ChildItem -Path "C:\HSP_Samples\v36\" -Filter "*.ax" -Recurse

foreach ($file in $axFiles) {
    Write-Host "Analyzing: $($file.FullName)"
    & "C:\HSPdecom\HSPdecom.exe" --log-unknown "C:\logs\unknown_$($file.BaseName).csv" $file.FullName
}

Write-Host "All files analyzed. Check C:\logs\ for results."
```

## トラブルシューティング

### Q: ログファイルが生成されない

**A**: 以下を確認してください:
- 書き込み権限があるディレクトリか
- 既にファイルが開かれていないか
- コマンドライン引数が正しいか

### Q: 同じコードが何度も記録される

**A**: `HashSet<string> loggedCodes`で重複を防いでいますが、複数ファイルを処理する場合は、ログファイルをアペンドモードで開く必要があります。

### Q: 既知の命令もログに出る

**A**: Dictionary.csvが古い可能性があります。最新の辞書ファイルを使用してください。

## 次のステップ

1. [手法2: テストスクリプト解析法](method2-test-script-analysis.md) - より体系的な検出
2. [手法5: 辞書マージツール](method5-dictionary-merger.md) - 自動マージ機能
3. [バージョン管理戦略](version-management-strategy.md) - 複数バージョンの辞書管理

## 参考

- [Hsp3Dictionary.cs](../Ax3ToAs/Hsp3Dictionary.cs)
- [HspDictionaryKey.cs](../Ax3ToAs/Dictionary/HspDictionaryKey.cs)
- [Dictionary.csv フォーマット仕様](dictionary-format-spec.md)
