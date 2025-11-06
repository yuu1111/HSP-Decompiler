using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
    /// <summary>
    /// 代入式.
    /// </summary>
    internal sealed class UnknownLine : LogicalLine
    {
        private UnknownLine()
        {
        }

        internal UnknownLine(List<PrimitiveToken> primitives)
        {
            this.tokens = new PrimitiveToken[primitives.Count];
            primitives.CopyTo(this.tokens);
        }

        private readonly PrimitiveToken[] tokens;

        internal override int TokenOffset
        {
            get
            {
                if ((this.tokens == null) || (this.tokens.Length == 0))
                {
                    return -1;
                }

                return this.tokens[0].TokenOffset;
            }
        }

        public override string ToString()
        {
            if ((this.tokens == null) || (this.tokens.Length == 0))
            {
                return "//空";
            }

            StringBuilder builder = new StringBuilder("//");
            foreach (PrimitiveToken token in this.tokens)
            {
                builder.Append(' ');
                builder.Append(token);
            }

            return builder.ToString();
        }
    }
}
