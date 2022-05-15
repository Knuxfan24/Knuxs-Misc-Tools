namespace Knuxs_Misc_Tools.WrathOfCortex.HGO_Chunk
{
    public class TextureSet
    {
        public List<Texture> Read(BinaryReaderEx reader)
        {
            List<Texture> Textures = new();

            // Basic Chunk Header.
            string chunkType = reader.ReadNullPaddedString(4);
            uint chunkSize = reader.ReadUInt32();

            // Read the TextureSet Counter chunk.
            string textureCounter = reader.ReadNullPaddedString(4);
            if (textureCounter != "0HST")
                throw new Exception($"Expected '0HST', got '{textureCounter}'.");
            uint textureCounterChunkSize = reader.ReadUInt32();
            uint textureCount = reader.ReadUInt32();

            // Loop through based on the amount of textures listed in the Counter and read them.
            for (int i = 0; i < textureCount; i++)
            {
                string textureHeader = reader.ReadNullPaddedString(4);
                if (textureHeader != "0MXT")
                    throw new Exception($"Expected '0MXT', got '{textureHeader}'.");
                uint textureChunkSize = reader.ReadUInt32();

                // Read the Texture Data.
                Texture texture = new()
                {
                    Type = reader.ReadUInt32(),
                    Width = reader.ReadUInt32(),
                    Height = reader.ReadUInt32(),
                    Data = reader.ReadBytes(reader.ReadInt32())
                };

                // Add this texture to our list.
                Textures.Add(texture);
            }

            // Align to 0x4.
            reader.FixPadding();

            return Textures;
        }
    }

    public class Texture
    {
        public uint Type { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }

        public byte[] Data { get; set; }
    }
}
