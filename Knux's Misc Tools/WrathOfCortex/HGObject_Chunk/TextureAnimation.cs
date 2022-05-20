namespace Knuxs_Misc_Tools.WrathOfCortex.HGObject_Chunk
{
    public class TextureAnimation
    {
        public List<Animation> Animations { get; set; } = new();

        public List<ushort> UnknownUInt16s { get; set; } = new();

        public void Read(BinaryReaderEx reader)
        {
            // Basic Chunk Header.
            string chunkType = reader.ReadNullPaddedString(4);
            uint chunkSize = reader.ReadUInt32();

            uint animationCount = reader.ReadUInt32();
            uint UnknownUInt32_1 = reader.ReadUInt32();
            
            for (int i = 0; i < animationCount; i++)
            {
                Animation animation = new()
                {
                    UnknownFloat_1 = reader.ReadSingle(),
                    UnknownUInt32_1 = reader.ReadUInt32(),
                    UnknownUInt32_2 = reader.ReadUInt32(),
                    UnknownUInt32_3 = reader.ReadUInt32(),
                    UnknownUInt32_4 = reader.ReadUInt32(),
                    UnknownUInt32_5 = reader.ReadUInt32(),
                    UnknownUInt32_6 = reader.ReadUInt32(),
                    UnknownUInt32_7 = reader.ReadUInt32()
                };
                Animations.Add(animation);
            }

            uint unknownCount = reader.ReadUInt32(); // Material Indicies?
            for (int i = 0; i < unknownCount; i++)
                UnknownUInt16s.Add(reader.ReadUInt16());

            // Align to 0x4.
            reader.FixPadding();
        }
    }

    public class Animation
    {
        public float UnknownFloat_1 { get; set; }

        public uint UnknownUInt32_1 { get; set; }

        public uint UnknownUInt32_2 { get; set; }

        public uint UnknownUInt32_3 { get; set; }

        public uint UnknownUInt32_4 { get; set; }

        public uint UnknownUInt32_5 { get; set; }

        public uint UnknownUInt32_6 { get; set; }

        public uint UnknownUInt32_7 { get; set; }
    }
}
