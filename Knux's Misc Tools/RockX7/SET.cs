using Marathon.IO;
using System.Numerics;

namespace Knuxs_Misc_Tools.RockX7
{
    public class SET : FileBase
    {
        public class SetObject
        {
            public uint UnknownUInt32_1 { get; set; }

            public uint ObjectType { get; set; }

            public uint Behaviour { get; set; }

            public uint UnknownUInt32_2 { get; set; }

            public float UnknownFloat_1 { get; set; }

            public float UnknownFloat_2 { get; set; }

            public float UnknownFloat_3 { get; set; }

            public uint UnknownUInt32_3 { get; set; }

            public uint UnknownUInt32_4 { get; set; }

            public float UnknownFloat_4 { get; set; }

            public uint UnknownUInt32_5 { get; set; }

            public float UnknownFloat_5 { get; set; }

            public float UnknownFloat_6 { get; set; }

            public float UnknownFloat_7 { get; set; }

            public float UnknownFloat_8 { get; set; }

            public float UnknownFloat_9 { get; set; }

            public float UnknownFloat_10 { get; set; }

            public float UnknownFloat_11 { get; set; }

            public float UnknownFloat_12 { get; set; }

            public float UnknownFloat_13 { get; set; }

            public float UnknownFloat_14 { get; set; }

            public float UnknownFloat_15 { get; set; }

            public uint UnknownUInt32_6 { get; set; }

            public Vector3 Position { get; set; }

            public float UnknownFloat_16 { get; set; }
        }

        public List<SetObject> Objects = new();

        public override void Load(string filepath)
        {
            BinaryReaderEx reader = new(File.OpenRead(filepath));
            uint objectCount = reader.ReadUInt32();

            for (int i = 0; i < objectCount - 1; i++)
            {
                SetObject obj = new()
                {
                    UnknownUInt32_1 = reader.ReadUInt32(),
                    ObjectType = reader.ReadUInt32(),
                    Behaviour = reader.ReadUInt32(),
                    UnknownUInt32_2 = reader.ReadUInt32(),
                    UnknownFloat_1 = reader.ReadSingle(),
                    UnknownFloat_2 = reader.ReadSingle(),
                    UnknownFloat_3 = reader.ReadSingle(),
                    UnknownUInt32_3 = reader.ReadUInt32(),
                    UnknownUInt32_4 = reader.ReadUInt32(),
                    UnknownFloat_4 = reader.ReadSingle(),
                    UnknownUInt32_5 = reader.ReadUInt32(),
                    UnknownFloat_5 = reader.ReadSingle(),
                    UnknownFloat_6 = reader.ReadSingle(),
                    UnknownFloat_7 = reader.ReadSingle(),
                    UnknownFloat_8 = reader.ReadSingle(),
                    UnknownFloat_9 = reader.ReadSingle(),
                    UnknownFloat_10 = reader.ReadSingle(),
                    UnknownFloat_11 = reader.ReadSingle(),
                    UnknownFloat_12 = reader.ReadSingle(),
                    UnknownFloat_13 = reader.ReadSingle(),
                    UnknownFloat_14 = reader.ReadSingle(),
                    UnknownFloat_15 = reader.ReadSingle(),
                    UnknownUInt32_6 = reader.ReadUInt32(),
                    Position = reader.ReadVector3(),
                    UnknownFloat_16 = reader.ReadSingle()
                };
                Objects.Add(obj);
            }
        }
    }
}
