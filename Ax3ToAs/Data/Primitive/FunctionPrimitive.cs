namespace KttK.HspDecompiler.Ax3ToAs.Data.Primitive
{
    internal abstract class FunctionPrimitive : PrimitiveToken
    {
        protected FunctionPrimitive()
        {
        }

        internal FunctionPrimitive(PrimitiveTokenDataSet dataSet)
            : base(dataSet)
        {
        }
    }

    internal sealed class UserFunctionPrimitive : FunctionPrimitive
    {
        private UserFunctionPrimitive()
        {
        }

        internal UserFunctionPrimitive(PrimitiveTokenDataSet dataSet)
            : base(dataSet)
        {
            this.func = dataSet.Parent.GetUserFunction(this.Value);
        }

        private readonly Function func;

        public override string ToString()
        {
            if (this.func == null)
            {
                return this.DefaultName;
            }

            return this.func.FunctionName;
        }
    }

    internal sealed class DllFunctionPrimitive : FunctionPrimitive
    {
        private DllFunctionPrimitive()
        {
        }

        internal DllFunctionPrimitive(PrimitiveTokenDataSet dataSet)
            : base(dataSet)
        {
            this.func = dataSet.Parent.GetDllFunction(this.Value);
        }

        private readonly Function func;

        public override string ToString()
        {
            if (this.func == null)
            {
                return this.DefaultName;
            }

            return this.func.FunctionName;
        }
    }

    internal sealed class PlugInFunctionPrimitive : FunctionPrimitive
    {
        private PlugInFunctionPrimitive()
        {
        }

        internal PlugInFunctionPrimitive(PrimitiveTokenDataSet dataSet)
            : base(dataSet)
        {
            int pluginIndex = dataSet.DicValue.OparatorPriority;
            this.cmd = dataSet.Parent.AddCmd(pluginIndex, this.Value);
        }

        private readonly Cmd cmd;

        public override string ToString()
        {
            if (this.cmd == null)
            {
                return this.DefaultName;
            }

            return this.cmd.FunctionName;
        }
    }

    internal sealed class ComFunctionPrimitive : FunctionPrimitive
    {
        private ComFunctionPrimitive()
        {
        }

        internal ComFunctionPrimitive(PrimitiveTokenDataSet dataSet)
            : base(dataSet)
        {
            this.func = dataSet.Parent.GetDllFunction(this.Value - 0x1000);
        }

        private readonly Function func;

        public override string ToString()
        {
            if (this.func == null)
            {
                return this.DefaultName;
            }

            return this.func.FunctionName;
        }
    }
}
