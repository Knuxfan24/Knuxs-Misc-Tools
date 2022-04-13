using Marathon.IO;
using System.Numerics;

namespace Knuxs_Misc_Tools.WrathOfCortex.HGO
{
    public class HGO_Material
    {
        public uint UnknownUInt32_1 { get; set; }

        public uint UnknownUInt32_2 { get; set; }

        public uint UnknownUInt32_3 { get; set; }

        public uint UnknownUInt32_4 { get; set; }

        public uint UnknownUInt32_5 { get; set; }

        public Vector3 Colours { get; set; }

        public float UnknownFloat_1 { get; set; }

        public float UnknownFloat_2 { get; set; }

        public uint UnknownUInt32_6 { get; set; }

        public uint UnknownUInt32_7 { get; set; }

        public float UnknownFloat_3 { get; set; }

        public float UnknownFloat_4 { get; set; }

        public uint BitmapIndex { get; set; }

        public float UnknownFloat_5 { get; set; }

        public float UnknownFloat_6 { get; set; }

        public float UnknownFloat_7 { get; set; }

        public float UnknownFloat_8 { get; set; }

        public float UnknownFloat_9 { get; set; }

        public float UnknownFloat_10 { get; set; }

        public void ReadMaterial(BinaryReaderEx reader)
        {
            UnknownUInt32_1 = reader.ReadUInt32();
            UnknownUInt32_2 = reader.ReadUInt32();
            UnknownUInt32_3 = reader.ReadUInt32();
            UnknownUInt32_4 = reader.ReadUInt32();
            UnknownUInt32_5 = reader.ReadUInt32();
            Colours = reader.ReadVector3();
            UnknownFloat_1 = reader.ReadSingle();
            UnknownFloat_2 = reader.ReadSingle();
            UnknownUInt32_6 = reader.ReadUInt32();
            UnknownUInt32_7 = reader.ReadUInt32();
            UnknownFloat_3 = reader.ReadSingle();
            UnknownFloat_4 = reader.ReadSingle();
            BitmapIndex = reader.ReadUInt32();
            UnknownFloat_5 = reader.ReadSingle();
            UnknownFloat_6 = reader.ReadSingle();
            UnknownFloat_7 = reader.ReadSingle();
            UnknownFloat_8 = reader.ReadSingle();
            UnknownFloat_9 = reader.ReadSingle();
            UnknownFloat_10 = reader.ReadSingle();
        }
    }
}
