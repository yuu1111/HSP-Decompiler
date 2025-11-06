using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
    internal sealed class LiteralToken : OperandToken
    {
        private LiteralToken()
        {
        }

        internal LiteralToken(LiteralPrimitive token)
        {
            this.token = token;
        }

        private readonly LiteralPrimitive token;

        internal bool IsNegativeNumber
        {
            get
            {
                if (this.token == null)
                {
                    return false;
                }

                return this.token.IsNegativeNumber;
            }
        }

        internal bool IsMinusOne
        {
            get { return this.token.IsMinusOne; }
        }

        internal override int TokenOffset
        {
            get
            {
                if (this.token == null)
                {
                    return -1;
                }

                return this.token.TokenOffset;
            }
        }

        public override string ToString()
        {
            if ((this.token.CodeType == HspCodeType.Symbol) && (this.token.ToString() == "?"))
            {
                return string.Empty;
            }

            return this.token.ToString();
        }

        internal override int Priority
        {
            get
            {
                if (this.IsNegativeNumber)
                {
                    return -1;
                }

                return 100;
            }
        }

        internal override void CheckLabel()
        {
            LabelPrimitive? label = this.token as LabelPrimitive;
            if (label != null)
            {
                label.LabelIsUsed();
            }
        }
    }
}
