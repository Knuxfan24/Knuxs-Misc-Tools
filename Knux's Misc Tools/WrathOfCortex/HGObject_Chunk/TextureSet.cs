namespace Knuxs_Misc_Tools.WrathOfCortex.HGObject_Chunk
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

        public void Write(BinaryWriterEx writer, List<Texture> textures)
        {
            // Chunk Identifier.
            writer.Write("0TST");

            // Save the position we'll need to write the chunk's size to and add a dummy value in its place.
            long chunkSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");

            // TextureSet Counter Chunk
            // Chunk Identifier.
            writer.Write("0HST");

            // Write the TextureSet Counter Chunk's size, I assume it's always 0xC just based on how it's set up? Need to verify.
            writer.Write(0xc);

            // Write the amount of textures in this file.
            writer.Write(textures.Count);

            // Write the textures.
            for (int i = 0; i < textures.Count; i++)
            {
                // Chunk Identifier.
                writer.Write("0MXT");

                // Save the position we'll need to write the texture chunk's size to and add a dummy value in its place.
                long textureChunkSizePos = writer.BaseStream.Position;
                writer.Write("SIZE");

                // Write the texture's data.
                writer.Write(textures[i].Type);
                writer.Write(textures[i].Width);
                writer.Write(textures[i].Height);
                writer.Write(textures[i].Data.Length);
                writer.Write(textures[i].Data);

                // Save our current position.
                long texturePos = writer.BaseStream.Position;

                // Calculate the texture's length.
                uint textureLength = (uint)(writer.BaseStream.Position - (textureChunkSizePos - 0x4));

                // Fill in the texture chunk's size.
                writer.BaseStream.Position = textureChunkSizePos;
                writer.Write(textureLength);

                // Jump to our saved position so we can continue.
                writer.BaseStream.Position = texturePos;
            }

            // Align to 0x4.
            writer.FixPadding(0x4);

            // Calculate the chunk size.
            uint chunkSize = (uint)(writer.BaseStream.Position - (chunkSizePos - 0x4));

            // Save our current position.
            long pos = writer.BaseStream.Position;

            // Fill in the chunk size.
            writer.BaseStream.Position = chunkSizePos;
            writer.Write(chunkSize);

            // Jump to our saved position so we can continue.
            writer.BaseStream.Position = pos;
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
