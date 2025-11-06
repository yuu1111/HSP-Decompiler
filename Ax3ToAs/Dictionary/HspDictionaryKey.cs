using System;

namespace KttK.HspDecompiler.Ax3ToAs.Dictionary
{
    internal struct HspDictionaryKey : IComparable<HspDictionaryKey>, IEquatable<HspDictionaryKey>
    {
        internal HspDictionaryKey(HspDictionaryKey key)
        {
            this.Type = key.Type;
            this.Value = key.Value;
            this.AllValue = key.AllValue;
        }

        internal HspDictionaryKey(string theType, string theValue)
        {
            this.Type = DicParser.StringToInt32(theType);
            this.Value = DicParser.StringToInt32(theValue);
            this.AllValue = false;
            if (this.Value == -1)
            {
                this.AllValue = true;
            }
        }

        internal int Type;
        internal int Value;
        internal bool AllValue;

        public override string ToString()
        {
            if (this.Value == -1)
            {
                return "Type:0x" + this.Type.ToString("X02") + "Value:0xFFFF";
            }

            return "Type:0x" + this.Type.ToString("X02") + "Value:0x" + this.Value.ToString("X04");
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(HspDictionaryKey))
            {
                throw new Exception("サポート外");
            }

            return this.Equals((HspDictionaryKey)obj);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode() ^ this.Value.GetHashCode();
        }

        public bool Equals(HspDictionaryKey other)
        {
            return this.Type.Equals(other.Type) && this.Value.Equals(other.Value);
        }

        public int CompareTo(HspDictionaryKey other)
        {
            int ret = this.Type.CompareTo(other.Type);
            if (ret != 0)
            {
                return ret;
            }

            return this.Value.CompareTo(other.Value);
        }
    }
}
