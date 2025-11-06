using KttK.HspDecompiler.Ax3ToAs.Data.Token;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
    internal sealed class Command : LogicalLine
    {
        private Command()
        {
        }

        internal Command(FunctionToken function)
        {
            this.function = function;
        }

        private readonly FunctionToken function;

        internal override bool TabIncrement
        {
            get
            {
                if ((this.function.Primitive.CodeExtraFlags & HspCodeExtraFlags.AddTab) == HspCodeExtraFlags.AddTab)
                {
                    return true;
                }

                return false;
            }
        }

        internal override bool TabDecrement
        {
            get
            {
                if ((this.function.Primitive.CodeExtraFlags & HspCodeExtraFlags.RemoveTab) == HspCodeExtraFlags.RemoveTab)
                {
                    return true;
                }

                return false;
            }
        }

        internal override bool HasFlagGhostGoto
        {
            get
            {
                if ((this.function.Primitive.CodeExtraFlags & HspCodeExtraFlags.HasGhostGoto) == HspCodeExtraFlags.HasGhostGoto)
                {
                    return true;
                }

                return false;
            }
        }

        internal override bool HasFlagIsGhost
        {
            get
            {
                if ((this.function.Primitive.CodeExtraFlags & HspCodeExtraFlags.IsGhost) == HspCodeExtraFlags.IsGhost)
                {
                    return true;
                }

                return false;
            }
        }

        internal override int TokenOffset
        {
            get { return this.function.TokenOffset; }
        }

        public override string ToString()
        {
            return this.function.ToString();
        }

        internal override void CheckLabel()
        {
            if (this.function != null)
            {
                this.function.CheckLabel();
            }
        }

        internal override bool CheckRpn()
        {
            if (this.function != null)
            {
                return this.function.CheckRpn();
            }

            return true;
        }
    }
}
