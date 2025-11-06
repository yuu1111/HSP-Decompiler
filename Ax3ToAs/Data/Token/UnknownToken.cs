namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
    internal sealed class UnknownToken : CodeToken
    {
        private UnknownToken()
        {
        }

        internal UnknownToken(PrimitiveToken token)
        {
            this.token = token;
        }

        PrimitiveToken token;

        internal override int TokenOffset
        {
            get { return token.TokenOffset; }
        }

        public override string ToString()
        {
            return " /*" + token + "*/";
        }
    }
}
