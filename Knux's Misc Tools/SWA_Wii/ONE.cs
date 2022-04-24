using Marathon.IO;

namespace Knuxs_Misc_Tools.SWA_Wii
{
    public class FileEntry
    {
        public string? FileName { get; set; }

        public byte[]? Data { get; set; }

        public override string ToString() => FileName;
    }

    public class ONE
    {
        public List<FileEntry> Files = new();

        /// <summary>
        /// Loads an uncompressed Sonic Unleashed Wii ONE Archive.
        /// </summary>
        /// <param name="filepath">The archive to load.</param>
        /// <param name="extractionPath">Path to extract the data to.</param>
        public void Load(string filepath, string? extractionPath = null)
        {
            BinaryReaderEx reader = new(File.OpenRead(filepath));

            // Check this is an uncompressed Unleashed Wii ONE archive.
            string SigCheck = reader.ReadNullPaddedString(4);
            
            // If this signature check fails, check again at 0x5 to see if this is a compressed file.
            if (SigCheck != "one.")
            {
                reader.JumpAhead();
                SigCheck = reader.ReadNullPaddedString(4);

                // If the signature check fails again, then assume this isn't a valid file and throw an exception.
                if (SigCheck != "one.")
                    throw new Exception($"File '{filepath}' does not appeared to be a Sonic Unleashed ON* archive.");

                // If it succeeds this time, then throw a placeholder exception until I do something with LZ11.
                else
                {
                    reader.JumpBehind(0x5);
                    throw new NotImplementedException("LZ11 decompression not yet implemented.");
                }
            }
            
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
                FileEntry file = new()
                {
                    FileName = fileName,
                    Data = binary
                };
                Files.Add(file);

                // Jump back for the next one.
                reader.JumpTo(position);
            }

            reader.Close();

            // If we have a path specified, then extract to it.
            if (extractionPath != null)
                Extract(extractionPath);
        }

        /// <summary>
        /// Saves the data to a ONE archive.
        /// TODO: Compression.
        /// </summary>
        /// <param name="filepath">Where to save the created ONE archive.</param>
        /// <param name="compressed">Whether or not to apply LZ11 compression.</param>
        public void Save(string filepath, bool compressed = false)
        {
            // Set up the writer.
            BinaryWriterEx writer = new(File.OpenWrite(filepath));

            // Header.
            writer.Write("one.");
            writer.Write(Files.Count);

            // Filenames, lengths and offsets.
            for (int i = 0; i < Files.Count; i++)
            {
                writer.WriteNullPaddedString(Files[i].FileName, 0x38);
                writer.AddOffset($"File{i}");
                writer.Write(Files[i].Data.Length);
            }
            
            // Weird padding fix. Not sure about this one.
            writer.FixPadding(0x10);
            writer.WriteNulls(0x10);

            // Actual file data.
            for (int i = 0; i < Files.Count; i++)
            {
                writer.FillOffset($"File{i}");
                writer.Write(Files[i].Data);

                // Even weirder padding fix. Even less sure about this one.
                if (writer.BaseStream.Position % 0x10 == 0)
                    writer.WriteNulls(0x10);
                writer.FixPadding(0x20);
            }

            // Third times the charm for weird padding that is probably wrong.
            writer.WriteNulls(0x10);

            // Close the writer.
            writer.Close();
        }

        /// <summary>
        /// Collects the files in a directory into an archive.
        /// </summary>
        /// <param name="directoryPath">The directory to package.</param>
        public void GetFiles(string directoryPath)
        {
            // Get the files in this directory.
            string[] files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);

            // Check if we have an order to reference, if not, then just do them sequentially.
            if (!File.Exists(@$"{directoryPath}\fileorder.log"))
            {
                // Loop through each file in the directory and add them.
                foreach (string? file in files)
                {
                    FileEntry entry = new()
                    {
                        FileName = Path.GetFileName(file),
                        Data = File.ReadAllBytes(file)
                    };
                    Files.Add(entry);
                }
            }
            else
            {
                // Read our order list.
                string[] fileOrder = File.ReadAllLines(@$"{directoryPath}\fileorder.log");

                // Loop through each file in the list.
                foreach (var file in fileOrder)
                {
                    // Check this file exists. If it does, then add it.
                    if (File.Exists(@$"{directoryPath}\{file}"))
                    {
                        FileEntry entry = new()
                        {
                            FileName = Path.GetFileName(@$"{directoryPath}\{file}"),
                            Data = File.ReadAllBytes(@$"{directoryPath}\{file}")
                        };
                        Files.Add(entry);
                    }

                    // If this file is that stupid space.bin padding, then create it.
                    else if (file == "space.bin")
                    {
                        FileEntry entry = new()
                        {
                            FileName = "space.bin",
                            Data = new byte[] { 0x73, 0x70, 0x61, 0x63, 0x65, 0x20, 0x20, 0x20 }
                        };
                        Files.Add(entry);
                    }

                    // If still not, then give up.
                    else
                    {
                        throw new Exception($"Couldn't find file '{file}' referenced by fileorder.");
                    }
                }
            }
        }

        /// <summary>
        /// Extracts the files to disc.
        /// </summary>
        /// <param name="directory">The directory to extract to.</param>
        public void Extract(string directory)
        {
            // Set up our file order log.
            StreamWriter log = new(File.Open($@"{directory}\fileorder.log", FileMode.Create));

            // Loop through each file.
            foreach (FileEntry file in Files)
            {
                // Write the file's name to the order log.
                log.WriteLine(file.FileName);

                // If this isn't a space.bin padding file, then write it to disk.
                if (file.FileName != "space.bin")
                {
                    Console.WriteLine($"Extracting {file.FileName}.");
                    File.WriteAllBytes($@"{directory}\{file.FileName}", file.Data);
                }
            }

            // Finish the order log.
            log.Close();
        }
    }
}
