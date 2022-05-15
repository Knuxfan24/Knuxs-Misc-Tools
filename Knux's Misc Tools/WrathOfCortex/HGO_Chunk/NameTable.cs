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
    }
}
