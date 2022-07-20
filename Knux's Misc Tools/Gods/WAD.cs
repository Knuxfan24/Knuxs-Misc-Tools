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
