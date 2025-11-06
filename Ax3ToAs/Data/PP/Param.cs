using System;
using System.IO;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
    internal class Param : Preprocessor
    {
        private Param()
        {
        }

        private Param(int paramIndex)
            : base(paramIndex)
        {
        }

        private string paramTypeName = "NULL";

        /// <summary>
        /// structのindex。.
        /// </summary>
        private short deffuncIndex;

        /// <summary>
        /// パラメーターの開始サイズまたはmodinit関数のindex.
        /// </summary>
        private int paramStartByte;

        internal static Param FromBinaryReader(BinaryReader reader, AxData parent, int index)
        {
            Param ret = new Param(index);
            ret.paramType = reader.ReadUInt16();
            if (!parent.Dictionary.ParamLookUp(ret.paramType, out ret.paramTypeName))
            {
                ret.paramTypeName = "NULL";
            }

            ret.deffuncIndex = reader.ReadInt16();
            ret.paramStartByte = reader.ReadInt32();

            return ret;
        }

        private bool paramNameIsUsed;
        private ushort paramType;
        private Function module;

        internal Function Module
        {
            get { return this.module; }
        }

        private bool isStructParameter;

        internal void SetFunction(AxData parent)
        {
            if (this.deffuncIndex < 0)
            {
                return;
            }

            this.module = parent.GetUserFunction(this.deffuncIndex);
            if (this.module == null)
            {
                return;
            }

            if (this.module.IsModuleFunction)
            {
                if (this.IsModuleType)
                {
                    this.nameFormatter = this.module.FunctionName;
                }
                else
                {
                    this.isStructParameter = true;
                }
            }
        }

        internal bool ParamNameIsUsed
        {
            get { return this.paramNameIsUsed; }
            set { this.paramNameIsUsed = value; }
        }

        private string nameFormatter = "prm_{0}";

        internal string ParamName
        {
            // if (module != null)
            // return module.FunctionName;
            get
            {
                if (this.isStructParameter)
                {
                    StringBuilder strbd = new StringBuilder();
                    strbd.Append(this.module.FunctionName);
                    strbd.Append('_');
                    strbd.Append(string.Format(this.nameFormatter, this.index));
                    return strbd.ToString();
                }

                return string.Format(this.nameFormatter, this.index);
            }
        }

        internal string ToString(bool force_Named, bool remove_type, bool localToVar)
        {
            StringBuilder strbd = new StringBuilder();
            if (!remove_type)
            {
                if (this.paramTypeName == "NULL")
                {
                    strbd.Append("/*不明な型 ");
                    strbd.Append(this.paramType.ToString("X04"));
                    strbd.Append("*/");
                }
                else if (localToVar && this.paramTypeName.Equals("local", StringComparison.Ordinal))
                {
                    strbd.Append("var");
                }
                else
                {
                    strbd.Append(this.paramTypeName);
                }
            }

            if (force_Named || this.paramNameIsUsed || this.IsModuleType)
            {
                if (strbd.Length > 0)
                {
                    strbd.Append(' ');
                }

                strbd.Append(string.Format(this.nameFormatter, this.index));
            }

            return strbd.ToString();
        }

        public override string ToString()
        {
            return this.ToString(false, false, false);
        }

        internal bool IsModuleType
        {
            get
            {
                switch (this.paramTypeName)
                {
                    case "modvar":
                    case "modinit":
                    case "modterm":
                    case "struct":
                        return true;
                }

                return false;
            }
        }
    }
}
