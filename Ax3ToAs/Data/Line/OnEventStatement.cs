using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
    /// <summary>
    /// if, else構文.
    /// </summary>
    internal sealed class OnEventStatement : LogicalLine
    {
        private OnEventStatement()
        {
        }

        internal OnEventStatement(OnEventFunctionPrimitive theToken, FunctionToken func)
        {
            this.token = theToken;
            this.func = func;
        }

        private readonly OnEventFunctionPrimitive token; // on####
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

            if (this.func != null)
            {
                builder.Append(' ');
                builder.Append(this.func);
            }

            return builder.ToString();
        }

        internal override void CheckLabel()
        {
            if (this.func != null)
            {
                this.func.CheckLabel();
            }
        }

        internal override bool CheckRpn()
        {
            if (this.func != null)
            {
                return this.func.CheckRpn();
            }

            return true;
        }
    }
}
