using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
    internal enum FunctionType
    {
        NULL = 0x00,
        Func = 0x01,
        Cfunc = 0x02,
        Deffunc = 0x03,
        Defcfunc = 0x04,
        Comfunc = 0x05,
        Module = 0x06,
    }

    internal enum FunctionFlags
    {
        NULL = 0,
        Onexit = 0x01,
    }

    internal class Function : Preprocessor
    {
        private Function()
        {
        }

        private Function(int index)
            : base(index)
        {
        }

        private int dllIndex;
        private int functionIndex;

        private List<Param> functionParams = new List<Param>();
        private int strIndex;
        private int paramSizeSum;
        private int labelIndex;
        private short int0;
        private int flags;

        internal static Function FromBinaryReader(BinaryReader reader, AxData parent, int index)
        {
            Function ret = new Function(index);
            ret.dllIndex = reader.ReadInt16();
            ret.functionIndex = reader.ReadInt16();

            int paramStart = reader.ReadInt32();
            int paramCount = reader.ReadInt32();
            if (paramCount != 0)
            {
                ret.functionParams = parent.FunctionParams.GetRange(paramStart, paramCount);
            }

            ret.strIndex = reader.ReadInt32();
            if (ret.strIndex >= 0)
            {
                ret.defaultName = parent.ReadStringLiteral(ret.strIndex);
            }

            ret.paramSizeSum = reader.ReadInt32();
            ret.labelIndex = reader.ReadInt32();

            ret.int0 = reader.ReadInt16();
            ret.flags = reader.ReadInt16();
            switch (ret.Type)
            {
                case FunctionType.Defcfunc:
                case FunctionType.Deffunc:
                    Label label = parent.GetLabel(ret.labelIndex);
                    if (label != null)
                    {
                        label.SetFunction(ret);
                    }

                    ret.label = label;
                    break;

                case FunctionType.Func:
                case FunctionType.Cfunc:
                case FunctionType.Comfunc:
                    Usedll dll = parent.GetUsedll(ret.dllIndex);
                    if (dll != null)
                    {
                        dll.AddFunction(ret);
                    }

                    ret.dll = dll;
                    break;
                case FunctionType.Module:
                    parent.Modules.Add(ret);
                    break;
            }

            return ret;
        }

        internal bool IsModuleFunction
        {
            get { return this.Type == FunctionType.Module; }
        }

        internal bool IsComFunction
        {
            get { return this.Type == FunctionType.Comfunc; }
        }

        internal bool IsUserFunction
        {
            get
            {
                switch (this.Type)
                {
                    case FunctionType.Deffunc:
                    case FunctionType.Defcfunc:
                        return true;
                }

                return false;
            }
        }

        internal bool IsDllFunction
        {
            get
            {
                switch (this.Type)
                {
                    case FunctionType.Func:
                    case FunctionType.Cfunc:
                        return true;
                }

                return false;
            }
        }

        private string defaultName;

        internal string DefaultName
        {
            get { return this.defaultName; }
        }

        internal Function ParentModule
        {
            get
            {
                if (this.functionParams.Count == 0)
                {
                    return null;
                }

                if (!this.functionParams[0].IsModuleType)
                {
                    return null;
                }

                return this.functionParams[0].Module;
            }
        }

        private string name;
        private Label label;
        private Usedll dll;

        internal FunctionType Type
        {
            get
            {
                if (this.dllIndex == -1)
                {
                    return FunctionType.Deffunc;
                }

                if (this.dllIndex == -2)
                {
                    return FunctionType.Defcfunc;
                }

                if (this.dllIndex == -3)
                {
                    return FunctionType.Module;
                }

                if (this.dllIndex >= 0)
                {
                    // if (strIndex == -1)
                    if (this.functionIndex == -7)
                    {
                        return FunctionType.Comfunc;
                    }

                    if (this.labelIndex == 2 || this.labelIndex == 6)
                    {
                        return FunctionType.Func;
                    }

                    if (this.labelIndex == 3) // func onexit
                    {
                        return FunctionType.Func;
                    }

                    if (this.labelIndex == 4)
                    {
                        return FunctionType.Cfunc;
                    }
                }

                return FunctionType.NULL;
            }
        }

        internal FunctionFlags Flags
        {
            get
            {
                if ((this.flags == 1) && (this.dllIndex == -1))
                {
                    return FunctionFlags.Onexit;
                }

                if ((this.dllIndex >= 0) && (this.labelIndex == 3))
                {
                    return FunctionFlags.Onexit;
                }

                return FunctionFlags.NULL;
            }
        }

        internal void SetName(string name)
        {
            this.name = name;
        }

        internal string FunctionName
        {
            get
            {
                if (this.name != null)
                {
                    return this.name;
                }

                if (this.defaultName == null)
                {
                    if (this.Type == FunctionType.Comfunc)
                    {
                        return "comfunc_" + this.index;
                    }

                    return null;
                }

                switch (this.Type)
                {
                    case FunctionType.Defcfunc:
                    case FunctionType.Deffunc:
                    case FunctionType.Module:
                        return this.defaultName;
                    case FunctionType.Func:
                    case FunctionType.Cfunc:
                        if (this.name != null)
                        {
                            return this.name;
                        }

                        return this.defaultName;
                    case FunctionType.Comfunc:
                        return "comfunc_" + this.index;
                    default:
                        break;
                }

                return null;
            }
        }

        private string ModFunctionToString()
        {
            StringBuilder strBld = new StringBuilder();
            switch (this.defaultName)
            {
                case "__init":
                    strBld.Append("#modinit");
                    break;
                case "__term":
                    strBld.Append("#modterm");
                    break;
                default:
                    strBld.Append("#modfunc");
                    strBld.Append(' ');
                    strBld.Append(this.FunctionName);
                    break;
            }

            if (this.functionParams.Count > 1)
            {
                for (int i = 1; i < this.functionParams.Count; i++)
                {
                    if (i != 1)
                    {
                        strBld.Append(',');
                    }

                    strBld.Append(' ');
                    strBld.Append(this.functionParams[i]);
                }
            }

            return strBld.ToString();
        }

        private string ModuleToString(bool useModuleStyle)
        {
            StringBuilder strBld = new StringBuilder();
            if (useModuleStyle)
            {
                strBld.Append("#module ");
                strBld.Append(this.FunctionName);
            }
            else
            {
                strBld.Append("#struct ");
                strBld.Append(this.FunctionName);
            }

            if (this.functionParams.Count > 1)
            {
                for (int i = 1; i < this.functionParams.Count; i++)
                {
                    if (i != 1)
                    {
                        strBld.Append(',');
                    }

                    strBld.Append(' ');
                    if (useModuleStyle)
                    {
                        strBld.Append(this.functionParams[i].ToString(true, true, true));
                    }
                    else
                    {
                        strBld.Append(this.functionParams[i].ToString(true, false, true));
                    }
                }
            }

            return strBld.ToString();
        }

        internal string ToString(bool useModuleStyle)
        {
            StringBuilder strBld = new StringBuilder();

            int paramStart = 0;
            switch (this.Type)
            {
                case FunctionType.Defcfunc:
                    strBld.Append("#defcfunc ");
                    strBld.Append(this.FunctionName);
                    break;
                case FunctionType.Module:
                    return this.ModuleToString(useModuleStyle);
                case FunctionType.Deffunc:
                    if (useModuleStyle)
                    {
                        if ((this.functionParams.Count != 0) && this.functionParams[0].IsModuleType)
                        {
                            return this.ModFunctionToString();
                        }
                    }

                    strBld.Append("#deffunc ");
                    strBld.Append(this.FunctionName);
                    if ((this.Flags & FunctionFlags.Onexit) == FunctionFlags.Onexit)
                    {
                        strBld.Append(" onexit");
                    }

                    break;
                case FunctionType.Func:
                    strBld.Append("#func ");
                    strBld.Append(this.FunctionName);
                    strBld.Append(' ');
                    if ((this.Flags & FunctionFlags.Onexit) == FunctionFlags.Onexit)
                    {
                        strBld.Append("onexit ");
                    }

                    strBld.Append('"');
                    strBld.Append(this.defaultName);
                    strBld.Append('"');
                    break;
                case FunctionType.Cfunc:
                    strBld.Append("#cfunc ");
                    strBld.Append(this.FunctionName);
                    strBld.Append(@" """);
                    strBld.Append(this.defaultName);
                    strBld.Append('"');
                    break;
                case FunctionType.Comfunc:
                    strBld.Append("#comfunc ");
                    strBld.Append(this.FunctionName);
                    strBld.Append(' ');
                    strBld.Append(this.labelIndex.ToString());
                    paramStart = 1;
                    break;
                default:
                    return "/*#deffunc?*/";
            }

            if (this.functionParams.Count > paramStart)
            {
                for (int i = paramStart; i < this.functionParams.Count; i++)
                {
                    if (i != paramStart)
                    {
                        strBld.Append(',');
                    }

                    strBld.Append(' ');
                    strBld.Append(this.functionParams[i]);
                }
            }

            return strBld.ToString();
        }

        public override string ToString()
        {
            return this.ToString(false);
        }
    }
}
