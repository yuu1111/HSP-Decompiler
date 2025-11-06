using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
    /// <summary>
    /// 引数.
    /// </summary>
    internal sealed class ArgumentToken : CodeToken
    {
        private ArgumentToken()
        {
        }

        internal ArgumentToken(List<ExpressionToken> theExps, bool hasBrackets, bool firstArgIsNull)
        {
            this.exps = theExps;
            this.hasBrackets = hasBrackets;
            this.firstArgIsNull = firstArgIsNull;
        }

        private readonly List<ExpressionToken> exps;
        private readonly bool hasBrackets;
        private readonly bool firstArgIsNull;

        internal List<ExpressionToken> Exps
        {
            get { return this.exps; }
        }

        internal override int TokenOffset
        {
            get
            {
                if ((this.exps == null) || (this.exps.Count == 0))
                {
                    return -1;
                }

                return this.exps[0].TokenOffset;
            }
        }

        public override string ToString()
        {
            return this.ToString(false);
        }

        public string ToString(bool mcall)
        {
            StringBuilder builder = new StringBuilder();
            if (this.hasBrackets)
            {
                builder.Append('(');
            }
            else
            {
                builder.Append(' ');
            }

            int i = 0;
            foreach (ExpressionToken exp in this.exps)
            {
                if ((i != 0) || (this.firstArgIsNull && !mcall))
                {
                    builder.Append(", ");
                }

                i++;
                builder.Append(exp);
            }

            if (this.hasBrackets)
            {
                builder.Append(')');
            }

            return builder.ToString();
        }

        internal override void CheckLabel()
        {
            if (this.exps != null)
            {
                foreach (ExpressionToken token in this.exps)
                {
                    token.CheckLabel();
                }
            }
        }

        internal override bool CheckRpn()
        {
            bool ret = true;
            if (this.exps != null)
            {
                foreach (ExpressionToken token in this.exps)
                {
                    ret &= token.CheckRpn();
                }
            }

            return ret;
        }
    }
}
