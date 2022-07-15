namespace Knuxs_Misc_Tools.Gods
{
    public class WAD : FileBase
    {
        public List<GenericFile> Files = new();

        // TODO: Read folder structure, all my attempts so far have failed and resulted in paths that make no sense.
        // TODO: Get samples of the WAD files from other Data Design Interactive games that run on their GODS engine.
        public void Load(string filepath, bool isWii = false)
        {
            // Set up the size of an entry in the file table for later maths.
            uint entrySize = 0x18; // Confirmed by the PC and PS2 versions of Ninjabread Man.
            if (isWii)
                entrySize = 0x20; // Confirmed by the Wii version of Ninjabread Man.

            BinaryReaderEx reader = new(File.OpenRead(filepath));

            // Read this WAD's header.
            reader.ReadSignature(4, "WADH");
            uint DataTableOffset = reader.ReadUInt32();
            uint NodeCount = reader.ReadUInt32();
            uint StringTableLength = reader.ReadUInt32();

            // Calculate where the String Table will be.
            uint StringTableOffset = (NodeCount * entrySize) + 0x10;

            // Loop through and read the files.
            // TODO: Better store the data here (including the Wii exclusive data), something to think about for after reading directories, as only then will I try and resave the file.
            for (int i = 0; i < NodeCount; i++)
            {
                // Skip root directory.
                if (i == 0)
                {
                    reader.JumpAhead(entrySize);
                    continue;
                }

                // Read this file entry.
                uint NameOffset = reader.ReadUInt32();
                if (isWii)
                {
                    uint UnknownUInt32_1 = reader.ReadUInt32();
                }
                uint DataOffset = reader.ReadUInt32();
                int DataSize = reader.ReadInt32();
                if (isWii)
                {
                    int UnknownInt32_1 = reader.ReadInt32();
                }
                bool UnknownBoolean_1 = reader.ReadBoolean(0x4);
                uint LastNodeIndex = reader.ReadUInt32();
                uint SiblingIndex = reader.ReadUInt32(); // ?

                // Save our current position so we can jump back for the next file.
                long pos = reader.BaseStream.Position;

                // Jump to and read this entry's name.
                reader.JumpTo(StringTableOffset + NameOffset);
                string Name = reader.ReadNullTerminatedString();

                // Directories SHOULD always have DataSize set to 0?
                if (DataSize != 0)
                {
                    reader.JumpTo(DataTableOffset + DataOffset);

                    GenericFile file = new()
                    {
                        FileName = Name,
                        Data = reader.ReadBytes(DataSize)
                    };
                    Files.Add(file);
                }

                // Jump back for the next file.
                reader.JumpTo(pos);
            }

            reader.Close();
        }

        // TODO: Replace with the Sonic 4 AMB Extraction when folders get involved.
        /// <summary>
        /// Extracts the files to disc.
        /// </summary>
        /// <param name="directory">The directory to extract to.</param>
        public void Extract(string directory)
        {
            // Create the extraction directory.
            Directory.CreateDirectory(directory);

            // Loop through each file to extract.
            foreach (GenericFile file in Files)
            {
                // Print the name of the file we're extracting.
                Console.WriteLine($"Extracting {file.FileName}.");

                // Extract the file.
                File.WriteAllBytes($@"{directory}\{file.FileName}", file.Data);
            }
        }
    }
}
