using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
    /// <summary>
    /// RPN解析用.
    /// </summary>
    internal sealed class SubExpressionToken : OperandToken
    {
        private SubExpressionToken()
        {
        }

        internal SubExpressionToken(OperandToken leftToken, OperandToken rightToken, OperatorToken opToken)
        {
            this.p1 = leftToken;
            this.p2 = rightToken;
            this.op = opToken;
        }

        /// <summary>
        /// 左オペランド.
        /// </summary>
        private readonly OperandToken p1;

        /// <summary>
        /// 右オペランド.
        /// </summary>
        private readonly OperandToken p2;

        /// <summary>
        /// 演算子.
        /// </summary>
        private readonly OperatorToken op;

        internal override int TokenOffset
        {
            get { return this.p1.TokenOffset; }
        }

        private string ToStringForceDefault()
        {
            StringBuilder builder = new StringBuilder();
            if (this.p1.Priority < this.op.Priority)
            {
                builder.Append('(');
                builder.Append(this.p1);
                builder.Append(')');
            }
            else
            {
                builder.Append(this.p1);
            }

            builder.Append(' ');
            builder.Append(this.op.ToString(false, true));
            builder.Append(' ');
            if (this.p2.Priority <= this.op.Priority)
            {
                builder.Append('(');
                builder.Append(this.p2);
                builder.Append(')');
            }
            else
            {
                builder.Append(this.p2);
            }

            return builder.ToString();
        }

        internal string ToString(bool force_default)
        {
            if (force_default)
            {
                return this.ToStringForceDefault();
            }

            LiteralToken? lit = this.p1 as LiteralToken;
            VariableToken? var = this.p2 as VariableToken;
            if ((lit == null) || (var == null))
            {
                lit = this.p2 as LiteralToken;
                var = this.p1 as VariableToken;
            }

            if ((lit == null) || (var == null))
            {
                return this.ToStringForceDefault();
            }

            if (!lit.IsMinusOne)
            {
                return this.ToStringForceDefault();
            }

            if (this.op.ToString() != "*")
            {
                return this.ToStringForceDefault();
            }

            StringBuilder builder = new StringBuilder();
            builder.Append('-');
            builder.Append(var);
            return builder.ToString();
        }

        public override string ToString()
        {
            return this.ToString(false);
        }

        internal override int Priority
        {
            get { return this.op.Priority; }
        }

        internal override void CheckLabel()
        {
            if (this.p1 != null)
            {
                this.p1.CheckLabel();
            }

            if (this.p2 != null)
            {
                this.p2.CheckLabel();
            }

            if (this.op != null)
            {
                this.op.CheckLabel();
            }
        }
    }
}
