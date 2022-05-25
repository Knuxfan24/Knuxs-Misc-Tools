namespace Knuxs_Misc_Tools.WrathOfCortex.HGObject_Chunk
{
    public class Spline
    {
        public List<SplineEntry> Read(BinaryReaderEx reader)
        {
            List<SplineEntry> entries = new();

            // Basic Chunk Header.
            string chunkType = reader.ReadNullPaddedString(4);
            uint chunkSize = reader.ReadUInt32();

            uint entryCount = reader.ReadUInt32();
            reader.JumpAhead(0x4); // Contains the size of this chunk minus 0x10 for some reason?

            for (int i = 0; i < entryCount; i++)
            {
                SplineEntry entry = new();

                uint pointCount = reader.ReadUInt32();
                entry.UnknownUInt32_1 = reader.ReadUInt32();

                for (int v = 0; v < pointCount; v++)
                    entry.SplinePoints.Add(reader.ReadVector3());

                entries.Add(entry);
            }

            // Align to 0x4.
            reader.FixPadding();

            return entries;
        }

        public void Write(BinaryWriterEx writer, List<SplineEntry> splineEntries)
        {
            // Chunk Identifier.
            writer.Write("0TSS");

            // Save the position we'll need to write the chunk's size to and add a dummy value in its place.
            long chunkSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");

            // Write the amount of spline entries in this file.
            writer.Write(splineEntries.Count);

            // Save the position we'll need to write that weird other size value.
            long lengthPos = writer.BaseStream.Position;
            writer.Write("SIZE");

            // Write the spline entries.
            for (int i = 0; i < splineEntries.Count; i++)
            {
                writer.Write(splineEntries[i].SplinePoints.Count);
                writer.Write(splineEntries[i].UnknownUInt32_1);

                foreach (Vector3 value in splineEntries[i].SplinePoints)
                    writer.Write(value);
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

            // Fill in that weird length value thing.
            writer.BaseStream.Position = lengthPos;
            writer.Write(chunkSize - 0x10);

            // Jump to our saved position so we can continue.
            writer.BaseStream.Position = pos;
        }
    }

    public class SplineEntry
    {
        public uint UnknownUInt32_1 { get; set; } // Type?

        public List<Vector3> SplinePoints { get; set; } = new();
    }
}
