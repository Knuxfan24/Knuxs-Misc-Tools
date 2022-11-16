using HedgeLib.Headers;

namespace Knuxs_Misc_Tools.SonicRangers
{
    internal class BulletInstance : FileBase
    {
        public class Instance
        {
            public string Name1 { get; set; } = "";

            public string Name2 { get; set; } = "";

            public Vector3 Position { get; set; }

            public Vector3 Rotation { get; set; }

            public uint UnknownUInt32_1 = 1;

            public Vector3 Scale { get; set; }

            public uint UnknownUInt32_2 = 0;
        }

        // Set up HedgeLib#'s BINAV2Header.
        public HedgeLib.Headers.BINAHeader Header = new BINAv2Header(210);

        // Set up the Signature we expect.
        public new const string Signature = "CPIC";

        public List<Instance> Instances = new();

        public override void Load(Stream stream)
        {
            // Set up our BINAReader and read the BINAV2 header.
            HedgeLib.IO.BINAReader reader = new(stream);
            Header = reader.ReadHeader();

            // Check this file's signature.
            string signature = reader.ReadSignature();
            if (signature != Signature)
                throw new Exception($"Invalid signature, got '{signature}', expected '{Signature}'.");

            // Check this file version. Always 2?
            uint version = reader.ReadUInt32();

            // Get the 64 bit offset to the Instance Table and the count of the instances in it.
            long instanceTableOffset = reader.ReadInt64();
            ulong instanceCount = reader.ReadUInt64();

            // Jump to the instance table (should already be here but lets play it safe).
            reader.JumpTo(instanceTableOffset, false);

            // Loop through each instance.
            for (ulong i = 0; i < instanceCount; i++)
            {
                Instance inst = new();

                // Get the name offsets for this instance.
                long name1Offset = reader.ReadInt64();
                long name2Offset = reader.ReadInt64();

                // Read the data of this instance.
                // Vector3s are done this way as HedgeLib# had a custom Vector3 thing for some reason???
                inst.Position = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                inst.Rotation = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                inst.UnknownUInt32_1 = reader.ReadUInt32();
                inst.Scale = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                inst.UnknownUInt32_2 = reader.ReadUInt32();

                // Realign for the next instance.
                reader.FixPadding(0x8);

                // Save our current position.
                long pos = reader.BaseStream.Position;

                // Jump to and read this instance's names.
                reader.JumpTo(name1Offset, false);
                inst.Name1 = reader.ReadNullTerminatedString();

                reader.JumpTo(name2Offset, false);
                inst.Name2 = reader.ReadNullTerminatedString();

                // Jump back for the next instance.
                reader.JumpTo(pos);

                // Save this instance.
                Instances.Add(inst);
            }
        }

        public override void Save(Stream stream)
        {
            // Set up our BINAWriter and write the BINAV2 header.
            HedgeLib.IO.BINAWriter writer = new(stream, Header);

            // Write our file signature.
            writer.WriteSignature(Signature);

            // Write the file version.
            writer.Write(2);

            // Set up the BINA Offset Table.
            writer.AddOffsetTable("instanceOffset", (uint)Instances.Count, 8);

            // Write how many Instances we have.
            writer.Write((ulong)Instances.Count);

            // Loop through each instance.
            for (int i = 0; i < Instances.Count; i++)
            {
                // Fill in the Offset in the Offset Table for this instance.
                writer.FillInOffsetLong($"instanceOffset_{i}", false, false);

                // Add the two strings to the BINA String Table and write their offsets.
                writer.AddString($"instance{i}name1", Instances[i].Name1, 8);
                writer.AddString($"instance{i}name2", Instances[i].Name2, 8);

                // Write this instance's position.
                writer.Write(Instances[i].Position.X);
                writer.Write(Instances[i].Position.Y);
                writer.Write(Instances[i].Position.Z);

                // Write this instance's rotation.
                writer.Write(Instances[i].Rotation.X);
                writer.Write(Instances[i].Rotation.Y);
                writer.Write(Instances[i].Rotation.Z);

                writer.Write(Instances[i].UnknownUInt32_1);

                // Write this instance's scale.
                writer.Write(Instances[i].Scale.X);
                writer.Write(Instances[i].Scale.Y);
                writer.Write(Instances[i].Scale.Z);

                writer.Write(Instances[i].UnknownUInt32_2);

                // All instances but the last one appear to be aligned, so if this isn't the last instance, align it.
                if (i != Instances.Count - 1)
                    writer.FixPadding(0x8);
            }

            // Finish writing the BINA information.
            writer.FinishWrite(Header);
        }
    }
}
