namespace Knuxs_Misc_Tools.WrathOfCortex
{
    public class NUS : FileBase
    {
        public override string Signature { get; } = "0CSG";

        public List<string>? Names { get; set; }

        public List<HGO_Chunk.Texture>? Textures { get; set; }

        public List<HGO_Chunk.Material>? Materials { get; set; }

        public List<HGO_Chunk.Geometry>? Geometry { get; set; }

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream, true);

            // Basic Chunk Header.
            reader.ReadSignature(4, Signature);
            uint fileSize = reader.ReadUInt32();

            while (reader.BaseStream.Position < fileSize)
            {
                string chunkType = reader.ReadNullPaddedString(4);
                uint chunkSize = reader.ReadUInt32();
                reader.JumpBehind(8);

                switch (chunkType)
                {
                    case "LBTN":
                        HGO_Chunk.NameTable? nameTable = new();
                        Names = nameTable.Read(reader);
                        break;

                    case "0TST":
                        HGO_Chunk.TextureSet textureSet = new();
                        Textures = textureSet.Read(reader);
                        break;

                    case "00SM":
                        HGO_Chunk.MaterialSet materialSet = new();
                        Materials = materialSet.Read(reader);
                        break;

                    case "0TSG":
                        HGO_Chunk.GeometrySet geometrySet = new();
                        Geometry = geometrySet.Read(reader);
                        break;

                    case "TSNI":
                        HGO_Chunk.INST inst = new();
                        inst.Read(reader);
                        break;

                    default:
                        Console.WriteLine($"NUS Chunk Type '{chunkType}' not yet handled.");
                        reader.JumpAhead(chunkSize);
                        break;
                }
            }
        }
    
        public void ExportOBJ(string filepath)
        {
            int VertexCount = 0;

            StreamWriter writer = File.CreateText(filepath);
            for (int i1 = 0; i1 < Geometry.Count; i1++)
            {
                writer.WriteLine($"\n# geometry{i1}\n");
                for (int i2 = 0; i2 < Geometry[i1].Meshes.Count; i2++)
                {
                    if (Geometry[i1].Meshes[i2].Primitive != null)
                    {
                        if (Geometry[i1].Meshes[i2].Primitive.Type == 6)
                        {
                            for (int i = 0; i < Geometry[i1].Meshes[i2].Vertices.Count; i++)
                            {
                                writer.WriteLine($"v {Geometry[i1].Meshes[i2].Vertices[i].Position.X} {Geometry[i1].Meshes[i2].Vertices[i].Position.Y} {Geometry[i1].Meshes[i2].Vertices[i].Position.Z}");
                            }

                            writer.WriteLine();
                            for (int i = 0; i < Geometry[i1].Meshes[i2].Vertices.Count; i++)
                            {
                                writer.WriteLine($"vt {Geometry[i1].Meshes[i2].Vertices[i].UV.X} {Geometry[i1].Meshes[i2].Vertices[i].UV.Y}");
                            }

                            writer.WriteLine();
                            for (int i = 0; i < Geometry[i1].Meshes[i2].Vertices.Count; i++)
                            {
                                writer.WriteLine($"vn {Geometry[i1].Meshes[i2].Vertices[i].Normals.X} {Geometry[i1].Meshes[i2].Vertices[i].Normals.Y} {Geometry[i1].Meshes[i2].Vertices[i].Normals.Z}");
                            }

                            writer.WriteLine();
                            writer.WriteLine($"o geometry{i1}mesh{i2}");
                            writer.WriteLine($"g geometry{i1}mesh{i2}");
                            writer.WriteLine($"usemtl Material{Geometry[i1].Meshes[i2].MaterialIndex}");
                            bool swap = false;
                            for (int i = 0; i < Geometry[i1].Meshes[i2].Primitive.FaceIndices.Count - 2; i++)
                            {
                                if (!swap)
                                {
                                    writer.WriteLine($"f {Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + 1 + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + 1 + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + 1 + VertexCount}" +
                                                        $" {Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + 1 + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + 1 + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + 1 + VertexCount}" +
                                                        $" {Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + 1 + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + 1 + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + 1 + VertexCount}");
                                    swap = true;
                                }
                                else
                                {
                                    writer.WriteLine($"f {Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + 1 + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + 1 + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + 1 + VertexCount} " +
                                                        $"{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + 1 + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + 1}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + 1 + VertexCount} " +
                                                        $"{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + 1 + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + 1 + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + 1 + VertexCount}");
                                    swap = false;
                                }
                            }

                            VertexCount += Geometry[i1].Meshes[i2].Vertices.Count;
                        }
                    }
                }
            }

            writer.Flush();
            writer.Close();
        }
    }
}
