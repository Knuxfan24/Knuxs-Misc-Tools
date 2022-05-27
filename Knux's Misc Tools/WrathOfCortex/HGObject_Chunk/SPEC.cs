namespace Knuxs_Misc_Tools.WrathOfCortex.HGObject_Chunk
{
    public class SPEC
    {
        public List<SPECEntry> Read(BinaryReaderEx reader)
        {
            List<SPECEntry> entries = new();
            List<uint> nameOffsets = new();

            // Basic Chunk Header.
            string chunkType = reader.ReadNullPaddedString(4);
            uint chunkSize = reader.ReadUInt32();

            uint entryCount = reader.ReadUInt32();

            for (int i = 0; i < entryCount; i++)
            {
                SPECEntry entry = new();
                entry.UnknownMatrix4x4_1 = reader.Read4x4Matrix();
                entry.ModelIndex = reader.ReadUInt32();
                nameOffsets.Add(reader.ReadUInt32());
                entry.UnknownUInt32_2 = reader.ReadUInt32();
                entry.UnknownUInt32_3 = reader.ReadUInt32();
                entries.Add(entry);
            }

            // Align to 0x4.
            reader.FixPadding();

            // Dumb shit to try and figure out names.

            // Save our current position.
            long pos = reader.BaseStream.Position;

            // Jump to the first non header chunk.
            reader.JumpTo(0x8);

            // If it's not the name table, then jump ahead by the specified amount.
            while (reader.ReadNullPaddedString(4) != "LBTN")
                reader.JumpAhead(reader.ReadUInt32() - 0x8);

            // Jump to the start of the name table.
            reader.JumpAhead(0x8);

            // Save the position.
            long nameTablePosition = reader.BaseStream.Position;

            for (int i = 0; i < entries.Count; i++)
            {
                reader.JumpAhead(nameOffsets[i]);
                entries[i].Name = reader.ReadNullTerminatedString();
                reader.JumpTo(nameTablePosition);
            }

            reader.JumpTo(pos);

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
                writer.Write(specEntries[i].ModelIndex);
                writer.Write(0); // TODO: How do I figure out the name table offset writing now?
                writer.Write(specEntries[i].UnknownUInt32_2);
                writer.Write(specEntries[i].UnknownUInt32_3);
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

        public uint ModelIndex { get; set; }

        public string Name { get; set; }

        public uint UnknownUInt32_2 { get; set; }

        public uint UnknownUInt32_3 { get; set; }
    }
}
