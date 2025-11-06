# 手法2: テストスクリプト解析法

## 概要

各HSPバージョンで全命令を網羅したテストスクリプトをコンパイルし、生成された.axファイルのバイトコードを比較分析する方法です。

## メリット

- **体系的**: すべての命令を網羅的にテスト可能
- **確実性が高い**: 実際のバイトコードを直接確認
- **バージョン間の差分が明確**: 追加・変更・削除された命令が一目瞭然
- **自動化可能**: スクリプトで処理を自動化できる

## デメリット

- **手間がかかる**: テストスクリプトの作成が必要
- **HSPの実行環境が必要**: 各バージョンのHSPコンパイラが必要
- **初期セットアップが複雑**: 環境構築に時間がかかる

## 実装手順

### ステップ1: テストスクリプトの作成

すべての命令を含む網羅的なテストスクリプトを作成します。

#### 1.1 基本命令テスト

```hsp
; test_basic_commands.hsp
; 基本命令のテスト

; 画面描画系
	mes "test message"
	print "test print"
	pos 10, 20
	color 255, 0, 0
	boxf 0, 0, 100, 100
	line 0, 0, 100, 100
	circle 50, 50, 100, 100
	cls 0

; 変数・演算系
	a = 10
	b = 20
	c = a + b
	d = a * b
	e = a / b

; 制御構文系
	if a > 0 {
		mes "positive"
	} else {
		mes "negative"
	}

	repeat 10
		mes "loop " + cnt
	loop

	goto *label1
*label1
	gosub *label2
	stop

*label2
	return

; ファイル操作系
	exist "test.txt"
	if strsize >= 0 {
		notesel buf
		noteload "test.txt"
		noteget line, 0
		mes line
	}

; システム系
	title "Test Window"
	width 640, 480
	wait 100
	randomize
	rnd_val = rnd(100)

stop
```

#### 1.2 拡張命令テスト

```hsp
; test_extended_commands.hsp
; 拡張命令のテスト

; ウィンドウ/GUI系
	button "Click", *on_click
	input str_input, 200
	mesbox str_memo, 640, 200
	listbox idx, 100, list_items
	combox idx, 100, combo_items

; グラフィック系
	buffer 1, 320, 240
	gsel 0
	gcopy 1, 0, 0, 320, 240
	gmode 2, 100, 100
	grotate 1, 0, 0, 0.5

; マルチメディア系
	mmload "sound.wav", 0
	mmplay 0

; ファイルシステム系
	dirlist buf, "*.*", 0
	mkdir "test_dir"
	chdir "test_dir"
	delete "test.txt"

; メモリ操作系
	sdim buf, 1024
	memset buf, 'A', 1024
	memcpy dst, src, 512

*on_click
	dialog "Clicked!"
	return

stop
```

#### 1.3 HSP 3.6新機能テスト（例）

```hsp
; test_hsp36_new_features.hsp
; HSP 3.6で追加された命令のテスト

; 新しい命令をここに追加
; （実際の命令名は公式ドキュメント参照）

stop
```

### ステップ2: 各バージョンでコンパイル

#### 2.1 バージョンごとのコンパイル

```powershell
# compile_all_versions.ps1

$versions = @(
    @{Version="3.4"; Path="C:\HSP34\hspcmp.exe"},
    @{Version="3.5b3"; Path="C:\HSP35b3\hspcmp.exe"},
    @{Version="3.5b4"; Path="C:\HSP35b4\hspcmp.exe"},
    @{Version="3.6"; Path="C:\HSP36\hspcmp.exe"}
)

$testScripts = @(
    "test_basic_commands.hsp",
    "test_extended_commands.hsp"
)

foreach ($version in $versions) {
    Write-Host "Compiling with HSP $($version.Version)..."

    foreach ($script in $testScripts) {
        $outputName = [System.IO.Path]::GetFileNameWithoutExtension($script)
        $outputDir = "compiled\v$($version.Version)"

        New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

        $outputAx = "$outputDir\$outputName.ax"

        # HSPコンパイラを実行
        & $version.Path $script /o$outputAx

        Write-Host "  -> $outputAx"
    }
}

Write-Host "Compilation completed!"
```

#### 2.2 ディレクトリ構造

```
project/
├── test_scripts/
│   ├── test_basic_commands.hsp
│   ├── test_extended_commands.hsp
│   └── test_hsp36_new_features.hsp
├── compiled/
│   ├── v3.4/
│   │   ├── test_basic_commands.ax
│   │   └── test_extended_commands.ax
│   ├── v3.5b4/
│   │   ├── test_basic_commands.ax
│   │   └── test_extended_commands.ax
│   └── v3.6/
│       ├── test_basic_commands.ax
│       ├── test_extended_commands.ax
│       └── test_hsp36_new_features.ax
└── analysis/
    └── (ここに解析結果を出力)
```

### ステップ3: バイトコード比較ツールの作成

#### 3.1 バイトコードダンプツール

```csharp
// BytecodeDumper.cs
using System;
using System.IO;
using System.Text;

namespace KttK.HspDecompiler.Tools
{
    class BytecodeDumper
    {
        internal static void DumpToCSV(string axFilePath, string outputCsvPath)
        {
            using (var reader = new BinaryReader(File.Open(axFilePath, FileMode.Open)))
            using (var writer = new StreamWriter(outputCsvPath, false, Encoding.UTF8))
            {
                // ヘッダー読み込み
                var header = AxHeader.FromBinaryReader(reader);

                writer.WriteLine("# Bytecode Dump");
                writer.WriteLine($"# File: {Path.GetFileName(axFilePath)}");
                writer.WriteLine("#Offset,TypeCode,ValueCode,Description");

                // コード領域の解析
                reader.BaseStream.Seek(header.CodeStart, SeekOrigin.Begin);

                long offset = header.CodeStart;
                while (offset < header.CodeEnd)
                {
                    byte typeCode = reader.ReadByte();
                    offset++;

                    if (typeCode == 0x00) // Operator
                    {
                        ushort valueCode = reader.ReadUInt16();
                        offset += 2;
                        writer.WriteLine($"0x{offset-3:X08},0x{typeCode:X02},0x{valueCode:X04},Operator");
                    }
                    else if (typeCode >= 0x08 && typeCode <= 0x0F)
                    {
                        ushort valueCode = reader.ReadUInt16();
                        offset += 2;
                        writer.WriteLine($"0x{offset-3:X08},0x{typeCode:X02},0x{valueCode:X04},Command");
                    }
                    else
                    {
                        writer.WriteLine($"0x{offset-1:X08},0x{typeCode:X02},----,Token");
                    }
                }
            }
        }
    }
}
```

#### 3.2 差分抽出ツール

```csharp
// BytecodeDiff.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KttK.HspDecompiler.Tools
{
    class BytecodeDiff
    {
        class CommandCode
        {
            public byte TypeCode;
            public ushort ValueCode;

            public override string ToString() => $"0x{TypeCode:X02},0x{ValueCode:X04}";
            public override int GetHashCode() => (TypeCode << 16) | ValueCode;
            public override bool Equals(object obj) =>
                obj is CommandCode other && TypeCode == other.TypeCode && ValueCode == other.ValueCode;
        }

        internal static void CompareVersions(string oldAxPath, string newAxPath, string outputPath)
        {
            var oldCodes = ExtractCommandCodes(oldAxPath);
            var newCodes = ExtractCommandCodes(newAxPath);

            // 追加された命令
            var added = newCodes.Except(oldCodes).ToList();

            // 削除された命令
            var removed = oldCodes.Except(newCodes).ToList();

            // 結果出力
            using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                writer.WriteLine($"# Comparison: {Path.GetFileName(oldAxPath)} vs {Path.GetFileName(newAxPath)}");
                writer.WriteLine($"# Old codes: {oldCodes.Count}, New codes: {newCodes.Count}");
                writer.WriteLine();

                if (added.Count > 0)
                {
                    writer.WriteLine("## Added Commands");
                    writer.WriteLine("#TypeCode,ValueCode,Name,Type,ExtraFlag");
                    writer.WriteLine("$Code");
                    foreach (var code in added.OrderBy(c => c.TypeCode).ThenBy(c => c.ValueCode))
                    {
                        writer.WriteLine($"{code},UNKNOWN,?,? # TODO: 命令名を調査");
                    }
                    writer.WriteLine("$End");
                    writer.WriteLine();
                }

                if (removed.Count > 0)
                {
                    writer.WriteLine("## Removed Commands");
                    foreach (var code in removed.OrderBy(c => c.TypeCode).ThenBy(c => c.ValueCode))
                    {
                        writer.WriteLine($"# {code} - この命令は削除された可能性");
                    }
                }
            }

            Console.WriteLine($"Added: {added.Count}, Removed: {removed.Count}");
            Console.WriteLine($"Results saved to: {outputPath}");
        }

        private static HashSet<CommandCode> ExtractCommandCodes(string axFilePath)
        {
            var codes = new HashSet<CommandCode>();

            using (var reader = new BinaryReader(File.Open(axFilePath, FileMode.Open)))
            {
                var header = AxHeader.FromBinaryReader(reader);
                reader.BaseStream.Seek(header.CodeStart, SeekOrigin.Begin);

                long offset = header.CodeStart;
                while (offset < header.CodeEnd)
                {
                    byte typeCode = reader.ReadByte();
                    offset++;

                    if ((typeCode >= 0x08 && typeCode <= 0x0F) || typeCode == 0x00)
                    {
                        if (offset + 1 < header.CodeEnd)
                        {
                            ushort valueCode = reader.ReadUInt16();
                            offset += 2;
                            codes.Add(new CommandCode { TypeCode = typeCode, ValueCode = valueCode });
                        }
                    }
                }
            }

            return codes;
        }
    }
}
```

### ステップ4: 自動比較スクリプト

```powershell
# analyze_differences.ps1

$baseVersion = "3.5b4"
$newVersion = "3.6"

$baseDir = "compiled\v$baseVersion"
$newDir = "compiled\v$newVersion"
$outputDir = "analysis\diff_${baseVersion}_to_${newVersion}"

New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

$testFiles = Get-ChildItem -Path $baseDir -Filter "*.ax"

foreach ($file in $testFiles) {
    $basePath = "$baseDir\$($file.Name)"
    $newPath = "$newDir\$($file.Name)"
    $outputPath = "$outputDir\$($file.BaseName)_diff.txt"

    if (Test-Path $newPath) {
        Write-Host "Comparing: $($file.Name)"

        # BytecodeDiff.exeを実行（要ビルド）
        & ".\Tools\BytecodeDiff.exe" $basePath $newPath $outputPath
    } else {
        Write-Host "  Warning: $newPath not found" -ForegroundColor Yellow
    }
}

Write-Host "`nAnalysis completed! Check $outputDir for results."
```

### ステップ5: 結果の確認と辞書への追加

生成された差分ファイルを確認:

```
# analysis/diff_3.5b4_to_3.6/test_basic_commands_diff.txt

# Comparison: test_basic_commands.ax vs test_basic_commands.ax
# Old codes: 245, New codes: 253

## Added Commands
#TypeCode,ValueCode,Name,Type,ExtraFlag
$Code
0x08,0x0050,UNKNOWN,?,? # TODO: 命令名を調査
0x08,0x0051,UNKNOWN,?,? # TODO: 命令名を調査
0x09,0x0100,UNKNOWN,?,? # TODO: 命令名を調査
0x09,0x0101,UNKNOWN,?,? # TODO: 命令名を調査
$End
```

HSPドキュメントで命令名を調査して記入:

```csv
0x08,0x0050,newcmd1,HspFunction,
0x08,0x0051,newcmd2,HspFunction,
0x09,0x0100,newcmd3,HspFunction,Priority_2
0x09,0x0101,newcmd4,HspFunction,
```

## 応用テクニック

### テクニック1: カテゴリ別テストスクリプト

命令をカテゴリごとに分けてテスト:

```
test_scripts/
├── category_graphics.hsp      # 画面描画系
├── category_file.hsp           # ファイル操作系
├── category_gui.hsp            # GUI系
├── category_system.hsp         # システム系
├── category_math.hsp           # 数値演算系
└── category_string.hsp         # 文字列操作系
```

### テクニック2: 自動ドキュメント生成

```powershell
# generate_command_list.ps1

$diffs = Get-ChildItem -Path "analysis\diff_*\*.txt"

$allAdded = @()

foreach ($diff in $diffs) {
    $content = Get-Content $diff
    $inAddedSection = $false

    foreach ($line in $content) {
        if ($line -match "^## Added Commands") {
            $inAddedSection = $true
        }
        elseif ($line -match "^\$End") {
            $inAddedSection = $false
        }
        elseif ($inAddedSection -and $line -match "^0x") {
            $allAdded += $line
        }
    }
}

# 重複を削除してソート
$unique = $allAdded | Sort-Object -Unique

# Markdown形式でレポート生成
$reportPath = "analysis\new_commands_report.md"
"# HSP 3.6 新規命令リスト`n" | Out-File $reportPath
"検出された新規命令: $($unique.Count) 個`n" | Add-Content $reportPath
"## 命令一覧`n" | Add-Content $reportPath
"| TypeCode | ValueCode | 命令名 | 備考 |" | Add-Content $reportPath
"|----------|-----------|--------|------|" | Add-Content $reportPath

foreach ($cmd in $unique) {
    if ($cmd -match "^(0x\w+),(0x\w+)") {
        "| $($matches[1]) | $($matches[2]) | ? | 要調査 |" | Add-Content $reportPath
    }
}

Write-Host "Report generated: $reportPath"
```

## トラブルシューティング

### Q: コンパイルエラーが発生する

**A**:
- HSPのバージョンによって使えない命令がある可能性
- バージョン別にテストスクリプトを分ける

### Q: 差分が多すぎる

**A**:
- テストスクリプトが複雑すぎる可能性
- シンプルな命令テストから始める
- カテゴリ別に分割する

### Q: 同じ命令でもバイトコードが異なる

**A**:
- コンパイラの最適化により変わることがある
- コンテキストに依存する命令がある
- 複数のテストで確認する

## 次のステップ

1. [手法1: 未知命令検出機能](method1-unknown-code-detection.md) - 実行時検出と組み合わせる
2. [手法3: 公式ソースコード解析](method3-official-source-analysis.md) - より確実な情報
3. [手法5: 辞書マージツール](method5-dictionary-merger.md) - 自動的に辞書に統合

## 参考

- [HSP公式サイト](https://hsp.tv/)
- [AxHeader.cs](../Ax3ToAs/Data/AxHeader.cs)
- [バイトコード構造仕様](bytecode-structure-spec.md)
