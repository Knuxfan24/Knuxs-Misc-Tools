using HedgeLib.Headers;

namespace Knuxs_Misc_Tools.SonicRangers
{
    public class Node
    {
        public ushort Index { get; set; } // Almost certainly the wrong term for this.

        public string Name { get; set; } = "";

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

            // Read this file's header(?).
            reader.JumpAhead(0x4); // Always 00 02 00 00.
            long NodeTable = reader.ReadInt64(); // Always 0x68.
            ulong NodeCount = reader.ReadUInt64();
            reader.JumpAhead(0x10); // Always the same as NodeCount, followed by eight nulls.
            long StringTableOffset = reader.ReadInt64();
            reader.JumpAhead(0x18); // Both values here are the same as NodeCount, followed by eight nulls.
            long UnknownOffset = reader.ReadInt64();
            reader.JumpAhead(0x18); // Both values here are the same as NodeCount, followed by eight nulls.

            // Read this skeleton's node table.
            // Jump to the node table (should already be here but just to be safe).
            reader.JumpTo(NodeTable, false);

            // TODO: Properly name this. Index feels way off.
            for (ulong i = 0; i < NodeCount; i++)
            {
                Node node = new() { Index = reader.ReadUInt16() };
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
            }

            // Read whatever data is at the UnknownOffset.
            // Jump to the UnknownOffset (should already be here but just to be safe).
            reader.JumpTo(UnknownOffset, false);

            // TODO: Examine this data.
            for (ulong i = 0; i < NodeCount; i++)
            {
                Nodes[(int)i].UnknownFloats[0] = reader.ReadSingle();
                Nodes[(int)i].UnknownFloats[1] = reader.ReadSingle();
                Nodes[(int)i].UnknownFloats[2] = reader.ReadSingle();
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
    }
}
