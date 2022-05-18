namespace Knuxs_Misc_Tools.WrathOfCortex.HGO_Chunk
{
    public class SST
    {
        public List<SSTEntry> Read(BinaryReaderEx reader)
        {
            List<SSTEntry> entries = new();

            // Basic Chunk Header.
            string chunkType = reader.ReadNullPaddedString(4);
            uint chunkSize = reader.ReadUInt32();

            uint entryCount = reader.ReadUInt32();
            reader.JumpAhead(0x4); // Contains the size of this chunk minus 0x10 for some reason?

            for (int i = 0; i < entryCount; i++)
            {
                SSTEntry entry = new();

                uint VectorCount = reader.ReadUInt32();
                entry.UnknownUInt32_1 = reader.ReadUInt32();

                for (int v = 0; v < VectorCount; v++)
                    entry.UnknownVector3s.Add(reader.ReadVector3());

                entries.Add(entry);
            }

            // Align to 0x4.
            reader.FixPadding();

            return entries;
        }
    }

    public class SSTEntry
    {
        public uint UnknownUInt32_1 { get; set; }

        public List<Vector3> UnknownVector3s { get; set; } = new();
    }
}
