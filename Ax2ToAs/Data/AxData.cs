using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace KttK.HspDecompiler.Ax2ToAs.Data
{
    /// <summary>
    /// AxData の概要の説明です。.
    /// </summary>
    internal class AxData
    {
        internal AxData()
        {
            // TODO: コンストラクタ ロジックをここに追加してください。
        }

        private Header header;
        private Label[] labels;
        private Dll[] dlls;
        private Func[] funcs;
        private Deffunc[] deffuncs;
        private Module[] modules;

        private byte[] labelData;
        private byte[] dllData;
        private byte[] funcData;
        private byte[] deffuncData;
        private byte[] moduleData;
        private byte[] tokenData;

        internal byte[] TokenData
        {
            get { return this.tokenData; }
        }

        private byte[] stringData;

        private void ReadData(Stream stream)
        {
            long startPosition = stream.Position;
            byte[] headerBuffer = new byte[80];
            if (stream.Read(headerBuffer, 0, 80) < 80)
            {
                throw new HspDecoderException("AxData", "ファイルヘッダーが見つかりません");
            }

            int[] buffer = new int[20];
            for (int i = 0; i < 20; i++)
            {
                buffer[i] = BitConverter.ToInt32(headerBuffer, i * 4);
            }

            try
            {
                this.header = Header.FromIntArray(buffer);
            }
            catch (Exception e)
            {
                throw new HspDecoderException("AxHeader", "ヘッダー解析中に想定外のエラー", e);
            }

            if (this.header == null)
            {
                throw new HspDecoderException("AxHeader", "ヘッダー解析に失敗");
            }

            try
            {
                Header head = this.header;
                this.tokenData = new byte[head.ScriptByte];
                stream.Seek(startPosition + head.ScriptOffset, SeekOrigin.Begin);
                stream.Read(this.tokenData, 0, head.ScriptByte);

                this.dllData = new byte[head.DllByte];
                stream.Seek(startPosition + head.DllOffset, SeekOrigin.Begin);
                stream.Read(this.dllData, 0, head.DllByte);

                this.funcData = new byte[head.FuncByte];
                stream.Seek(startPosition + head.FuncOffset, SeekOrigin.Begin);
                stream.Read(this.funcData, 0, head.FuncByte);

                this.deffuncData = new byte[head.DeffuncByte];
                stream.Seek(startPosition + head.DeffuncOffset, SeekOrigin.Begin);
                stream.Read(this.deffuncData, 0, head.DeffuncByte);

                this.moduleData = new byte[head.ModuleByte];
                stream.Seek(startPosition + head.ModuleOffset, SeekOrigin.Begin);
                stream.Read(this.moduleData, 0, head.ModuleByte);

                this.labelData = new byte[head.LabelByte];
                stream.Seek(startPosition + head.LabelOffset, SeekOrigin.Begin);
                stream.Read(this.labelData, 0, head.LabelByte);

                this.stringData = new byte[head.TextByte];
                stream.Seek(startPosition + head.TextOffset, SeekOrigin.Begin);
                stream.Read(this.stringData, 0, head.TextByte);
            }
            catch (Exception e)
            {
                throw new HspDecoderException("AxHeader", "ストリームの読み取り中に想定外のエラー", e);
            }

            stream.Seek(startPosition, SeekOrigin.Begin);
        }

        internal static AxData FromStream(Stream stream)
        {
            AxData data = new AxData();

            data.ReadData(stream);

            return data;
        }

        internal string GetString(int offset)
        {
            return this.ReadString(offset, this.stringData);
        }

        private string ReadString(int offset)
        {
            return this.ReadString(offset, this.stringData);
        }

        private string ReadString(int offset, byte[] dumpData)
        {
            Encoding encode = Encoding.GetEncoding("SHIFT-JIS");
            List<byte> buffer = new List<byte>();
            byte token;
            while (offset < dumpData.Length)
            {
                token = dumpData[offset];
                offset++;
                if (token == 0)
                {
                    break;
                }

                buffer.Add(token);
            }

            if (buffer.Count == 0)
            {
                return string.Empty;
            }

            byte[] bytes = new byte[buffer.Count];
            buffer.CopyTo(bytes);
            return encode.GetString(bytes);
        }

        private void ReadLabels()
        {
            this.labels = new Label[this.header.LabelCount];
            for (int i = 0; i < this.header.LabelCount; i++)
            {
                int offset = i * 4;
                this.labels[i] = new Label(i, BitConverter.ToInt32(this.labelData, offset));
            }
        }

        private void ReadDlls()
        {
            this.dlls = new Dll[this.header.DllCount];

            for (int i = 0; i < this.header.DllCount; i++)
            {
                int offset = 4 + (i * 24);
                this.dlls[i].Name = this.ReadString(offset, this.dllData);
            }
        }

        private void ReadFuncs()
        {
            this.funcs = new Func[this.header.FuncCount];
            for (int i = 0; i < this.header.FuncCount; i++)
            {
                int offset = i * 16;
                this.funcs[i].DllIndex = BitConverter.ToInt16(this.funcData, offset);
                offset += 4;
                this.funcs[i].HikiType = BitConverter.ToInt16(this.funcData, offset);
                offset += 4;
                int funcnameOffset = BitConverter.ToInt32(this.funcData, offset);
                this.funcs[i].Name = this.ReadString(funcnameOffset);
            }
        }

        private void ReadModules()
        {
            if (this.header.ModuleCount == 0)
            {
                return;
            }

            this.modules = new Module[this.header.ModuleCount];

            for (int i = 0; i < this.header.ModuleCount; i++)
            {
                int offset = 4 + (i * 24);
                this.modules[i].Name = this.ReadString(offset, this.dllData);
            }
        }

        private void ReadDeffuncs()
        {
            this.deffuncs = new Deffunc[this.header.DeffuncCount];

            for (int i = 0; i < this.header.DeffuncCount; i++)
            {
                int offset = i * 16;
                int labelIndex = BitConverter.ToInt32(this.deffuncData, offset) - 0x1000;
                this.labels[labelIndex].Deffunc = i;

                offset += 4;
                this.deffuncs[i].HikiType = BitConverter.ToInt16(this.deffuncData, offset);
                offset += 2;
                this.deffuncs[i].HikiCount = BitConverter.ToInt16(this.deffuncData, offset);
                offset += 2;
                int deffuncnameOffset = BitConverter.ToInt32(this.deffuncData, offset);
                this.deffuncs[i].Name = this.ReadString(deffuncnameOffset);
                this.labels[labelIndex].Name = this.deffuncs[i].ToString();
            }
        }

        private List<string> lines = new List<string>();

        internal void Decompile()
        {
            int startTime = Environment.TickCount;
            Token.CurrentData = this;
            this.lines.Clear();

            this.ReadLabels();
            this.ReadDlls();

            // ReadScript();
            this.ReadFuncs();
            this.ReadModules();
            this.ReadDeffuncs();

            if (this.dlls != null)
            {
                for (int i = 0; i < this.dlls.Length; i++)
                {
                    this.lines.Add(this.dlls[i].ToString());
                    if (this.funcs != null)
                    {
                        for (int j = 0; j < this.funcs.Length; j++)
                        {
                            if (this.funcs[j].DllIndex == i)
                            {
                                this.lines.Add(this.funcs[j].ToString());
                            }
                        }
                    }
                }
            }

            Token.SetZero();
            Token token;

            // ラベルが呼び出される回数を調べる
            try
            {
                while ((token = Token.GetNext()) != null)
                {
                    if (token.LabelIndex != -1)
                    {
                        this.labels[token.LabelIndex].LoadCount += 1;
                    }
                }
            }
            catch (Exception e)
            {
                throw new HspDecoderException("AxHeader", "ラベルの読み取り中に復帰できないエラー", e);
            }

            this.enabledCount = 0;
            for (int i = 0; i < this.labels.Length; i++)
            {
                if (this.labels[i].LoadCount > 0)
                {
                    this.labels[i].Enabled = true;
                }
                else
                {
                    this.labels[i].Enabled = false;
                }

                if (this.labels[i].Enabled)
                {
                    this.enabledCount++;
                }
            }

            string line;
            Token.SetZero();
            while ((line = this.GetLine()) != null)
            {
                this.lines.Add(line);
            }

            int scoopCount = this.tabNo - 1;

            // if (scoopCount != 0)
            //    MainProc.Process.WriteLog("※警告※ " + scoopCount.ToString() + "個の未解決スコープが残りました");
            return;
        }

        private void AddLabel()
        {
            for (int i = 0; i < this.labels.Length; i++)
            {
                if (!this.labels[i].Enabled)
                {
                    continue;
                }

                if (Token.Index >= this.labels[i].TokenIndex)
                {
                    this.lines.Add(this.labels[i].ToString());
                    this.labels[i].Enabled = false;
                }
            }
        }

        private string GetTab(int tab)
        {
            string ret = string.Empty;
            Debug.Assert(tab >= 0);
            for (int i = 0; i < tab; i++)
            {
                ret += "\t";
            }

            return ret;
        }

        private int tabNo = 1;
        private List<int> ifEnd = new List<int>();
        private int unknownCount;
        private int usedCount;
        private int enabledCount;

        private string GetLine()
        {
            string line = string.Empty;
            Token token = Token.GetNext();
            if (token == null)
            {
                return null;
            }

            for (int i = 0; i < this.ifEnd.Count; i++)
            {
                if ((token.Id == (int)this.ifEnd[i]) || (token.IfJumpId == (int)this.ifEnd[i]))
                {
                    this.tabNo--;
                    this.lines.Add(this.GetTab(this.tabNo) + "}");
                    this.ifEnd.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < this.labels.Length; i++)
            {
                if (!this.labels[i].Enabled)
                {
                    continue;
                }

                if (token.Id == this.labels[i].TokenIndex)
                {
                    this.lines.Add(this.labels[i].Name);
                    this.labels[i].Enabled = false;
                    this.usedCount++;
                }
            }

            bool tabPlus = token.TabPlus;
            int ifJumpTo = token.IfJumpTo;
            if (token.TabMinus)
            {
                this.tabNo--;
            }

            line = this.GetTab(this.tabNo);
            line += token.GetString();

            if (!token.IsKnown)
            {
                // MainProc.Process.WriteLog("解釈できないコード: " + (lines.Count + 1).ToString() + "行目 :" + token.GetString());
                this.unknownCount++;
            }

            if (!token.IsLineend)
            {
                while ((token = Token.GetNext()) != null)
                {
                    string add = token.GetString();
                    if (token.IsArg)
                    {
                        line += ", ";
                    }
                    else
                    {
                        line += " ";
                    }

                    line += add;
                    if (!token.IsKnown)
                    {
                        // MainProc.Process.WriteLog("解釈できないコード: " + (lines.Count + 1).ToString() + "行目 :" + token.GetString());
                        this.unknownCount++;
                    }

                    if (token.IsLineend)
                    {
                        break;
                    }
                }
            }

            if (tabPlus)
            {
                this.tabNo++;
            }

            if (ifJumpTo >= 0)
            {
                line += " {";
                this.ifEnd.Add(ifJumpTo);
            }

            return line;
        }

        internal string GetDeffuncName(int index)
        {
            if ((index >= this.deffuncs.Length) || (index < 0))
            {
                return null;
            }

            return this.deffuncs[index].Name;
        }

        internal string GetFuncName(int index)
        {
            if ((index >= this.funcs.Length) || (index < 0))
            {
                return null;
            }

            return this.funcs[index].Name;
        }

        internal List<string> GetLines()
        {
            return this.lines;
        }
    }
}
