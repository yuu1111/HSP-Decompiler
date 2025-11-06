using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
    internal sealed class FunctionToken : OperandToken
    {
        private FunctionToken()
        {
        }

        internal FunctionToken(FunctionPrimitive token)
        {
            this.primitive = token;
        }

        internal FunctionToken(FunctionPrimitive token, ArgumentToken theArg)
        {
            this.primitive = token;
            this.arg = theArg;
        }

        private readonly FunctionPrimitive primitive;

        internal FunctionPrimitive Primitive
        {
            get { return this.primitive; }
        }

        private readonly ArgumentToken arg;

        internal override int TokenOffset
        {
            get { return this.primitive.TokenOffset; }
        }

        public override string ToString()
        {
            if (this.arg == null)
            {
                return this.primitive.ToString();
            }

            StringBuilder builder = new StringBuilder(this.primitive.ToString());
            builder.Append(this.arg);
            return builder.ToString();
        }

        internal override int Priority
        {
            get { return 100; }
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
