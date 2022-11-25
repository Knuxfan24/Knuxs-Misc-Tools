using HedgeLib.Headers;

namespace Knuxs_Misc_Tools.SonicRangers
{
    public class Node
    {
        public ushort Index { get; set; } = 0xFFFF; // Not actually a thing, used for easier ID in JSON.

        public ushort ParentNodeIndex { get; set; }

        public string Name { get; set; } = "";

        public string? ParentNodeName { get; set; } // Not actually a thing, used for easier ID in JSON.

        public float[] UnknownFloats { get; set; } = new float[12];
    }

    internal class BulletSkeleton : FileBase
    {
        // Set up HedgeLib#'s BINAV2Header.
        public HedgeLib.Headers.BINAHeader Header = new BINAv2Header(210);

        public List<Node> Nodes = new();

        // Set up the Signature we expect.
        public new const string Signature = "KSXP";

        public override void Load(Stream stream)
        {
            // Set up our BINAReader and read the BINAV2 header.
            HedgeLib.IO.BINAReader reader = new(stream);
            Header = reader.ReadHeader();

            // Check this file's signature.
            string signature = reader.ReadSignature();
            if (signature != Signature)
                throw new Exception($"Invalid signature, got '{signature}', expected '{Signature}'.");

            // Read this file's header.
            reader.JumpAhead(0x4); // Always 00 02 00 00.
            long HierarchyTableOffset = reader.ReadInt64(); // Always 0x68.
            ulong NodeCount = reader.ReadUInt64();
            reader.JumpAhead(0x10); // Always the same as NodeCount, followed by eight nulls.
            long StringTableOffset = reader.ReadInt64();
            reader.JumpAhead(0x18); // Both values here are the same as NodeCount, followed by eight nulls.
            long DataOffset = reader.ReadInt64();
            reader.JumpAhead(0x18); // Both values here are the same as NodeCount, followed by eight nulls.

            // Read this skeleton's node hierarchy.
            // Jump to the hierarchy table (should already be here but just to be safe).
            reader.JumpTo(HierarchyTableOffset, false);

            // Read each Node's Parent Index. Also store the index of the node itself for reference.
            for (ulong i = 0; i < NodeCount; i++)
            {
                Node node = new()
                {
                    Index = (ushort)i,
                    ParentNodeIndex = reader.ReadUInt16()
                };
                Nodes.Add(node);
            }

            // Realign the padding.
            reader.FixPadding(0x4);

            // Read the string table.
            // Jump to the string table. (should already be here but just to be safe).
            reader.JumpTo(StringTableOffset, false);

            for (ulong i = 0; i < NodeCount; i++)
            {
                // Read the offset to this node's name.
                long stringOffset = reader.ReadInt64();

                // Skip the next eight bytes.
                reader.JumpAhead(0x8);

                // Save our current position.
                long pos = reader.BaseStream.Position;

                // Jump to and read the name of this node.
                reader.JumpTo(stringOffset, false);
                Nodes[(int)i].Name = reader.ReadNullTerminatedString();

                // Jump back.
                reader.JumpTo(pos);

                // Fill in the name of the Parent Node for reference.
                if (Nodes[(int)i].ParentNodeIndex != 0xFFFF)
                    Nodes[(int)i].ParentNodeName = Nodes[Nodes[(int)i].ParentNodeIndex].Name;
            }

            // Read whatever data is at the UnknownOffset.
            // Jump to the UnknownOffset (should already be here but just to be safe).
            reader.JumpTo(DataOffset, false);

            // TODO: Examine this data.
            for (ulong i = 0; i < NodeCount; i++)
            {
                Nodes[(int)i].UnknownFloats[0] = reader.ReadSingle(); // Position X?
                Nodes[(int)i].UnknownFloats[1] = reader.ReadSingle(); // Position Y?
                Nodes[(int)i].UnknownFloats[2] = reader.ReadSingle(); // Position Z?
                Nodes[(int)i].UnknownFloats[3] = reader.ReadSingle();
                Nodes[(int)i].UnknownFloats[4] = reader.ReadSingle();
                Nodes[(int)i].UnknownFloats[5] = reader.ReadSingle();
                Nodes[(int)i].UnknownFloats[6] = reader.ReadSingle();
                Nodes[(int)i].UnknownFloats[7] = reader.ReadSingle();
                Nodes[(int)i].UnknownFloats[8] = reader.ReadSingle();
                Nodes[(int)i].UnknownFloats[9] = reader.ReadSingle();
                Nodes[(int)i].UnknownFloats[10] = reader.ReadSingle();
                Nodes[(int)i].UnknownFloats[11] = reader.ReadSingle();
            }
        }

        // TODO: Comment this and tidy it up.
        public override void Save(Stream stream)
        {
            // Set up our BINAWriter and write the BINAV2 header.
            HedgeLib.IO.BINAWriter writer = new(stream, Header);

            writer.WriteSignature(Signature);
            writer.Write(512);
            writer.AddOffset("NodeTableOffset", 8);
            writer.Write((long)Nodes.Count);
            writer.Write((long)Nodes.Count);
            writer.WriteNulls(0x8);
            writer.AddOffset("StringTableOffset", 8);
            writer.Write((long)Nodes.Count);
            writer.Write((long)Nodes.Count);
            writer.WriteNulls(0x8);
            writer.AddOffset("UnknownDataOffset", 8);
            writer.Write((long)Nodes.Count);
            writer.Write((long)Nodes.Count);
            writer.WriteNulls(0x8);

            writer.FillInOffsetLong($"NodeTableOffset", false, false);
            for (int i = 0; i < Nodes.Count; i++)
                writer.Write(Nodes[i].ParentNodeIndex);
            writer.FixPadding(0x4);

            writer.FillInOffsetLong($"StringTableOffset", false, false);
            for (int i = 0; i < Nodes.Count; i++)
            {
                writer.AddString($"Node{i}Name", Nodes[i].Name, 8);
                writer.WriteNulls(0x8);
            }

            writer.FillInOffsetLong($"UnknownDataOffset", false, false);
            for (int i = 0; i < Nodes.Count; i++)
            {
                writer.Write(Nodes[i].UnknownFloats[0]);
                writer.Write(Nodes[i].UnknownFloats[1]);
                writer.Write(Nodes[i].UnknownFloats[2]);
                writer.Write(Nodes[i].UnknownFloats[3]);
                writer.Write(Nodes[i].UnknownFloats[4]);
                writer.Write(Nodes[i].UnknownFloats[5]);
                writer.Write(Nodes[i].UnknownFloats[6]);
                writer.Write(Nodes[i].UnknownFloats[7]);
                writer.Write(Nodes[i].UnknownFloats[8]);
                writer.Write(Nodes[i].UnknownFloats[9]);
                writer.Write(Nodes[i].UnknownFloats[10]);
                writer.Write(Nodes[i].UnknownFloats[11]);
            }


            writer.FinishWrite(Header);
        }
    }
}
