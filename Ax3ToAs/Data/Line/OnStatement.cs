using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
    /// <summary>
    /// on ～ goto/gosub label構文
    /// </summary>
    internal sealed class OnStatement : LogicalLine
    {
        private OnStatement()
        {
        }

        internal OnStatement(OnFunctionPrimitive theToken, ExpressionToken exp, FunctionToken func)
        {
            token = theToken;
            this.exp = exp;
            this.func = func;
        }

        private readonly OnFunctionPrimitive token; //on
        private readonly ExpressionToken exp; //条件
        private readonly FunctionToken func; //goto/gosub ～

        internal override int TokenOffset
        {
            get { return token.TokenOffset; }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (token != null)
            {
                builder.Append(token);
            }

            if (exp != null)
            {
                builder.Append(' ');
                builder.Append(exp);
            }

            if (func != null)
            {
                builder.Append(' ');
                builder.Append(func);
            }

            return builder.ToString();
        }

        internal override void CheckLabel()
        {
            if (exp != null)
                exp.CheckLabel();
            if (func != null)
                func.CheckLabel();

        }

        internal override bool CheckRpn()
        {
            bool ret = true;
            if (exp != null)
                ret &= exp.CheckRpn();
            if (func != null)
                ret &= func.CheckRpn();
            return ret;
        }
    }
}
