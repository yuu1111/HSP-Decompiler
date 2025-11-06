using System.Collections.Generic;
using System.IO;
using BYTE = System.Byte;
using DWORD = System.UInt32;
using WORD = System.UInt16;

namespace KttK.HspDecompiler.DpmToAx
{
    /// <summary>
    /// "PE\0\0"で始まるやつ.
    /// </summary>
    internal sealed class IMAGE_NT_HEADERS
    {
        /// <summary>
        /// リトルエンディアンでPE(50 45).
        /// </summary>
        private const int IMAGENTSIGNATURE = 0x4550;
        internal uint Signature;
        internal IMAGE_FILE_HEADER FileHeader;
        internal IMAGE_OPTIONAL_HEADER OptionalHeader;

        internal static IMAGE_NT_HEADERS FromBinaryReader(BinaryReader reader)
        {
            IMAGE_NT_HEADERS ret = new IMAGE_NT_HEADERS();
            ret.Signature = reader.ReadUInt32();
            if (ret.Signature != IMAGENTSIGNATURE)
            {
                return null;
            }

            ret.FileHeader = IMAGE_FILE_HEADER.FromBinaryReader(reader);
            if (ret.FileHeader == null)
            {
                return null;
            }

            ret.OptionalHeader = IMAGE_OPTIONAL_HEADER.FromBinaryReader(reader);
            if (ret.OptionalHeader == null)
            {
                return null;
            }

            return ret;
        }
    }

    internal sealed class IMAGE_FILE_HEADER
    {
        internal ushort Machine;
        internal ushort NumberOfSections;
        internal uint TimeDateStamp;
        internal uint PointerToSymbolTable;
        internal uint NumberOfSymbols;
        internal ushort SizeOfOptionalHeader;
        internal ushort Characteristics;

        internal static IMAGE_FILE_HEADER FromBinaryReader(BinaryReader reader)
        {
            IMAGE_FILE_HEADER ret = new IMAGE_FILE_HEADER();
            try
            {
                ret.Machine = reader.ReadUInt16();
                ret.NumberOfSections = reader.ReadUInt16();
                ret.TimeDateStamp = reader.ReadUInt32();
                ret.PointerToSymbolTable = reader.ReadUInt32();
                ret.NumberOfSymbols = reader.ReadUInt32();
                ret.SizeOfOptionalHeader = reader.ReadUInt16();
                ret.Characteristics = reader.ReadUInt16();
            }
            catch
            {
                return null;
            }

            return ret;
        }
    }

    internal sealed class IMAGE_OPTIONAL_HEADER
    {
        private const int IMAGENUMBEROFDIRECTORYENTRIES = 16;
        internal ushort Magic;
        internal byte MajorLinkerVersion;
        internal byte MinorLinkerVersion;
        internal uint SizeOfCode;
        internal uint SizeOfInitializedData;
        internal uint SizeOfUninitializedData;
        internal uint AddressOfEntryPoint;
        internal uint BaseOfCode;
        internal uint BaseOfData;
        internal uint ImageBase;
        internal uint SectionAlignment;
        internal uint FileAlignment;
        internal ushort MajorOperatingSystemVersion;
        internal ushort MinorOperatingSystemVersion;
        internal ushort MajorImageVersion;
        internal ushort MinorImageVersion;
        internal ushort MajorSubsystemVersion;
        internal ushort MinorSubsystemVersion;
        internal uint Win32VersionValue;
        internal uint SizeOfImage;
        internal uint SizeOfHeaders;
        internal uint CheckSum;
        internal ushort Subsystem;
        internal ushort DllCharacteristics;
        internal uint SizeOfStackReserve;
        internal uint SizeOfStackCommit;
        internal uint SizeOfHeapReserve;
        internal uint SizeOfHeapCommit;
        internal uint LoaderFlags;
        internal uint NumberOfRvaAndSizes;
        internal IMAGE_DATA_DIRECTORY[] DataDirectory = new IMAGE_DATA_DIRECTORY[IMAGENUMBEROFDIRECTORYENTRIES];

        internal static IMAGE_OPTIONAL_HEADER FromBinaryReader(BinaryReader reader)
        {
            IMAGE_OPTIONAL_HEADER ret = new IMAGE_OPTIONAL_HEADER();
            try
            {
                ret.Magic = reader.ReadUInt16();
                ret.MajorLinkerVersion = reader.ReadByte();
                ret.MinorLinkerVersion = reader.ReadByte();
                ret.SizeOfCode = reader.ReadUInt32();
                ret.SizeOfInitializedData = reader.ReadUInt32();
                ret.SizeOfUninitializedData = reader.ReadUInt32();
                ret.AddressOfEntryPoint = reader.ReadUInt32();
                ret.BaseOfCode = reader.ReadUInt32();
                ret.BaseOfData = reader.ReadUInt32();
                ret.ImageBase = reader.ReadUInt32();
                ret.SectionAlignment = reader.ReadUInt32();
                ret.FileAlignment = reader.ReadUInt32();
                ret.MajorOperatingSystemVersion = reader.ReadUInt16();
                ret.MinorOperatingSystemVersion = reader.ReadUInt16();
                ret.MajorImageVersion = reader.ReadUInt16();
                ret.MinorImageVersion = reader.ReadUInt16();
                ret.MajorSubsystemVersion = reader.ReadUInt16();
                ret.MinorSubsystemVersion = reader.ReadUInt16();
                ret.Win32VersionValue = reader.ReadUInt32();
                ret.SizeOfImage = reader.ReadUInt32();
                ret.SizeOfHeaders = reader.ReadUInt32();
                ret.CheckSum = reader.ReadUInt32();
                ret.Subsystem = reader.ReadUInt16();
                ret.DllCharacteristics = reader.ReadUInt16();
                ret.SizeOfStackReserve = reader.ReadUInt32();
                ret.SizeOfStackCommit = reader.ReadUInt32();
                ret.SizeOfHeapReserve = reader.ReadUInt32();
                ret.SizeOfHeapCommit = reader.ReadUInt32();
                ret.LoaderFlags = reader.ReadUInt32();
                ret.NumberOfRvaAndSizes = reader.ReadUInt32();

                for (int i = 0; i < IMAGENUMBEROFDIRECTORYENTRIES; i++)
                {
                    ret.DataDirectory[i] = new IMAGE_DATA_DIRECTORY();
                    ret.DataDirectory[i].VirtualAddress = reader.ReadUInt32();
                    ret.DataDirectory[i].Size = reader.ReadUInt32();
                }
            }
            catch
            {
                return null;
            }

            return ret;
        }
    }

    internal sealed class IMAGE_DATA_DIRECTORY
    {
        internal uint VirtualAddress;
        internal uint Size;
    }

    internal sealed class IMAGE_SECTION_HEADER
    {
        internal const int IMAGESIZEOFSHORTNAME = 8;
        internal byte[] Name = new byte[IMAGESIZEOFSHORTNAME];
        internal uint PhysicalAddress;

        internal uint VirtualSize
        {
            get { return this.PhysicalAddress; }
            set { this.PhysicalAddress = value; }
        }

        internal uint VirtualAddress;
        internal uint SizeOfRawData;
        internal uint PointerToRawData;
        internal uint PointerToRelocations;
        internal uint PointerToLinenumbers;
        internal ushort NumberOfRelocations;
        internal ushort NumberOfLinenumbers;
        internal uint Characteristics;

        internal static IMAGE_SECTION_HEADER FromBinaryReader(BinaryReader reader)
        {
            IMAGE_SECTION_HEADER ret = new IMAGE_SECTION_HEADER();
            try
            {
                bool allZero = true;
                for (int i = 0; i < IMAGESIZEOFSHORTNAME; i++)
                {
                    ret.Name[i] = reader.ReadByte();
                    allZero &= ret.Name[i] == 0;
                }

                if (allZero)
                {
                    return null;
                }

                ret.PhysicalAddress = reader.ReadUInt32();
                ret.VirtualAddress = reader.ReadUInt32();
                ret.SizeOfRawData = reader.ReadUInt32();
                ret.PointerToRawData = reader.ReadUInt32();
                ret.PointerToRelocations = reader.ReadUInt32();
                ret.PointerToLinenumbers = reader.ReadUInt32();
                ret.NumberOfRelocations = reader.ReadUInt16();
                ret.NumberOfLinenumbers = reader.ReadUInt16();
                ret.Characteristics = reader.ReadUInt32();
            }
            catch
            {
                return null;
            }

            return ret;
        }
    }

    internal sealed class IMAGE_DOS_HEADER
    { // DOS .EXE header

        /// <summary>
        /// リトルエンディアンでMZ(4D 5A).
        /// </summary>
        private const int IMAGEDOSSIGNATURE = 0x5A4D;
        internal ushort EMagic; // Magic number
        internal ushort ECblp; // Bytes on last page of file
        internal ushort ECp; // Pages in file
        internal ushort ECrlc; // Relocations
        internal ushort ECparhdr; // Size of header in paragraphs
        internal ushort EMinalloc; // Minimum extra paragraphs needed
        internal ushort EMaxalloc; // Maximum extra paragraphs needed
        internal ushort ESs; // Initial (relative) SS value
        internal ushort ESp; // Initial SP value
        internal ushort ECsum; // Checksum
        internal ushort EIp; // Initial IP value
        internal ushort ECs; // Initial (relative) CS value
        internal ushort ELfarlc; // File address of relocation table
        internal ushort EOvno; // Overlay number
        internal ushort[] ERes = new ushort[4]; // Reserved words
        internal ushort EOemid; // OEM identifier (for e_oeminfo)
        internal ushort EOeminfo; // OEM information; e_oemid specific
        internal ushort[] ERes2 = new ushort[10]; // Reserved words
        internal uint ELfanew; // File address of new exe header

        internal static IMAGE_DOS_HEADER FromBinaryReader(BinaryReader reader)
        {
            IMAGE_DOS_HEADER ret = new IMAGE_DOS_HEADER();
            try
            {
                ret.EMagic = reader.ReadUInt16(); // Magic number
                if (ret.EMagic != IMAGEDOSSIGNATURE)
                {
                    return null;
                }

                ret.ECblp = reader.ReadUInt16(); // Bytes on last page of file
                ret.ECp = reader.ReadUInt16(); // Pages in file
                ret.ECrlc = reader.ReadUInt16(); // Relocations
                ret.ECparhdr = reader.ReadUInt16(); // Size of header in paragraphs
                ret.EMinalloc = reader.ReadUInt16(); // Minimum extra paragraphs needed
                ret.EMaxalloc = reader.ReadUInt16(); // Maximum extra paragraphs needed
                ret.ESs = reader.ReadUInt16(); // Initial (relative) SS value
                ret.ESp = reader.ReadUInt16(); // Initial SP value
                ret.ECsum = reader.ReadUInt16(); // Checksum
                ret.EIp = reader.ReadUInt16(); // Initial IP value
                ret.ECs = reader.ReadUInt16(); // Initial (relative) CS value
                ret.ELfarlc = reader.ReadUInt16(); // File address of relocation table
                ret.EOvno = reader.ReadUInt16(); // Overlay number
                for (int i = 0; i < 4; i++)
                {
                    ret.ERes[i] = reader.ReadUInt16();
                }

                ret.EOemid = reader.ReadUInt16();
                ret.EOeminfo = reader.ReadUInt16();

                for (int i = 0; i < 10; i++)
                {
                    ret.ERes2[i] = reader.ReadUInt16();
                }

                ret.ELfanew = reader.ReadUInt32();
            }
            catch
            {
                return null;
            }

            return ret;
        }
    }

    internal sealed class IMAGE_IMPORT_DESCRIPTOR
    {
        internal uint OriginalFirstThunk;
        internal uint TimeDataStamp;
        internal uint ForwarderChain;
        internal uint Name;
        internal uint FirstThunk;

        internal static IMAGE_IMPORT_DESCRIPTOR FromBinaryReader(BinaryReader reader)
        {
            IMAGE_IMPORT_DESCRIPTOR ret = new IMAGE_IMPORT_DESCRIPTOR();
            try
            {
                ret.OriginalFirstThunk = reader.ReadUInt32();
                ret.TimeDataStamp = reader.ReadUInt32();
                ret.ForwarderChain = reader.ReadUInt32();
                ret.Name = reader.ReadUInt32();
                ret.FirstThunk = reader.ReadUInt32();
            }
            catch
            {
                return null;
            }

            return ret;
        }
    }

    internal sealed class Win32ExeHeader
    {
        private IMAGE_DOS_HEADER dosHeader;
        private IMAGE_NT_HEADERS ntHeader;
        private List<IMAGE_SECTION_HEADER> sectionHeaders = new List<IMAGE_SECTION_HEADER>();

        internal long EndOfExecutableRegion
        {
            get
            {
                if (this.sectionHeaders.Count == 0)
                {
                    return -1;
                }

                IMAGE_SECTION_HEADER section = this.sectionHeaders[this.sectionHeaders.Count - 1];
                return section.PointerToRawData + section.SizeOfRawData;
            }
        }

        internal static Win32ExeHeader FromBinaryReader(BinaryReader reader)
        {
            try
            {
                long startPosition = reader.BaseStream.Position;
                long length = reader.BaseStream.Length - startPosition;
                if (length < 0x1000)
                {
                    return null;
                }

                Win32ExeHeader ret = new Win32ExeHeader();
                ret.dosHeader = IMAGE_DOS_HEADER.FromBinaryReader(reader);
                if (ret.dosHeader == null)
                {
                    return null;
                }

                if (ret.dosHeader.ELfanew <= 0)
                {
                    return ret;
                }

                reader.BaseStream.Seek(startPosition + ret.dosHeader.ELfanew, SeekOrigin.Begin);
                ret.ntHeader = IMAGE_NT_HEADERS.FromBinaryReader(reader);
                if (ret.ntHeader == null)
                {
                    return null;
                }

                IMAGE_SECTION_HEADER section = IMAGE_SECTION_HEADER.FromBinaryReader(reader);
                while (section != null)
                {
                    ret.sectionHeaders.Add(section);
                    section = IMAGE_SECTION_HEADER.FromBinaryReader(reader);
                }

                return ret;
            }
            catch
            {
                return null;
            }
        }
    }
}
