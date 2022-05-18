namespace Knuxs_Misc_Tools.WrathOfCortex.HGO_Chunk
{
    // TODO: What the hell is this? Some sort of matrix table? What's the extra data afterwards?
    public class INST
    {
        public List<INSTEntry1> UnknownDataStruct_1 { get; set; } = new();

        public List<INSTEntry2> UnknownDataStruct_2 { get; set; } = new();

        public void Read(BinaryReaderEx reader)
        {
            // Basic Chunk Header.
            string chunkType = reader.ReadNullPaddedString(4);
            uint chunkSize = reader.ReadUInt32();

            uint matrixCount = reader.ReadUInt32(); // Not sure what this data is, but it looks to have a Matrix as part of it.

            for (int i = 0; i < matrixCount; i++)
            {
                INSTEntry1 entry = new();

                entry.UnknownMatrix4x4_1 = reader.Read4x4Matrix();
                entry.MatrixIndex = reader.ReadUInt32(); // TODO: Verify.
                entry.UnknownUInt32_1 = reader.ReadUInt32();
                entry.UnknownUInt32_2 = reader.ReadUInt32();
                reader.JumpAhead(0x4); // Always 0.

                UnknownDataStruct_1.Add(entry);
            }

            uint UnknownCount = reader.ReadUInt32(); // Count of whatever this is.
            for (int i = 0; i < UnknownCount; i++)
            {
                INSTEntry2 entry = new();

                reader.JumpAhead(0x40); // Literally always 0? Not sure what the hell this is about.
                entry.UnknownFloat_1 = reader.ReadSingle();
                entry.UnknownFloat_2 = reader.ReadSingle();
                entry.UnknownFloat_3 = reader.ReadSingle();
                reader.JumpAhead(0x4); // Always 1.
                entry.UnknownFloat_4 = reader.ReadSingle();
                reader.JumpAhead(0x8); // Always 0.
                entry.UnknownFloat_5 = reader.ReadSingle();

                UnknownDataStruct_2.Add(entry);
            }

            // Align to 0x4.
            reader.FixPadding();
        }
    }

    public class INSTEntry1
    {
        public Matrix4x4 UnknownMatrix4x4_1 { get; set; }

        public uint MatrixIndex { get; set; }

        public uint UnknownUInt32_1 { get; set; }

        public uint UnknownUInt32_2 { get; set; }
    }

    public class INSTEntry2
    {
        public float UnknownFloat_1 { get; set; }

        public float UnknownFloat_2 { get; set; }

        public float UnknownFloat_3 { get; set; }

        public float UnknownFloat_4 { get; set; }

        public float UnknownFloat_5 { get; set; }
    }
}
