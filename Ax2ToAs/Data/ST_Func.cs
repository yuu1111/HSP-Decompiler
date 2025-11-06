namespace KttK.HspDecompiler.Ax2ToAs.Data
{
    internal struct Func
    {
        private string name;
        private int hikiType;
        private int dllIndex;

        internal string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        internal int HikiType
        {
            get { return this.hikiType; }
            set { this.hikiType = value; }
        }

        internal int DllIndex
        {
            get { return this.dllIndex; }
            set { this.dllIndex = value; }
        }

        public override string ToString()
        {
            return "#func func_" + this.name + " " + this.name + " $" + this.hikiType.ToString("x4");
        }
    }
}
