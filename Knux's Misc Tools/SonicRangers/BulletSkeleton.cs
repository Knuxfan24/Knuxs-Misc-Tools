using HedgeLib.Headers;

namespace Knuxs_Misc_Tools.SonicRangers
{
    public class Node
    {
        public ushort Index { get; set; } = 0xFFFF; // Not actually a thing, used for easier ID in JSON.

        public ushort ParentNodeIndex { get; set; }

        public string Name { get; set; } = "";

        public string? ParentNodeName { get; set; } // Not actually a thing, used for easier ID in JSON.

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Scale { get; set; }
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

            // Read this file's data table.
            reader.JumpAhead(0x4); // Always 00 02 00 00.
            long HierarchyTableOffset = reader.ReadInt64(); // Always 0x68.
            ulong NodeCount = reader.ReadUInt64();
            reader.JumpAhead(0x10); // Always the same as NodeCount, followed by eight nulls.
            long StringTableOffset = reader.ReadInt64();
            reader.JumpAhead(0x18); // Both values here are the same as NodeCount, followed by eight nulls.
            long TransformTableOffset = reader.ReadInt64();
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

            // Read the transform table.
            // Jump to the transform table (should already be here but just to be safe).
            reader.JumpTo(TransformTableOffset, false);

            // TODO: Confirm this data.
            for (ulong i = 0; i < NodeCount; i++)
            {
                Nodes[(int)i].Position = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                reader.JumpAhead(0x4);
                Nodes[(int)i].Rotation = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Nodes[(int)i].Scale = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                reader.JumpAhead(0x4);
            }
        }

        public override void Save(Stream stream)
        {
            // Set up our BINAWriter and write the BINAV2 header.
            HedgeLib.IO.BINAWriter writer = new(stream, Header);

            // Wrie this file's data table.
            writer.WriteSignature(Signature);
            writer.Write(0x200);
            writer.AddOffset("HierarchyTableOffset", 8);
            writer.Write((long)Nodes.Count);
            writer.Write((long)Nodes.Count);
            writer.WriteNulls(0x8);
            writer.AddOffset("StringTableOffset", 8);
            writer.Write((long)Nodes.Count);
            writer.Write((long)Nodes.Count);
            writer.WriteNulls(0x8);
            writer.AddOffset("TransformTableOffset", 8);
            writer.Write((long)Nodes.Count);
            writer.Write((long)Nodes.Count);
            writer.WriteNulls(0x8);

            // Write this file's parent index table.
            writer.FillInOffsetLong($"HierarchyTableOffset", false, false);
            for (int i = 0; i < Nodes.Count; i++)
                writer.Write(Nodes[i].ParentNodeIndex);
            writer.FixPadding(0x4);

            // Write this file's string offset table.
            writer.FillInOffsetLong($"StringTableOffset", false, false);
            for (int i = 0; i < Nodes.Count; i++)
            {
                writer.AddString($"Node{i}Name", Nodes[i].Name, 8);
                writer.WriteNulls(0x8);
            }

            // Write this file's transform data.
            writer.FillInOffsetLong($"TransformTableOffset", false, false);
            for (int i = 0; i < Nodes.Count; i++)
            {
                writer.Write(Nodes[i].Position.X);
                writer.Write(Nodes[i].Position.Y);
                writer.Write(Nodes[i].Position.Z);
                writer.WriteNulls(0x4);
                writer.Write(Nodes[i].Rotation.X);
                writer.Write(Nodes[i].Rotation.Y);
                writer.Write(Nodes[i].Rotation.Z);
                writer.Write(Nodes[i].Rotation.W);
                writer.Write(Nodes[i].Scale.X);
                writer.Write(Nodes[i].Scale.Y);
                writer.Write(Nodes[i].Scale.Z);
                writer.WriteNulls(0x4);
            }

            // Finish writing the BINA information.
            writer.FinishWrite(Header);
        }
    }
}
