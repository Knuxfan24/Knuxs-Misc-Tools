namespace Knuxs_Misc_Tools.SonicRangers
{
    internal class BulletMesh : FileBase
    {
        public class FormatData
        {
            public List<Shape>? Shapes { get; set; }

            public List<byte[]>? UnknownData { get; set; }
        }

        public class Shape
        {
            public List<Vector3> Vertices { get; set; } = new();

            public List<CollisionFace> Faces { get; set; } = new();

            public byte[]? UnknownData_1 { get; set; }

            public List<uint> UnknownData_2 { get; set; } = new(); // Are these always 00 00 00 01?
        }

        public class CollisionFace
        {
            public ushort VertexA { get; set; }

            public ushort VertexB { get; set; }

            public ushort VertexC { get; set; }
        }

        public FormatData Data = new();

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream)
            {
                Offset = 0x40
            };

            // Skip the BINA header.
            reader.JumpTo(0x40);

            reader.JumpAhead(0x8); // Always 00 00 00 00 03 00 00 00.
            long ShapesOffset = reader.ReadInt64();
            uint ShapesCount = reader.ReadUInt32();
            uint UnknownDataCount = reader.ReadUInt32();
            long UnknownDataOffset = reader.ReadInt64();

            if (ShapesCount != 0)
            {
                Data.Shapes = new();
                reader.JumpTo(ShapesOffset, true);

                for (int i = 0; i < ShapesCount; i++)
                {
                    Shape shape = new();
                    uint ShapeUnknownUInt32_1 = reader.ReadUInt32();
                    uint ShapeUnknownUInt32_2 = reader.ReadUInt32();
                    uint ShapeVertexCount = reader.ReadUInt32();
                    uint ShapeFaceCount = reader.ReadUInt32();
                    int UnknownOffset1Length = reader.ReadInt32();
                    uint UnknownOffset2Count = reader.ReadUInt32();
                    reader.JumpAhead(0x8); // Always 0, Padding?
                    long VertexOffset = reader.ReadInt64();
                    long FaceOffset = reader.ReadInt64();
                    long UnknownOffset1 = reader.ReadInt64();
                    long UnknownOffset2 = reader.ReadInt64();

                    long pos = reader.BaseStream.Position;

                    // Vertices.
                    reader.JumpTo(VertexOffset, true);

                    for (int vertices = 0; vertices < ShapeVertexCount; vertices++)
                        shape.Vertices.Add(reader.ReadVector3());

                    // Faces
                    reader.JumpTo(FaceOffset, true);

                    for (int faces = 0; faces < ShapeFaceCount; faces++)
                    {
                        CollisionFace face = new()
                        {
                            VertexA = reader.ReadUInt16(),
                            VertexB = reader.ReadUInt16(),
                            VertexC = reader.ReadUInt16()
                        };
                        shape.Faces.Add(face);
                    }

                    // Unknown Data 1.
                    reader.JumpTo(UnknownOffset1, true);
                    shape.UnknownData_1 = reader.ReadBytes(UnknownOffset1Length);

                    // Unknown Data 2.
                    reader.JumpTo(UnknownOffset2, true);

                    for (int unknown = 0; unknown < UnknownOffset2Count; unknown++)
                        shape.UnknownData_2.Add(reader.ReadUInt32());

                    Data.Shapes.Add(shape);
                    reader.JumpTo(pos);
                }
            }

            // TODO: Confirm this and figure out what the hell it is. Doesn't crash when I read every file with it so that's something.
            if (UnknownDataCount != 0)
            {
                Data.UnknownData = new();
                reader.JumpTo(UnknownDataOffset, true);

                for (int i = 0; i < UnknownDataCount; i++)
                    Data.UnknownData.Add(reader.ReadBytes(0x30));
            }
        }

        public void ExportOBJ(string filePath)
        {
            for (int i = 0; i < Data.Shapes.Count; i++)
            {
                // Create a SteamWriter.
                StreamWriter obj = new($"{Path.GetDirectoryName(filePath)}\\{Path.GetFileNameWithoutExtension(filePath)}_shape{i}.obj");

                foreach (var vertex in Data.Shapes[i].Vertices)
                    obj.WriteLine($"v {vertex.X} {vertex.Y} {vertex.Z}");

                obj.WriteLine($"\ng shape{i}");

                foreach (CollisionFace face in Data.Shapes[i].Faces)
                    obj.WriteLine($"f {face.VertexA + 1} {face.VertexB + 1} {face.VertexC + 1}");

                // Close the StreamWriter.
                obj.Close();
            }
        }
    }
}
