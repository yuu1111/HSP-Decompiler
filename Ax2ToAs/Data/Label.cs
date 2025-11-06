namespace KttK.HspDecompiler.Ax2ToAs.Data
{
    /// <summary>
    /// Label の概要の説明です。.
    /// </summary>
    internal class Label
    {
        private Label()
        {
            // TODO: コンストラクタ ロジックをここに追加してください。
        }

        internal Label(int p_index, int p_tokenIndex)
        {
            this.index = p_index;
            this.tokenIndex = p_tokenIndex;
            this.name = "*label_" + this.index;
        }

        private int index;
        private int tokenIndex;
        private int loadCount;
        private string name;
        private bool enabled;
        private int deffunc = -1;

        internal string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        internal int TokenIndex
        {
            get { return this.tokenIndex; }
        }

        internal int Deffunc
        {
            get { return this.deffunc; }
            set { this.deffunc = value; }
        }

        internal int LoadCount
        {
            get { return this.loadCount; }
            set { this.loadCount = value; }
        }

        internal bool Enabled
        {
            get
            {
                if (this.deffunc != -1)
                {
                    return true;
                }

                return this.enabled;
            }

            set
            {
                this.enabled = value;
            }
        }
    }
}
