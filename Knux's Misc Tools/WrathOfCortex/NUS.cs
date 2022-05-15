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
    }
}
