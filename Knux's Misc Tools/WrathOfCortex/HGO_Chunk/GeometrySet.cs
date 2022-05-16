namespace Knuxs_Misc_Tools.WrathOfCortex.HGO_Chunk
{
    public class GeometrySet
    {
        public List<Geometry> Read(BinaryReaderEx reader)
        {
            List<Geometry> Geometry = new();

            // Basic Chunk Header.
            string chunkType = reader.ReadNullPaddedString(4);
            uint chunkSize = reader.ReadUInt32();

            // Count of how many Geometry entries are in this chunk.
            uint geometryCount = reader.ReadUInt32();

            // Loop through based on count.
            for (int i = 0; i < geometryCount; i++)
            {
                // Create a new Geometry entry.
                Geometry geometry = new();

                reader.JumpAhead(0x4); // Always 1.
                uint meshType = reader.ReadUInt32();

                if (meshType == 0)
                {
                    reader.JumpAhead(0xC); // Always 0.
                    uint MeshCount = reader.ReadUInt32();

                    for (int m = 0; m < MeshCount; m++)
                    {
                        Mesh mesh = new();
                        mesh.MaterialIndex = reader.ReadUInt32();
                        uint VertexCount = reader.ReadUInt32();

                        for (int v = 0; v < VertexCount; v++)
                        {
                            // TODO: Is this right?
                            Vertex vertex = new()
                            {
                                Position = reader.ReadVector3(),
                                Normals = reader.ReadVector3(),
                                Colours = reader.ReadBytes(4),
                                TextureCoordinates = reader.ReadVector2()
                            };
                            mesh.Vertices.Add(vertex);
                        }

                        reader.JumpAhead(0x8); // Always a 0 then a 1.

                        mesh.Primitive = new();
                        mesh.Primitive.Type = reader.ReadUInt32();
                        uint FaceCount = reader.ReadUInt32();

                        for (int f = 0; f < FaceCount; f++)
                            mesh.Primitive.FaceIndices.Add(reader.ReadUInt16());

                        reader.JumpAhead(0x8); // Always 0.

                        geometry.Meshes.Add(mesh);
                    }

                    Geometry.Add(geometry);
                }
                else
                {

                    uint MeshCount = reader.ReadUInt32();
                    for (int m = 0; m < MeshCount; m++)
                    {
                        Mesh mesh = new();
                        mesh.MaterialIndex = reader.ReadUInt32();
                        uint VertexCount = reader.ReadUInt32();
                        uint UnknownUInt32_4 = reader.ReadUInt32(); // Seems to be either 0 or 1?
                        reader.JumpAhead(0x4); // Always 0.

                        for (int v = 0; v < VertexCount; v++)
                        {
                            // TODO: Is this right? I seriously have doubts about this one especially.
                            Vertex vertex = new()
                            {
                                Position = reader.ReadVector3(),
                                TextureCoordinates = reader.ReadVector2(),
                                Colours = reader.ReadBytes(4)
                            };
                            mesh.Vertices.Add(vertex);
                        }

                        geometry.Meshes.Add(mesh);
                    }

                    Geometry.Add(geometry);
                }
            }

            // Align to 0x4.
            reader.FixPadding();

            return Geometry;
        }
    }

    public class Geometry
    {
        public List<Mesh> Meshes { get; set; } = new();
    }

    public class Mesh
    {
        public uint MaterialIndex { get; set; }

        public List<Vertex> Vertices { get; set; } = new();

        public Primitive Primitive { get; set; }
    }

    public class Vertex
    {
        public Vector3 Position { get; set; }

        public Vector3 Normals { get; set; }

        public byte[] Colours { get; set; }

        public Vector2 TextureCoordinates { get; set; }
    }

    public class Primitive
    {
        public uint Type { get; set; } // Either 5 or 6? 5 is standard faces, 6 is a triangle strip of some sort.

        public List<ushort> FaceIndices { get; set; } = new();
    }
}
