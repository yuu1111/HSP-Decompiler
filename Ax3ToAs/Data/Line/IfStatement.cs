using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
    /// <summary>
    /// if, else構文.
    /// </summary>
    internal sealed class IfStatement : LogicalLine
    {
        private IfStatement()
        {
        }

        internal IfStatement(IfStatementPrimitive token)
        {
            this.ifToken = token;
        }

        internal IfStatement(IfStatementPrimitive token, ArgumentToken theArg)
        {
            this.ifToken = token;
            this.arg = theArg;
        }

        private readonly IfStatementPrimitive ifToken;
        private readonly ArgumentToken arg;

        internal override int TokenOffset
        {
            get
            {
                if (this.ifToken == null)
                {
                    return -1;
                }

                return this.ifToken.TokenOffset;
            }
        }

        internal int JumpToOffset
        {
            get { return this.ifToken.JumpToOffset; }
        }

        internal bool IsIfStatement
        {
            get
            {
                if ((this.ifToken.CodeType & HspCodeType.IfStatement) == HspCodeType.IfStatement)
                {
                    return true;
                }

                return false;
            }
        }

        internal bool IsElseStatement
        {
            get
            {
                if ((this.ifToken.CodeType & HspCodeType.ElseStatement) == HspCodeType.ElseStatement)
                {
                    return true;
                }

                return false;
            }
        }

        private bool scoopEndIsDefined;

        internal bool ScoopEndIsDefined
        {
            get { return this.scoopEndIsDefined; }
            set { this.scoopEndIsDefined = value; }
        }

        internal override bool TabIncrement
        {
            get { return this.scoopEndIsDefined; }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(this.ifToken.ToString());
            if (this.arg != null)
            {
                builder.Append(" (");
                builder.Append(this.arg);
                builder.Append(" )");
            }

            builder.Append(" {");
            return builder.ToString();
        }

        internal override void CheckLabel()
        {
            if (this.arg != null)
            {
                this.arg.CheckLabel();
            }
        }

        internal override bool CheckRpn()
        {
            if (this.arg != null)
            {
                return this.arg.CheckRpn();
            }

            return true;
        }
    }
}
