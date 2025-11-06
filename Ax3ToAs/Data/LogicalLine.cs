using System.Collections.Generic;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
    // internal enum LineType
    // {
    //    NONE = 0,
    //    Function = 1,
    //    Assignment = 2,
    //    IfStatement = 3,
    //    StatementEnd = 4,
    //    Preprocessor = 5,
    //    Label = 6,
    //    Unknown = 7,
    // }
    internal abstract class LogicalLine
    {
        internal abstract int TokenOffset { get; }

        protected int tabCount;

        internal virtual int TabCount
        {
            get { return this.tabCount; }
            set { this.tabCount = value; }
        }

        protected List<string> errorMes = new List<string>();

        internal List<string> GetErrorMes()
        {
            return this.errorMes;
        }

        internal void AddError(string error)
        {
            this.errorMes.Add(error);
        }

        public override abstract string ToString();

        private bool visible = true;

        internal bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        internal virtual bool TabIncrement
        {
            get { return false; }
        }

        internal virtual bool TabDecrement
        {
            get { return false; }
        }

        internal virtual bool HasFlagGhostGoto
        {
            get { return false; }
        }

        internal virtual bool HasFlagIsGhost
        {
            get { return false; }
        }

        internal virtual void CheckLabel()
        {
        }

        internal virtual bool CheckRpn()
        {
            return true;
        }
    }
}
