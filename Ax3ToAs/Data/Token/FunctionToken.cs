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
            primitive = token;

        }

        internal FunctionToken(FunctionPrimitive token, ArgumentToken theArg)
        {
            primitive = token;
            arg = theArg;
        }

        readonly FunctionPrimitive primitive;

        internal FunctionPrimitive Primitive
        {
            get { return primitive; }
        }

        readonly ArgumentToken arg;

        internal override int TokenOffset
        {
            get { return primitive.TokenOffset; }
        }

        public override string ToString()
        {
            if (arg == null)
                return primitive.ToString();

            StringBuilder builder = new StringBuilder(primitive.ToString());
            builder.Append(arg);
            return builder.ToString();
        }


        internal override int Priority
        {
            get { return 100; }
        }

        internal override void CheckLabel()
        {
            if (arg != null)
                arg.CheckLabel();
        }

        internal override bool CheckRpn()
        {
            if (arg != null)
                return arg.CheckRpn();

            return true;
        }
    }
}
