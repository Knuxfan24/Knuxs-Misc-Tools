namespace Knuxs_Misc_Tools.SWA
{
    public class ArcInfo : FileBase
    {
        public List<ArchiveEntry> entries = new();

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream, true);

            uint fileSize = reader.ReadUInt32();
            uint unknownUInt32_1 = reader.ReadUInt32();
            uint stringTableEnd = reader.ReadUInt32();
            uint offsetSize = reader.ReadUInt32();

            reader.Offset = offsetSize;

            uint UnknownUInt32_2 = reader.ReadUInt32(); // End of the string table but with the 0x18 addition already applied???
            uint UnknownUInt32_3 = reader.ReadUInt32();
            uint archiveCount = reader.ReadUInt32();
            uint UnknownUInt32_4 = reader.ReadUInt32();
            uint unknownOffset = reader.ReadUInt32(); // Offset to that table of 3s?

            for (int i = 0; i < archiveCount; i++)
            {
                // Read the offset to this archive.
                uint stringOffset = reader.ReadUInt32();

                // Save our current position so we can jump back afterwards.
                long pos = reader.BaseStream.Position;

                // Jump to the offset in the string table.
                reader.JumpTo(stringOffset, true);

                // Read the string into a new archive entry and save it.
                ArchiveEntry entry = new() { Archive = reader.ReadNullTerminatedString() };
                entries.Add(entry);

                // Jump back to where we were.
                reader.JumpTo(pos);
            }

            // Loop through the weird byte table thing.
            for (int i = 0; i < archiveCount; i++)
                entries[i].UnknownByte_1 = reader.ReadByte();

            // Jump past the string table.
            reader.JumpTo(stringTableEnd, true);

            // Rest of the file seems to be a list of numbers incrementing by 0x4 every time? And a count of them?
            uint offsetCount = reader.ReadUInt32(); // Might be archiveCount + 2?
        }

        public override void Save(Stream stream)
        {
            BinaryWriterEx writer = new(stream, true) { Offset = 0x18 };

            writer.Write("SIZE"); // Placeholder to fill in later.
            writer.Write(0x1);
            writer.Write("SEND"); // Placeholder to fill in later.
            writer.Write(0x18);
            writer.Write("SEND"); // Placeholder to fill in later.
            writer.Write(0x0);
            writer.Write(entries.Count);
            writer.Write(0xC);
            writer.AddOffset("ByteTable");

            // Add the offsets for the string table.
            for (int i = 0; i < entries.Count; i++)
                writer.AddOffset($"Entry{i}Archive");

            // Fill in the offset for this byte table.
            writer.FillOffset("ByteTable", true);

            // Fill in the byte table.
            for (int i = 0; i < entries.Count; i++)
                writer.Write(entries[i].UnknownByte_1);

            // Align to 0x4.
            writer.FixPadding(0x4);

            // Fill in the archive names.
            for (int i = 0; i < entries.Count; i++)
            {
                writer.FillOffset($"Entry{i}Archive", true);
                writer.WriteNullTerminatedString(entries[i].Archive);
            }

            // Align to 0x4.
            writer.FixPadding(0x4);

            // Save the position to the end offset table.
            uint offsetTablePosition = (uint)writer.BaseStream.Position;

            // Write this weird offset table thing. // TODO: Is this right?
            writer.Write(entries.Count + 0x2);

            // Write this weird offset table.
            for (int i = 0; i < entries.Count + 0x2; i++)
                writer.Write(0x4 * (i + 1));

            // Write the file size.
            writer.BaseStream.Position = 0x0;
            writer.Write((uint)writer.BaseStream.Length);

            // Write the offset table position shit.
            writer.BaseStream.Position = 0x8;
            writer.Write(offsetTablePosition - 0x18);
            writer.BaseStream.Position = 0x10;
            writer.Write(offsetTablePosition);
        }
    }

    public class ArchiveEntry
    {
        public string Archive { get; set; }

        public byte UnknownByte_1 { get; set; }
    }
}
