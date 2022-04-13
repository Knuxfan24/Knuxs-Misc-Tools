using Marathon.IO;

namespace Knuxs_Misc_Tools.WrathOfCortex.HGO
{
    internal class HGO_Animation
    {
        public uint UnknownUInt32_1 { get; set; }

        public uint UnknownUInt32_2 { get; set; }

        public uint UnknownUInt32_3 { get; set; }

        public uint UnknownUInt32_4 { get; set; }

        public uint UnknownUInt32_5 { get; set; }

        public uint UnknownUInt32_6 { get; set; }

        public uint UnknownUInt32_7 { get; set; }

        public uint UnknownUInt32_8 { get; set; }

        public void ReadAnimation(BinaryReaderEx reader)
        {
            UnknownUInt32_1 = reader.ReadUInt32();
            UnknownUInt32_2 = reader.ReadUInt32();
            UnknownUInt32_3 = reader.ReadUInt32();
            UnknownUInt32_4 = reader.ReadUInt32();
            UnknownUInt32_5 = reader.ReadUInt32();
            UnknownUInt32_6 = reader.ReadUInt32();
            UnknownUInt32_7 = reader.ReadUInt32();
            UnknownUInt32_8 = reader.ReadUInt32();
        }
    }
}
