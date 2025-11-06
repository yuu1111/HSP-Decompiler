namespace KttK.HspDecompiler.Ax2ToAs.Data
{
    internal struct Dll
    {
        private string name;

        internal string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public override string ToString()
        {
            return "#uselib " + "\"" + this.name + "\"";
        }
    }
}
