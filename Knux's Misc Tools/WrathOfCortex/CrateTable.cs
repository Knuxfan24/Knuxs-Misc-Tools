namespace Knuxs_Misc_Tools.WrathOfCortex
{
    public class CrateTable : FileBase
    {
        public List<Group> Groups = new();

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream, true);
            reader.ReadSignature(4);
            ushort GroupCount = reader.ReadUInt16();

            for (int i = 0; i < GroupCount; i++)
            {
                Group group = new();
                Crate crate = new();

                crate.Position = reader.ReadVector3();
                group.ID = reader.ReadUInt16();
                ushort crateCount = reader.ReadUInt16(); // Including this first crate that's mixed in here.
                group.UnknownUShort_1 = reader.ReadUInt16();
                group.Rotation = reader.ReadVector3();
                reader.JumpAhead(0x4); // Always 0.
                crate.UnknownByte_1 = reader.ReadByte();
                crate.UnknownByte_2 = reader.ReadByte();
                crate.UnknownByte_3 = reader.ReadByte();
                crate.UnknownByte_4 = reader.ReadByte();
                crate.UnknownByte_5 = reader.ReadByte();
                crate.UnknownByte_6 = reader.ReadByte();
                crate.Type1 = reader.ReadByte();
                crate.TimeTrialType = reader.ReadByte();
                crate.Type3 = reader.ReadByte();
                crate.Type4 = reader.ReadByte();
                crate.UnknownByte_7 = reader.ReadByte();
                crate.UnknownByte_8 = reader.ReadByte();
                crate.UnknownByte_9 = reader.ReadByte();
                crate.UnknownByte_10 = reader.ReadByte();
                crate.UnknownByte_11 = reader.ReadByte();
                crate.UnknownByte_12 = reader.ReadByte();
                crate.UnknownByte_13 = reader.ReadByte();
                crate.UnknownByte_14 = reader.ReadByte();
                crate.UnknownByte_15 = reader.ReadByte();
                crate.UnknownByte_16 = reader.ReadByte();
                crate.UnknownByte_17 = reader.ReadByte();
                crate.UnknownByte_18 = reader.ReadByte();
                crate.UnknownByte_19 = reader.ReadByte();
                crate.UnknownByte_20 = reader.ReadByte();
                group.Crates.Add(crate);

                for (int c = 0; c < crateCount - 1; c++)
                {
                    crate = new();
                    crate.Position = reader.ReadVector3();
                    reader.JumpAhead(0x4); // Always 0.
                    crate.UnknownByte_1 = reader.ReadByte();
                    crate.UnknownByte_2 = reader.ReadByte();
                    crate.UnknownByte_3 = reader.ReadByte();
                    crate.UnknownByte_4 = reader.ReadByte();
                    crate.UnknownByte_5 = reader.ReadByte();
                    crate.UnknownByte_6 = reader.ReadByte();
                    crate.Type1 = reader.ReadByte();
                    crate.TimeTrialType = reader.ReadByte();
                    crate.Type3 = reader.ReadByte();
                    crate.Type4 = reader.ReadByte();
                    crate.UnknownByte_7 = reader.ReadByte();
                    crate.UnknownByte_8 = reader.ReadByte();
                    crate.UnknownByte_9 = reader.ReadByte();
                    crate.UnknownByte_10 = reader.ReadByte();
                    crate.UnknownByte_11 = reader.ReadByte();
                    crate.UnknownByte_12 = reader.ReadByte();
                    crate.UnknownByte_13 = reader.ReadByte();
                    crate.UnknownByte_14 = reader.ReadByte();
                    crate.UnknownByte_15 = reader.ReadByte();
                    crate.UnknownByte_16 = reader.ReadByte();
                    crate.UnknownByte_17 = reader.ReadByte();
                    crate.UnknownByte_18 = reader.ReadByte();
                    crate.UnknownByte_19 = reader.ReadByte();
                    crate.UnknownByte_20 = reader.ReadByte();
                    group.Crates.Add(crate);
                }

                Groups.Add(group);
            }
        }

        public override void Save(Stream stream)
        {
            // Set up the writer.
            BinaryWriterEx writer = new(stream, true);

            writer.Write(0x4);
            writer.Write((ushort)Groups.Count);

            for (int i = 0; i < Groups.Count; i++)
            {
                writer.Write(Groups[i].Crates[0].Position);
                writer.Write(Groups[i].ID);
                writer.Write((ushort)Groups[i].Crates.Count);
                writer.Write(Groups[i].UnknownUShort_1);
                writer.Write(Groups[i].Rotation);
                writer.WriteNulls(0x4);
                writer.Write(Groups[i].Crates[0].UnknownByte_1);
                writer.Write(Groups[i].Crates[0].UnknownByte_2);
                writer.Write(Groups[i].Crates[0].UnknownByte_3);
                writer.Write(Groups[i].Crates[0].UnknownByte_4);
                writer.Write(Groups[i].Crates[0].UnknownByte_5);
                writer.Write(Groups[i].Crates[0].UnknownByte_6);
                writer.Write(Groups[i].Crates[0].Type1);
                writer.Write(Groups[i].Crates[0].TimeTrialType);
                writer.Write(Groups[i].Crates[0].Type3);
                writer.Write(Groups[i].Crates[0].Type4);
                writer.Write(Groups[i].Crates[0].UnknownByte_7);
                writer.Write(Groups[i].Crates[0].UnknownByte_8);
                writer.Write(Groups[i].Crates[0].UnknownByte_9);
                writer.Write(Groups[i].Crates[0].UnknownByte_10);
                writer.Write(Groups[i].Crates[0].UnknownByte_11);
                writer.Write(Groups[i].Crates[0].UnknownByte_12);
                writer.Write(Groups[i].Crates[0].UnknownByte_13);
                writer.Write(Groups[i].Crates[0].UnknownByte_14);
                writer.Write(Groups[i].Crates[0].UnknownByte_15);
                writer.Write(Groups[i].Crates[0].UnknownByte_16);
                writer.Write(Groups[i].Crates[0].UnknownByte_17);
                writer.Write(Groups[i].Crates[0].UnknownByte_18);
                writer.Write(Groups[i].Crates[0].UnknownByte_19);
                writer.Write(Groups[i].Crates[0].UnknownByte_20);

                for (int c = 1; c < Groups[i].Crates.Count; c++)
                {
                    writer.Write(Groups[i].Crates[c].Position);
                    writer.WriteNulls(0x4);
                    writer.Write(Groups[i].Crates[c].UnknownByte_1);
                    writer.Write(Groups[i].Crates[c].UnknownByte_2);
                    writer.Write(Groups[i].Crates[c].UnknownByte_3);
                    writer.Write(Groups[i].Crates[c].UnknownByte_4);
                    writer.Write(Groups[i].Crates[c].UnknownByte_5);
                    writer.Write(Groups[i].Crates[c].UnknownByte_6);
                    writer.Write(Groups[i].Crates[c].Type1);
                    writer.Write(Groups[i].Crates[c].TimeTrialType);
                    writer.Write(Groups[i].Crates[c].Type3);
                    writer.Write(Groups[i].Crates[c].Type4);
                    writer.Write(Groups[i].Crates[c].UnknownByte_7);
                    writer.Write(Groups[i].Crates[c].UnknownByte_8);
                    writer.Write(Groups[i].Crates[c].UnknownByte_9);
                    writer.Write(Groups[i].Crates[c].UnknownByte_10);
                    writer.Write(Groups[i].Crates[c].UnknownByte_11);
                    writer.Write(Groups[i].Crates[c].UnknownByte_12);
                    writer.Write(Groups[i].Crates[c].UnknownByte_13);
                    writer.Write(Groups[i].Crates[c].UnknownByte_14);
                    writer.Write(Groups[i].Crates[c].UnknownByte_15);
                    writer.Write(Groups[i].Crates[c].UnknownByte_16);
                    writer.Write(Groups[i].Crates[c].UnknownByte_17);
                    writer.Write(Groups[i].Crates[c].UnknownByte_18);
                    writer.Write(Groups[i].Crates[c].UnknownByte_19);
                    writer.Write(Groups[i].Crates[c].UnknownByte_20);
                }
            }
        }

        public class Group
        {
            public ushort ID { get; set; }

            public ushort UnknownUShort_1 { get; set; }

            public Vector3 Rotation { get; set; }

            public List<Crate> Crates { get; set; } = new();
        }

        public class Crate
        {
            public Vector3 Position { get; set; }
            public byte UnknownByte_1 { get; set; }
            public byte UnknownByte_2 { get; set; }
            public byte UnknownByte_3 { get; set; }
            public byte UnknownByte_4 { get; set; }
            public byte UnknownByte_5 { get; set; }
            public byte UnknownByte_6 { get; set; }
            public byte Type1 { get; set; }
            public byte TimeTrialType { get; set; }
            public byte Type3 { get; set; } // Something to do with Switch Crates maybe? Might also be to do with Roulette Crates?
            public byte Type4 { get; set; } // Something to do with Switch Crates in Time Trial maybe?
            public byte UnknownByte_7 { get; set; }
            public byte UnknownByte_8 { get; set; }
            public byte UnknownByte_9 { get; set; }
            public byte UnknownByte_10 { get; set; }
            public byte UnknownByte_11 { get; set; }
            public byte UnknownByte_12 { get; set; }
            public byte UnknownByte_13 { get; set; }
            public byte UnknownByte_14 { get; set; }
            public byte UnknownByte_15 { get; set; }
            public byte UnknownByte_16 { get; set; }
            public byte UnknownByte_17 { get; set; }
            public byte UnknownByte_18 { get; set; }
            public byte UnknownByte_19 { get; set; }
            public byte UnknownByte_20 { get; set; }
        }
    }
}
