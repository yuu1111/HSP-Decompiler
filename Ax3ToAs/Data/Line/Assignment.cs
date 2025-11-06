using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
    /// <summary>
    /// 代入式.
    /// </summary>
    internal sealed class Assignment : LogicalLine
    {
        private Assignment()
        {
        }

        internal Assignment(VariableToken theVar, OperatorToken theOp)
        {
            this.var = theVar;
            this.op = theOp;
        }

        internal Assignment(VariableToken theVar, OperatorToken theOp, ArgumentToken theArg)
        {
            this.var = theVar;
            this.op = theOp;
            this.arg = theArg;
        }

        private readonly VariableToken var;
        private readonly OperatorToken op;

        // 普通はひとつの式だが、配列変数にはたくさん代入することもある。
        private readonly ArgumentToken arg;

        internal override int TokenOffset
        {
            get
            {
                if (this.var == null)
                {
                    return -1;
                }

                return this.var.TokenOffset;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(this.var.ToString());
            if (this.arg != null)
            {
                builder.Append(' ');
                builder.Append(this.op.ToString(true, this.arg != null));
                builder.Append(this.arg);
            }
            else
            {
                builder.Append(this.op.ToString(true, this.arg != null)); // a++とか。HSPではこの辺の書式はいい加減みたい。
            }

            return builder.ToString();
        }

        internal override void CheckLabel()
        {
            if (this.var != null)
            {
                this.var.CheckLabel();
            }

            if (this.op != null)
            {
                this.op.CheckLabel();
            }

            if (this.arg != null)
            {
                this.arg.CheckLabel();
            }
        }

        internal override bool CheckRpn()
        {
            bool ret = true;
            if (this.var != null)
            {
                ret &= this.var.CheckRpn();
            }

            if (this.arg != null)
            {
                ret &= this.arg.CheckRpn();
            }

            return true;
        }
    }
}
