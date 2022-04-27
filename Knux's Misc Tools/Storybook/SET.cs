// TODO: Literally all of this code feels like shit. What is half the data here and what does it do?

namespace Knuxs_Misc_Tools.Storybook
{
    public class SetObject
    {
        public Vector3 Position { get; set; }
        public uint UnknownUInt32_1 { get; set; }

        public uint UnknownUInt32_2 { get; set; } // Potentially a BAMs rotation? Need to check it more.
        public uint UnknownUInt32_3 { get; set; }
        public byte UnknownByte_1 { get; set; } // Can cause the game to stop loading every other object after if set incorrectly???
        public byte UnknownByte_2 { get; set; } 
        public byte UnknownByte_3 { get; set; } 
        public byte UnknownByte_4 { get; set; } 

        public byte UnknownByte_5 { get; set; }
        public byte UnknownByte_6 { get; set; } // Object Table? Or Draw Distance maybe??????????
        public byte UnknownByte_7 { get; set; } // Object Type?
        public byte UnknownByte_8 { get; set; }

        public uint UnknownUInt32_6 { get; set; }
        public uint UnknownUInt32_7 { get; set; } // Always 0
        public uint UnknownUInt32_8 { get; set; }
        public uint UnknownUInt32_9 { get; set; } // Something to do with Parameters???????
    }

    // Oh god why this is not good at all yikes.
    public class SetParameter
    {
        public List<uint> Parameter_UInts { get; set; } = new();

        public List<float> Parameter_Floats { get; set; } = new();
    }

    public class FormatData
    {
        public uint UnknownUInt32_1 { get; set; } // Feels like something that shouldn't be hardcoded???

        public List<SetObject> Objects { get; set; } = new();

        public List<SetParameter> Parameters { get; set; } = new();
    }

    public class SET : FileBase
    {
        public FormatData Data = new();

        public override void Load(string filepath)
        {
            BinaryReaderEx reader = new(File.OpenRead(filepath));

            string sig = reader.ReadNullPaddedString(0x4);

            if (!sig.StartsWith("ST"))
                throw new Exception($"'{filepath}' does not appear to be a Sonic and the Secret Rings or Sonic and the Black Knight SET file.");

            uint objectCount = reader.ReadUInt32();
            uint parameterCount = reader.ReadUInt32(); // I think.
            Data.UnknownUInt32_1 = reader.ReadUInt32(); // Important in some way, but what the fuck is it?

            for (int i = 0; i < objectCount; i++)
            {
                SetObject obj = new()
                {
                    Position = reader.ReadVector3(),
                    UnknownUInt32_1 = reader.ReadUInt32(),
                    UnknownUInt32_2 = reader.ReadUInt32(),
                    UnknownUInt32_3 = reader.ReadUInt32(),
                    UnknownByte_1 = reader.ReadByte(),
                    UnknownByte_2 = reader.ReadByte(),
                    UnknownByte_3 = reader.ReadByte(),
                    UnknownByte_4 = reader.ReadByte(),
                    UnknownByte_5 = reader.ReadByte(),
                    UnknownByte_6 = reader.ReadByte(),
                    UnknownByte_7 = reader.ReadByte(),
                    UnknownByte_8 = reader.ReadByte(),
                    UnknownUInt32_6 = reader.ReadUInt32(),
                    UnknownUInt32_7 = reader.ReadUInt32(),
                    UnknownUInt32_8 = reader.ReadUInt32(),
                    UnknownUInt32_9 = reader.ReadUInt32()
                };
                Data.Objects.Add(obj);
            }

            Dictionary<int, byte> ParameterLengths = new();
            for (int i = 0; i < parameterCount; i++)
            {
                reader.JumpAhead(); // Always 1.
                reader.JumpAhead(); // Always 0.
                ParameterLengths.Add(i, reader.ReadByte()); // TODO: Verify this always adds up to the remaining space in the file (minus any padding).
                reader.JumpAhead(); // Always 0.
                reader.JumpAhead(0x4); // Always 0.
            }

            for (int i = 0; i < parameterCount; i++)
            {
                SetParameter parameter = new();
                for (int p = 0; p < ParameterLengths[i] / 4; p++)
                {
                    parameter.Parameter_UInts.Add(reader.ReadUInt32());
                    reader.JumpBehind(0x4);
                    parameter.Parameter_Floats.Add(reader.ReadSingle());
                }
                Data.Parameters.Add(parameter);
            }

            reader.Close();
        }

        public void Save(string filepath)
        {
            // Set up the writer.
            File.Delete(filepath);
            BinaryWriterEx writer = new(File.OpenWrite(filepath));

            writer.Write("STKX"); // Does this actually do anything? Could I just drop whatever dumb shit I want here? lmao worked????
            writer.Write(Data.Objects.Count);
            writer.Write(Data.Parameters.Count);
            writer.Write(Data.UnknownUInt32_1);

            for (int i = 0; i < Data.Objects.Count; i++)
            {
                writer.Write(Data.Objects[i].Position);
                writer.Write(Data.Objects[i].UnknownUInt32_1);
                writer.Write(Data.Objects[i].UnknownUInt32_2);
                writer.Write(Data.Objects[i].UnknownUInt32_3);
                writer.Write(Data.Objects[i].UnknownByte_1);
                writer.Write(Data.Objects[i].UnknownByte_2);
                writer.Write(Data.Objects[i].UnknownByte_3);
                writer.Write(Data.Objects[i].UnknownByte_4);
                writer.Write(Data.Objects[i].UnknownByte_5);
                writer.Write(Data.Objects[i].UnknownByte_6);
                writer.Write(Data.Objects[i].UnknownByte_7);
                writer.Write(Data.Objects[i].UnknownByte_8);
                writer.Write(Data.Objects[i].UnknownUInt32_6);
                writer.Write(Data.Objects[i].UnknownUInt32_7);
                writer.Write(Data.Objects[i].UnknownUInt32_8);
                writer.Write(Data.Objects[i].UnknownUInt32_9);
            }

            for (int i = 0; i < Data.Parameters.Count; i++)
            {
                writer.Write((byte)1);
                writer.Write((byte)0);
                writer.Write((byte)(Data.Parameters[i].Parameter_UInts.Count * 4));
                writer.Write((byte)0);
                writer.WriteNulls(0x4);
            }

            for (int i = 0; i < Data.Parameters.Count; i++)
            {
                foreach (var param in Data.Parameters[i].Parameter_UInts)
                {
                    writer.Write(param);
                }
            }

            writer.FixPadding(0x10); // TODO, are all files 0x10 aligned?
            writer.Close();
        }
    }
}
