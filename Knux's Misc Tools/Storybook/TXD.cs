namespace Knuxs_Misc_Tools.Storybook
{
    public class TXD : FileBase
    {
        public List<GenericFile> Textures = new();

        public override string Signature { get; } = "TXAG";

        /// <summary>
        /// Loads a Storybook TXD.
        /// </summary>
        /// <param name="stream">The TXD to load.</param>
        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream, true);

            // Check the file signature.
            reader.ReadSignature(4, Signature);

            // Get the amount of textures in this TXD.
            uint fileCount = reader.ReadUInt32();
            
            // Loop through the textures in this TXD.
            for (int i = 0; i < fileCount; i++)
            {
                // Get the offset to this texture.
                uint textureOffset = reader.ReadUInt32();

                // Get the binary size of this texture.
                int textureSize = reader.ReadInt32();

                // Create a new texture entry.
                GenericFile texture = new();

                // Read the name of this texture, padded to 0x20 bytes.
                texture.FileName = reader.ReadNullPaddedString(0x20);

                // Save our current position so we can jump back for the next texture.
                long position = reader.BaseStream.Position;

                // Jump to this texture's data.
                reader.JumpTo(textureOffset);

                // Read this texture's data.
                texture.Data = reader.ReadBytes(textureSize);

                // Jump back for the next texture.
                reader.JumpTo(position);

                // Save this texture entry.
                Textures.Add(texture);
            }
        }

        /// <summary>
        /// Saves a TXD archive.
        /// </summary>
        /// <param name="stream">The path to save the TXD to.</param>
        public override void Save(Stream stream)
        {
            BinaryWriterEx writer = new(stream, true);

            // Write the TXAG signature.
            writer.WriteSignature(Signature);

            // Write the amount of textures in this TXD.
            writer.Write(Textures.Count);

            // Fill out the table of offsets, sizes and names.
            for (int i = 0; i < Textures.Count; i++)
            {
                writer.AddOffset($"Texture{i}Offset");
                writer.Write(Textures[i].Data.Length);
                writer.WriteNullPaddedString(Textures[i].FileName, 0x20);
            }

            // Fix the padding.
            // TODO: Is this correct? stg010.txd ended up identical with this value, need to test others.
            writer.FixPadding(0x20);

            // Write the texture data to the file.
            for (int i = 0; i < Textures.Count; i++)
            {
                writer.FillOffset($"Texture{i}Offset");
                writer.Write(Textures[i].Data);
            }
        }

        /// <summary>
        /// Extracts the textures to disc.
        /// </summary>
        /// <param name="directory">The directory to extract to.</param>
        public void Extract(string directory)
        {
            Directory.CreateDirectory(directory);

            foreach (GenericFile texture in Textures)
            {
                Console.WriteLine($"Extracting {texture.FileName}.");
                File.WriteAllBytes($@"{directory}\{texture.FileName}.gvr", texture.Data);
            }
        }

        /// <summary>
        /// Collects the .gvr files in a directory into a TXD.
        /// </summary>
        /// <param name="directoryPath">The directory to package.</param>
        public void GetFiles(string directoryPath)
        {
            // Get the .gvr files in this directory.
            string[] files = Directory.GetFiles(directoryPath, "*.gvr");

            // Loop through each file in the directory and add them.
            foreach (string? file in files)
            {
                GenericFile texture = new()
                {
                    FileName = Path.GetFileName(file),
                    Data = File.ReadAllBytes(file)
                };
                Textures.Add(texture);
            }
        }
    }
}
