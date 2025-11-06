# 手法3: HSP公式ソースコード解析法

## 概要

HSPの公式ソースコード（OpenHSP）から命令定義テーブルを直接抽出し、Dictionary.csvを生成する方法です。

## メリット

- **最も正確**: 公式ソースから直接情報を取得
- **網羅的**: すべての命令を漏れなく取得可能
- **ドキュメント付き**: ソースコードのコメントから追加情報を得られる
- **バージョン管理**: Gitのタグ/ブランチから各バージョンの差分を追跡可能

## デメリット

- **技術的ハードル**: C++コードの読解が必要
- **初回セットアップが複雑**: ソースコードの解析ツール作成が必要
- **メンテナンス**: HSPの内部構造変更に対応が必要

## HSPソースコードの構造

### リポジトリ情報

- **リポジトリ**: https://github.com/onitama/OpenHSP
- **主要ブランチ**: `master` (最新版), `hsp35` (HSP 3.5系)
- **ライセンス**: BSD License

### 重要なファイル

```
OpenHSP/
├── hsp3/
│   ├── hspcmd.cpp          ← 命令コード定義（最重要）
│   ├── token.cpp           ← トークン定義
│   ├── code.h              ← 定数定義
│   ├── hspvar_*.cpp        ← 型システム定義
│   └── hspwnd.cpp          ← ウィンドウ系命令
├── hsp3dish/               ← マルチプラットフォーム版
└── hspcmp/
    ├── hspcmd.cpp          ← コンパイラ側の命令定義
    └── token.cpp           ← コンパイラのトークン処理
```

## 実装手順

### ステップ1: OpenHSPリポジトリの取得

```bash
# リポジトリのクローン
git clone https://github.com/onitama/OpenHSP.git
cd OpenHSP

# 特定バージョンをチェックアウト（例: HSP 3.6）
git checkout hsp36

# または、複数バージョンをクローン
git clone -b hsp35 https://github.com/onitama/OpenHSP.git OpenHSP_35
git clone -b hsp36 https://github.com/onitama/OpenHSP.git OpenHSP_36
```

### ステップ2: 命令定義の構造を理解

#### 2.1 hspcmd.cpp の構造

```cpp
// OpenHSP/hsp3/hspcmd.cpp
// 命令定義テーブルの例

static HSPEXINFO hspexinfo[] = {
    // TypeCode, ValueCode, CommandName, FunctionPointer, Flags

    // TypeCode 0x08 - 基本命令
    { 0x08, 0x00, "dim",     cmdfunc_dim,     CMDFUNC_DEFAULT },
    { 0x08, 0x01, "sdim",    cmdfunc_sdim,    CMDFUNC_DEFAULT },
    { 0x08, 0x11, "exist",   cmdfunc_exist,   CMDFUNC_DEFAULT },
    { 0x08, 0x12, "delete",  cmdfunc_delete,  CMDFUNC_DEFAULT },
    { 0x08, 0x13, "mkdir",   cmdfunc_mkdir,   CMDFUNC_DEFAULT },

    // TypeCode 0x09 - 拡張命令
    { 0x09, 0x00, "button",  cmdfunc_button,  CMDFUNC_DEFAULT },
    { 0x09, 0x01, "chgdisp", cmdfunc_chgdisp, CMDFUNC_DEFAULT },
    { 0x09, 0x10, "title",   cmdfunc_title,   CMDFUNC_DEFAULT },
    { 0x09, 0x11, "pos",     cmdfunc_pos,     CMDFUNC_DEFAULT },
    { 0x09, 0x12, "circle",  cmdfunc_circle,  CMDFUNC_DEFAULT },

    // 終端
    { -1 }
};

// Operator定義
static const char *operators[] = {
    "+",    // 0x00
    "-",    // 0x01
    "*",    // 0x02
    "/",    // 0x03
    "\\",   // 0x04
    "&",    // 0x05
    "|",    // 0x06
    "^",    // 0x07
    "=",    // 0x08
    "!",    // 0x09
    ">",    // 0x0A
    "<",    // 0x0B
    ">=",   // 0x0C
    "<=",   // 0x0D
    ">>",   // 0x0E
    "<<",   // 0x0F
    NULL
};
```

#### 2.2 code.h の定数定義

```cpp
// OpenHSP/hsp3/code.h
// TypeCode定数

#define TYPE_MARK       0x00    // Operator
#define TYPE_VAR        0x01    // Variable
#define TYPE_STRING     0x02    // String literal
#define TYPE_DNUM       0x03    // Double
#define TYPE_INUM       0x04    // Integer
#define TYPE_STRUCT     0x05    // Structure/Parameter
#define TYPE_LABEL      0x07    // Label
#define TYPE_INTCMD     0x08    // Internal command
#define TYPE_EXTCMD     0x09    // Extended command
#define TYPE_EXTSYSVAR  0x0A    // System variable
#define TYPE_CMPCMD     0x0B    // Compiler command
#define TYPE_MODCMD     0x0C    // Module command
#define TYPE_INTFUNC    0x0D    // Internal function
#define TYPE_SYSVAR     0x0E    // System variable
#define TYPE_PROGCMD    0x0F    // Program control
#define TYPE_DLLFUNC    0x10    // DLL function
#define TYPE_DLLCTRL    0x11    // COM control
#define TYPE_USERDEF    0x12    // User defined (plugin start)
```

### ステップ3: 自動抽出ツールの作成

#### 3.1 C++パーサー（Python実装）

```python
#!/usr/bin/env python3
# extract_commands.py
# OpenHSPソースから命令定義を抽出

import re
import sys
import csv
from pathlib import Path

def extract_commands_from_cpp(cpp_file):
    """
    C++ソースファイルから命令定義を抽出
    """
    commands = []

    with open(cpp_file, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()

    # HSPEXINFO テーブルを検索
    # { 0x08, 0x11, "exist", cmdfunc_exist, CMDFUNC_DEFAULT },
    pattern = r'\{\s*0x([0-9A-Fa-f]+)\s*,\s*0x([0-9A-Fa-f]+)\s*,\s*"([^"]+)"\s*,\s*(\w+)\s*,\s*(\w+)\s*\}'

    matches = re.finditer(pattern, content)

    for match in matches:
        type_code = match.group(1)
        value_code = match.group(2)
        cmd_name = match.group(3)
        func_name = match.group(4)
        flags = match.group(5)

        commands.append({
            'type_code': f'0x{type_code.upper()}',
            'value_code': f'0x{value_code.upper()}',
            'name': cmd_name,
            'func': func_name,
            'flags': flags
        })

    return commands

def extract_operators_from_cpp(cpp_file):
    """
    演算子定義を抽出
    """
    operators = []

    with open(cpp_file, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()

    # static const char *operators[] = { ... }
    pattern = r'static\s+const\s+char\s*\*\s*operators\[\]\s*=\s*\{([^}]+)\}'
    match = re.search(pattern, content, re.DOTALL)

    if match:
        ops_text = match.group(1)
        # "+" の形式を抽出
        ops_pattern = r'"([^"]+)"'
        ops_matches = re.finditer(ops_pattern, ops_text)

        idx = 0
        for op_match in ops_matches:
            op_name = op_match.group(1)
            if op_name != "NULL":
                operators.append({
                    'type_code': '0x00',
                    'value_code': f'0x{idx:04X}',
                    'name': op_name,
                    'type': 'Operator'
                })
                idx += 1

    return operators

def determine_command_type(type_code):
    """
    TypeCodeから命令タイプを判定
    """
    type_map = {
        '0x00': 'Operator',
        '0x01': 'Variable',
        '0x02': 'String',
        '0x03': 'Double',
        '0x04': 'Integer',
        '0x05': 'Param',
        '0x07': 'Label',
        '0x08': 'HspFunction',
        '0x09': 'HspFunction',
        '0x0A': 'SystemVar',
        '0x0C': 'UserFunction',
        '0x0D': 'HspFunction',
        '0x0F': 'OnStatement',
        '0x10': 'DllFunction',
        '0x11': 'ComFunction',
    }

    return type_map.get(type_code.upper(), 'Unknown')

def export_to_csv(commands, output_file):
    """
    Dictionary.csv形式で出力
    """
    with open(output_file, 'w', encoding='utf-8', newline='') as f:
        f.write('#HSP Commands Dictionary\n')
        f.write('#Generated from OpenHSP source code\n')
        f.write('#\n')
        f.write('#Keys,,Values,,\n')
        f.write('#TypeCode,ValueCode,Name,Type,ExtraFlag\n')
        f.write('$Code\n')

        # TypeCode, ValueCode でソート
        sorted_cmds = sorted(commands, key=lambda x: (x['type_code'], x['value_code']))

        for cmd in sorted_cmds:
            cmd_type = determine_command_type(cmd['type_code'])

            # ExtraFlagの判定（簡易版）
            extra = ''
            if 'priority' in cmd.get('flags', '').lower():
                extra = 'Priority_2'  # デフォルト

            f.write(f"{cmd['type_code']},{cmd['value_code']},{cmd['name']},{cmd_type},{extra}\n")

        f.write('$End\n')

def main():
    if len(sys.argv) < 2:
        print("Usage: python extract_commands.py <path_to_OpenHSP>")
        sys.exit(1)

    openhsp_path = Path(sys.argv[1])

    # 主要なソースファイル
    source_files = [
        openhsp_path / 'hsp3' / 'hspcmd.cpp',
        openhsp_path / 'hsp3' / 'hspwnd.cpp',
        openhsp_path / 'hspcmp' / 'hspcmd.cpp',
    ]

    all_commands = []

    for src_file in source_files:
        if src_file.exists():
            print(f"Parsing: {src_file}")

            # 演算子抽出
            if 'hspcmd.cpp' in str(src_file):
                operators = extract_operators_from_cpp(src_file)
                all_commands.extend(operators)

            # 命令抽出
            commands = extract_commands_from_cpp(src_file)
            all_commands.extend(commands)

            print(f"  Found {len(commands)} commands")
        else:
            print(f"Warning: {src_file} not found")

    # 重複除去
    unique_commands = []
    seen = set()

    for cmd in all_commands:
        key = (cmd['type_code'], cmd['value_code'])
        if key not in seen:
            seen.add(key)
            unique_commands.append(cmd)

    print(f"\nTotal unique commands: {len(unique_commands)}")

    # CSV出力
    output_file = 'Dictionary_extracted.csv'
    export_to_csv(unique_commands, output_file)

    print(f"Dictionary exported to: {output_file}")

if __name__ == '__main__':
    main()
```

#### 3.2 実行方法

```bash
# Python3が必要
python extract_commands.py C:\OpenHSP

# 出力: Dictionary_extracted.csv
```

#### 3.3 高度な抽出（C#実装）

```csharp
// CppCommandExtractor.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace KttK.HspDecompiler.Tools
{
    class CommandDefinition
    {
        public string TypeCode { get; set; }
        public string ValueCode { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Extra { get; set; }
        public string SourceFile { get; set; }
        public int LineNumber { get; set; }
    }

    class CppCommandExtractor
    {
        private static readonly Regex CommandPattern = new Regex(
            @"\{\s*0x([0-9A-Fa-f]+)\s*,\s*0x([0-9A-Fa-f]+)\s*,\s*""([^""]+)""\s*,\s*(\w+)\s*,\s*(\w+)\s*\}",
            RegexOptions.Compiled);

        public static List<CommandDefinition> ExtractFromDirectory(string openHspPath)
        {
            var commands = new List<CommandDefinition>();

            var sourceFiles = new[]
            {
                Path.Combine(openHspPath, "hsp3", "hspcmd.cpp"),
                Path.Combine(openHspPath, "hsp3", "hspwnd.cpp"),
                Path.Combine(openHspPath, "hspcmp", "hspcmd.cpp")
            };

            foreach (var file in sourceFiles)
            {
                if (File.Exists(file))
                {
                    Console.WriteLine($"Parsing: {file}");
                    var extracted = ExtractFromFile(file);
                    commands.AddRange(extracted);
                    Console.WriteLine($"  Found {extracted.Count} commands");
                }
            }

            // 重複除去
            return commands
                .GroupBy(c => $"{c.TypeCode}_{c.ValueCode}")
                .Select(g => g.First())
                .OrderBy(c => c.TypeCode)
                .ThenBy(c => c.ValueCode)
                .ToList();
        }

        private static List<CommandDefinition> ExtractFromFile(string filePath)
        {
            var commands = new List<CommandDefinition>();
            var lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                var match = CommandPattern.Match(lines[i]);
                if (match.Success)
                {
                    commands.Add(new CommandDefinition
                    {
                        TypeCode = $"0x{match.Groups[1].Value.ToUpper()}",
                        ValueCode = $"0x{match.Groups[2].Value.ToUpper()}",
                        Name = match.Groups[3].Value,
                        Type = DetermineType(match.Groups[1].Value),
                        Extra = "",
                        SourceFile = Path.GetFileName(filePath),
                        LineNumber = i + 1
                    });
                }
            }

            return commands;
        }

        private static string DetermineType(string typeCode)
        {
            var tc = Convert.ToInt32(typeCode, 16);

            return tc switch
            {
                0x00 => "Operator",
                0x08 or 0x09 or 0x0D => "HspFunction",
                0x0F => "OnStatement",
                _ => "HspFunction"
            };
        }

        public static void ExportToCsv(List<CommandDefinition> commands, string outputPath)
        {
            using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                writer.WriteLine("#HSP Commands Dictionary");
                writer.WriteLine("#Generated from OpenHSP source code");
                writer.WriteLine("#");
                writer.WriteLine("#Keys,,Values,,");
                writer.WriteLine("#TypeCode,ValueCode,Name,Type,ExtraFlag");
                writer.WriteLine("$Code");

                foreach (var cmd in commands)
                {
                    writer.WriteLine($"{cmd.TypeCode},{cmd.ValueCode},{cmd.Name},{cmd.Type},{cmd.Extra}");
                }

                writer.WriteLine("$End");
            }

            Console.WriteLine($"Exported {commands.Count} commands to {outputPath}");
        }
    }
}
```

### ステップ4: バージョン間の差分抽出

```bash
#!/bin/bash
# compare_hsp_versions.sh

# HSP 3.5と3.6の差分を抽出

echo "Extracting commands from HSP 3.5..."
python extract_commands.py OpenHSP_35 > /dev/null
mv Dictionary_extracted.csv Dictionary_hsp35.csv

echo "Extracting commands from HSP 3.6..."
python extract_commands.py OpenHSP_36 > /dev/null
mv Dictionary_extracted.csv Dictionary_hsp36.csv

# 差分を出力
echo "Comparing versions..."
diff Dictionary_hsp35.csv Dictionary_hsp36.csv > hsp35_to_36_diff.txt

echo "Differences saved to: hsp35_to_36_diff.txt"
```

### ステップ5: 命令の詳細情報を追加

ソースコードのコメントから追加情報を抽出:

```python
def extract_command_comments(cpp_file, command_name):
    """
    命令の関数定義からコメントを抽出
    """
    with open(cpp_file, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()

    # 関数定義を検索
    pattern = rf'/\*\*.*?\*/\s*void\s+cmdfunc_{command_name}\s*\('
    match = re.search(pattern, content, re.DOTALL)

    if match:
        comment = match.group(0)
        # コメント部分を抽出
        comment_pattern = r'/\*\*(.*?)\*/'
        comment_match = re.search(comment_pattern, comment, re.DOTALL)

        if comment_match:
            return comment_match.group(1).strip()

    return None

# 使用例
comment = extract_command_comments('hsp3/hspcmd.cpp', 'exist')
# "ファイルの存在チェック"
```

## 実践例

### 例1: HSP 3.6の完全な辞書生成

```bash
# OpenHSPをクローン
git clone -b hsp36 https://github.com/onitama/OpenHSP.git

# 命令を抽出
python extract_commands.py OpenHSP

# 結果確認
head -20 Dictionary_extracted.csv

# Dictionary.csvと置き換え
cp Dictionary_extracted.csv Dictionary.csv
```

### 例2: 複数バージョンの一括生成

```powershell
# generate_all_versions.ps1

$versions = @("hsp34", "hsp35", "hsp36")

foreach ($ver in $versions) {
    Write-Host "Processing HSP $ver..."

    # クローン
    if (-not (Test-Path "OpenHSP_$ver")) {
        git clone -b $ver https://github.com/onitama/OpenHSP.git "OpenHSP_$ver"
    }

    # 抽出
    python extract_commands.py "OpenHSP_$ver"

    # リネーム
    Move-Item Dictionary_extracted.csv "Dictionary_$ver.csv" -Force

    Write-Host "  -> Dictionary_$ver.csv"
}

Write-Host "All versions processed!"
```

## トラブルシューティング

### Q: リポジトリにアクセスできない

**A**: GitHubが利用できない場合は、HSP公式サイトからソースコードをダウンロード

### Q: C++の構造が変わっている

**A**: 正規表現パターンを調整するか、最新のソースコード構造に合わせて修正

### Q: すべての命令が抽出できない

**A**:
- 複数のソースファイルから抽出する必要がある
- プラグイン命令は別ファイルに定義されている
- マニュアルで補完が必要な場合もある

## 次のステップ

1. [手法2: テストスクリプト解析](method2-test-script-analysis.md) - 実際の動作確認
2. [手法4: コミュニティ情報収集](method4-community-info-gathering.md) - 追加情報の取得
3. [手法5: 辞書マージツール](method5-dictionary-merger.md) - 複数ソースの統合

## 参考

- [OpenHSP GitHub](https://github.com/onitama/OpenHSP)
- [HSP公式サイト](https://hsp.tv/)
- [HSP開発wiki](https://hsp.moe/)
- [C++パーサーライブラリ](https://github.com/tree-sitter/tree-sitter-cpp)
