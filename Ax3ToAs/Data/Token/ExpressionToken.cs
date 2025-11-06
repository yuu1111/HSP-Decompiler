using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
    /// <summary>
    /// 式.
    /// </summary>
    internal sealed class ExpressionToken : CodeToken
    {
        private ExpressionToken()
        {
        }

        internal ExpressionToken(List<ExpressionTermToken> elements)
        {
            this.tokens = elements;
        }

        private readonly List<ExpressionTermToken> tokens;
        private ExpressionTermToken convertedToken;
        private bool tryConvert;

        internal bool CanRpnConvert
        {
            get
            {
                if (this.convertedToken != null)
                {
                    return true;
                }

                if (!this.tryConvert)
                {
                    return this.RpnConvert();
                }

                return false;
            }
        }

        internal bool RpnConvert()
        {
            if (this.convertedToken != null)
            {
                return true;
            }

            if (this.tokens.Count == 0)
            {
                return false;
            }

            this.tryConvert = true;
            if (this.tokens.Count == 1)
            {
                this.convertedToken = this.tokens[0];
                return true;
            }

            List<ExpressionTermToken> stack = new List<ExpressionTermToken>();
            List<ExpressionTermToken> source = new List<ExpressionTermToken>();
            try
            {
                source.AddRange(this.tokens);
                while (source.Count != 0)
                {
                    ExpressionTermToken token = source[0];
                    source.RemoveAt(0);
                    if (token.IsOperator)
                    {
                        OperandToken right = (OperandToken)stack[stack.Count - 1];
                        stack.RemoveAt(stack.Count - 1);
                        OperandToken left = (OperandToken)stack[stack.Count - 1];
                        stack.RemoveAt(stack.Count - 1);
                        stack.Add((ExpressionTermToken)new SubExpressionToken(left, right, (OperatorToken)token));
                    }
                    else
                    {
                        stack.Add(token);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            if (stack.Count != 1)
            {
                return false;
            }

            this.convertedToken = stack[0];
            return true;
        }

        internal override int TokenOffset
        {
            get
            {
                if ((this.tokens == null) || (this.tokens.Count == 0))
                {
                    return -1;
                }

                CodeToken token = this.tokens[0] as CodeToken;
                if (token == null)
                {
                    return -1;
                }

                return token.TokenOffset;
            }
        }

        internal string ToString(bool getRpnConverted)
        {
            if (getRpnConverted && (this.convertedToken != null))
            {
                return this.convertedToken.ToString();
            }

            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (ExpressionTermToken token in this.tokens)
            {
                if (i != 0)
                {
                    builder.Append(' ');
                }

                builder.Append(token);
                i++;
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            return this.ToString(true);
        }

        internal override void CheckLabel()
        {
            foreach (CodeToken token in this.tokens)
            {
                token.CheckLabel();
            }
        }

        internal override bool CheckRpn()
        {
            return this.CanRpnConvert;
        }
    }
}
