using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Primitive
{
    internal sealed class IfStatementPrimitive : HspFunctionPrimitive
    {
        private IfStatementPrimitive()
        {
        }

        internal IfStatementPrimitive(PrimitiveTokenDataSet dataSet, int extraValue)
            : base(dataSet)
        {
            this.extraValue = extraValue;
        }

        private readonly int extraValue = -1;

        internal int JumpToOffset
        {
            get
            {
                if (this.extraValue == -1)
                {
                    return -1;
                }

                int ret = this.extraValue + this.TokenOffset;
                if (this.HasLongTypeValue)
                {
                    ret += 4;
                }
                else
                {
                    ret += 3;
                }

                return ret;
            }
        }

        internal override string DefaultName
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
                    builder.Append(this.Value.ToString("X08"));
                }
                else
                {
                    builder.Append(this.Value.ToString("X04"));
                }

                builder.Append(' ');
                builder.Append(this.extraValue.ToString("X04"));
                builder.Append("*/");
                return builder.ToString();
            }
        }
    }
}
