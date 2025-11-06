namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
    internal class EndOfModule : LogicalLine
    {
        internal override int TokenOffset
        {
            get { return -1; }
        }

        internal override int TabCount
        {
            get { return 0; }
        }

        public override string ToString()
        {
            return "#global";
        }
    }
}
