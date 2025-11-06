using System;
using System.Collections.Generic;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Analyzer
{
    internal sealed class TokenCollection
    {
        private List<PrimitiveToken> primitives = new List<PrimitiveToken>();

        internal List<PrimitiveToken> Primitives
        {
            get { return this.primitives; }
        }

        private int position;

        internal int Position
        {
            get
            {
                return this.position;
            }

            set
            {
                if (this.position < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (this.position > this.primitives.Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                this.position = value;
            }
        }

        /// <summary>
        /// 抜け道なので出来るだけ使わないように。
        /// 変更とか絶対ダメ。.
        /// </summary>
        /// <returns></returns>
        internal List<PrimitiveToken> GetPrimives()
        {
            return this.primitives;
        }

        internal PrimitiveToken this[int i]
        {
            get { return this.primitives[i]; }
        }

        internal int Count
        {
            get { return this.primitives.Count; }
        }

        internal PrimitiveToken NextToken
        {
            get
            {
                if (this.NextIsEndOfStream)
                {
                    return null;
                }

                return this.primitives[this.position];
            }
        }

        internal bool NextNextTokenIsGotoFunction
        {
            get
            {
                if (this.NextIsEndOfStream)
                {
                    return false;
                }

                if ((this.position + 1) >= this.primitives.Count)
                {
                    return false;
                }

                PrimitiveToken token = this.primitives[this.position + 1];
                return (token.CodeExtraFlags & HspCodeExtraFlags.GotoFunction) == HspCodeExtraFlags.GotoFunction;
            }
        }

        internal TokenCollection GetLine()
        {
            if (this.NextIsEndOfStream)
            {
                return null;
            }

            List<PrimitiveToken> list = new List<PrimitiveToken>();
            list.Add(this.primitives[this.position]);
            this.position++;
            while (this.position < this.primitives.Count)
            {
                if (this.primitives[this.position].IsLineHead)
                {
                    break;
                }

                list.Add(this.primitives[this.position]);
                this.position++;
            }

            TokenCollection ret = new TokenCollection();
            ret.primitives = list;
            return ret;
        }

        internal PrimitiveToken GetNextToken()
        {
            if (this.position >= this.primitives.Count)
            {
                return null;
            }

            PrimitiveToken ret = this.primitives[this.position];
            this.position++;
            return ret;
        }

        internal bool NextIsEndOfStream
        {
            get { return this.position >= this.primitives.Count; }
        }

        internal bool NextIsEndOfLine
        {
            get
            {
                if (this.NextIsEndOfStream)
                {
                    return true;
                }

                if (this.primitives[this.position].IsLineHead)
                {
                    return true;
                }

                return false;
            }
        }

        internal bool NextIsEndOfParam
        {
            get
            {
                if (this.NextIsEndOfStream)
                {
                    return true;
                }

                if (this.primitives[this.position].IsLineHead)
                {
                    return true;
                }

                if (this.primitives[this.position].IsParamHead)
                {
                    return true;
                }

                return false;
            }
        }

        internal bool NextIsBracketStart
        {
            get
            {
                if (this.NextIsEndOfStream)
                {
                    return false;
                }

                PrimitiveToken token = this.primitives[this.position];
                if ((token.CodeExtraFlags & HspCodeExtraFlags.BracketStart) == HspCodeExtraFlags.BracketStart)
                {
                    return true;
                }

                return false;
            }
        }

        internal bool NextIsBracketEnd
        {
            get
            {
                if (this.NextIsEndOfStream)
                {
                    return false;
                }

                PrimitiveToken token = this.primitives[this.position];
                if ((token.CodeExtraFlags & HspCodeExtraFlags.BracketEnd) == HspCodeExtraFlags.BracketEnd)
                {
                    return true;
                }

                return false;
            }
        }

        // internal bool StartOfStream
        // {
        //    get
        //    {
        //        return (position <= 0);
        //    }
        // }

        // internal void Unget()
        // {
        //    position--;
        // }
        internal void Add(PrimitiveToken token)
        {
            this.primitives.Add(token);
        }
    }
}
