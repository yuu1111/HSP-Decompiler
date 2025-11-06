namespace KttK.HspDecompiler.Ax2ToAs.Data
{
    internal struct Deffunc
    {
        private string name;
        private int hikiType;
        private int hikiCount;

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

        internal int HikiCount
        {
            get { return this.hikiCount; }
            set { this.hikiCount = value; }
        }

        public override string ToString()
        {
            string hiki = string.Empty;
            if (this.hikiCount >= 1)
            {
                if ((this.hikiType & 1) != 0)
                {
                    hiki = "val";
                }
                else if ((this.hikiType & 2) != 0)
                {
                    hiki = "str";
                }
                else
                {
                    hiki = "int";
                }
            }

            if (this.hikiCount >= 2)
            {
                if ((this.hikiType & 0x10) != 0)
                {
                    hiki += ", val";
                }
                else if ((this.hikiType & 0x20) != 0)
                {
                    hiki += ", str";
                }
                else
                {
                    hiki += ", int";
                }
            }

            for (int i = 0; i < (this.hikiCount - 2); i++)
            {
                hiki += ", int";
            }

            return "#deffunc " + this.name + " " + hiki;
        }
    }
}
