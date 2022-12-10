using HedgeLib.Headers;

namespace Knuxs_Misc_Tools.SonicRangers
{
    internal class PhysicsBasedAnimation : FileBase
    {
        public class Node
        {
            public string Bone { get; set; } = "";

            public string? OtherBone { get; set; }

            public Vector3 UnknownVector3_1 { get; set; }

            public Vector3 UnknownVector3_2 { get; set; }

            public Vector3 UnknownVector3_3 { get; set; }

            public Vector3 UnknownVector3_4 { get; set; }

            public Vector3 UnknownVector3_5 { get; set; }

            public Vector3 UnknownVector3_6 { get; set; }

            public byte[]? PlaceholderBytes { get; set; }
        }

        public List<Node> Nodes = new();

        // Set up HedgeLib#'s BINAV2Header.
        public HedgeLib.Headers.BINAHeader Header = new BINAv2Header(210);

        // Set up the Signature we expect.
        public new const string Signature = "PBA ";

        public override void Load(Stream stream)
        {
            // Set up our BINAReader and read the BINAV2 header.
            HedgeLib.IO.BINAReader reader = new(stream);
            Header = reader.ReadHeader();

            // Check this file's signature.
            string signature = reader.ReadSignature();
            if (signature != Signature)
                throw new Exception($"Invalid signature, got '{signature}', expected '{Signature}'.");

            reader.JumpAhead(0x4); // Always 1, version?
            long StringTableOffset = reader.ReadInt64(); // Maybe actually a reference to the root node of the skeleton?

            uint UnknownCount1 = reader.ReadUInt32();
            uint UnknownCount2 = reader.ReadUInt32();

            long UnknownOffset_1 = reader.ReadInt64();
            long UnknownOffset_2 = reader.ReadInt64(); // Not used by Sage.
            ulong UnknownCount3 = reader.ReadUInt64(); // Only used by Sage.
            long UnknownOffset_3 = reader.ReadInt64(); // Only used by Sage.
            reader.JumpAhead(0x8); // Always 0.

            reader.JumpTo(UnknownOffset_1, false); // Should already be here but just to be safe.
            for (int i = 0; i < UnknownCount1; i++)
            {
                Node node = new();

                long BoneNameOffset = reader.ReadInt64();
                node.UnknownVector3_1 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                node.UnknownVector3_2 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                node.UnknownVector3_3 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                node.UnknownVector3_4 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                node.UnknownVector3_5 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                node.UnknownVector3_6 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                long pos = reader.BaseStream.Position;

                reader.JumpTo(BoneNameOffset, false);

                node.Bone = reader.ReadNullTerminatedString();

                Nodes.Add(node);

                reader.JumpTo(pos);
            }

            reader.JumpTo(UnknownOffset_2, false); // Should already be here but just to be safe.
            for (int i = 0; i < UnknownCount2; i++)
            {
                long BoneNameOffset = reader.ReadInt64();
                Nodes[i].PlaceholderBytes = reader.ReadBytes(0xC8); // TODO: Read these properly.

                long pos = reader.BaseStream.Position;

                reader.JumpTo(BoneNameOffset, false);

                string bone = reader.ReadNullTerminatedString();
                if (bone != Nodes[i].Bone)
                    Nodes[i].OtherBone = bone;

                reader.JumpTo(pos);
            }

            // TODO: Finish this, only in Sage's file so it's a BITCH to RE.
            reader.JumpTo(UnknownOffset_3, false);
            for (ulong i = 0; i < UnknownCount3; i++)
            {
                long BoneNameOffset = reader.ReadInt64();
                Vector3 UnknownVector3_1 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Vector3 UnknownVector3_2 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Vector3 UnknownVector3_3 = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                float UnknownFloat_1 = reader.ReadSingle();
                float UnknownFloat_2 = reader.ReadSingle(); // Might not be a float?
                uint UnknownCount_4 = reader.ReadUInt32();
                long UnknownOffset_4 = reader.ReadInt64(); // Points behind??????
                long UnknownOffset_5 = reader.ReadInt64();
                long UnknownOffset_6 = reader.ReadInt64();
            }
        }
    }
}
