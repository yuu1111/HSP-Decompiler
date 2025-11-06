using System;
using System.Collections.Generic;
using System.IO;

namespace KttK.HspDecompiler.DpmToAx
{
    internal sealed class Undpm : IDisposable
    {
        private Undpm()
        {
        }

        internal static Undpm FromBinaryReader(BinaryReader reader)
        {
            Undpm ret = new Undpm();
            try
            {
                ret.reader = reader;
                Console.WriteLine("ReadHeader()を呼び出します");
                if (ret.ReadHeader())
                {
                    Console.WriteLine("ReadHeader()が成功しました");
                    return ret;
                }
                else
                {
                    Console.WriteLine("ReadHeader()がfalseを返しました");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("例外が発生しました: {0}", ex.Message));
                Console.WriteLine(string.Format("スタックトレース: {0}", ex.StackTrace));
                return null;
            }

            return null;
        }

        private long startPosition;
        private long streamLength;
        private long fileOffsetStart;

        private long FindDpmxSignature()
        {
            long originalPosition = this.reader.BaseStream.Position;
            this.reader.BaseStream.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[4096];
            byte[] dpmxSignature = new byte[] { (byte)'D', (byte)'P', (byte)'M', (byte)'X' };
            long currentPos = 0;
            long fileLength = this.reader.BaseStream.Length;
            long lastProgressReport = 0;
            const long progressInterval = 1024 * 1024; // 1MBごとに進捗報告

            Console.WriteLine(string.Format("スキャン開始: ファイルサイズ = {0:N0} バイト", fileLength));

            while (currentPos < fileLength)
            {
                // 進捗報告（1MBごと、または最初と最後）
                if (currentPos - lastProgressReport >= progressInterval || currentPos == 0)
                {
                    double progress = (double)currentPos / fileLength * 100.0;
                    Console.WriteLine(string.Format("スキャン中... 位置: 0x{0:X08} ({1:F1}%)", currentPos, progress));
                    lastProgressReport = currentPos;
                }

                this.reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
                int bytesRead = this.reader.BaseStream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                int searchEnd = Math.Min(bytesRead, bytesRead - 3);
                for (int i = 0; i <= searchEnd; i++)
                {
                    if (i + 3 < bytesRead &&
                        buffer[i] == dpmxSignature[0] &&
                        buffer[i + 1] == dpmxSignature[1] &&
                        buffer[i + 2] == dpmxSignature[2] &&
                        buffer[i + 3] == dpmxSignature[3])
                    {
                        long foundPosition = currentPos + i;
                        Console.WriteLine(string.Format("DPMXシグネチャを0x{0:X08}で発見", foundPosition));
                        this.reader.BaseStream.Seek(originalPosition, SeekOrigin.Begin);
                        return foundPosition;
                    }
                }

                // Move forward, overlapping by 3 bytes to handle signatures at buffer boundaries
                currentPos += bytesRead - 3;
                if (currentPos <= 0 || currentPos >= this.reader.BaseStream.Length)
                {
                    break;
                }
            }

            Console.WriteLine(string.Format("スキャン完了: {0:N0} バイト処理", fileLength));
            Console.WriteLine("DPMXシグネチャが見つかりません");
            this.reader.BaseStream.Seek(originalPosition, SeekOrigin.Begin);
            return -1;
        }

        private bool ReadHeader()
        {
            this.startPosition = this.reader.BaseStream.Position;
            this.streamLength = this.reader.BaseStream.Length - this.startPosition;
            Console.WriteLine(string.Format("ファイルサイズ: {0} バイト", this.streamLength));

            char[] identifier = this.reader.ReadChars(4);
            if (identifier.Length < 4)
            {
                Console.WriteLine("ファイルが小さすぎます（4バイト未満）");
                return false;
            }

            Console.WriteLine(string.Format("最初の4バイト: {0}{1}{2}{3}", identifier[0], identifier[1], identifier[2], identifier[3]));

            this.reader.BaseStream.Seek(this.startPosition, SeekOrigin.Begin);
            if ((identifier[0] == 'M') && (identifier[1] == 'Z'))
            {
                Console.WriteLine("MZヘッダー検出（Windows実行ファイル）");
                Win32ExeHeader winHeader = Win32ExeHeader.FromBinaryReader(this.reader);
                bool foundDpmx = false;

                if (winHeader != null && winHeader.EndOfExecutableRegion > 0)
                {
                    // Try the traditional method first
                    long traditionalPosition = this.startPosition + winHeader.EndOfExecutableRegion;
                    Console.WriteLine(string.Format("従来のメソッド: EndOfExecutableRegion = 0x{0:X08}", winHeader.EndOfExecutableRegion));
                    Console.WriteLine(string.Format("DPMXを0x{0:X08}で確認中...", traditionalPosition));

                    this.reader.BaseStream.Seek(traditionalPosition, SeekOrigin.Begin);
                    identifier = this.reader.ReadChars(4);

                    if (identifier.Length >= 4)
                    {
                        Console.WriteLine(string.Format("0x{0:X08}の4バイト: {1}{2}{3}{4}",
                            traditionalPosition, identifier[0], identifier[1], identifier[2], identifier[3]));
                    }

                    if (identifier.Length >= 4 &&
                        identifier[0] == 'D' && identifier[1] == 'P' &&
                        identifier[2] == 'M' && (identifier[3] == 'X' || identifier[3] == '2'))
                    {
                        // Traditional method succeeded
                        string format = identifier[3] == 'X' ? "DPMX" : "DPM2";
                        Console.WriteLine(string.Format("従来のメソッドで成功 ({0}フォーマット)", format));
                        this.startPosition = traditionalPosition;
                        this.streamLength = this.reader.BaseStream.Length - this.startPosition;
                        foundDpmx = true;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("従来のメソッドでDPM/DPMXが見つかりませんでした（見つかったシグネチャ: {0}{1}{2}{3}）",
                            identifier[0], identifier[1], identifier[2], identifier[3]));
                    }
                }
                else
                {
                    if (winHeader == null)
                    {
                        Console.WriteLine("Win32ExeHeaderの解析に失敗");
                    }
                    else
                    {
                        Console.WriteLine(string.Format("EndOfExecutableRegionが無効: {0}", winHeader.EndOfExecutableRegion));
                    }
                }

                if (!foundDpmx)
                {
                    // Traditional method failed or PE header couldn't be parsed, search for DPMX signature
                    Console.WriteLine("ファイル全体をスキャンしてDPMXシグネチャを検索中...");
                    long dpmxPosition = this.FindDpmxSignature();
                    if (dpmxPosition == -1)
                    {
                        return false;
                    }

                    this.startPosition = dpmxPosition;
                    this.streamLength = this.reader.BaseStream.Length - this.startPosition;
                    this.reader.BaseStream.Seek(this.startPosition, SeekOrigin.Begin);
                    identifier = this.reader.ReadChars(4);
                    if (identifier.Length < 4)
                    {
                        Console.WriteLine("DPMXシグネチャ位置での読み取りに失敗");
                        return false;
                    }
                }
            }
            else
            {
                Console.WriteLine("MZヘッダーではありません（非Windows実行ファイル）");
            }

            if (!((identifier[0] == 'D') && (identifier[1] == 'P') && (identifier[2] == 'M') && (identifier[3] == 'X' || identifier[3] == '2')))
            {
                Console.WriteLine(string.Format("有効なDPMシグネチャではありません: {0}{1}{2}{3}", identifier[0], identifier[1], identifier[2], identifier[3]));
                return false;
            }

            bool isDpm2 = identifier[3] == '2';
            string detectedFormat = isDpm2 ? "DPM2" : "DPMX";
            Console.WriteLine(string.Format("{0}フォーマットを検出しました", detectedFormat));

            this.reader.BaseStream.Seek(this.startPosition, SeekOrigin.Begin);

            if (isDpm2)
            {
                return this.ReadDpm2Header();
            }
            else
            {
                return this.ReadDpmxHeader();
            }
        }

        private bool ReadDpmxHeader()
        {
            // DPMX format (旧形式)
            this.reader.ReadInt32(); // "DPMX"
            this.reader.ReadInt32();
            int fileCount = this.reader.ReadInt32();
            this.reader.ReadInt32();
            Console.WriteLine(string.Format("ファイル数: {0}", fileCount));
            this.files.Capacity = fileCount;
            this.fileOffsetStart = this.startPosition + 0x10 + (fileCount * 0x20);
            Console.WriteLine(string.Format("ファイルオフセット開始位置: 0x{0:X08}", this.fileOffsetStart));

            for (int i = 0; i < fileCount; i++)
            {
                DpmFileState file = default(DpmFileState);
                char[] chars = this.reader.ReadChars(16);
                int stringLength = 16;
                for (int j = 0; j < 16; j++)
                {
                    if (chars[j] == '\0')
                    {
                        stringLength = j;
                        break;
                    }
                }

                file.FileName = new string(chars, 0, stringLength);
                file.Unknown = this.reader.ReadInt32();
                file.Encryptionkey = this.reader.ReadInt32();
                file.FileOffset = this.reader.ReadInt32();
                file.FileSize = this.reader.ReadInt32();
                file.Parent = this;

                Console.WriteLine(string.Format("ファイル[{0}]: {1}, オフセット=0x{2:X08}, サイズ=0x{3:X08}",
                    i, file.FileName, file.FileOffset, file.FileSize));

                if ((file.FileOffset + file.FileSize) > this.streamLength)
                {
                    Console.WriteLine(string.Format("エラー: ファイル[{0}]のオフセット+サイズ({1})がストリーム長({2})を超えています",
                        i, file.FileOffset + file.FileSize, this.streamLength));
                    return false;
                }

                this.files.Add(file);
            }

            Console.WriteLine("ファイルリスト読み込み完了");
            return true;
        }

        private bool ReadDpm2Header()
        {
            // DPM2 format (HSP 3.7+)
            // HFPHED structure (32 bytes)
            this.reader.ReadInt32(); // "DPM2" (magic, already checked)
            int maxFile = this.reader.ReadInt32(); // max_file
            int strTable = this.reader.ReadInt32(); // strtable offset
            int fileTable = this.reader.ReadInt32(); // filetable offset
            int myname = this.reader.ReadInt32(); // myname (string table ID)
            int crc32 = this.reader.ReadInt32(); // crc32
            int seed = this.reader.ReadInt32(); // seed
            int salt = this.reader.ReadInt32(); // salt

            Console.WriteLine(string.Format("ファイル数: {0}", maxFile));
            Console.WriteLine(string.Format("文字列テーブルオフセット: 0x{0:X08}", strTable));
            Console.WriteLine(string.Format("ファイルテーブルオフセット: 0x{0:X08}", fileTable));

            this.files.Capacity = maxFile;
            this.fileOffsetStart = this.startPosition + fileTable;

            // Read string table
            long strTablePosition = this.startPosition + strTable;
            int strTableSize = fileTable - strTable;
            this.reader.BaseStream.Seek(strTablePosition, SeekOrigin.Begin);
            byte[] stringTableData = this.reader.ReadBytes(strTableSize);

            // Read file objects (HFPOBJ array, starts right after HFPHED at offset 32)
            this.reader.BaseStream.Seek(this.startPosition + 32, SeekOrigin.Begin);

            for (int i = 0; i < maxFile; i++)
            {
                DpmFileState file = default(DpmFileState);

                // HFPOBJ structure (32 bytes)
                short flag = this.reader.ReadInt16(); // flag
                short slot = this.reader.ReadInt16(); // slot
                int nameId = this.reader.ReadInt32(); // name (string table ID)
                long size = this.reader.ReadInt64(); // size (uint64_t)
                long offset = this.reader.ReadInt64(); // offset (uint64_t)
                int folderId = this.reader.ReadInt32(); // folder (string table ID)
                int crypt = this.reader.ReadInt32(); // crypt

                // Get filename from string table
                string filename = this.GetStringFromTable(stringTableData, nameId);
                string foldername = string.Empty;
                if (folderId > 0)
                {
                    foldername = this.GetStringFromTable(stringTableData, folderId);
                }

                // Combine folder and filename
                if (!string.IsNullOrEmpty(foldername))
                {
                    file.FileName = foldername + filename;
                }
                else
                {
                    file.FileName = filename;
                }

                file.Unknown = flag;
                file.Encryptionkey = crypt;
                file.FileOffset = (int)offset;
                file.FileSize = (int)size;
                file.Parent = this;

                Console.WriteLine(string.Format("ファイル[{0}]: {1}, オフセット=0x{2:X08}, サイズ=0x{3:X08}, crypt=0x{4:X08}",
                    i, file.FileName, file.FileOffset, file.FileSize, file.Encryptionkey));

                // Don't validate against streamLength for DPM2 - offsets are relative to filetable
                this.files.Add(file);
            }

            Console.WriteLine("ファイルリスト読み込み完了");
            return true;
        }

        private string GetStringFromTable(byte[] stringTable, int offset)
        {
            if (offset < 0 || offset >= stringTable.Length)
            {
                return string.Empty;
            }

            int length = 0;
            while (offset + length < stringTable.Length && stringTable[offset + length] != 0)
            {
                length++;
            }

            if (length == 0)
            {
                return string.Empty;
            }

            return System.Text.Encoding.UTF8.GetString(stringTable, offset, length);
        }

        private BinaryReader reader;
        private List<DpmFileState> files = new List<DpmFileState>();

        internal List<DpmFileState> FileList
        {
            get { return this.files; }
        }

        internal byte[] GetFile(int fileOffset, int fileSize)
        {
            this.reader.BaseStream.Seek(fileOffset, SeekOrigin.Begin);
            byte[] buffer = new byte[fileSize];
            this.reader.BaseStream.Read(buffer, 0, fileSize);
            return buffer;
        }

        internal DpmFileState? GetStartAx()
        {
            foreach (DpmFileState file in this.files)
            {
                if (file.FileName == "start.ax")
                {
                    return file;
                }
            }

            return null;
        }

        internal bool SeekStartAx()
        {
            foreach (DpmFileState file in this.files)
            {
                if (file.FileName.Equals("start.ax", StringComparison.Ordinal))
                {
                    this.reader.BaseStream.Seek(file.FileOffset + this.fileOffsetStart, SeekOrigin.Begin);
                    return true;
                }
            }

            return false;
        }

        internal bool Seek(DpmFileState file)
        {
            try
            {
                this.reader.BaseStream.Seek(file.FileOffset + this.fileOffsetStart, SeekOrigin.Begin);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
