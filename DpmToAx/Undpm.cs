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
                if (ret.ReadHeader())
                {
                    return ret;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        private long startPosition;
        private long streamLength;
        private long fileOffsetStart;

        private bool ReadHeader()
        {
            this.startPosition = this.reader.BaseStream.Position;
            this.streamLength = this.reader.BaseStream.Length - this.startPosition;
            char[] identifier = this.reader.ReadChars(4);
            if (identifier.Length < 4)
            {
                return false;
            }

            this.reader.BaseStream.Seek(this.startPosition, SeekOrigin.Begin);
            if ((identifier[0] == 'M') && (identifier[1] == 'Z'))
            {
                Win32ExeHeader winHeader = Win32ExeHeader.FromBinaryReader(this.reader);
                if (winHeader == null)
                {
                    return false;
                }

                this.startPosition += winHeader.EndOfExecutableRegion;
                this.streamLength = this.reader.BaseStream.Length - this.startPosition;
                this.reader.BaseStream.Seek(this.startPosition, SeekOrigin.Begin);
                identifier = this.reader.ReadChars(4);
                if (identifier.Length < 4)
                {
                    return false;
                }
            }

            if (!((identifier[0] == 'D') && (identifier[1] == 'P') && (identifier[2] == 'M') && (identifier[3] == 'X')))
            {
                return false;
            }

            this.reader.BaseStream.Seek(this.startPosition, SeekOrigin.Begin);
            this.reader.ReadInt32();
            this.reader.ReadInt32();
            int fileCount = this.reader.ReadInt32();
            this.reader.ReadInt32();
            this.files.Capacity = fileCount;
            this.fileOffsetStart = this.startPosition + 0x10 + (fileCount * 0x20);
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
                if ((file.FileOffset + file.FileSize) > this.streamLength)
                {
                    return false;
                }

                this.files.Add(file);
            }

            return true;
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
