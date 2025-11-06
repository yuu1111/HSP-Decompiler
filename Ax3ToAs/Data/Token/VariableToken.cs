using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
    internal sealed class VariableToken : OperandToken
    {
        private VariableToken()
        {
        }

        internal VariableToken(VariablePrimitive var)
        {
            this.primitive = var;
            this.arg = null;
        }

        internal VariableToken(VariablePrimitive var, ArgumentToken theArg)
        {
            this.primitive = var;
            this.arg = theArg;
        }

        private readonly VariablePrimitive primitive;
        private readonly ArgumentToken arg;

        internal override int TokenOffset
        {
            get
            {
                if (this.primitive == null)
                {
                    return -1;
                }

                return this.primitive.TokenOffset;
            }
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
