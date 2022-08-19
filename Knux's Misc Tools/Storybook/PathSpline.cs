using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Knuxs_Misc_Tools.Storybook
{
    public class PathSpline : FileBase
    {
        [Flags]
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Type : ushort
        {
            Test = 1,
            SecretRings = 3,
            BlackKnight = 4
        }

        public class FormatData
        {
            public Type Type { get; set; } = Type.SecretRings;

            public float UnknownFloat_1 { get; set; }

            public uint UnknownUInt32_1 { get; set; }

            public string? Name { get; set; }

            // TODO: How do these actually work????
            public List<Node> Nodes { get; set; } = new();
        }

        public class Node
        {
            public int? UnknownInt32_1 { get; set; } = null; // Not used by Type 4?

            public Vector3 Position { get; set; }

            public Vector3? UnknownVector3_1 { get; set; } = null; // Not used by Type 1?

            public Vector3? UnknownVector3_2 { get; set; } = null; // Only used by Type 4.

            public Vector3? UnknownVector3_3 { get; set; } = null; // Only used by Type 4.
        }

        public FormatData Data = new();

        public void Load(string filepath, bool isBlackKnight = true)
        {
            BinaryReaderEx reader = new(File.OpenRead(filepath));

            // File Header.
            Data.Type = (Type)reader.ReadUInt16(); // Always 3 in Secret Rings (barring some being 1 in stg902 (a test stage?)), always 4 in Black Knight (besides a handful of test stages?).
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
                Node node = new();
                switch (Data.Type)
                {
                    // TODO: How is this set up? Looks like an int (always 0), a vector3 then a float? Maybe swap those two???
                    case Type.Test:
                        reader.JumpAhead(0x14);
                        break;

                    case Type.SecretRings:
                        node = new()
                        {
                            UnknownInt32_1 = reader.ReadInt32(),
                            UnknownVector3_1 = reader.ReadVector3(), // Controls how far Sonic can go left and right? Might not be a vector3? Only the second value seemed to do anything?
                            Position = reader.ReadVector3() // Actual Path Node Position
                        };
                        Data.Nodes.Add(node);
                        break;

                    // TODO: What are the other values? Finding whatever UnknownVector3_1 in Type3 is here would allow path porting from Black Knight to Secret Rings in theory?
                    case Type.BlackKnight:
                        node = new()
                        {
                            Position = reader.ReadVector3(),
                            UnknownVector3_1 = reader.ReadVector3(),
                            UnknownVector3_2 = reader.ReadVector3(),
                            UnknownVector3_3 = reader.ReadVector3()
                        };
                        Data.Nodes.Add(node);
                        break;
                }
            }
        }

        public override void Save(Stream stream)
        {
            BinaryWriterEx writer = new(stream);
            
            // File Header.
            writer.Write((ushort)Data.Type);
            writer.Write((ushort)Data.Nodes.Count);
            writer.Write(Data.UnknownFloat_1);
            writer.Write(0x10);
            writer.Write(Data.UnknownUInt32_1);

            // Only write the name if it exists.
            if (Data.Name != null)
                writer.WriteNullPaddedString(Data.Name, 0x20);

            // TODO: Handle 0x1.
            foreach (Node node in Data.Nodes)
            {
                switch (Data.Type)
                {
                    case Type.Test:
                        throw new NotImplementedException();

                    case Type.SecretRings:
                        writer.Write((int)node.UnknownInt32_1);
                        writer.Write((Vector3)node.UnknownVector3_1);
                        writer.Write(node.Position);
                        break;

                    case Type.BlackKnight:
                        writer.Write(node.Position);
                        writer.Write((Vector3)node.UnknownVector3_1);
                        writer.Write((Vector3)node.UnknownVector3_2);
                        writer.Write((Vector3)node.UnknownVector3_3);
                        break;
                }
            }
        }
    }
}
