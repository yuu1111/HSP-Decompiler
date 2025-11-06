using System;

namespace KttK.HspDecompiler
{
    internal sealed class HspDecoderException : ApplicationException
    {
        internal HspDecoderException()
            : base()
        {
        }

        internal HspDecoderException(string message)
            : base(message)
        {
        }

        internal HspDecoderException(string source, string message)
            : base(message)
        {
            Source = source;
        }

        internal HspDecoderException(string message, Exception e)
            : base(message, e)
        {
        }

        internal HspDecoderException(string source, string message, Exception e)
            : base(message, e)
        {
            Source = source;
        }

        internal HspDecoderException(string message, HspDecoderException e)
            : base(e.Message, e.InnerException)
        {
            Source = e.Source;
        }

        internal HspDecoderException(HspDecoderException e)
            : base(e.Message, e.InnerException)
        {
            Source = e.Source;
        }
    }
}
