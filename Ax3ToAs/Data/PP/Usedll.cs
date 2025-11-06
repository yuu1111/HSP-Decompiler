using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
    internal enum UsedllType
    {
        None = 0x00,
        Uselib = 0x01,
        Usecom = 0x02,
    }

    internal sealed class Usedll : Preprocessor
    {
        private Usedll()
        {
        }

        private Usedll(int index)
            : base(index)
        {
        }

        private string name;
        private string clsName;
        private int type;
        private int int2;

        internal static Usedll FromBinaryReader(BinaryReader reader, AxData parent, int index)
        {
            Usedll ret = new Usedll(index);
            ret.type = reader.ReadInt32();
            int nameOffset = reader.ReadInt32();
            ret.int2 = reader.ReadInt32();
            int clsNameOffset = reader.ReadInt32();
            switch (ret.Type)
            {
                case UsedllType.Usecom:
                    ret.name = parent.ReadIidCodeLiteral(nameOffset);
                    ret.clsName = parent.ReadStringLiteral(clsNameOffset);
                    break;
                case UsedllType.Uselib:
                    ret.name = parent.ReadStringLiteral(nameOffset);
                    break;
            }

            return ret;
        }

        private List<Function> functions = new List<Function>();

        internal UsedllType Type
        {
            get
            {
                switch (this.type)
                {
                    case 1:
                        return UsedllType.Uselib;
                    case 4:
                        return UsedllType.Usecom;
                }

                return UsedllType.None;
            }
        }

        public override string ToString()
        {
            if (this.name == null)
            {
                return @"//#uselib? //dll名不明";
            }

            StringBuilder strBld = new StringBuilder();
            switch (this.Type)
            {
                case UsedllType.Uselib:
                    strBld.Append(@"#uselib """);
                    strBld.Append(this.name);
                    strBld.Append(@"""");
                    break;
                case UsedllType.Usecom:
                    strBld.Append(@"#usecom");
                    if (this.functions.Count != 0)
                    {
                        strBld.Append(' ');
                        strBld.Append(this.functions[0].FunctionName);
                    }
                    else
                    {
                        strBld.Append(' ');
                        strBld.Append("/*関数なし*/");
                    }

                    strBld.Append(' ');
                    strBld.Append('"');
                    strBld.Append(this.name);
                    strBld.Append('"');
                    strBld.Append(' ');
                    strBld.Append('"');
                    strBld.Append(this.clsName);
                    strBld.Append('"');
                    break;
                default:
                    return @"//#uselib? //未対応の形式";
            }

            return strBld.ToString();
        }

        internal void AddFunction(Function ret)
        {
            this.functions.Add(ret);
        }

        internal List<Function> GetFunctions()
        {
            if ((this.Type == UsedllType.Usecom) && (this.functions.Count != 0))
            {
                List<Function> ret = new List<Function>(this.functions);
                ret.RemoveAt(0);
                return ret;
            }

            return this.functions;
        }
    }
}
