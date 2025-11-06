using System;

namespace KttK.HspDecompiler.Ax3ToAs.Dictionary
{
    internal struct HspDictionaryValue
    {
        internal HspDictionaryValue(string theName, string theType, string[] theExtras)
        {
            this.Name = theName;
            this.Type = (HspCodeType)Enum.Parse(typeof(HspCodeType), theType);
            this.Extra = HspCodeExtraFlags.NONE;
            this.OparatorPriority = -1;
            foreach (string theExtra in theExtras)
            {
                string testString = theExtra.Trim();
                if (testString.Length == 0)
                {
                    continue;
                }

                if (testString.StartsWith("Priority_"))
                {
                    this.OparatorPriority = int.Parse(testString.Substring(9));
                    continue;
                }

                this.Extra |= (HspCodeExtraFlags)Enum.Parse(typeof(HspCodeExtraFlags), testString);
            }
        }

        internal string Name;
        internal HspCodeType Type;
        internal HspCodeExtraFlags Extra;
        internal int OparatorPriority;

        public override string ToString()
        {
            if (this.Name.Length == 0)
            {
                return this.Type.ToString();
            }

            return this.Type + "  \"" + this.Name + "\"";
        }
    }
}
