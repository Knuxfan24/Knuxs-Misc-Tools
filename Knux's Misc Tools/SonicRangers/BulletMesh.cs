using HedgeLib.Headers;

namespace Knuxs_Misc_Tools.SonicRangers
{
    internal class BulletMesh : FileBase
    {
        public class FormatData
        {
            public List<Shape> Shapes { get; set; } = new();

            public List<UnknownDataStruct> UnknownData { get; set; } = new();
        }

        public class Shape
        {
            public uint UnknownUInt32_1 { get; set; }

            public uint UnknownUInt32_2 { get; set; }

            public List<Vector3> Vertices { get; set; } = new();

            public List<CollisionFace> Faces { get; set; } = new();

            public BoundingVolumeHierarchy? BVH { get; set; }

            public uint? UnknownCollisionTag { get; set; }
        }

        public class BoundingVolumeHierarchy
        {
            public Vector3 UnknownVector3_1 { get; set; }

            public Vector3 UnknownVector3_2 { get; set; }

            public Vector3 UnknownVector3_3 { get; set; }

            public uint UnknownUInt32_1 { get; set; }
        }

        public class CollisionFace
        {
            public ushort VertexA { get; set; }

            public ushort VertexB { get; set; }

            public ushort VertexC { get; set; }

            public uint CollisionTag { get; set; } // First byte is the surface type, maybe split this into four bytes once the others are messed with?
        }

        // Verify that this matches up in every file.
        public class UnknownDataStruct
        {
            public uint UnknownUInt32_1 { get; set; }

            public uint UnknownUInt32_2 { get; set; }

            public Vector3 UnknownVector3_1 { get; set; }

            public Vector3 UnknownVector3_2 { get; set; }

            public Vector3 UnknownVector3_3 { get; set; }
        }

        // Set up HedgeLib#'s BINAV2Header.
        public HedgeLib.Headers.BINAHeader Header = new BINAv2Header(210);

        public FormatData Data = new();

        public override void Load(Stream stream)
        {
            // Set up our BINAReader and read the BINAV2 header.
            HedgeLib.IO.BINAReader reader = new(stream);
            Header = reader.ReadHeader();

            reader.JumpAhead(0x8); // Always 00 00 00 00 03 00 00 00.
            long ShapesOffset = reader.ReadInt64();
            uint ShapesCount = reader.ReadUInt32();
            uint UnknownDataCount = reader.ReadUInt32();
            long UnknownDataOffset = reader.ReadInt64();

            if (ShapesCount != 0)
            {
                reader.JumpTo(ShapesOffset, false);

                for (int i = 0; i < ShapesCount; i++)
                {
                    Shape shape = new();
                    shape.UnknownUInt32_1 = reader.ReadUInt32();
                    shape.UnknownUInt32_2 = reader.ReadUInt32();
                    uint ShapeVertexCount = reader.ReadUInt32();
                    uint ShapeFaceCount = reader.ReadUInt32();
                    int BVHLength = reader.ReadInt32();
                    uint CollisionTagCount = reader.ReadUInt32(); // Only ever different from ShapeFaceCount if that is 0.
                    reader.JumpAhead(0x8); // Always 0, Padding?
                    long VertexOffset = reader.ReadInt64();
                    long FaceOffset = reader.ReadInt64();
                    long BVHOffset = reader.ReadInt64();
                    long CollisionTagTableOffset = reader.ReadInt64();

                    long pos = reader.BaseStream.Position;

                    // Vertices.
                    reader.JumpTo(VertexOffset, false);

                    for (int vertices = 0; vertices < ShapeVertexCount; vertices++)
                        shape.Vertices.Add(new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));

                    // Faces.
                    reader.JumpTo(FaceOffset, false);

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

                    // BoundingVolumeHierarchy.
                    if (BVHLength != 0)
                    {
                        BoundingVolumeHierarchy bvh = new();

                        reader.JumpTo(BVHOffset, false);
                        reader.JumpAhead(0x10); // Always 0s.

                        // TODO: Are these three Vector3s padded to 0x10 each?
                        bvh.UnknownVector3_1 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        reader.FixPadding(0x10);
                        bvh.UnknownVector3_2 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        reader.FixPadding(0x10);
                        bvh.UnknownVector3_3 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        reader.FixPadding(0x10);

                        reader.JumpAhead(0x4); // Always 0x120.
                        bvh.UnknownUInt32_1 = reader.ReadUInt32(); // Some sort of type? Later data seems to be determined by this.
                        reader.JumpAhead(0xB0); // Always the same pattern of bytes: 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                        uint UnknownUInt32_2 = reader.ReadUInt32(); // Some sort of count?
                        reader.JumpAhead(0x4); // Always 0.

                        // TODO: Figure out how the Type(?) value factors in to the remaining data, type 9 seems to read 0xB0 bytes while type 1 reads 0x30.

                        shape.BVH = bvh;
                    }

                    // Collision Tags.
                    reader.JumpTo(CollisionTagTableOffset, false);

                    if (ShapeFaceCount != 0)
                        for (int tag = 0; tag < CollisionTagCount; tag++)
                            shape.Faces[tag].CollisionTag = reader.ReadUInt32();
                    else
                        shape.UnknownCollisionTag = reader.ReadUInt32();

                    Data.Shapes.Add(shape);
                    reader.JumpTo(pos);
                }
            }

            // TODO: Confirm this and figure out what the hell it is.
            if (UnknownDataCount != 0)
            {
                reader.JumpTo(UnknownDataOffset, false);

                for (int i = 0; i < UnknownDataCount; i++)
                {
                    UnknownDataStruct unknown = new();
                    unknown.UnknownUInt32_1 = reader.ReadUInt32();
                    unknown.UnknownUInt32_2 = reader.ReadUInt32();
                    unknown.UnknownVector3_1 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    unknown.UnknownVector3_2 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    unknown.UnknownVector3_3 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    Data.UnknownData.Add(unknown);
                }
            }
        }

        // TODO: Finish this once the BVH stuff is reading and try to get the BINA Footer writing correctly.
        public override void Save(Stream stream)
        {
            // Set up our BINAWriter and write the BINAV2 header.
            HedgeLib.IO.BINAWriter writer = new(stream, Header);

            writer.Write(0x00);
            writer.Write(0x03);

            writer.AddOffset("ShapesOffset", 8);
            writer.Write(Data.Shapes.Count);
            writer.Write(Data.UnknownData.Count);
            writer.AddOffset("UnknownDataOffset", 8);

            if (Data.Shapes.Count != 0)
            {
                writer.FillInOffsetLong("ShapesOffset", false, false);
                for (int i = 0; i < Data.Shapes.Count; i++)
                {
                    writer.Write(Data.Shapes[i].UnknownUInt32_1);
                    writer.Write(Data.Shapes[i].UnknownUInt32_2);
                    writer.Write(Data.Shapes[i].Vertices.Count);
                    writer.Write(Data.Shapes[i].Faces.Count);
                    //writer.Write(Data.Shapes[i].UnknownData.Length);
                    writer.Write(Data.Shapes[i].Faces.Count); // TODO: Handle things that don't have face data.
                    writer.WriteNulls(0x8);
                    writer.AddOffset($"Shape{i}VertexTable", 8);
                    writer.AddOffset($"Shape{i}FaceTable", 8);
                    writer.AddOffset($"Shape{i}UnknownDataOffset", 8);
                    writer.AddOffset($"Shape{i}CollisionTagTable", 8);
                }

                for (int i = 0; i < Data.Shapes.Count; i++)
                {
                    writer.FillInOffsetLong($"Shape{i}VertexTable", false, false);
                    foreach (Vector3 vertex in Data.Shapes[i].Vertices)
                    {
                        writer.Write(vertex.X);
                        writer.Write(vertex.Y);
                        writer.Write(vertex.Z);
                    }
                    writer.FixPadding(0x10);
                }

                for (int i = 0; i < Data.Shapes.Count; i++)
                {
                    writer.FillInOffsetLong($"Shape{i}FaceTable", false, false);
                    foreach (var face in Data.Shapes[i].Faces)
                    {
                        writer.Write(face.VertexA);
                        writer.Write(face.VertexB);
                        writer.Write(face.VertexC);
                    }
                    writer.FixPadding(0x10);
                }

                for (int i = 0; i < Data.Shapes.Count; i++)
                {
                    writer.FillInOffsetLong($"Shape{i}UnknownDataOffset", false, false);
                    //writer.Write(Data.Shapes[i].UnknownData);
                    writer.FixPadding(0x10);
                }

                for (int i = 0; i < Data.Shapes.Count; i++)
                {
                    writer.FillInOffsetLong($"Shape{i}CollisionTagTable", false, false);
                    foreach (var face in Data.Shapes[i].Faces)
                        writer.Write(face.CollisionTag);

                    if (i != Data.Shapes.Count - 1)
                        writer.FixPadding(0x10);
                }
            }

            if (Data.UnknownData.Count != 0)
            {
                writer.FillInOffsetLong("UnknownDataOffset", false, false);
                for (int i = 0; i < Data.UnknownData.Count; i++)
                {
                    writer.Write(Data.UnknownData[i].UnknownUInt32_1);
                    writer.Write(Data.UnknownData[i].UnknownUInt32_2);

                    writer.Write(Data.UnknownData[i].UnknownVector3_1.X);
                    writer.Write(Data.UnknownData[i].UnknownVector3_1.Y);
                    writer.Write(Data.UnknownData[i].UnknownVector3_1.Z);

                    writer.Write(Data.UnknownData[i].UnknownVector3_2.X);
                    writer.Write(Data.UnknownData[i].UnknownVector3_2.Y);
                    writer.Write(Data.UnknownData[i].UnknownVector3_2.Z);

                    writer.Write(Data.UnknownData[i].UnknownVector3_3.X);
                    writer.Write(Data.UnknownData[i].UnknownVector3_3.Y);
                    writer.Write(Data.UnknownData[i].UnknownVector3_3.Z);
                }
            }

            // Finish writing the BINA information.
            writer.FinishWrite(Header);
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
