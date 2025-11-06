# 手法4: コミュニティ情報収集法

## 概要

HSPコミュニティ、公式ドキュメント、既存ツールから命令情報を収集し、Dictionary.csvを補完・更新する方法です。

## メリット

- **実践的な情報**: 実際の使用例やTips付き
- **日本語情報が豊富**: HSPは日本発なので日本語リソースが充実
- **既存資産の活用**: 他のツールの辞書データを参考にできる
- **コミュニティサポート**: 不明点を質問できる

## デメリット

- **情報の正確性**: 公式以外の情報は検証が必要
- **網羅性に欠ける**: すべての命令がカバーされているとは限らない
- **手作業が多い**: 自動化が困難
- **情報の鮮度**: 古い情報が混在している可能性

## 情報源一覧

### 1. 公式リソース

#### 1.1 HSP公式サイト

- **URL**: https://hsp.tv/
- **内容**:
  - 最新情報、ダウンロード
  - 公式マニュアル
  - 命令リファレンス

#### 1.2 HSPヘルプファイル (hshelp.chm)

- **場所**: HSPインストールディレクトリ
- **内容**:
  - 全命令の詳細説明
  - 構文、パラメータ、サンプルコード
  - バージョン別の差異情報

**抽出方法**:

```powershell
# CHMファイルを展開
hh.exe -decompile output_folder "C:\hsp36\hsphelp.chm"

# HTMLファイルから命令リストを抽出
$htmlFiles = Get-ChildItem output_folder -Filter "*.htm" -Recurse

foreach ($file in $htmlFiles) {
    $content = Get-Content $file.FullName -Encoding UTF8

    # <h2>命令名</h2> を検索
    if ($content -match '<h2>(\w+)</h2>') {
        $cmdName = $matches[1]
        Write-Output $cmdName
    }
}
```

#### 1.3 HSP拡張プラグイン公式ページ

- **hsp3dish**: https://hsp.tv/make/hsp3dish.html
- **HGIMG4**: https://hsp.tv/make/hgimg4.html
- **各プラグインのドキュメント**

### 2. コミュニティリソース

#### 2.1 HSP開発wiki

- **URL**: https://hsp.moe/ (例)
- **内容**:
  - Tips集
  - サンプルコード
  - FAQ

#### 2.2 HSP掲示板・フォーラム

- **HSPTV!掲示板**: https://hsp.tv/bbs/
  - 技術的な質問・回答
  - 新機能の議論
  - バグ報告

#### 2.3 GitHub

- **OpenHSP Issues**: https://github.com/onitama/OpenHSP/issues
  - バグレポート
  - 機能リクエスト
  - 開発者とのディスカッション

#### 2.4 SNS・ブログ

- **Twitter (X)**: `#HSP` タグ
- **Qiita**: HSPタグの記事
- **個人ブログ**: HSP関連の技術記事

### 3. 既存ツール・プロジェクト

#### 3.1 他のHSPデコンパイラ

GitHub等で公開されている他のデコンパイラプロジェクトを参考:

```bash
# GitHubで検索
# "HSP decompiler", "HSP disassembler", "HSP bytecode"
```

**参考にできる情報**:
- 命令コード対応表
- バイトコード構造の解析結果
- 既知の問題と対処法

#### 3.2 HSPエディタプラグイン

- **HSP Code Assistant**: 命令補完データ
- **Syntax Highlighter**: キーワードリスト

#### 3.3 IDE/エディタ拡張

- **VS Code HSP拡張**: 命令定義JSON
- **Vim/Emacs構文ファイル**: キーワードリスト

### 4. 公式ドキュメントアーカイブ

#### 4.1 過去バージョンのマニュアル

各バージョンのドキュメントを比較:

```
HSP_Manuals/
├── HSP_3.4_manual.pdf
├── HSP_3.5_manual.pdf
├── HSP_3.6_manual.pdf
└── changes_log.txt  ← バージョン間の変更履歴
```

#### 4.2 変更履歴ファイル

- **history.txt**: HSPアーカイブに含まれる
- **更新履歴**: 公式サイトのニュース

## 実装手順

### ステップ1: CHMヘルプからの命令リスト抽出

#### 1.1 CHMファイルの展開

```powershell
# extract_chm.ps1

$hspDir = "C:\hsp36"
$chmFile = "$hspDir\hsphelp.chm"
$outputDir = "extracted_help"

# CHMを展開
hh.exe -decompile $outputDir $chmFile

Write-Host "CHM extracted to: $outputDir"
```

#### 1.2 命令リストの抽出

```python
#!/usr/bin/env python3
# extract_commands_from_chm.py

import os
import re
from bs4 import BeautifulSoup
from pathlib import Path

def extract_commands_from_html(html_dir):
    commands = []

    for html_file in Path(html_dir).rglob('*.htm'):
        with open(html_file, 'r', encoding='shift-jis', errors='ignore') as f:
            soup = BeautifulSoup(f.read(), 'html.parser')

            # <h2>命令名</h2> を検索
            h2_tags = soup.find_all('h2')

            for h2 in h2_tags:
                cmd_name = h2.text.strip()

                # 構文を検索 (通常は<pre>タグ内)
                pre_tag = h2.find_next('pre')

                if pre_tag:
                    syntax = pre_tag.text.strip()

                    # パラメータを解析
                    params = re.findall(r'(\w+)\s+var,', syntax)

                    commands.append({
                        'name': cmd_name,
                        'syntax': syntax,
                        'params': params,
                        'file': html_file.name
                    })

    return commands

def export_to_csv(commands, output_file):
    import csv

    with open(output_file, 'w', encoding='utf-8', newline='') as f:
        writer = csv.DictWriter(f, fieldnames=['name', 'syntax', 'params', 'file'])
        writer.writeheader()
        writer.writerows(commands)

if __name__ == '__main__':
    commands = extract_commands_from_html('extracted_help')
    export_to_csv(commands, 'commands_from_chm.csv')
    print(f"Extracted {len(commands)} commands")
```

### ステップ2: コミュニティデータの収集

#### 2.1 GitHub検索スクリプト

```python
#!/usr/bin/env python3
# search_github_hsp_projects.py

import requests
import json

def search_hsp_decompilers():
    """
    GitHubでHSP関連プロジェクトを検索
    """
    query = "HSP decompiler OR HSP disassembler language:C++"
    url = f"https://api.github.com/search/repositories?q={query}"

    response = requests.get(url)
    data = response.json()

    projects = []
    for repo in data.get('items', []):
        projects.append({
            'name': repo['name'],
            'url': repo['html_url'],
            'description': repo['description'],
            'stars': repo['stargazers_count']
        })

    return projects

if __name__ == '__main__':
    projects = search_hsp_decompilers()

    print(f"Found {len(projects)} HSP-related projects:\n")
    for proj in sorted(projects, key=lambda x: x['stars'], reverse=True):
        print(f"★ {proj['stars']:3d} - {proj['name']}")
        print(f"     {proj['url']}")
        print(f"     {proj['description']}\n")
```

#### 2.2 既存プロジェクトからの辞書データ抽出

```bash
# clone_and_extract.sh

# 関連プロジェクトをクローン
git clone https://github.com/example/hsp-decompiler.git temp_project

# 辞書ファイルを検索
find temp_project -name "*.csv" -o -name "*.json" -o -name "*dict*" -o -name "*command*"

# Dictionary.csvがあれば比較
if [ -f "temp_project/dictionary.csv" ]; then
    diff Dictionary.csv temp_project/dictionary.csv > differences.txt
    echo "Differences saved to differences.txt"
fi

# クリーンアップ
rm -rf temp_project
```

### ステップ3: 掲示板・フォーラムの情報収集

#### 3.1 手動での情報収集

**チェックポイント**:
1. バージョンアップ告知スレッド
2. 新機能の議論
3. 不具合報告（命令の追加・削除情報）
4. Tips・テクニック集

**記録フォーマット**:

```markdown
# フォーラム情報メモ

## 日付: 2025-11-07
## ソース: HSPTV!掲示板

### 新規命令の情報

- **命令名**: `newcmd`
- **追加バージョン**: HSP 3.6
- **TypeCode**: 不明
- **説明**: ファイルの高速読み込み
- **構文**: `newcmd filename, buf`
- **リンク**: https://hsp.tv/bbs/...

---

## 日付: 2025-10-15
...
```

#### 3.2 Webスクレイピング（注意点）

**⚠️ 注意**: スクレイピングはサイトの利用規約を確認してから実施

```python
#!/usr/bin/env python3
# scrape_hsp_forum.py (例)

import requests
from bs4 import BeautifulSoup

def scrape_forum_threads(url):
    """
    掲示板から最新スレッドを取得
    （実際のサイト構造に合わせて実装）
    """
    response = requests.get(url)
    soup = BeautifulSoup(response.content, 'html.parser')

    threads = []
    # サイト構造に応じて解析
    # ...

    return threads

# 利用規約を確認し、適切な間隔でアクセスすること
```

### ステップ4: 情報の統合とバリデーション

#### 4.1 情報源の信頼度評価

| 情報源 | 信頼度 | 優先順位 |
|--------|--------|----------|
| 公式ソースコード | ★★★★★ | 1 |
| 公式ヘルプ | ★★★★★ | 1 |
| 公式掲示板 | ★★★★☆ | 2 |
| GitHub Issues | ★★★☆☆ | 3 |
| 個人ブログ | ★★☆☆☆ | 4 |
| 匿名掲示板 | ★☆☆☆☆ | 5 |

#### 4.2 情報の統合

```python
#!/usr/bin/env python3
# merge_community_data.py

def merge_command_info(sources):
    """
    複数の情報源から命令情報を統合
    """
    merged = {}

    for source in sources:
        for cmd in source['commands']:
            key = (cmd['type_code'], cmd['value_code'])

            if key not in merged:
                merged[key] = {
                    'name': cmd['name'],
                    'type': cmd.get('type', 'Unknown'),
                    'sources': [],
                    'confidence': 0
                }

            # ソースを記録
            merged[key]['sources'].append({
                'source': source['name'],
                'confidence': source['confidence']
            })

            # 信頼度を更新
            merged[key]['confidence'] = max(
                merged[key]['confidence'],
                source['confidence']
            )

            # 情報が一致しない場合は警告
            if merged[key]['name'] != cmd['name']:
                print(f"Warning: Name mismatch for {key}: "
                      f"{merged[key]['name']} vs {cmd['name']}")

    return merged

# 使用例
sources = [
    {
        'name': 'Official CHM',
        'confidence': 5,
        'commands': commands_from_chm
    },
    {
        'name': 'GitHub Project A',
        'confidence': 3,
        'commands': commands_from_github
    }
]

merged_data = merge_command_info(sources)
```

### ステップ5: コミュニティへの貢献

#### 5.1 不明点の質問

**質問テンプレート**:

```
件名: HSP 3.6の新規命令について

お世話になります。
HSPのデコンパイラを開発しているのですが、
以下の命令コードに対応する命令名が分かりません。

- TypeCode: 0x08
- ValueCode: 0x0050

HSP 3.6でコンパイルされた.axファイルに含まれていました。
ご存知の方がいらっしゃいましたら、ご教示いただけると幸いです。

よろしくお願いいたします。
```

#### 5.2 情報の共有

- **GitHub**: 収集した情報をIssue/Wikiで共有
- **掲示板**: 調査結果を投稿
- **ブログ**: 詳細な解析記事を執筆

## 実践例

### 例1: HSP 3.6の新規命令調査

```
1. 公式サイトで更新情報を確認
   → "新規命令: fastload, quicksave"

2. hsphelp.chmで詳細を確認
   → 構文、パラメータを記録

3. テストスクリプトで動作確認
   → バイトコードを解析

4. TypeCode/ValueCodeを特定
   → 0x08, 0x0050 = fastload
   → 0x08, 0x0051 = quicksave

5. Dictionary.csvに追加
```

### 例2: 既存プロジェクトとの比較

```bash
# 他のデコンパイラプロジェクトをクローン
git clone https://github.com/example/hsp-tools.git

# 辞書ファイルを比較
diff Dictionary.csv hsp-tools/commands.csv

# 差分から新規情報を抽出
# 53a54,56
# > 0x08,0x0052,newcmd,HspFunction,
# > 0x08,0x0053,anothercmd,HspFunction,

# 情報を確認して追加
```

## トラブルシューティング

### Q: 情報が見つからない

**A**:
- 複数のキーワードで検索
- 英語・日本語両方で検索
- 古いバージョンのドキュメントも確認

### Q: 情報が矛盾している

**A**:
- より信頼できる情報源を優先
- 実際に動作確認を行う
- 複数の情報源で裏付けを取る

### Q: コミュニティから返答がない

**A**:
- 質問を具体的にする
- 調査済みの内容を記載
- 適切なフォーラム・タイミングで質問

## 次のステップ

1. [手法1: 未知命令検出](method1-unknown-code-detection.md) - 自動検出と併用
2. [手法3: 公式ソース解析](method3-official-source-analysis.md) - 最も正確な情報
3. [辞書マージツール](method5-dictionary-merger.md) - 情報を統合

## 参考

- [HSP公式サイト](https://hsp.tv/)
- [HSPTV!掲示板](https://hsp.tv/bbs/)
- [OpenHSP GitHub](https://github.com/onitama/OpenHSP)
- [BeautifulSoup ドキュメント](https://www.crummy.com/software/BeautifulSoup/)
