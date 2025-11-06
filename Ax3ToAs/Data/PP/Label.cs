using System;
using System.IO;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
    internal class Label : Preprocessor, IComparable<Label>
    {
        private Label()
        {
        }

        private Label(int index)
            : base(index)
        {
        }

        private int tokenOffset = -1;

        internal int TokenOffset
        {
            get { return this.tokenOffset; }
        }

        internal static Label FromBinaryReader(BinaryReader reader, AxData parent, int index)
        {
            Label ret = new Label(index);
            ret.tokenOffset = reader.ReadInt32();
            return ret;
        }

        private bool visible;

        internal bool Visible
        {
            get
            {
                if (this.function != null)
                {
                    return true;
                }

                return this.visible;
            }

            set
            {
                this.visible = value;
            }
        }

        private string labelName = "*label";

        internal string LabelName
        {
            get { return this.labelName; }
            set { this.labelName = value; }
        }

        public override string ToString()
        {
            if (this.function != null)
            {
                return this.function.ToString();
            }

            return this.labelName;
        }

        public int CompareTo(Label other)
        {
            int ret = this.tokenOffset.CompareTo(other.tokenOffset);
            if (ret != 0)
            {
                return ret;
            }

            return this.index.CompareTo(other.index);
        }

        private Function function;

        internal void SetFunction(Function f)
        {
            this.function = f;
            this.visible = true;
        }
    }
}
