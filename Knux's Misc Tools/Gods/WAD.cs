namespace Knuxs_Misc_Tools.Gods
{
    public class WAD : FileBase
    {
        public class Node
        {
            // The name of this node.
            public string Name { get; set; } = string.Empty;

            public int? UnknownWiiInt32_1 { get; set; } = null;

            // The bytes that make up this node (if any).
            public byte[]? Data { get; set; }

            public int? UnknownWiiInt32_2 { get; set; } = null;

            public bool UnknownBoolean_1 { get; set; }

            // The index of the last node in this node's root.
            public int LastRootNodeIndex { get; set; } = -1;

            // This node's sibling index.
            public int SiblingIndex { get; set; } = -1;

            // Show the node's name in the debugger rather than Knuxs_Misc_Tools.Gods.WAD.WADFileEntry.
            public override string ToString() => Name;
        }

        public List<Node> Nodes = new();

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

            // Loop through and read the nodes.
            for (int i = 0; i < NodeCount; i++)
            {
                // Skip root directory.
                if (i == 0)
                {
                    reader.JumpAhead(entrySize);
                    continue;
                }

                Node node = new();

                // Read this node.
                uint NameOffset = reader.ReadUInt32();
                if (isWii) node.UnknownWiiInt32_1 = reader.ReadInt32();
                uint DataOffset = reader.ReadUInt32();
                int DataSize = reader.ReadInt32();
                if (isWii) node.UnknownWiiInt32_2 = reader.ReadInt32();
                node.UnknownBoolean_1 = reader.ReadBoolean(0x4);
                node.LastRootNodeIndex = reader.ReadInt32();
                node.SiblingIndex = reader.ReadInt32();

                // Save our current position so we can jump back for the next node.
                long pos = reader.BaseStream.Position;

                // Jump to and read this entry's name.
                reader.JumpTo(StringTableOffset + NameOffset);
                node.Name = reader.ReadNullTerminatedString();

                // If this entry has a data size, then jump to and read it.
                if (DataSize != 0)
                {
                    reader.JumpTo(DataTableOffset + DataOffset);
                    node.Data = reader.ReadBytes(DataSize);
                }

                // Store ths entry.
                Nodes.Add(node);

                // Jump back for the next node.
                reader.JumpTo(pos);
            }

            reader.Close();

            // Awful system to handle directories, but I can't think of a different way to really do this...
            // Create a list of filepaths with an empty entry in it to line up the Sibling Indices.
            List<string> paths = new() { "" };

            // Set up a string for the file's full path.
            string fullPath = "";

            // Loop through all our entries.
            for (int i = 0; i < Nodes.Count; i++)
            {
                // If this entry does not have a sibling, then simply add its name onto the full path.
                if (Nodes[i].SiblingIndex == -1)
                    fullPath += $@"\{Nodes[i].Name}";

                // If it does, then set the fullPath to the sibling's path, but remove the name of the sibling entry and add this entry's in its place.
                else
                    fullPath = paths[Nodes[i].SiblingIndex].Replace(Nodes[Nodes[i].SiblingIndex - 1].Name, Nodes[i].Name);

                // Add this entry's path for future entries to reference.
                paths.Add(fullPath);
            }

            // Remove the empty root path.
            paths.RemoveAt(0);

            // Set the node names to their paths.
            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].Name = paths[i];
        }

        // TODO: Inaccurate, duplicates node names where the original file just points to the earlier existing one.
        public override void Save(string filepath, bool isWii = false)
        {
            BinaryWriterEx writer = new(File.OpenWrite(filepath));

            // WADH header.
            writer.Write("WADH");
            writer.AddOffset("DataTable");
            writer.Write(Nodes.Count + 1);

            // Placeholder size value to be filled in later.
            long stringTableSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");

            // Root Node
            // TODO: Wii version.
            writer.Write(-1);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(1);
            writer.Write(-1);

            // Set up a value to calculate string table offsets.
            int stringTableLength = 0;
            for (int i = 0; i < Nodes.Count; i++)
            {
                // Write the current string table length.
                writer.Write(stringTableLength);

                // Calculate how many characters this node should add.
                stringTableLength += Nodes[i].Name[(Nodes[i].Name.LastIndexOf('\\') + 1)..].Length + 1;

                if (isWii)
                    writer.Write((int)Nodes[i].UnknownWiiInt32_1);

                writer.AddOffset($"Node{i}DataOffset");

                if (Nodes[i].Data != null)
                    writer.Write(Nodes[i].Data.Length);
                else
                    writer.Write(0);

                if (isWii)
                    writer.Write((int)Nodes[i].UnknownWiiInt32_2);

                writer.WriteBoolean32(Nodes[i].UnknownBoolean_1);
                writer.Write(Nodes[i].LastRootNodeIndex);
                writer.Write(Nodes[i].SiblingIndex);
            }

            // Write the name for this node.
            for (int i = 0; i < Nodes.Count; i++)
                writer.WriteNullTerminatedString(Nodes[i].Name[(Nodes[i].Name.LastIndexOf('\\') + 1)..]);

            writer.FixPadding(0x20);
            writer.FillOffset("DataTable");

            // Set up a value to calculate data offsets.
            uint dataOffset = 0;

            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Data != null)
                {
                    // Write this node's data if any exists.
                    writer.FillOffset($"Node{i}DataOffset", dataOffset);
                    dataOffset += (uint)Nodes[i].Data.Length;
                    writer.Write(Nodes[i].Data);
                }
            }

            // Go back and fill in the string table size.
            writer.BaseStream.Position = stringTableSizePos;
            writer.Write(stringTableLength);

            // Close the writer.
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Extracts the files to disc.
        /// </summary>
        /// <param name="directory">The directory to extract to.</param>
        public void Extract(string directory)
        {
            // Create the extraction directory.
            Directory.CreateDirectory(directory);

            // Loop through each node to extract.
            foreach (Node node in Nodes)
            {
                // Skip this node if it's only a directory entry.
                if (node.Data == null)
                    continue;

                // Print the name of the file we're extracting.
                Console.WriteLine($"Extracting {node.Name}.");

                // The GODS Engine can use sub directories in its archives. Create the directory if needed.
                if (!Directory.Exists($@"{directory}\{Path.GetDirectoryName(node.Name)}"))
                    Directory.CreateDirectory($@"{directory}\{Path.GetDirectoryName(node.Name)}");

                // Extract the file.
                File.WriteAllBytes($@"{directory}\{node.Name}", node.Data);
            }
        }
    }
}
