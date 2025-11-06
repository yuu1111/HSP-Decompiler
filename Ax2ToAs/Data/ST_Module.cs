namespace KttK.HspDecompiler.Ax2ToAs.Data
{
    internal struct Module
    {
        private string name;

        internal string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public override string ToString()
        {
            return "#module " + this.name;
        }
    }
}
