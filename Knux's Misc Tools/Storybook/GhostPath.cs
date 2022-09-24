namespace Knuxs_Misc_Tools.Storybook
{
    internal class GhostPath : FileBase
    {
        public class Point
        {
            public Vector3 Position { get; set; }

            public float UnknownFloat_1 { get; set; } // Something to do with the Distance value maybe?
        }

        public class FormatData
        {
            public uint UnknownUInt32_1 { get; set; }

            public uint UnknownUInt32_2 { get; set; }

            public List<Point> Points { get; set; } = new();
        }

        public FormatData Data = new();

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream, true);

            Data.UnknownUInt32_1 = reader.ReadUInt32();
            Data.UnknownUInt32_2 = reader.ReadUInt32();
            uint PointCount = reader.ReadUInt32();
            reader.JumpAhead(0x24); // Always 00 00 04 38 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01.

            for (int i = 0; i < PointCount; i++)
            {
                Point point = new()
                {
                    Position = reader.ReadVector3(),
                    UnknownFloat_1 = reader.ReadSingle()
                };
                Data.Points.Add(point);
            }
        }

        public override void Save(Stream stream)
        {
            BinaryWriterEx writer = new(stream, true);

            writer.Write(Data.UnknownUInt32_1);
            writer.Write(Data.UnknownUInt32_2);
            writer.Write(Data.Points.Count);

            writer.Write(0x00000438);
            writer.Write(0x00000006);
            writer.Write(0x00000000);
            writer.Write(0x00000000);
            writer.Write(0x00000000);
            writer.Write(0x00000000);
            writer.Write(0x00000000);
            writer.Write(0x00000000);
            writer.Write(0x00000001);

            foreach (Point point in Data.Points)
            {
                writer.Write(point.Position);
                writer.Write(point.UnknownFloat_1);
            }
        }
    }
}
