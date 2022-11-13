namespace Knuxs_Misc_Tools.SonicRangers
{
    internal class BulletInstance : FileBase
    {
        public class Instance
        {
            public string Name1 { get; set; } = "";

            public string Name2 { get; set; } = "";

            public Vector3 Position { get; set; }

            public Vector3 Rotation { get; set; }

            public uint UnknownUInt32_1 = 1;

            public Vector3 Scale { get; set; }

            public uint UnknownUInt32_2 = 0;
        }

        public List<Instance> Instances = new List<Instance>();

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream)
            {
                Offset = 0x40
            };

            // Skip the BINA header.
            reader.JumpTo(0x40);

            reader.ReadSignature(4, "CPIC");
            uint version = reader.ReadUInt32();
            long instanceOffset = reader.ReadInt64();
            ulong instanceCount = reader.ReadUInt64();

            reader.JumpTo(instanceOffset, false);
            for (ulong i = 0; i < instanceCount; i++)
            {
                Instance inst = new();

                long name1Offset = reader.ReadInt64();
                long name2Offset = reader.ReadInt64();

                inst.Position = reader.ReadVector3();
                inst.Rotation = reader.ReadVector3();
                inst.UnknownUInt32_1 = reader.ReadUInt32();
                inst.Scale = reader.ReadVector3();
                inst.UnknownUInt32_2 = reader.ReadUInt32();

                reader.FixPadding(0x8);

                long pos = reader.BaseStream.Position;

                reader.JumpTo(name1Offset, false);
                inst.Name1 = reader.ReadNullTerminatedString();

                reader.JumpTo(name2Offset, false);
                inst.Name2 = reader.ReadNullTerminatedString();

                reader.JumpTo(pos);

                Instances.Add(inst);
            }
        }
    }
}
