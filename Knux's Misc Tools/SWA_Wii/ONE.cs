﻿using Marathon.IO;

namespace Knuxs_Misc_Tools.SWA_Wii
{
    internal class ONE
    {
        public Dictionary<string, byte[]> Files = new();

        static string Signature { get; } = "one.";

        /// <summary>
        /// Loads an uncompressed Sonic Unleashed Wii ONE Archive.
        /// </summary>
        /// <param name="filepath">The archive to load.</param>
        /// <param name="extractionPath">Path to extract the data to.</param>
        public void Load(string filepath, string? extractionPath = null)
        {
            BinaryReaderEx reader = new(File.OpenRead(filepath));

            // Check this is an uncompressed Unleashed Wii ONE archive.
            reader.ReadSignature(4, Signature);
            
            // Read how many files this archive has.
            uint fileCount = reader.ReadUInt32();

            // Loop through each file.
            for (int i = 0; i < fileCount; i++)
            {
                // Get the file name.
                string fileName = reader.ReadNullPaddedString(0x38);

                // Get the location and length of the file's binary data.
                uint fileOffset = reader.ReadUInt32();
                int fileLength = reader.ReadInt32();

                // Save our current position so we can jump back for the next file.
                long position = reader.BaseStream.Position;

                // Jump to this file's location.
                reader.JumpTo(fileOffset);

                // Read the specified amount of bytes.
                byte[] binary = reader.ReadBytes(fileLength);

                // Save this file.
                Files.Add(fileName, binary);

                // Jump back for the next one.
                reader.JumpTo(position);
            }

            reader.Close();

            // If we have a path specified, then extract to it.
            if (extractionPath != null)
                Extract(extractionPath);
        }

        /// <summary>
        /// Extracts the files to disc.
        /// </summary>
        /// <param name="directory">The directory to extract to.</param>
        public void Extract(string directory)
        {
            foreach (KeyValuePair<string, byte[]> file in Files)
                File.WriteAllBytes($@"{directory}\{file.Key}", file.Value);
        }
    }
}
