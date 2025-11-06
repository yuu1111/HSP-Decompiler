namespace KttK.HspDecompiler.Ax2ToAs.Data
{
    /// <summary>
    /// Header の概要の説明です。.
    /// </summary>
    internal class Header
    {
        private Header()
        {
            // TODO: コンストラクタ ロジックをここに追加してください。
        }

        private int allDataByte;
        private int scriptOffset;
        private int scriptByte;
        private int textOffset;
        private int textByte;
        private int labelOffset;
        private int labelByte;
        private int dllOffset;
        private int dllByte;
        private int funcOffset;
        private int funcByte;
        private int deffuncOffset;
        private int deffuncByte;
        private int moduleOffset;
        private int moduleByte;

        internal int AllDataByte
        {
            get { return this.allDataByte; }
        }

        internal int ScriptOffset
        {
            get { return this.scriptOffset; }
        }

        internal int ScriptByte
        {
            get { return this.scriptByte; }
        }

        internal int TextOffset
        {
            get { return this.textOffset; }
        }

        internal int TextByte
        {
            get { return this.textByte; }
        }

        internal int LabelOffset
        {
            get { return this.labelOffset; }
        }

        internal int LabelByte
        {
            get { return this.labelByte; }
        }

        internal int DllOffset
        {
            get { return this.dllOffset; }
        }

        internal int DllByte
        {
            get { return this.dllByte; }
        }

        internal int FuncOffset
        {
            get { return this.funcOffset; }
        }

        internal int FuncByte
        {
            get { return this.funcByte; }
        }

        internal int DeffuncOffset
        {
            get { return this.deffuncOffset; }
        }

        internal int DeffuncByte
        {
            get { return this.deffuncByte; }
        }

        internal int ModuleOffset
        {
            get { return this.moduleOffset; }
        }

        internal int ModuleByte
        {
            get { return this.moduleByte; }
        }

        internal int ScriptCount
        {
            get { return this.scriptByte / 2; }
        }

        internal int ScriptEndOffset
        {
            get { return this.scriptOffset + this.scriptByte; }
        }

        internal int LabelCount
        {
            get { return this.labelByte / 4; }
        }

        internal int DllCount
        {
            get { return this.dllByte / 24; }
        }

        internal int FuncCount
        {
            get { return this.funcByte / 16; }
        }

        internal int DeffuncCount
        {
            get { return this.deffuncByte / 16; }
        }

        internal int ModuleCount
        {
            get { return this.moduleByte / 24; }
        }

        internal static Header FromIntArray(int[] data)
        {
            if (data == null)
            {
                return null;
            }

            if (data.Length < 20)
            {
                return null;
            }

            Header ret = new Header();
            ret.allDataByte = data[3];
            ret.scriptOffset = data[4];
            ret.scriptByte = data[5];
            ret.textOffset = data[6];
            ret.textByte = data[7];
            ret.labelOffset = data[8];
            ret.labelByte = data[9];

            ret.dllOffset = data[12];
            ret.dllByte = data[13];
            ret.funcOffset = data[14];
            ret.funcByte = data[15];
            ret.deffuncOffset = data[16];
            ret.deffuncByte = data[17];
            ret.moduleOffset = data[18];
            ret.moduleByte = data[19];
            return ret;
        }
    }
}
