namespace Knuxs_Misc_Tools.Storybook
{
    internal class SET : FileBase
    {
        public class SetObject
        {
            public Vector3 Position { get; set; }

            public Vector3 Rotation { get; set; } // TODO: Verify if the three values after position are actually rotation in BAMs format.

            // Has three values in Black Knight, 01 seems to imply no parameters, 09 is the most comman and 0D shows up in one object.
            // Secret Rings also has 19 and 89.
            public byte UnknownByte_1 { get; set; }

            public byte UnknownByte_2 { get; set; } // TODO: Find out what this is.

            public byte UnknownByte_3 { get; set; } // TODO: Find out what this is.

            public byte UnknownByte_4 { get; set; } // TODO: Find out what this is.

            public byte UnknownByte_5 { get; set; } // TODO: Find out what this is.

            public byte DrawDistance { get; set; }

            public byte ID { get; set; }

            public byte Table { get; set; }

            public uint UnknownUInt32_1 { get; set; } // TODO: Find out what this is.

            public uint UnknownUInt32_2 { get; set; } // TODO: Find out what this is.

            // TODO: Better way to handle parameters, if there was a data type indicator in there that'd be lovely...
            public List<uint>? Parameters { get; set; } 
            public List<float>? ParametersF { get; set; }

            public string? Name { get; set; } // Not actually a thing these have, filled in by function for ease of research.

            public override string ToString() => Name;
        }

        public class FormatData
        {
            public string Signature { get; set; } = "STP1";

            public List<SetObject> Objects { get; set; } = new();
        }

        public FormatData Data = new();

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream);

            // Read the header.
            Data.Signature = reader.ReadNullPaddedString(0x4);

            if (!Data.Signature.StartsWith("ST"))
                throw new Exception($"'{((FileStream)stream).Name}' does not appear to be a Sonic and the Secret Rings or Sonic and the Black Knight SET file.");

            uint objectCount = reader.ReadUInt32();
            uint parameterCount = reader.ReadUInt32();
            uint parameterDataTableLength = reader.ReadUInt32();

            // Calculate the locations of the Parameters.
            uint parameterTableOffset = (objectCount * 0x30) + 0x10;
            uint parameterDataTableOffset = parameterTableOffset + (parameterCount * 0x8);

            for(int i = 0; i < objectCount; i++)
            {
                // Read the object entry.
                SetObject obj = new();
                obj.Position = reader.ReadVector3();
                obj.Rotation = new(reader.ReadInt32() * 360.0f / 65535.0f, reader.ReadInt32() * 360.0f / 65535.0f, reader.ReadInt32() * 360.0f / 65535.0f);
                obj.UnknownByte_1 = reader.ReadByte();
                obj.UnknownByte_2 = reader.ReadByte();
                obj.UnknownByte_3 = reader.ReadByte();
                obj.UnknownByte_4 = reader.ReadByte();
                obj.UnknownByte_5 = reader.ReadByte();
                obj.DrawDistance = reader.ReadByte();
                obj.ID = reader.ReadByte();
                obj.Table = reader.ReadByte();
                obj.UnknownUInt32_1 = reader.ReadUInt32();
                reader.JumpAhead(0x4); // Always 0.
                obj.UnknownUInt32_2 = reader.ReadUInt32();
                uint parameterIndex = reader.ReadUInt32();

                // Objects with 0x1 in UnknownByte_1 don't have any parameters, so stop reading the object here.
                if (obj.UnknownByte_1 == 0x1)
                {
                    // Save this object.
                    Data.Objects.Add(obj);

                    continue;
                }

                // Save our position to jump back for the next object.
                long pos = reader.BaseStream.Position;

                // Jump to the parameter table.
                reader.JumpTo(parameterTableOffset + 0x2);

                // Calculate the position of this object's parameters data in the data table.
                uint parameterOffset = 0;
                for (int i2 = 0; i2 < parameterIndex; i2++)
                {
                    parameterOffset += reader.ReadByte();
                    reader.JumpAhead(0x7);
                }

                // Calculate how many parameters this object has.
                int objectParameterCount = reader.ReadByte() / 4;

                // Jump to the apporiate location in the parameter data table.
                reader.JumpTo(parameterDataTableOffset + parameterOffset);

                // Read the parameters.
                // TODO: Actually handle their data types. I have no clue if that's even in here (like in '06) or not...
                obj.Parameters = new();
                obj.ParametersF = new();
                for (int p = 0; p < objectParameterCount; p++)
                {
                    obj.Parameters.Add(reader.ReadUInt32());
                    reader.JumpBehind(0x4);
                    obj.ParametersF.Add(reader.ReadSingle());
                }

                // Jump back for our next object.
                reader.JumpTo(pos);

                // Save this object.
                Data.Objects.Add(obj);
            }
        }

        public override void Save(Stream stream)
        {
            BinaryWriterEx writer = new(stream);

            writer.Write(Data.Signature);
            writer.Write(Data.Objects.Count);
            writer.Write("COUT"); // Placeholder value for Parameter Count
            writer.Write("SIZE"); // Placeholder Size value for the Parameter Table Length

            int parameterCount = 0;

            // Write Objects.
            for (int i = 0; i < Data.Objects.Count; i++)
            {
                writer.Write(Data.Objects[i].Position);
                writer.Write((int)(Data.Objects[i].Rotation.X * 65535.0f / 360.0f));
                writer.Write((int)(Data.Objects[i].Rotation.Y * 65535.0f / 360.0f));
                writer.Write((int)(Data.Objects[i].Rotation.Z * 65535.0f / 360.0f));
                writer.Write(Data.Objects[i].UnknownByte_1);
                writer.Write(Data.Objects[i].UnknownByte_2);
                writer.Write(Data.Objects[i].UnknownByte_3);
                writer.Write(Data.Objects[i].UnknownByte_4);
                writer.Write(Data.Objects[i].UnknownByte_5);
                writer.Write(Data.Objects[i].DrawDistance);
                writer.Write(Data.Objects[i].ID);
                writer.Write(Data.Objects[i].Table);
                writer.Write(Data.Objects[i].UnknownUInt32_1);
                writer.Write(0);
                writer.Write(Data.Objects[i].UnknownUInt32_2);
                if (Data.Objects[i].UnknownByte_1 != 0x1)
                {
                    writer.Write(parameterCount);
                    parameterCount++;
                }
                else
                {
                    writer.WriteNulls(0x4);
                }
            }

            // Write Parameter Table.
            for (int i = 0; i < Data.Objects.Count; i++)
            {
                if (Data.Objects[i].UnknownByte_1 != 0x1)
                {
                    writer.Write((byte)0x1);
                    writer.Write((byte)0x0);
                    writer.Write((byte)(Data.Objects[i].Parameters.Count * 4));
                    writer.WriteNulls(0x5);
                }
            }

            // Set up parameter table length calculation.
            long parameterTableLength = writer.BaseStream.Position;

            // Write Parameters.
            for (int i = 0; i < Data.Objects.Count; i++)
            {
                if (Data.Objects[i].UnknownByte_1 != 0x1)
                {
                    foreach (var parameter in Data.Objects[i].Parameters)
                    {
                        writer.Write(parameter);
                    }
                }
            }

            // Calculate parameter table length.
            parameterTableLength = writer.BaseStream.Position - parameterTableLength;

            // Pad the end of the file to match the original.
            writer.FixPadding(0x20);

            // Write parameter count.
            writer.BaseStream.Position = 0x8;
            writer.Write(parameterCount);

            // Write parameter table length.
            writer.BaseStream.Position = 0xC;
            writer.Write((uint)parameterTableLength);
        }

        /// <summary>
        /// Adds the object names.
        /// </summary>
        /// <param name="items">The class parsed from the setobjectbin_objitems.bin file.</param>
        public void FillObjectNames(SETItems items)
        {
            foreach (SetObject obj in Data.Objects)
                foreach (SETItems.ObjectEntry entry in items.Data.Objects)
                    if (entry.Table == obj.Table && entry.ID == obj.ID)
                        obj.Name = entry.Name;
        }
    }
}
