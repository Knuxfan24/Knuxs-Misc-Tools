namespace Knuxs_Misc_Tools.Storybook
{
    public enum FormatType : ulong
    {
        SecretRings = 14757395258967641292,
        BlackKnight = 0
    }

    public class BlockEntry
    {
        public uint UnknownUInt32_1 { get; set; } // TODO: Does this do anything? Or is it just an old index value? Not always linear though. Priority maybe? Doesn't seem right.

        public Vector3 UnknownVector3_1 { get; set; } // Position?

        public Vector3 UnknownVector3_2 { get; set; } // TODO: Always 0, except for ONE file in Black Knight (stg221), which has the Y value be nonsensical? Could this be a BAMs rotation????

        public Vector3 UnknownVector3_3 { get; set; } // Some sort of inner boundry?

        public float UnknownFloat_1 { get; set; } // Radius???

        public uint? UnknownUInt32_2 { get; set; } = null; // Seems to only exist in Black Knight?

        public byte[] SectorIndices { get; set; } = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }

    public class VisibilityTable : FileBase
    {
        public List<BlockEntry> Blocks = new();

        public void Load(string filepath, FormatType? type = null)
        {
            BinaryReaderEx reader = new(File.OpenRead(filepath), true);

            reader.ReadSignature(4, "LDBK");
            uint blockCount = reader.ReadUInt32();

            // Get the game's type if it wasn't specified.
            // This will go wrong on a couple of Black Knight's test BLKs that have the Secret Rings value of 8 0xCC bytes here but oh well.
            if (type == null)
                type = (FormatType?)reader.ReadUInt64();
            else
                reader.JumpAhead(0x8);

            for (int i = 0; i < blockCount; i++)
            {
                BlockEntry entry = new()
                {
                    UnknownUInt32_1 = reader.ReadUInt32(),
                    UnknownVector3_1 = reader.ReadVector3(),
                    UnknownVector3_2 = new(reader.ReadInt32() * 360.0f / 65535.0f, reader.ReadInt32() * 360.0f / 65535.0f, reader.ReadInt32() * 360.0f / 65535.0f),
                    UnknownVector3_3 = reader.ReadVector3(),
                    UnknownFloat_1 = reader.ReadSingle()
                };

                reader.JumpAhead(0x4); // Always a float of -10.

                // This value only exists in a Black Knight format.
                if (type == FormatType.BlackKnight)
                    entry.UnknownUInt32_2 = reader.ReadUInt32();

                entry.SectorIndices = reader.ReadBytes(0x10);

                Blocks.Add(entry);
            }

            reader.Close();
        }

        public void Save(string filepath, FormatType type)
        {
            BinaryWriterEx writer = new(File.OpenWrite(filepath), true);

            writer.Write("LDBK");
            writer.Write(Blocks.Count);
            writer.Write((ulong)type);

            foreach (var entry in Blocks)
            {
                writer.Write(entry.UnknownUInt32_1);
                writer.Write(entry.UnknownVector3_1);
                writer.Write(entry.UnknownVector3_2);
                writer.Write(entry.UnknownVector3_3);
                writer.Write(entry.UnknownFloat_1);
                writer.Write(-10f);

                if (type == FormatType.BlackKnight)
                    writer.Write((uint)entry.UnknownUInt32_2);

                writer.Write(entry.SectorIndices);
            }

            // Close the writer.
            writer.Flush();
            writer.Close();
        }
    }
}
