# プロジェクト概要

HSP (Hot Soup Processor) 2/3 でコンパイルされたファイル(*.ax, *.exe, *.dpm)をソースファイル(*.hsp, *.as)に逆コンパイルするWindows Formsアプリケーション。

# ビルド・実行

```bash
dotnet build              # デバッグビルド (コンソール出力あり、OutputType=Exe)
dotnet build -c Release   # リリースビルド (コンソール非表示、OutputType=WinExe)
dotnet run                # 実行
```

ビルド時に `BuildInfo.cs` が自動生成される(Gitコミットハッシュ、ビルドタイムスタンプ)。`obj/` 配下に出力されるため手動編集不要。

テストプロジェクトは存在しない。

# 技術スタック

- .NET 10 / C# 13.0 / Windows Forms
- `ImplicitUsings: disable` — using文は明示的に書く
- `Nullable: enable`
- StyleCop.Analyzers によるコード規約
- アセンブリ名: `HSPdecom`、ルート名前空間: `KttK.HspDecompiler`

# コードアーキテクチャ

逆コンパイルは3段階のパイプラインで処理される:

1. **ExeToDpm** — EXEファイルからDPMデータを抽出(`Unexe`)
2. **DpmToAx** — DPMパッケージからAXファイルを展開(`Undpm`)。DPMXフォーマット(旧形式)とDPM2フォーマット(HSP 3.7+)の両方に対応。暗号化ファイルの復号もここで行う(`HspCrypto/`)
3. **Ax2ToAs / Ax3ToAs** — AXバイトコードをHSPソースコードに変換。HSP2は`Ax2Decoder`、HSP3は`Ax3Decoder`が担当

`HspDecoder` がエントリポイントとなり、AXファイルの先頭4バイト("HSP2" or "HSP3")でデコーダを切り替える。両デコーダは `AbstractAxDecoder` を継承。

## HSP3の逆コンパイル処理 (Ax3ToAs)

HSP3の逆コンパイルは以下の段階で進む:

- `AxData` / `AxHeader` — バイナリからヘッダーとデータセグメントを読み取り
- `LexicalAnalyzer` → `PrimitiveToken` — バイトコードをプリミティブトークンに変換
- `SyntacticAnalyzer` → `LogicalLine` — トークン列を論理行(コマンド、代入文、if文など)に構文解析
- `Hsp3Dictionary` — `Dictionary.csv` から命令名・関数名の辞書を読み込み、数値コードを名前に変換

## UIとユーティリティ

- `Dialog/deHspDialog` — メインダイアログ(ファイル選択、逆コンパイル実行、結果表示)
- `Dialog/AboutDialog` — バージョン情報ダイアログ
- `HspConsole` — ログ出力ユーティリティ
- `Dictionary.csv` — HSP命令の辞書データ。ビルド時に出力ディレクトリにコピーされる

# コード規約

- `AllowDecryption` は `#if` で条件コンパイルに使用される(Debug/Release共に有効)
- using文は名前空間の外に配置 (`outside_namespace`)
- Allman style ブレース(すべての制御構造で改行後に `{`)
- `var` は型が明らかな場合のみ使用、組み込み型には明示的な型名を使う

# リリース

`v*` タグのプッシュで GitHub Actions が自動リリース。`dotnet publish` でビルドし、ZIPをGitHub Releasesに添付。
