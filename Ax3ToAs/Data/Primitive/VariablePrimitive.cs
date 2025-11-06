using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Primitive
{
    internal abstract class VariablePrimitive : OperandPrimitive
    {
        protected VariablePrimitive()
        {
        }

        internal VariablePrimitive(PrimitiveTokenDataSet dataSet)
            : base(dataSet)
        {
        }
    }

    internal sealed class GlobalVariablePrimitive : VariablePrimitive
    {
        private readonly string varName;

        private GlobalVariablePrimitive()
        {
        }

        internal GlobalVariablePrimitive(PrimitiveTokenDataSet dataSet)
            : base(dataSet)
        {
            this.varName = dataSet.Parent.GetVariableName(this.Value);
        }

        public override string ToString()
        {
            if (this.varName != null)
            {
                return this.varName;
            }

            StringBuilder bld = new StringBuilder("var");
            bld.Append("_");
            bld.Append(this.Value.ToString());
            return bld.ToString();
        }

#if DEBUG
        public static string ToString(int value)
        {
            StringBuilder bld = new StringBuilder("var");
            bld.Append("_");
            bld.Append(value.ToString());
            return bld.ToString();
        }
#endif
    }

    internal sealed class ParameterPrimitive : VariablePrimitive
    {
        private ParameterPrimitive()
        {
        }

        internal ParameterPrimitive(PrimitiveTokenDataSet dataSet)
            : base(dataSet)
        {
            this.param = dataSet.Parent.GetParam(this.Value);
            if (this.param != null)
            {
                this.param.ParamNameIsUsed = true;
            }
        }

        private readonly Param param;

        public override string ToString()
        {
            if (this.param != null)
            {
                return this.param.ParamName;
            }

            return this.DefaultName;
        }
    }
}
