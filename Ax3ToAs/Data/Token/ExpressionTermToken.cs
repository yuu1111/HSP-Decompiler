namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
    internal abstract class ExpressionTermToken : CodeToken
    {
        internal abstract bool IsOperand { get; }

        internal abstract bool IsOperator { get; }

        internal virtual bool IsLabel
        {
            get { return false; }
        }

        internal abstract int Priority { get; }
    }
}
