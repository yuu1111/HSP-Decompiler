using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
    internal class McallStatement : LogicalLine
    {
        private McallStatement()
        {
        }

        internal McallStatement(McallFunctionPrimitive theToken, VariablePrimitive var, ExpressionToken exp, ArgumentToken arg)
        {
            this.token = theToken;
            this.var = var;
            this.exp = exp;
            this.arg = arg;
        }

        private readonly McallFunctionPrimitive token;
        private readonly VariablePrimitive var; // 配列変数も受け付けない。
        private readonly ExpressionToken exp;
        private readonly ArgumentToken arg;

        internal override int TokenOffset
        {
            get { return this.token.TokenOffset; }
        }

        private string ToStringFunctionStyle()
        {
            if (this.arg == null)
            {
                return this.token.ToString();
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(this.token);
            if (this.var != null)
            {
                builder.Append(' ');
                builder.Append(this.var);
                if (this.exp != null)
                {
                    builder.Append(' ');
                    builder.Append(',');
                    builder.Append(this.var);
                }
            }

            if (this.arg != null)
            {
                builder.Append(this.arg);
            }

            return builder.ToString();
        }

        internal string ToString(bool convertMcall)
        {
            if (!convertMcall)
            {
                return this.ToStringFunctionStyle();
            }

            if (this.var == null)
            {
                return this.ToStringFunctionStyle();
            }

            if (this.exp == null)
            {
                return this.ToStringFunctionStyle();
            }

            if (this.arg == null)
            {
                return this.ToStringFunctionStyle();
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(this.var);
            builder.Append("->");
            builder.Append(this.exp);

            // deHSP1.20のバグ修正。expとargの間にカンマを入れないように修正。
            if (this.arg != null)
            {
                builder.Append(this.arg.ToString(true));
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            return this.ToString(true);
        }

        internal override void CheckLabel()
        {
            if (this.exp != null)
            {
                this.exp.CheckLabel();
            }

            if (this.arg != null)
            {
                this.arg.CheckLabel();
            }
        }

        internal override bool CheckRpn()
        {
            bool ret = true;
            if (this.exp != null)
            {
                ret &= this.exp.CheckRpn();
            }

            if (this.arg != null)
            {
                ret &= this.arg.CheckRpn();
            }

            return ret;
        }
    }
}
