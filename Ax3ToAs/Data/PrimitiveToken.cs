using System;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Dictionary;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
    internal class PrimitiveTokenDataSet
    {
        internal AxData Parent;
        internal int TokenOffset;
        internal int Type;
        internal int Flag;
        internal int Value;
        internal string Name;
        internal HspDictionaryValue DicValue;
    }

    internal abstract class PrimitiveToken
    {
        protected PrimitiveToken()
        {
        }

        internal PrimitiveToken(PrimitiveTokenDataSet dataSet)
        {
            this.parent = dataSet.Parent;
            this.codeType = dataSet.DicValue.Type;
            this.codeExtraFlags = dataSet.DicValue.Extra;
            this.dicValueName = dataSet.DicValue.Name;
            this.oparatorPriority = dataSet.DicValue.OparatorPriority;
            this.tokenOffset = dataSet.TokenOffset;
            this.type = dataSet.Type;
            this.flag = dataSet.Flag;
            this.value = dataSet.Value;
            this.name = dataSet.Name;
        }

        protected string dicValueName = "null";
        protected readonly int type;
        protected readonly HspCodeType codeType;

        protected readonly int flag;
        protected readonly int value;

        internal int Value
        {
            get { return this.value; }
        }

        private readonly HspCodeExtraFlags codeExtraFlags;
        private readonly AxData parent;
        private int oparatorPriority;
        private string name;
        private int tokenOffset;

        internal bool HasGhostLabel
        {
            get
            {
                if (!this.IsLineHead)
                {
                    return false;
                }

                if ((this.codeExtraFlags & HspCodeExtraFlags.HasGhostLabel) == HspCodeExtraFlags.HasGhostLabel)
                {
                    return true;
                }

                return false;
            }
        }

        internal HspCodeType CodeType
        {
            get { return this.codeType; }
        }

        internal HspCodeExtraFlags CodeExtraFlags
        {
            get { return this.codeExtraFlags; }
        }

        internal int OperatorPriority
        {
            get
            {
                if (this.codeType != HspCodeType.Operator)
                {
                    throw new InvalidOperationException("演算子でないプリミティブに優先度が要求されました");
                }

                return this.oparatorPriority;
            }
        }

        internal bool HasLongTypeValue
        {
            get { return (this.flag & 0x80) == 0x80; }
        }

        internal bool IsParamHead
        {
            get { return (this.flag & 0x40) == 0x40; }
        }

        internal bool IsLineHead
        {
            get { return (this.flag & 0x20) == 0x20; }
        }

        internal string Name
        {
            get { return this.name; }
        }

        internal int TokenOffset
        {
            get { return this.tokenOffset; }
        }

        internal void SetName()
        {
            switch (this.codeType)
            {
                case HspCodeType.Label:

                    this.name = this.dicValueName + this.value;
                    return;

                case HspCodeType.Integer:
                case HspCodeType.Param:
                case HspCodeType.Variable:
                    this.name = this.dicValueName + this.value;
                    return;
                case HspCodeType.UserFunction:
                case HspCodeType.DllFunction:
                // defaultName = "not supported";
                // break;
                case HspCodeType.NONE:
                default:
                    break;
            }
        }

        public override string ToString()
        {
            return this.name;
        }

        internal virtual string DefaultName
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("/*");
                builder.Append(this.type.ToString("X02"));
                builder.Append(' ');
                builder.Append(this.flag.ToString("X02"));
                builder.Append(' ');
                if (this.HasLongTypeValue)
                {
                    builder.Append(this.value.ToString("X08"));
                }
                else
                {
                    builder.Append(this.value.ToString("X04"));
                }

                builder.Append("*/");
                return builder.ToString();
            }
        }
    }
}
