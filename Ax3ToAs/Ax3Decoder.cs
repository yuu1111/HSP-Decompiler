using System;
using System.Collections.Generic;
using System.IO;
using KttK.HspDecompiler.Ax3ToAs.Data;

#if AllowDecryption
#endif
using KttK.HspDecompiler.Ax3ToAs.Data.Analyzer;

namespace KttK.HspDecompiler.Ax3ToAs
{
    internal class Ax3Decoder : AbstractAxDecoder
    {
        internal Ax3Decoder()
        {
        }

        private Hsp3Dictionary dictionary;

        internal Hsp3Dictionary Dictionary
        {
            get { return this.dictionary; }
            set { this.dictionary = value; }
        }

        // internal List<string> Decode(string axPath)
#if AllowDecryption
        // internal List<string> DecodeAndDecrypt(BinaryReader reader,int fileSize)

#endif

        public override List<string> Decode(BinaryReader reader)
        {
            AxData data = new AxData();
            LexicalAnalyzer? lex = null;
            TokenCollection? stream = null;
            SyntacticAnalyzer? synt = null;
            List<LogicalLine>? lines = null;
            List<string> stringLines = new List<string>();
            try
            {
                HspConsole.Write("ヘッダー解析中...");
                data.LoadStart(reader, this.dictionary);
                data.ReadHeader();
                HspConsole.Write("プリプロセッサ解析中...");
                data.ReadPreprocessor(this.dictionary);
                HspConsole.Write("字句解析中...");
                lex = new LexicalAnalyzer(this.dictionary);
                stream = lex.Analyze(data);
                data.LoadEnd();
                HspConsole.Write("構文解析中...");
                synt = new SyntacticAnalyzer();
                lines = synt.Analyze(stream, data);
                HspConsole.Write("出力ファイル作成中...");
                foreach (LogicalLine line in lines)
                {
                    if (line.Visible)
                    {
                        string str = new string('\t', line.TabCount);
                        stringLines.Add(str + line);
                    }
                }
            }
            catch (SystemException e)
            {
                throw new HspDecoderException("AxData", "想定外のエラー", e);
            }

            return stringLines;
        }

        /*private AxData preprocessorAnalyze()
        {
        }*/
    }
}
