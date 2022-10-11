namespace Knuxs_Misc_Tools.Storybook
{
    internal class PartyCompressed : FileBase
    {
        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream, true);
            reader.JumpAhead(0x4); // Always 1.
            reader.IsBigEndian = false;
            uint fileCount = reader.ReadUInt32();
            reader.IsBigEndian = true;
            uint unknownUInt32_1 = reader.ReadUInt32(); // Seems to match the number in the file name if there is one?

            for (int i = 0; i < fileCount; i++)
            {
                uint fileOffset = reader.ReadUInt32();
                long pos = reader.BaseStream.Position;
                reader.JumpTo(fileOffset);
                string compressionIndicator = reader.ReadNullTerminatedString();
                reader.JumpAhead(0x2); // Always 40 00.
                uint unknownUInt32_2 = reader.ReadUInt32();
                uint unknownByte_1 = reader.ReadByte();
                // TODO: What the hell is this compression then? Are file names a thing in it?

                reader.JumpTo(pos);
            }

        }
    }
}
