using System;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Analyzer
{
    /// <summary>
    /// LogicalLine解析中の例外
    /// 投げてよいのは２つのFactoryの中だけ
    /// 受け止められるのはLogicalLineのinternalメソッドだけ.
    /// </summary>
    internal class HspLogicalLineException : ApplicationException
    {
        internal HspLogicalLineException()
            : base()
        {
        }

        internal HspLogicalLineException(string message)
            : base(message)
        {
        }

        internal HspLogicalLineException(string source, string message)
            : base(message)
        {
            this.Source = source;
        }

        internal HspLogicalLineException(string message, Exception e)
            : base(message, e)
        {
        }

        internal HspLogicalLineException(string source, string message, Exception e)
            : base(message, e)
        {
            this.Source = source;
        }
    }
}
