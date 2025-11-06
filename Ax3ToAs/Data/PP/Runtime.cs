using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
    internal class Runtime : Preprocessor
    {
        private Runtime()
        {
        }

        internal Runtime(string theName)
        {
            this.name = theName;
        }

        private string name;

        public override string ToString()
        {
            StringBuilder strbd = new StringBuilder();
            strbd.Append("#runtime ");
            strbd.Append(@"""");
            strbd.Append(this.name);
            strbd.Append(@"""");
            return strbd.ToString();
        }
    }
}
