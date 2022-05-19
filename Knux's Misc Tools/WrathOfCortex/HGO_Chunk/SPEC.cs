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

        public void Write(BinaryWriterEx writer, List<SPECEntry> specEntries)
        {
            // Chunk Identifier.
            writer.Write("CEPS");

            // Save the position we'll need to write the chunk's size to and add a dummy value in its place.
            long chunkSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");

            // Write the amount of spec entries in this file.
            writer.Write(specEntries.Count);

            // Write the spec entries.
            for (int i = 0; i < specEntries.Count; i++)
            {
                writer.Write(specEntries[i].UnknownMatrix4x4_1);
                writer.Write(specEntries[i].UnknownUInt32_1);
                writer.Write(specEntries[i].UnknownUInt32_2);
                writer.Write(specEntries[i].UnknownUInt32_3);
                writer.Write(specEntries[i].UnknownUInt32_4);
            }

            // Align to 0x4.
            writer.FixPadding(0x4);

            // Calculate the chunk size.
            uint chunkSize = (uint)(writer.BaseStream.Position - (chunkSizePos - 0x4));

            // Save our current position.
            long pos = writer.BaseStream.Position;

            // Fill in the chunk size.
            writer.BaseStream.Position = chunkSizePos;
            writer.Write(chunkSize);

            // Jump to our saved position so we can continue.
            writer.BaseStream.Position = pos;
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
