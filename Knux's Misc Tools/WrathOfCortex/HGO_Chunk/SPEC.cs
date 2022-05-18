namespace Knuxs_Misc_Tools.WrathOfCortex.HGO_Chunk
{
    public class SPEC
    {
        public List<SPECEntry> Read(BinaryReaderEx reader)
        {
            List<SPECEntry> entries = new();

            // Basic Chunk Header.
            string chunkType = reader.ReadNullPaddedString(4);
            uint chunkSize = reader.ReadUInt32();

            uint entryCount = reader.ReadUInt32();

            for (int i = 0; i < entryCount; i++)
            {
                SPECEntry entry = new()
                {
                    UnknownMatrix4x4_1 = reader.Read4x4Matrix(),
                    UnknownUInt32_1 = reader.ReadUInt32(),
                    UnknownUInt32_2 = reader.ReadUInt32(),
                    UnknownUInt32_3 = reader.ReadUInt32(),
                    UnknownUInt32_4 = reader.ReadUInt32()
                };
                entries.Add(entry);
            }

            // Align to 0x4.
            reader.FixPadding();

            return entries; 
        }
    }

    public class SPECEntry
    {
        public Matrix4x4 UnknownMatrix4x4_1 { get; set; }

        public uint UnknownUInt32_1 { get; set; }

        public uint UnknownUInt32_2 { get; set; }

        public uint UnknownUInt32_3 { get; set; }

        public uint UnknownUInt32_4 { get; set; }
    }
}
