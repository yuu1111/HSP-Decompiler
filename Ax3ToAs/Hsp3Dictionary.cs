using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Dictionary;
using Microsoft.VisualBasic.FileIO;

namespace KttK.HspDecompiler.Ax3ToAs
{
    internal class Hsp3Dictionary
    {
        private Hsp3Dictionary()
        {
        }

        private Dictionary<HspDictionaryKey, HspDictionaryValue> codeDictionary = new Dictionary<HspDictionaryKey, HspDictionaryValue>();
        private Dictionary<int, string> paramDictionary = new Dictionary<int, string>();

        internal static Hsp3Dictionary FromFile(string filePath)
        {
            Hsp3Dictionary? ret = null;
            Stream? stream = null;
            TextFieldParser? parser = null;
            try
            {
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                parser = new TextFieldParser(stream, Encoding.UTF8);
                ret = FromParser(parser);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (parser != null)
                {
                    parser.Close();
                }
                else if (stream != null)
                {
                    stream.Close();
                }
            }

            return ret;
        }

        private static Hsp3Dictionary FromParser(TextFieldParser parser)
        {
            Hsp3Dictionary ret = new Hsp3Dictionary();
            parser.SetDelimiters(new string[] { "," });
            parser.CommentTokens = new string[] { "#" };
            while (!parser.EndOfData)
            {
                string[] tokens = parser.ReadFields();
                if (tokens.Length == 0)
                {
                    continue;
                }

                if (tokens[0].StartsWith("$"))
                {
                    switch (tokens[0])
                    {
                        case "$Code":
                            ret.LoadCodeDictionary(parser);
                            break;
                        case "$ParamType":
                            ret.LoadParamDictionary(parser);
                            break;
                    }
                }
            }

            return ret;
        }

        private void LoadCodeDictionary(TextFieldParser stream)
        {
            while (!stream.EndOfData)
            {
                string[] tokens = stream.ReadFields();
                if (tokens.Length == 0)
                {
                    continue;
                }

                if (tokens[0] == "$End")
                {
                    return;
                }

                if (tokens.Length >= 4)
                {
                    string[] extraFlags = new string[tokens.Length - 4];
                    Array.Copy(tokens, 4, extraFlags, 0, tokens.Length - 4);
                    HspDictionaryKey key = new HspDictionaryKey(tokens[0], tokens[1]);
                    HspDictionaryValue value = new HspDictionaryValue(tokens[2], tokens[3], extraFlags);
                    this.codeDictionary.Add(key, value);
                }
            }
        }

        private void LoadParamDictionary(TextFieldParser stream)
        {
            while (!stream.EndOfData)
            {
                string[] tokens = stream.ReadFields();
                if (tokens.Length == 0)
                {
                    continue;
                }

                if (tokens[0] == "$End")
                {
                    return;
                }

                if (tokens.Length >= 2)
                {
                    int key = DicParser.StringToInt32(tokens[0]);
                    string value = tokens[1];
                    this.paramDictionary.Add(key, value);
                }
            }
        }

        internal bool CodeLookUp(HspDictionaryKey key, out HspDictionaryValue value)
        {
            if (this.codeDictionary.TryGetValue(key, out value))
            {
                return true;
            }

            HspDictionaryKey newkey = new HspDictionaryKey(key);
            newkey.Value = -1;
            newkey.AllValue = true;
            if (this.codeDictionary.TryGetValue(newkey, out value))
            {
                return true;
            }

            if ((key.Type == 0x11) && (key.Value >= 0x1000)) // ComFunction
            {
                value.Name = "comfunc";
                value.Type = HspCodeType.ComFunction;
                value.Extra = HspCodeExtraFlags.NONE;
                return true;
            }

            if (key.Type >= 0x12) // PlugInFunction
            {
                value.Name = "pluginFuction";
                value.OparatorPriority = key.Type - 0x12;
                value.Type = HspCodeType.PlugInFunction;
                value.Extra = HspCodeExtraFlags.NONE;
                return true;
            }

            return false;
        }

        internal bool ParamLookUp(int paramKey, out string paramTypeName)
        {
            return this.paramDictionary.TryGetValue(paramKey, out paramTypeName);
        }

        internal List<string> GetAllFuncName()
        {
            List<string> ret = new List<string>();
            foreach (KeyValuePair<HspDictionaryKey, HspDictionaryValue> pair in this.codeDictionary)
            {
                switch (pair.Value.Type)
                {
                    case HspCodeType.HspFunction:
                    case HspCodeType.IfStatement:
                    case HspCodeType.OnEventStatement:
                    case HspCodeType.OnStatement:
                    case HspCodeType.McallStatement:
                        ret.Add(pair.Value.Name.ToLower());
                        break;
                }
            }

            foreach (KeyValuePair<int, string> pair in this.paramDictionary)
            {
                ret.Add(pair.Value.ToLower());
            }

            return ret;
        }
    }
}
