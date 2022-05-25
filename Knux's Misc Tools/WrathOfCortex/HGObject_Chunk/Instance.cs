namespace Knuxs_Misc_Tools.WrathOfCortex.HGObject_Chunk
{
    // TODO: What is the second struct?
    public class Instance
    {
        public List<InstanceEntry> Instances { get; set; } = new();

        public List<INSTEntry2> UnknownDataStruct_1 { get; set; } = new();

        public void Read(BinaryReaderEx reader)
        {
            // Basic Chunk Header.
            string chunkType = reader.ReadNullPaddedString(4);
            uint chunkSize = reader.ReadUInt32();

            uint matrixCount = reader.ReadUInt32(); // Not sure what this data is, but it looks to have a Matrix as part of it.

            for (int i = 0; i < matrixCount; i++)
            {
                InstanceEntry entry = new();

                entry.UnknownMatrix4x4_1 = reader.Read4x4Matrix();
                entry.ModelIndex = reader.ReadUInt32();
                entry.UnknownUInt32_1 = reader.ReadUInt32();
                entry.UnknownUInt32_2 = reader.ReadUInt32();
                reader.JumpAhead(0x4); // Always 0.

                Instances.Add(entry);
            }

            uint UnknownCount = reader.ReadUInt32(); // Count of whatever this is.
            for (int i = 0; i < UnknownCount; i++)
            {
                INSTEntry2 entry = new();

                reader.JumpAhead(0x40); // Literally always 0? Not sure what the hell this is about.
                entry.UnknownFloat_1 = reader.ReadSingle();
                entry.UnknownFloat_2 = reader.ReadSingle();
                entry.UnknownFloat_3 = reader.ReadSingle();
                reader.JumpAhead(0x4); // Always 1 as a floating point number.
                entry.UnknownFloat_4 = reader.ReadSingle();
                reader.JumpAhead(0x8); // Always 0.
                entry.UnknownFloat_5 = reader.ReadSingle();

                UnknownDataStruct_1.Add(entry);
            }

            // Align to 0x4.
            reader.FixPadding();
        }

        public void Write(BinaryWriterEx writer)
        {
            // Chunk Identifier.
            writer.Write("TSNI");

            // Save the position we'll need to write the chunk's size to and add a dummy value in its place.
            long chunkSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");

            // Write how many of the first entry type there is in this file.
            writer.Write(Instances.Count);

            // Write all the first entry types.
            for (int i = 0; i < Instances.Count; i++)
            {
                writer.Write(Instances[i].UnknownMatrix4x4_1);
                writer.Write(Instances[i].ModelIndex);
                writer.Write(Instances[i].UnknownUInt32_1);
                writer.Write(Instances[i].UnknownUInt32_2);
                writer.WriteNulls(0x4);
            }

            // Write how many of the second entry type there is in this file.
            writer.Write(UnknownDataStruct_1.Count);

            // Write all the second entry types.
            for (int i = 0; i < UnknownDataStruct_1.Count; i++)
            {
                writer.WriteNulls(0x40);
                writer.Write(UnknownDataStruct_1[i].UnknownFloat_1);
                writer.Write(UnknownDataStruct_1[i].UnknownFloat_2);
                writer.Write(UnknownDataStruct_1[i].UnknownFloat_3);
                writer.Write((float)1);
                writer.Write(UnknownDataStruct_1[i].UnknownFloat_4);
                writer.WriteNulls(0x8);
                writer.Write(UnknownDataStruct_1[i].UnknownFloat_5);
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

    public class InstanceEntry
    {
        public Matrix4x4 UnknownMatrix4x4_1 { get; set; }

        public uint ModelIndex { get; set; }

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
