namespace Knuxs_Misc_Tools.WrathOfCortex.HGO_Chunk
{
    public class NameTable
    {
        public List<string> Read(BinaryReaderEx reader)
        {
            List<string> Names = new();

            // Basic Chunk Header.
            string chunkType = reader.ReadNullPaddedString(4);
            uint chunkSize = reader.ReadUInt32();

            // Calculate where the string table ends.
            long tableEnd = reader.ReadUInt32() + reader.BaseStream.Position;

            // As long as we haven't reached the end of the table, add an entry to the list.
            while (reader.BaseStream.Position < tableEnd)
                Names.Add(reader.ReadNullTerminatedString());

            // Align to 0x4.
            reader.FixPadding();

            return Names;
        }

        public void Write(BinaryWriterEx writer, List<string> Names)
        {
            // Chunk Identifier.
            writer.Write("LBTN");

            // Save the position we'll need to write the chunk's size to and add a dummy value in its place.
            long chunkSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");

            // Save the position we'll need to write the node table's length to and add a dummy value in its place.
            long nameTableLengthPos = writer.BaseStream.Position;
            writer.Write("SIZE");

            // Write all the names.
            for (int i = 0; i < Names.Count; i++)
                writer.WriteNullTerminatedString(Names[i]);

            // Calculate the name table length.
            uint nameTableLength = (uint)(writer.BaseStream.Position - (nameTableLengthPos + 0x4));

            // Align to 0x4.
            writer.FixPadding();

            // Calculate the chunk size.
            uint chunkSize = (uint)(writer.BaseStream.Position - (chunkSizePos - 0x4));

            // Save our current position.
            long pos = writer.BaseStream.Position;

            // Fill in the chunk size.
            writer.BaseStream.Position = chunkSizePos;
            writer.Write(chunkSize);

            // Fill in the node table length.
            writer.BaseStream.Position = nameTableLengthPos;
            writer.Write(nameTableLength);

            // Jump to our saved position so we can continue.
            writer.BaseStream.Position = pos;
        }
    }
}
