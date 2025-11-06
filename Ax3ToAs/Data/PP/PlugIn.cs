using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
    internal class PlugIn : Preprocessor
    {
        private PlugIn()
        {
        }

        private PlugIn(int index)
            : base(index)
        {
        }

        private int int0;
        private string dllName;
        private string exportName;
        private int int3;

        internal static PlugIn FromBinaryReader(BinaryReader reader, AxData parent, int index)
        {
            PlugIn ret = new PlugIn(index);
            ret.int0 = reader.ReadInt32();
            int dllNameOffset = reader.ReadInt32();
            int exportNameOffset = reader.ReadInt32();
            ret.int3 = reader.ReadInt32();
            ret.dllName = parent.ReadStringLiteral(dllNameOffset);
            ret.exportName = parent.ReadStringLiteral(exportNameOffset);
            return ret;
        }

        private Dictionary<int, Cmd> cmds = new Dictionary<int, Cmd>();
        private int extendedTypeCount;

        internal int ExtendedTypeCount
        {
            get { return this.extendedTypeCount; }
            set { this.extendedTypeCount = value; }
        }

        internal Cmd AddCmd(int methodIndex)
        {
            Cmd? cmd = null;
            if (this.cmds.TryGetValue(methodIndex, out cmd))
            {
                return cmd;
            }

            cmd = new Cmd(this.index, methodIndex);
            this.cmds.Add(methodIndex, cmd);
            return cmd;
        }

        internal Dictionary<int, Cmd> GetCmds()
        {
            return this.cmds;
        }

        public override string ToString()
        {
            StringBuilder strbd = new StringBuilder();
            strbd.Append("#regcmd");
            strbd.Append(' ');
            strbd.Append('"');
            strbd.Append(this.exportName);
            strbd.Append('"');
            strbd.Append(',');
            strbd.Append(' ');
            strbd.Append('"');
            strbd.Append(this.dllName);
            strbd.Append('"');
            if (this.extendedTypeCount != 0)
            {
                strbd.Append(',');
                strbd.Append(' ');
                strbd.Append(this.extendedTypeCount.ToString());
            }

            return strbd.ToString();
        }
    }
}
