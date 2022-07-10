namespace Knuxs_Misc_Tools.SonicPortable
{
    public class FileEntry
    {
        public string? FileName { get; set; }

        public byte[]? Data { get; set; }

        public override string ToString() => FileName;
    }

    public class AMB : FileBase
    {
        public List<FileEntry> Files = new();

        public void Load(string filepath, string? extractionPath = null)
        {
            BinaryReaderEx reader = new(File.OpenRead(filepath));

            // #AMB Header
            reader.ReadSignature(4, "#AMB");

            reader.JumpAhead(0xC); // Always 20 00 00 00 00 00 04 00 00 00 00 00

            uint FileCount = reader.ReadUInt32();
            uint FileTableOffset = reader.ReadUInt32();
            uint DataTableOffset = reader.ReadUInt32();
            uint StringTableOffset = reader.ReadUInt32();

            // Should already be here but just to be safe.
            reader.JumpTo(FileTableOffset);

            // Read each file.
            for (int i = 0; i < FileCount; i++)
            {
                uint FileStart = reader.ReadUInt32();
                int FileLength = reader.ReadInt32();
                reader.JumpAhead(0x8); // Always FF then 0. Unless the file entry is completely empty.

                // Don't bother reading this file if it's empty.
                if (FileStart != 0)
                {
                    FileEntry file = new();

                    // Save our current position so we can jump back for the next file.
                    long pos = reader.BaseStream.Position;

                    // Jump to this file's location.
                    reader.JumpTo(FileStart);

                    // Read the specified amount of bytes.
                    file.Data = reader.ReadBytes(FileLength);

                    // If this AMB has a String Table, then jump to and read this file's name from it.
                    if (StringTableOffset != 0)
                    {
                        reader.JumpTo(StringTableOffset + (0x20 * i));
                        file.FileName = reader.ReadNullTerminatedString();
                    }
                    // If not, just set the file's name in a linear order.
                    else
                    {
                        file.FileName = $"file{i}";
                    }

                    Files.Add(file);

                    // Jump back for the next one.
                    reader.JumpTo(pos);
                }
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
            // Create the extraction directory.
            Directory.CreateDirectory(directory);

            // Loop through each file to extract.
            foreach (FileEntry file in Files)
            {
                // Print the name of the file we're extracting.
                Console.WriteLine($"Extracting {file.FileName}.");

                // Sonic 4 can use sub directories in its archives. Create the directory if needed.
                if (!Directory.Exists($@"{directory}\{Path.GetDirectoryName(file.FileName)}"))
                    Directory.CreateDirectory($@"{directory}\{Path.GetDirectoryName(file.FileName)}");

                // Extract the file.
                File.WriteAllBytes($@"{directory}\{file.FileName}", file.Data);
            }
        }
    }
}
