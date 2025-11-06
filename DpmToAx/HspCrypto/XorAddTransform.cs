#if AllowDecryption
namespace KttK.HspDecompiler.DpmToAx.HspCrypto
{
    internal struct XorAddTransform
    {
        internal byte XorByte;
        internal byte AddByte;

        // deHSP 100 HSP3.3でXORとSUMの適用順序が変わった？
        internal bool XorSum; // XORを先に適用するタイプ。旧式。

        public override string ToString()
        {
            return "xor:0x" + this.XorByte.ToString("X02") + "    " + "add:0x" + this.AddByte.ToString("X02") + "    " + "Farst xor:" + this.XorSum;
        }

        internal byte Encode(byte b)
        {
            if (this.XorSum)
            {
                return Sum(Xor(b, this.XorByte), this.AddByte);
            }

            return Xor(Sum(b, this.AddByte), this.XorByte);
        }

        internal byte Decode(byte b)
        {
            if (this.XorSum)
            {
                return Xor(Dif(b, this.AddByte), this.XorByte);
            }

            return Dif(Xor(b, this.XorByte), this.AddByte);
        }

        internal static byte GetXorByte(byte add, byte plain, byte encrypted, bool xorSum)
        {
            if (xorSum)
            {
                return (byte)(Dif(encrypted, add) ^ plain);
            }

            return Xor(encrypted, Sum(plain, add));
        }

        internal static byte Xor(byte b1, byte b2)
        {
            return (byte)(b1 ^ b2);
        }

        internal static byte Sum(byte b1, byte b2)
        {
            return (byte)((b1 + b2) & 0xFF);
        }

        internal static byte Dif(byte b1, byte b2)
        {
            return (byte)((0x100 + b1 - b2) & 0xFF);
        }
    }
}
#endif
