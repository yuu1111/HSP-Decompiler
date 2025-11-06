# HSP逆コンパイラ (Fork)

Decompiler for Hot Soup Processor 2/3. Original source code written by Kitsutsuki.

**このプロジェクトについて:**
- オリジナル: [YSRKEN/HSP-Decompiler](https://github.com/YSRKEN/HSP-Decompiler)
- Fork元: [kaz-mighty/HSP-Decompiler](https://github.com/kaz-mighty/HSP-Decompiler)
- このリポジトリ: [yuu1111/HSP-Decompiler](https://github.com/yuu1111/HSP-Decompiler)

## 概要

[HSP](http://hsp.tv/)でコンパイルされたファイル(*.ax, *.exe, *.dpm)をソースファイル(*.hsp, *.as)に戻すソフトです。

**対応バージョン:**
- HSP2
- HSP3 (HSP 3.7+のDPM2フォーマットにも対応)

## 主な機能

- HSP2/HSP3のコンパイル済みファイルの逆コンパイル
- DPM2フォーマット(HSP 3.7+)のサポート
- 変数名復元機能
- Dictionary.csv による辞書データサポート
- ビルド情報の自動埋め込み(Gitコミットハッシュ、ビルド日時)

## このForkでの変更点

### Ver 1.0.0
- .NET 9への移行
- HSP 3.7+のDPM2フォーマット対応
  - HFPHED/HFPOBJヘッダー構造のサポート
  - 新しいファイルパック形式の解析
- Dictionary.csv のUTF-8エンコーディングサポート
- ビルド情報の自動生成機能
  - Gitコミットハッシュの埋め込み
  - ビルドタイムスタンプの記録
  - リポジトリURLの表示
- コード品質の向上
  - StyleCop.Analyzers によるコード規約の適用
  - Nullable参照型の有効化
  - C# 13.0 の機能活用

## 実行環境

- Windows
- .NET 9 Runtime

## 開発環境

- .NET 9 SDK
- C# 13.0
- Visual Studio 2022 / JetBrains Rider / VS Code

## ビルド方法

```bash
# プロジェクトのクローン
git clone https://github.com/yuu1111/HSP-Decompiler.git
cd HSP-Decompiler

# ビルド
dotnet build

# リリースビルド
dotnet build -c Release
```

ビルド時に自動的に以下の情報が埋め込まれます:
- Gitコミットハッシュ
- ビルドタイムスタンプ (UTC)
- リポジトリURL

## ライセンス

PDSライセンス(HSPdecoはzlib/libpngライセンス。詳しくは[LICENSE](LICENSE)ファイルを参照)

## クレジット

このソフトウェアは以下の方々の成果物をベースにしています:
- [きつつき](http://www.vector.co.jp/vpack/browse/person/an043697.html) - オリジナル作者
- [minorshift](https://osdn.jp/users/minorshift/) - HSPdeco
- [したぷる](https://www.blogger.com/profile/00794326060600750840) - HSPdecom
- [YSRKEN](https://github.com/YSRKEN) - HSPdecom、GitHub移行
- [kaz-mighty](https://github.com/kaz-mighty) - 鍵の重複対策、HSP3.5 beta3対応

## 参考資料

- [HSP公式サイト](http://hsp.tv/)
- [OpenHSP](https://github.com/onitama/OpenHSP) - HSPのオープンソース実装
