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

        public class FormatData
        {
            public string RootBone { get; set; } = "";

            public List<Node> Nodes = new();
        }

        public FormatData Data = new();

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

            long RootBoneNameOffset = reader.ReadInt64();
            long pos = reader.BaseStream.Position;
            reader.JumpTo(RootBoneNameOffset, false);
            Data.RootBone = reader.ReadNullTerminatedString();
            reader.JumpTo(pos);

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

                pos = reader.BaseStream.Position;

                reader.JumpTo(BoneNameOffset, false);

                node.Bone = reader.ReadNullTerminatedString();

                Data.Nodes.Add(node);

                reader.JumpTo(pos);
            }

            reader.JumpTo(UnknownOffset_2, false); // Should already be here but just to be safe.
            for (int i = 0; i < UnknownCount2; i++)
            {
                long BoneNameOffset = reader.ReadInt64();
                Data.Nodes[i].PlaceholderBytes = reader.ReadBytes(0xC8); // TODO: Read these properly.

                pos = reader.BaseStream.Position;

                reader.JumpTo(BoneNameOffset, false);

                string bone = reader.ReadNullTerminatedString();
                if (bone != Data.Nodes[i].Bone)
                    Data.Nodes[i].OtherBone = bone;

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

        public override void Save(Stream stream)
        {
            // Set up our BINAWriter and write the BINAV2 header.
            HedgeLib.IO.BINAWriter writer = new(stream, Header);
            // Wrie this file's data table.
            writer.WriteSignature(Signature);
            writer.Write(1u);
            writer.AddString("RootBoneName", Data.RootBone, 8);
            writer.Write(Data.Nodes.Count);
            int count2 = 0;

            foreach (var node in Data.Nodes)
                if (node.PlaceholderBytes != null)
                    count2++;
            writer.Write(count2);

            writer.AddOffset("UnknownOffset_1", 8);
            writer.AddOffset("UnknownOffset_2", 8);

            // TODO: Write these.
            writer.Write(0L);
            writer.Write(0L);
            writer.WriteNulls(0x8);

            writer.FillInOffset("UnknownOffset_1", false);
            for (int i = 0; i < Data.Nodes.Count; i++)
            {
                writer.AddString($"Node{i}BoneName", Data.Nodes[i].Bone, 8);
                writer.Write(Data.Nodes[i].UnknownVector3_1.X);
                writer.Write(Data.Nodes[i].UnknownVector3_1.Y);
                writer.Write(Data.Nodes[i].UnknownVector3_1.Z);
                writer.Write(Data.Nodes[i].UnknownVector3_2.X);
                writer.Write(Data.Nodes[i].UnknownVector3_2.Y);
                writer.Write(Data.Nodes[i].UnknownVector3_2.Z);
                writer.Write(Data.Nodes[i].UnknownVector3_3.X);
                writer.Write(Data.Nodes[i].UnknownVector3_3.Y);
                writer.Write(Data.Nodes[i].UnknownVector3_3.Z);
                writer.Write(Data.Nodes[i].UnknownVector3_4.X);
                writer.Write(Data.Nodes[i].UnknownVector3_4.Y);
                writer.Write(Data.Nodes[i].UnknownVector3_4.Z);
                writer.Write(Data.Nodes[i].UnknownVector3_5.X);
                writer.Write(Data.Nodes[i].UnknownVector3_5.Y);
                writer.Write(Data.Nodes[i].UnknownVector3_5.Z);
                writer.Write(Data.Nodes[i].UnknownVector3_6.X);
                writer.Write(Data.Nodes[i].UnknownVector3_6.Y);
                writer.Write(Data.Nodes[i].UnknownVector3_6.Z);
            }

            writer.FillInOffset("UnknownOffset_2", false);
            for (int i = 0; i < Data.Nodes.Count; i++)
            {
                if (Data.Nodes[i].PlaceholderBytes == null)
                    continue;

                if (Data.Nodes[i].OtherBone != null)
                    writer.AddString($"Node{i}OtherBoneName", Data.Nodes[i].OtherBone, 8);
                else
                    writer.AddString($"Node{i}OtherBoneName", Data.Nodes[i].Bone, 8);

                writer.Write(Data.Nodes[i].PlaceholderBytes);
            }

            writer.FinishWrite(Header);
        }
    }
}
