using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using KttK.HspDecompiler.DpmToAx;
using KttK.HspDecompiler.Ax2ToAs;
using KttK.HspDecompiler.Ax3ToAs;

namespace KttK.HspDecompiler
{
    internal sealed class HspDecoder
    {
        private const string dictionaryFileName = "Dictionary.csv";
        private static Hsp3Dictionary dictionary;

        internal static bool Initialize()
        {
            string dictionaryPath = Program.ExeDir + dictionaryFileName;
            try
            {
                dictionary = Hsp3Dictionary.FromFile(dictionaryPath);
            }
            catch
            {
                goto err;
            }

            if (dictionary != null)
            {
                HspConsole.WriteLog("load " + dictionaryFileName + ":succeeded");
                return true;
            }

            err:
            HspConsole.WriteLog("load " + dictionaryFileName + ":failed");
            MessageBox.Show(dictionaryFileName + "の読込に失敗しました");
            dictionary = null;
            return false;
        }

        internal void DecompressDpm(BinaryReader reader, ListView dpmFileListView, string outputDir)
        {
            if (reader == null)
                throw new ArgumentNullException();
            if (dpmFileListView == null)
                throw new ArgumentNullException();

            dpmFileListView.Items.Clear();
            HspConsole.Write("DPMヘッダーの開始位置を検索中...");
            Undpm undpm = Undpm.FromBinaryReader(reader);
            if (undpm == null)
                throw new HspDecoderException("DPMヘッダーが見つかりません(HSPの実行ファイルではありません)");
            if ((undpm.FileList == null) || (undpm.FileList.Count == 0))
                throw new HspDecoderException("DPMにファイルが含まれていません");

            int encryptCount = 0;
            int fileCount = undpm.FileList.Count;
            foreach (DpmFileState file in undpm.FileList)
            {
                string[] itemParams = new string[4];
                itemParams[0] = file.FileName;
                if (file.IsEncrypted)
                {
                    itemParams[1] = "有";
#if AllowDecryption
                    //deHSP100 start.axは暗号ファイルに数えないことにする
                    if (!file.FileName.Equals("start.ax", StringComparison.OrdinalIgnoreCase))
#endif
                        encryptCount++;
                }
                else
                    itemParams[1] = "－";

                itemParams[2] = string.Format("0x{0:X08}", file.FileOffset);
                itemParams[3] = string.Format("0x{0:X08}", file.FileSize);
                dpmFileListView.Items.Add(new ListViewItem(itemParams));
            }

            Thread.Sleep(0);
            if ((fileCount - encryptCount) <= 0)
            {
                MessageBox.Show("すべてのファイルが暗号化されています", fileCount + "ファイル中、" + encryptCount + "ファイルが暗号化されています。", MessageBoxButtons.OK);
                HspConsole.Write("展開中断");
                return;
            }

            if (encryptCount > 0)
            {
                DialogResult result = MessageBox.Show("暗号化されたファイルがあります。" + Environment.NewLine + "暗号化されたファイルを無視して展開を続けますか？",
                    fileCount + "ファイル中、" + encryptCount + "ファイルが暗号化されています。", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes)
                {
                    HspConsole.Write("展開中断");
                    return;
                }
            }

            if (!Directory.Exists(outputDir))
            {
                try
                {
                    Directory.CreateDirectory(outputDir);
                }
                catch
                {
                    throw new HspDecoderException("ディレクトリ" + outputDir + "の作成に失敗しました");

                }
            }

            byte[] buffer = null;
            FileStream saveStream = null;
            foreach (DpmFileState file in undpm.FileList)
            {
                if (file.IsEncrypted)
                {
#if AllowDecryption
                    if (!file.FileName.Equals("start.ax", StringComparison.OrdinalIgnoreCase))
#endif
                    {
                        HspConsole.Write(file.FileName + "は暗号化されています");
                        continue;
                    }
                }

                string outputPath = outputDir + file.FileName;
                if (File.Exists(outputPath))
                {
                    HspConsole.Write(file.FileName + "と同名のファイルが既に存在します");
                    continue;
                }

                if (!undpm.Seek(file))
                {
                    HspConsole.Write(file.FileName + "の開始位置に移動できませんでした");
                    continue;
                }

                buffer = reader.ReadBytes(file.FileSize);
#if AllowDecryption
                if (file.IsEncrypted)
                {
                    HspConsole.Write(file.FileName + "の復号中...");

                    DpmToAx.HspCrypto.HspCryptoTransform decrypter =
                        DpmToAx.HspCrypto.HspCryptoTransform.CrackEncryption(buffer, dictionary, outputPath); // 変更
                    if (decrypter == null)
                    {
                        HspConsole.Write(file.FileName + "の復号に失敗しました");

                        continue;
                    }

                    buffer = decrypter.Decryption(buffer);
                }
#endif
                try
                {
                    saveStream = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write);
                    saveStream.Write(buffer, 0, buffer.Length);
                }
                catch
                {
                    HspConsole.Warning(file.FileName + "の保存に失敗しました");
                }
                finally
                {
                    if (saveStream != null)
                        saveStream.Close();
                }

            }

            HspConsole.Write("展開終了");
        }

        internal void Decode(BinaryReader reader, string outputPath)
        {
            if (reader == null)
                throw new ArgumentNullException();
            if (dictionary == null)
                throw new InvalidOperationException();

            HspConsole.StartParagraph();
            List<string> lines = null;
            HspConsole.Write("逆コンパイル中...");
            HspConsole.StartParagraph();
            lines = getDecoder(reader).Decode(reader);

            HspConsole.EndParagraph();
            HspConsole.Write("逆コンパイル終了");
            HspConsole.EndParagraph();
            HspConsole.Write(Path.GetFileName(outputPath) + "に出力");

            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(outputPath, false, Encoding.GetEncoding("SHIFT-JIS"));
                foreach (string line in lines)
                    writer.WriteLine(line);
                HspConsole.Write("解析終了");
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        AbstractAxDecoder getDecoder(BinaryReader reader)
        {
            long startPosition = reader.BaseStream.Position;
            char[] buffer = reader.ReadChars(4);
            string bufStr = new string(buffer);
            reader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
            if (bufStr.Equals("HSP2", StringComparison.Ordinal))
            {
                return new Ax2Decoder();
            }
            else if (bufStr.Equals("HSP3", StringComparison.Ordinal))
            {
                Ax3Decoder decoder = new Ax3Decoder();
                decoder.Dictionary = dictionary;
                return decoder;
            }

            throw new HspDecoderException("HSP2でもHSP3でもない形式");


        }
    }
}
