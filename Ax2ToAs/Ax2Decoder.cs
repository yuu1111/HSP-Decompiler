using System;
using System.Collections.Generic;
using System.IO;
using KttK.HspDecompiler.Ax2ToAs.Data;

namespace KttK.HspDecompiler.Ax2ToAs
{
    class Ax2Decoder : AbstractAxDecoder
    {
        public override List<string> Decode(BinaryReader reader)
        {
            AxData data = null;
            AxData.FromStream(reader.BaseStream);
            try
            {
                HspConsole.Write("ヘッダー解析中...");
                data = AxData.FromStream(reader.BaseStream);

                HspConsole.Write("解析中...");
                data.Decompile();
            }
            catch (SystemException e)
            {
                throw new HspDecoderException("AxData", "想定外のエラー", e);
            }

            List<string> lines = data.GetLines();

            return data.GetLines();
        }
    }
}
