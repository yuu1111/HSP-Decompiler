using System;

namespace KttK.HspDecompiler.DpmToAx
{
    internal struct DpmFileState
    {
        internal string FileName;
        internal int Unknown;
        internal int Encryptionkey;
        internal int FileOffset;
        internal int FileSize;

        internal bool IsEncrypted
        {
            get { return this.Encryptionkey != 0; }
        }

        internal Undpm Parent;

        internal byte[] GetFile()
        {
            if (this.IsEncrypted)
            {
                throw new Exception("暗号化ファイルには対応していません");
            }

            if (this.Parent == null)
            {
                throw new Exception("親ファイル未設定");
            }

            return this.Parent.GetFile(this.FileOffset, this.FileSize);
        }
    }
}
