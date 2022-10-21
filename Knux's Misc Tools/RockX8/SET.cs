using System.Xml.Linq;

namespace Knuxs_Misc_Tools.RockX8
{
    internal class SET : FileBase
    {
        public class SetObject
        {
            // TODO: Are these always 0?
            public uint UnknownUInt32_1 { get; set; }
            public uint UnknownUInt32_2 { get; set; }
            public uint UnknownUInt32_3 { get; set; }
            public uint UnknownUInt32_4 { get; set; }

            public Vector3 Position { get; set; }

            // TODO: Verify.
            public uint XRotation { get; set; }
            public uint YRotation { get; set; }
            public uint ZRotation { get; set; }

            public string Type { get; set; } // Seemingly always Prm[xxxx]?

            public byte[] Data { get; set; } // TODO: Actually fuck around with this and see if it's like parameter data.

            public override string ToString() => Type;
        }

        public List<SetObject> Objects = new();

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream);
            reader.ReadSignature(3, "OSE");
            reader.FixPadding();

            uint UnknownUInt32_1 = reader.ReadUInt32(); // TODO: Always 1?
            uint DataSize = reader.ReadUInt32(); // Name might not be accurate? Seems to be the file size minus 0x0C.
            uint ObjectCount = reader.ReadUInt32();

            // TODO: Are these always 0?
            uint UnknownUInt32_2 = reader.ReadUInt32(); 
            uint UnknownUInt32_3 = reader.ReadUInt32(); 
            uint UnknownUInt32_4 = reader.ReadUInt32(); 
            uint UnknownUInt32_5 = reader.ReadUInt32();

            uint UnknownUInt32_6 = reader.ReadUInt32();
            uint UnknownUInt32_7 = reader.ReadUInt32();
            uint UnknownUInt32_8 = reader.ReadUInt32();
            uint UnknownUInt32_9 = reader.ReadUInt32();

            uint UnknownUInt32_10 = reader.ReadUInt32();
            uint UnknownUInt32_11 = reader.ReadUInt32();
            uint UnknownUInt32_12 = reader.ReadUInt32();
            uint UnknownUInt32_13 = reader.ReadUInt32();

            uint UnknownUInt32_14 = reader.ReadUInt32();
            uint UnknownUInt32_15 = reader.ReadUInt32();
            uint UnknownUInt32_16 = reader.ReadUInt32();

            for (int i = 0; i < ObjectCount; i++)
            {
                SetObject obj = new()
                {
                    UnknownUInt32_1 = reader.ReadUInt32(),
                    UnknownUInt32_2 = reader.ReadUInt32(),
                    UnknownUInt32_3 = reader.ReadUInt32(),
                    UnknownUInt32_4 = reader.ReadUInt32(),
                    Position = reader.ReadVector3(),
                    XRotation = reader.ReadUInt32(),
                    YRotation = reader.ReadUInt32(),
                    ZRotation = reader.ReadUInt32(),
                    Type = reader.ReadNullTerminatedString(),
                    Data = reader.ReadBytes(0x20)
                };
                Objects.Add(obj);
            }
        }

        public override void Save(Stream stream)
        {
            BinaryWriterEx writer = new(stream);
            writer.Write("OSE");
            writer.FixPadding();
            writer.Write(1u);

            long DataSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");

            writer.Write(Objects.Count);

            writer.WriteNulls(0x3C);

            foreach (SetObject obj in Objects)
            {
                writer.Write(obj.UnknownUInt32_1);
                writer.Write(obj.UnknownUInt32_2);
                writer.Write(obj.UnknownUInt32_3);
                writer.Write(obj.UnknownUInt32_4);
                writer.Write(obj.Position);
                writer.Write(obj.XRotation);
                writer.Write(obj.YRotation);
                writer.Write(obj.ZRotation);
                writer.WriteNullTerminatedString(obj.Type);
                writer.Write(obj.Data);
            }

            writer.BaseStream.Position = DataSizePos;
            writer.Write((uint)(writer.BaseStream.Length - 0x0C));
        }
    }
}
