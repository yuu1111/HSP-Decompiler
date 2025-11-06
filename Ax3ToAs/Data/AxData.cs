using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Analyzer;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
    internal class AxData
    {
        private AxHeader header;
        private TokenCollection tokens = new TokenCollection();
        private List<Label> labels = new List<Label>();
        private List<Usedll> dlls = new List<Usedll>();
        List<Function> functions = new List<Function>();
        List<Param> functionParams = new List<Param>();
        List<PlugIn> plugIns = new List<PlugIn>();
        Runtime runtime;
        List<Function> modules = new List<Function>();
        List<string> variableName = new List<string>();

        internal List<Function> Modules
        {
            get { return this.modules; }
        }

        internal List<PlugIn> PlugIns
        {
            get { return this.plugIns; }
        }

        internal Runtime Runtime
        {
            get { return this.runtime; }
        }

        internal AxHeader Header
        {
            get { return this.header; }
        }

        internal TokenCollection Tokens
        {
            get { return this.tokens; }
        }

        internal List<Usedll> Usedlls
        {
            get { return this.dlls; }
        }

        internal List<Label> Labels
        {
            get { return this.labels; }
        }

        internal List<Function> Functions
        {
            get { return this.functions; }
        }

        internal List<Param> FunctionParams
        {
            get { return this.functionParams; }
        }

        internal Label GetLabel(int index)
        {
            return this.Labels[index];
        }

        internal Function GetUserFunction(int index)
        {
            if (index < 0)
            {
                return null;
            }

            if (index >= this.functions.Count)
            {
                return null;
            }

            return this.functions[index];

            // int count = 0;
            // for (int i = 0; i < functions.Count; i++)
            // {
            //    if (functions[i].IsUserFunction)
            //    {
            //        if (count == functionIndex)
            //            return functions[i];
            //        count++;
            //    }
            // }
            // return null;
        }

        internal Function GetDllFunction(int index)
        {
            if (index < 0)
            {
                return null;
            }

            if (index >= this.functions.Count)
            {
                return null;
            }

            return this.functions[index];

            // int count = 0;
            // for (int i = 0; i < functions.Count; i++)
            // {
            //    if (functions[i].IsDllFunction)
            //    {
            //        if (count == functionIndex)
            //            return functions[i];
            //        count++;
            //    }
            // }
            // return null;
        }

        internal Usedll GetUsedll(int index)
        {
            if (index < 0)
            {
                return null;
            }

            if (index >= this.dlls.Count)
            {
                return null;
            }

            return this.dlls[index];
        }

        internal Param GetParam(int index)
        {
            if (index < 0)
            {
                return null;
            }

            if (index >= this.functionParams.Count)
            {
                return null;
            }

            return this.functionParams[index];
        }

        internal string GetVariableName(int index)
        {
            if (index < 0)
            {
                return null;
            }

            if (index >= this.variableName.Count)
            {
                return null;
            }

            return this.variableName[index];
        }

        internal Cmd AddCmd(int pluginIndex, int methodIndex)
        {
            if (pluginIndex < 0)
            {
                return null;
            }

            if (pluginIndex >= this.plugIns.Count)
            {
                return null;
            }

            return this.plugIns[pluginIndex].AddCmd(methodIndex);
        }

        internal string ReadString(int offset, int max_count)
        {
            long seekOffset = this.seekOrigin + offset;
            long nowPosition = this.reader.BaseStream.Position;
            this.reader.BaseStream.Seek(seekOffset, SeekOrigin.Begin);
            List<char> chars = new List<char>();
            char token = '\0';
            int count = 0;
            while ((token = this.reader.ReadChar()) != '\0')
            {
                switch (token)
                {
                    case '\\':
                        chars.Add('\\');
                        chars.Add('\\');
                        break;
                    case '\"':
                        chars.Add('\\');
                        chars.Add('\"');
                        break;
                    case '\t':
                        chars.Add('\\');
                        chars.Add('t');
                        break;
                    case '\n':
                        chars.Add('\\');
                        chars.Add('n');
                        break;
                    case '\r':
                        break;
                    default:
                        chars.Add(token);
                        break;
                }

                count++;
                if (count >= max_count)
                {
                    break;
                }
            }

            char[] arrayChars = new char[chars.Count];
            chars.CopyTo(arrayChars);
            this.reader.BaseStream.Seek(nowPosition, SeekOrigin.Begin);
            return new string(arrayChars);
        }

        internal string ReadStringLiteral(int offset)
        {
            return this.ReadString((int)(this.header.LiteralStart + offset), (int)(this.header.LiteralSize - offset));
        }

        internal double ReadDoubleLiteral(int offset)
        {
            double ret = 0.0;
            long seekOffset = this.seekOrigin + this.header.LiteralStart + offset;
            long nowPosition = this.reader.BaseStream.Position;
            this.reader.BaseStream.Seek(seekOffset, SeekOrigin.Begin);
            ret = this.reader.ReadDouble();
            this.reader.BaseStream.Seek(nowPosition, SeekOrigin.Begin);
            return ret;
        }

        internal string ReadIidCodeLiteral(int offset)
        {
            StringBuilder strbd = new StringBuilder();
            byte[]? buf = null;
            long seekOffset = this.seekOrigin + this.header.LiteralStart + offset;
            long nowPosition = this.reader.BaseStream.Position;
            this.reader.BaseStream.Seek(seekOffset, SeekOrigin.Begin);
            buf = this.reader.ReadBytes(0x10);
            this.reader.BaseStream.Seek(nowPosition, SeekOrigin.Begin);
            strbd.Append(@"{");
            strbd.Append(buf[0x03].ToString("X02"));
            strbd.Append(buf[0x02].ToString("X02"));
            strbd.Append(buf[0x01].ToString("X02"));
            strbd.Append(buf[0x00].ToString("X02"));
            strbd.Append('-');
            strbd.Append(buf[0x05].ToString("X02"));
            strbd.Append(buf[0x04].ToString("X02"));
            strbd.Append('-');
            strbd.Append(buf[0x07].ToString("X02"));
            strbd.Append(buf[0x06].ToString("X02"));
            strbd.Append('-');
            strbd.Append(buf[0x08].ToString("X02"));
            strbd.Append(buf[0x09].ToString("X02"));
            strbd.Append('-');
            strbd.Append(buf[0x0A].ToString("X02"));
            strbd.Append(buf[0x0B].ToString("X02"));
            strbd.Append(buf[0x0C].ToString("X02"));
            strbd.Append(buf[0x0D].ToString("X02"));
            strbd.Append(buf[0x0E].ToString("X02"));
            strbd.Append(buf[0x0F].ToString("X02"));
            strbd.Append(@"}");
            return strbd.ToString();
        }

        internal void LoadStart(BinaryReader theReader, Hsp3Dictionary theDictionary)
        {
            if (theReader == null)
            {
                throw new ArgumentNullException("readerにnull値を指定できません");
            }

            if (theDictionary == null)
            {
                throw new ArgumentNullException("dictionaryにnull値を指定できません");
            }

            this.seekOrigin = theReader.BaseStream.Position;
            this.reader = theReader;
            this.dictionary = theDictionary;
            this.isStarted = true;
        }

        internal void LoadEnd()
        {
            this.seekOrigin = -1;
            this.reader = null;
            this.dictionary = null;
            this.isStarted = false;
        }

        private long seekOrigin;
        private BinaryReader reader;
        private Hsp3Dictionary dictionary;
        private bool isStarted;

        internal bool IsStarted
        {
            get { return this.isStarted; }
        }

        internal BinaryReader Reader
        {
            get { return this.reader; }
            set { this.reader = value; }
        }

        internal long StartOfCode
        {
            get { return this.header.CodeStart + this.seekOrigin; }
        }

        internal Hsp3Dictionary Dictionary
        {
            get { return this.dictionary; }
            set { this.dictionary = value; }
        }

        internal void ReadHeader()
        {
            if (!this.isStarted)
            {
                throw new InvalidOperationException("LoadStartが呼び出されていません");
            }

            long streamSize = this.reader.BaseStream.Length - this.seekOrigin;
            if (streamSize < 0x60)
            {
                throw new HspDecoderException("AxData", "ファイルヘッダーが見つかりません");
            }

            try
            {
                this.header = AxHeader.FromBinaryReader(this.reader);
            }
            catch (SystemException e)
            {
                throw new HspDecoderException("AxHeader", "ヘッダー解析中に想定外のエラー", e);
            }

            return;
        }

        internal void ReadPreprocessor(Hsp3Dictionary dictionary)
        {
            if (!this.isStarted)
            {
                throw new InvalidOperationException("LoadStartが呼び出されていません");
            }

            if (this.header == null)
            {
                throw new InvalidOperationException("ヘッダーが読み込まれていません");
            }

            if (this.header.RuntimeStart != 0)
            {
                string runtimeName = this.ReadString((int)this.header.RuntimeStart, (int)(this.header.CodeStart - this.header.RuntimeStart));
                if (runtimeName != null)
                {
                    this.runtime = new Runtime(runtimeName);
                }
            }

            uint count = this.header.LabelCount;
            for (int i = 0; i < count; i++)
            {
                long offset = this.seekOrigin + this.header.LabelStart + ((int)HeaderDataSize.Label * i);
                this.reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                this.labels.Add(Label.FromBinaryReader(this.reader, this, i));
            }

            count = this.header.DllCount;
            for (int i = 0; i < count; i++)
            {
                long offset = this.seekOrigin + this.header.DllStart + ((int)HeaderDataSize.Dll * i);
                this.reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                this.dlls.Add(Usedll.FromBinaryReader(this.reader, this, i));
            }

            count = this.header.ParameterCount;
            for (int i = 0; i < count; i++)
            {
                long offset = this.seekOrigin + this.header.ParameterStart + ((int)HeaderDataSize.Parameter * i);
                this.reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                this.functionParams.Add(Param.FromBinaryReader(this.reader, this, i));
            }

            count = this.header.FunctionCount;
            for (int i = 0; i < count; i++)
            {
                long offset = this.seekOrigin + this.header.FunctionStart + ((int)HeaderDataSize.Function * i);
                this.reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                this.functions.Add(Function.FromBinaryReader(this.reader, this, i));
            }

            count = this.header.PluginCount;
            for (int i = 0; i < count; i++)
            {
                long offset = this.seekOrigin + this.header.PluginStart + ((int)HeaderDataSize.Plugin * i);
                this.reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                this.plugIns.Add(PlugIn.FromBinaryReader(this.reader, this, i));
            }

            if ((count != 0) && (this.header.PluginParameterCount != 0))
            {
                this.plugIns[0].ExtendedTypeCount = (int)this.header.PluginParameterCount;
            }

            foreach (Param param in this.functionParams)
            {
                param.SetFunction(this);
            }

            this.RenameFunctions(dictionary);

            this.ReadDebugInfo();
        }

        internal void DeleteInvisibleLables()
        {
            this.labels = this.labels.FindAll(this.LabelIsVisible);
        }

        private bool LabelIsVisible(Label label)
        {
            return label.Visible;
        }

        private void RenameFunctions(Hsp3Dictionary dictionary)
        {
            // #funcなどの名前かぶり対策
            List<string> functionNames = new List<string>();
            List<Function> initializer = new List<Function>();
            List<Function> comfuncs = new List<Function>();
            List<Function> dllfuncs = new List<Function>();

            // ver1.20　標準命令を避けるように
            functionNames.AddRange(dictionary.GetAllFuncName());
            foreach (Function func in this.functions)
            {
                switch (func.Type)
                {
                    case FunctionType.Cfunc:
                    case FunctionType.Func:
                        dllfuncs.Add(func);
                        break;
                    case FunctionType.Comfunc:
                        comfuncs.Add(func);
                        break;

                    case FunctionType.Defcfunc:
                    case FunctionType.Deffunc:
                    case FunctionType.Module:
                        if (
                            func.ParentModule !=
                            null) // (func.DefaultName.Equals("__init", StringComparison.OrdinalIgnoreCase) || func.DefaultName.Equals("__term", StringComparison.OrdinalIgnoreCase))
                        {
                            initializer.Add(func);
                        }
                        else
                        {
                            func.SetName(func.DefaultName);
                            functionNames.Add(func.DefaultName.ToLower());
                        }

                        break;
                }
            }

            foreach (Function func in initializer)
            {
                string defName = func.DefaultName;
                if (!functionNames.Contains(defName.ToLower()))
                {
                    func.SetName(defName);
                    functionNames.Add(defName.ToLower());
                    continue;
                }

                string newName = defName;
                int index = 1;
                do
                {
                    newName = string.Format("{0}_{1}", defName, index);
                    index++;
                }
                while (functionNames.Contains(newName));

                func.SetName(newName);
                functionNames.Add(newName.ToLower());
            }

            foreach (Function func in dllfuncs)
            {
                string defName = func.DefaultName;
                string newName = defName;
                if (newName.StartsWith("_", StringComparison.Ordinal) && (newName.Length > 1))
                {
                    newName = newName.Substring(1);
                }

                int atIndex = newName.IndexOf("@", StringComparison.Ordinal);
                if (atIndex > 0)
                {
                    newName = newName.Substring(0, atIndex);
                }

                if (!functionNames.Contains(newName.ToLower()))
                {
                    func.SetName(newName);
                    functionNames.Add(newName.ToLower());
                    continue;
                }

                int index = 1;
                do
                {
                    newName = string.Format("func_{0}", index);
                    index++;
                }
                while (functionNames.Contains(newName));

                func.SetName(newName);
                functionNames.Add(newName.ToLower());
            }

            foreach (Function func in comfuncs)
            {
                string newName = string.Empty;
                int index = 1;
                do
                {
                    newName = string.Format("comfunc_{0}", index);
                    index++;
                }
                while (functionNames.Contains(newName)); // .ToLower()

                func.SetName(newName);
                functionNames.Add(newName.ToLower());
            }
        }

        internal void RenameLables()
        {
            if (this.labels.Count <= 0)
            {
                return;
            }

            this.labels.Sort();
            int keta = ((int)Math.Log10(this.labels.Count)) + 1;
            string formatBase = "*label_{0:D0" + keta + "}";
            for (int i = 0; i < this.labels.Count; i++)
            {
                this.labels[i].LabelName = string.Format(formatBase, i);
            }

            return;
        }

        private bool ReadDebugInfo()
        {
            int var_no = 0;
            for (uint i = 0; i < this.header.DebugSize; i++)
            {
                long offset = this.seekOrigin + this.header.DebugStart + i;
                this.reader.BaseStream.Seek(offset, SeekOrigin.Begin);

                switch (this.reader.ReadByte())
                {
                    case 252:
                        i += 2;
                        break;
                    case 253:
                        int literalOffset = this.reader.ReadByte() ^ (this.reader.ReadByte() << 8) ^ (this.reader.ReadByte() << 16);
                        this.variableName.Add(this.ReadStringLiteral(literalOffset));
                        i += 5;
                        break;
                    case 254:
                        i += 5;
                        break;
                    case 255:
                        return true;
                }
            }

            return false;
        }
    }
}
