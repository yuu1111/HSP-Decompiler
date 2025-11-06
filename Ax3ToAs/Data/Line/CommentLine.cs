using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
    internal class CommentLine : LogicalLine
    {
        internal CommentLine()
        {
        }

        internal CommentLine(string str)
        {
            this.comment = str;
        }

        private readonly string comment;

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
            if (this.comment == null)
            {
                return string.Empty;
            }

            StringBuilder strbd = new StringBuilder();
            strbd.Append("//");
            strbd.Append(this.comment);
            return strbd.ToString();
        }
    }
}
