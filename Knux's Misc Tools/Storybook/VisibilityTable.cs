namespace Knuxs_Misc_Tools.Storybook
{
    public class BlockEntry
    {
        public uint UnknownUInt32_1 { get; set; } // Thought this was like block index or something, apparently not???
        public Vector3 UnknownVector3_1 { get; set; }
        public Vector3 UnknownVector3_2 { get; set; }
        public Vector3 UnknownVector3_3 { get; set; }
        public float UnknownFloat_1 { get; set; }
        public float UnknownFloat_2 { get; set; }
        public uint? UnknownUInt32_2 { get; set; } = null; // Seems to only exist in Black Knight?
        public byte[]? SectorIndices { get; set; }

    }

    public class VisibilityTable : FileBase
    {
        public List<BlockEntry> Blocks = new();

        public void Load(string filepath, bool isBlackKnight = true)
        {
            BinaryReaderEx reader = new(File.OpenRead(filepath), true);

            reader.ReadSignature(4, "LDBK");
            uint blockCount = reader.ReadUInt32();
            reader.JumpAhead(0x8); // TODO: I assume this is padding? Seems to end up as either all 0xCC or 0x00?

            for (int i = 0; i < blockCount; i++)
            {
                BlockEntry entry = new()
                {
                    UnknownUInt32_1 = reader.ReadUInt32(),
                    UnknownVector3_1 = reader.ReadVector3(),
                    UnknownVector3_2 = reader.ReadVector3(),
                    UnknownVector3_3 = reader.ReadVector3(),
                    UnknownFloat_1 = reader.ReadSingle(),
                    UnknownFloat_2 = reader.ReadSingle()
                };
                if (isBlackKnight)
                    entry.UnknownUInt32_2 = reader.ReadUInt32();

                entry.SectorIndices = reader.ReadBytes(0x10);

                Blocks.Add(entry);
            }

            reader.Close();
        }

        public void Save(string filepath, bool isBlackKnight = true)
        {
            BinaryWriterEx writer = new(File.OpenWrite(filepath), true);

            writer.Write("LDBK");
            writer.Write(Blocks.Count);
            writer.WriteNulls(0x8);

            foreach (var entry in Blocks)
            {
                writer.Write(entry.UnknownUInt32_1);
                writer.Write(entry.UnknownVector3_1);
                writer.Write(entry.UnknownVector3_2);
                writer.Write(entry.UnknownVector3_3);
                writer.Write(entry.UnknownFloat_1);
                writer.Write(entry.UnknownFloat_2);

                if (isBlackKnight)
                    writer.Write((uint)entry.UnknownUInt32_2);

                writer.Write(entry.SectorIndices);
            }

            // Close the writer.
            writer.Flush();
            writer.Close();
        }
    }
}
