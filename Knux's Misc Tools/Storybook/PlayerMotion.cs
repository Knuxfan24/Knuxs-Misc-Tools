namespace Knuxs_Misc_Tools.Storybook
{
    public class MotionEntry
    {
        public string? Name { get; set; }

        public uint UnknownUInt32_1 { get; set; }

        public uint UnknownUInt32_2 { get; set; }

        public float UnknownFloat_1 { get; set; }

        public float UnknownFloat_2 { get; set; }

        public uint UnknownUInt32_3 { get; set; }

        public float UnknownFloat_3 { get; set; }

        public float UnknownFloat_4 { get; set; }

        public override string ToString() => Name;
    }

    public class PlayerMotion : FileBase
    {
        public List<MotionEntry> Motions = new();

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream);

            uint fileSize = reader.ReadUInt32();
            uint animationCount = reader.ReadUInt32();
            uint dataOffset = reader.ReadUInt32(); // ? Think this is always 0x10?
            uint stringTableOffset = reader.ReadUInt32();
            
            for (int i = 0; i < animationCount; i++)
            {
                MotionEntry entry = new();
                entry.UnknownUInt32_1 = reader.ReadUInt32();
                uint positionalStringTableOffset = reader.ReadUInt32(); // TODO: Verify.
                reader.JumpAhead(0x4); // Always 0xFFFFFFFF
                entry.UnknownUInt32_2 = reader.ReadUInt32();
                entry.UnknownFloat_1 = reader.ReadSingle();
                entry.UnknownFloat_2 = reader.ReadSingle();
                entry.UnknownUInt32_3 = reader.ReadUInt32();
                entry.UnknownFloat_3 = reader.ReadSingle();
                entry.UnknownFloat_4 = reader.ReadSingle();
                long position = reader.BaseStream.Position;
                reader.JumpTo(stringTableOffset + positionalStringTableOffset);
                entry.Name = reader.ReadNullTerminatedString();
                reader.JumpTo(position);
                Motions.Add(entry);
            }
        }
    }
}
