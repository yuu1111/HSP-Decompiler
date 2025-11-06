using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
    /// <summary>
    /// on ～ goto/gosub label構文.
    /// </summary>
    internal sealed class OnStatement : LogicalLine
    {
        private OnStatement()
        {
        }

        internal OnStatement(OnFunctionPrimitive theToken, ExpressionToken exp, FunctionToken func)
        {
            this.token = theToken;
            this.exp = exp;
            this.func = func;
        }

        private readonly OnFunctionPrimitive token; // on
        private readonly ExpressionToken exp; // 条件
        private readonly FunctionToken func; // goto/gosub ～

        internal override int TokenOffset
        {
            get { return this.token.TokenOffset; }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (this.token != null)
            {
                builder.Append(this.token);
            }

            if (this.exp != null)
            {
                builder.Append(' ');
                builder.Append(this.exp);
            }

            if (this.func != null)
            {
                builder.Append(' ');
                builder.Append(this.func);
            }

            return builder.ToString();
        }

        internal override void CheckLabel()
        {
            if (this.exp != null)
            {
                this.exp.CheckLabel();
            }

            if (this.func != null)
            {
                this.func.CheckLabel();
            }
        }

        internal override bool CheckRpn()
        {
            bool ret = true;
            if (this.exp != null)
            {
                ret &= this.exp.CheckRpn();
            }

            if (this.func != null)
            {
                ret &= this.func.CheckRpn();
            }

            return ret;
        }
    }
}
