using Marathon.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knux_s_Misc_Tools
{
    public class S06Havok
    {
        public class HavokDataIndex
        {
            // Always either 3 or 4.
            public uint hdi_UnknownUInt32_1 { get; set; }

            public uint hdi_UnknownUInt32_2 { get; set; }

            // Always 0xFFFFFFFF if UnknownUInt32_1 is 4.
            public uint hdi_UnknownUInt32_3 { get; set; }
        }

        public class HavokData
        {
            public uint hd_UnknownUInt32_1 { get; set; }
        }

        public class FormatData
        {
            public List<string> HavokClassNames = new();
            public List<uint> HavokClassIndicies = new();
            public List<HavokDataIndex> HavokDataIndicies = new();
            public HavokData HavokData = new();
        }

        public FormatData Data = new();

        public void Load(string filepath)
        {
            // Set up the Extended Binary Reader.
            BinaryReaderEx reader = new(File.OpenRead(filepath), true);

            #region Generic Header Stuff, ends at 0x130
            // Header?
            uint header_UnknownUInt32_1 = reader.ReadUInt32(); // Always 57 E0 E0 57.
            uint header_UnknownUInt32_2 = reader.ReadUInt32(); // Always 10 C0 C0 10.
            uint header_UnknownUInt32_3 = reader.ReadUInt32(); // Always 00 00 00 00.
            uint header_UnknownUInt32_4 = reader.ReadUInt32(); // Always 00 00 00 03.
                 
            uint header_UnknownUInt32_5 = reader.ReadUInt32(); // Always 04 00 00 01.
            uint header_UnknownUInt32_6 = reader.ReadUInt32(); // Always 00 00 00 05.
            uint header_UnknownUInt32_7 = reader.ReadUInt32(); // Always 00 00 00 03.
            uint header_UnknownUInt32_8 = reader.ReadUInt32(); // Always 00 00 00 00.
                 
            uint header_UnknownUInt32_9 = reader.ReadUInt32(); // Always 00 00 00 00.
            uint header_UnknownUInt32_10 = reader.ReadUInt32(); // Offset? Not padded to 4 so maybe not.
            string HavokID = reader.ReadNullTerminatedString(); // Followed by 9 bytes of FF, likely as padding to realign.
            reader.FixPadding(0x10);

            // Classnames Header.
            string ClassnamesID = reader.ReadNullTerminatedString();
            reader.FixPadding(0x10);

            uint classnames_UnknownUInt32_1 = reader.ReadUInt32(); // Always 00 00 00 FF.
            uint classnames_Offset          = reader.ReadUInt32(); // Always 00 00 01 30. Offset to a Havok String Table it seems?
            uint classnames_Length          = reader.ReadUInt32(); // Length of the data pointed to by classnames_Offset maybe?
            uint classnames_UnknownUInt32_4 = reader.ReadUInt32(); // Always the same as classnames_Length.

            uint classnames_UnknownUInt32_5 = reader.ReadUInt32(); // Always the same as classnames_Length.
            uint classnames_UnknownUInt32_6 = reader.ReadUInt32(); // Always the same as classnames_Length.
            uint classnames_UnknownUInt32_7 = reader.ReadUInt32(); // Always the same as classnames_Length.
            reader.FixPadding(0x10);

            // Classindex Header.
            string ClassindexID = reader.ReadNullTerminatedString();
            reader.FixPadding(0x10);

            uint classindex_UnknownUInt32_1 = reader.ReadUInt32(); // Always 00 00 00 FF.
            uint classindex_Offset          = reader.ReadUInt32(); // Seemingly an offset to some data.
            uint classindex_Length          = reader.ReadUInt32(); // Length of the data pointed to by classindex_Offset maybe?
            uint classindex_UnknownUInt32_4 = reader.ReadUInt32(); // Always the same as classindex_Length.

            uint classindex_UnknownUInt32_5 = reader.ReadUInt32(); // Always the same as classindex_Length.
            uint classindex_UnknownUInt32_6 = reader.ReadUInt32(); // Always the same as classindex_Length.
            uint classindex_UnknownUInt32_7 = reader.ReadUInt32(); // Always the same as classindex_Length.
            reader.FixPadding(0x10);

            // Dataindex Header
            string DataindexID = reader.ReadNullTerminatedString();
            reader.FixPadding(0x10);

            uint dataindex_UnknownUInt32_1 = reader.ReadUInt32(); // Always 00 00 00 FF.
            uint dataindex_Offset          = reader.ReadUInt32(); // Seemingly an offset to some data.
            uint dataIndex_Length          = reader.ReadUInt32(); // Length of the data pointed to by dataindex_Offset maybe?
            uint dataindex_UnknownUInt32_4 = reader.ReadUInt32(); // Always the same as dataIndex_Length.

            uint dataindex_UnknownUInt32_5 = reader.ReadUInt32(); // Always the same as dataIndex_Length.
            uint dataindex_UnknownUInt32_6 = reader.ReadUInt32(); // Always the same as dataIndex_Length.
            uint dataindex_UnknownUInt32_7 = reader.ReadUInt32(); // Always the same as dataIndex_Length.
            reader.FixPadding(0x10);

            // Data Header
            string DataID = reader.ReadNullTerminatedString();
            reader.FixPadding(0x10);

            uint data_UnknownUInt32_1 = reader.ReadUInt32(); // Always 00 00 00 FF.
            uint data_Offset          = reader.ReadUInt32(); // Seemingly an offset to some data.
            uint data_UnknownUInt32_3 = reader.ReadUInt32(); // Length of the data pointed to by data_Offset maybe?
            uint data_UnknownUInt32_4 = reader.ReadUInt32();

            uint data_UnknownUInt32_5 = reader.ReadUInt32();
            uint data_UnknownUInt32_6 = reader.ReadUInt32(); // Always FF FF FF FF.
            uint data_UnknownUInt32_7 = reader.ReadUInt32(); // Seems to be the length of this whole block?
            uint data_UnknownUInt32_8 = reader.ReadUInt32(); // Always FF FF FF FF.

            // Types Header
            string TypesID = reader.ReadNullTerminatedString();
            reader.FixPadding(0x10);

            uint types_UnknownUInt32_1 = reader.ReadUInt32(); // Always 00 00 00 FF.
            uint types_Offset          = reader.ReadUInt32(); // Seemingly an offset to some data.
            uint types_UnknownUInt32_3 = reader.ReadUInt32(); // Length of the data pointed to by types_Offset maybe?
            uint types_UnknownUInt32_4 = reader.ReadUInt32();
                 
            uint types_UnknownUInt32_5 = reader.ReadUInt32();
            uint types_UnknownUInt32_6 = reader.ReadUInt32(); // Always FF FF FF FF.
            uint types_UnknownUInt32_7 = reader.ReadUInt32(); // Seems to be the length of this whole block?
            uint types_UnknownUInt32_8 = reader.ReadUInt32(); // Always FF FF FF FF.
            #endregion

            // Class Names
            // Already here in theory but just to be safe.
            reader.JumpTo(classnames_Offset);

            // Read the class names while we're within a range and the current byte doesn't indicate padding.
            while(reader.BaseStream.Position < (classnames_Offset + classnames_Length) && reader.ReadByte() != 0xFF)
            {
                reader.JumpBehind();
                Data.HavokClassNames.Add(reader.ReadNullTerminatedString());
            }

            // Class Index
            reader.JumpTo(classindex_Offset);

            // Seems to have the same amount of entries as the Class Names do, so loop through based on that value.
            for(int i = 0; i < Data.HavokClassNames.Count; i++)
            {
                reader.JumpAhead(0x4); // This value seems to always be 4.
                Data.HavokClassIndicies.Add(reader.ReadUInt32());
            }

            // Data Index
            reader.JumpTo(dataindex_Offset);

            // Read the data indicies while we're within a range and the current byte doesn't indicate padding.
            while (reader.BaseStream.Position < (dataindex_Offset + dataIndex_Length) && reader.ReadByte() != 0xFF)
            {
                reader.JumpBehind();
                HavokDataIndex dataIndex = new()
                {
                    hdi_UnknownUInt32_1 = reader.ReadUInt32(),
                    hdi_UnknownUInt32_2 = reader.ReadUInt32(),
                    hdi_UnknownUInt32_3 = reader.ReadUInt32()
                };
                Data.HavokDataIndicies.Add(dataIndex);
            }

            // Data
            reader.JumpTo(data_Offset);
            uint Data_UnknownUInt32_1 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_2 = reader.ReadUInt32(); // Always 00 00 00 01
            uint Data_UnknownUInt32_3 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_4 = reader.ReadUInt32(); // Always 00 00 00 00

            uint Data_UnknownUInt32_5 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_6 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_7 = reader.ReadUInt32(); // Always 00 00 00 00
            string Data_PhysicsData = reader.ReadNullTerminatedString();
            reader.FixPadding(0x10);

            // Padding?
            uint Data_UnknownUInt32_8 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_9 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_10 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_11 = reader.ReadUInt32(); // Always 00 00 00 00

            uint Data_UnknownUInt32_12 = reader.ReadUInt32(); // Always 00 00 00 01
            uint Data_UnknownUInt32_13 = reader.ReadUInt32(); // Always C0 00 00 01
            uint Data_UnknownUInt32_14 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_15 = reader.ReadUInt32(); // Always 00 00 00 00

            uint Data_UnknownUInt32_16 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_17 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_18 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_19 = reader.ReadUInt32(); // Always 00 00 00 00

            uint Data_UnknownUInt32_20 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_21 = reader.ReadUInt32(); // Always 00 00 00 00
            uint Data_UnknownUInt32_22 = reader.ReadUInt32(); // Always 00 00 00 00
            Data.HavokData.hd_UnknownUInt32_1 = reader.ReadUInt32(); // First value in this block that varies, 6C bytes later.
        }
    }
}
