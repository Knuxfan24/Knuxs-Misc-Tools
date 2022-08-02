namespace Knuxs_Misc_Tools.Storybook
{
    public class Path : FileBase
    {
        public class FormatData
        {
            public float UnknownFloat_1 { get; set; }

            public uint UnknownUInt32_1 { get; set; }

            public string? Name { get; set; }

            // TODO: How do these actually work????
            public List<List<Vector3>> Nodes { get; set; } = new();
        }

        public FormatData Data = new();

        public void Load(string filepath, bool isBlackKnight = true)
        {
            BinaryReaderEx reader = new(File.OpenRead(filepath));

            // File Header.
            ushort Type = reader.ReadUInt16(); // Always 3 in Secret Rings (barring some being 1 in stg902 (a test stage?)), always 4 in Black Knight (barring the Sand Oasis leftovers in stg902).
            ushort PointCount = reader.ReadUInt16();
            Data.UnknownFloat_1 = reader.ReadSingle();
            uint UnknownUInt32_1 = reader.ReadUInt32(); // Always 0x10. Pointless offset to data maybe?
            Data.UnknownUInt32_1 = reader.ReadUInt32(); // Always 0, 1 or 2.

            // Secret Rings doesn't put names in the Path Files.
            if (isBlackKnight)
                Data.Name = reader.ReadNullPaddedString(0x20);

            // Read the actual points.
            for (int i = 0; i < PointCount; i++)
            {
                switch (Type)
                {
                    // TODO: How is this set up? Looks like an int (always 0), a vector3 then a float? Maybe swap those two???
                    case 0x1:
                        reader.JumpAhead(0x14);
                        break;

                    // TODO: How is this set up? Looks like an int then two vector3s.
                    case 0x3:
                        reader.JumpAhead(0x1C);
                        break;

                    // TODO: How does this fit together to form the collision and spline?
                    case 0x4:
                        List<Vector3> NodeValues = new()
                        {
                            reader.ReadVector3(),
                            reader.ReadVector3(),
                            reader.ReadVector3(),
                            reader.ReadVector3()
                        };

                        Data.Nodes.Add(NodeValues);
                        break;
                }
            }
        }

        public override void Save(Stream stream)
        {
            BinaryWriterEx writer = new(stream);
            
            // File Header.
            writer.Write((ushort)Data.Nodes[0].Count);
            writer.Write((ushort)Data.Nodes.Count);
            writer.Write(Data.UnknownFloat_1);
            writer.Write(0x10);
            writer.Write(Data.UnknownUInt32_1);

            // Only write the name if it exists.
            if (Data.Name != null)
                writer.WriteNullPaddedString(Data.Name, 0x20);

            // TODO: This only handles Type 4, handle the others with a nicer data setup.
            foreach (List<Vector3> Node in Data.Nodes)
            {
                foreach (Vector3 value in Node)
                {
                    writer.Write(value);
                }
            }
        }
    }
}
