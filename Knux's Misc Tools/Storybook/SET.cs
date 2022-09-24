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

            public List<SetParameter> Parameters { get; set; } = new();

            public string? Name { get; set; } // Not actually a thing these have, filled in by function for ease of research.

            public override string ToString() => Name;
        }

        public class SetParameter
        {
            public object? Data { get; set; }

            public Type? DataType { get; set; }
        }

        public class FormatData
        {
            public string Signature { get; set; } = "STP1"; // The P1 part depends on the set file itself, also not sure if this is actually used for anything?

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

                // Calculate how many bytes make up the parameters for this object.
                int objectParameterLength = reader.ReadByte();

                // Jump to the apporiate location in the parameter data table.
                reader.JumpTo(parameterDataTableOffset + parameterOffset);

                // Read the parameters.
                ReadParameters(reader, objectParameterLength, obj);

                // Jump back for our next object.
                reader.JumpTo(pos);

                // Save this object.
                Data.Objects.Add(obj);
            }
        }

        // Maybe replace this with a template system rather than hardcoding all the types in here? Not sure how I'd do that personally.
        // TODO: Literally every object...
        private static void ReadParameters(BinaryReaderEx reader, int objectParameterLength, SetObject obj)
        {
            switch (obj.Table, obj.ID)
            {
                // RING
                case (0x00, 0x01):
                    obj.Parameters.Add(ReadParameter(reader, typeof(byte)));  // Type. 0 = single?, 1 = default, 2 = single that gives RingCount, 3 = same as 2?, 4 = same as 1?, 5 = vertical stack
                    obj.Parameters.Add(ReadParameter(reader, typeof(byte)));  // RingCount.
                    obj.Parameters.Add(ReadParameter(reader, typeof(byte)));  // Something to do with the pattern of the trail?
                    obj.Parameters.Add(ReadParameter(reader, typeof(bool)));  // IsSilverRing.
                    obj.Parameters.Add(ReadParameter(reader, typeof(float))); // Spacing.
                    obj.Parameters.Add(ReadParameter(reader, typeof(float))); // Unknown.
                    obj.Parameters.Add(ReadParameter(reader, typeof(float))); // Unknown.
                    break;

                // DASH_PANEL
                case (0x00, 0x13):
                    obj.Parameters.Add(ReadParameter(reader, typeof(float))); // Speed?
                    obj.Parameters.Add(ReadParameter(reader, typeof(uint)));  // OutOfControl in frames?
                    break;

                // INVISIBLE_COLLISION
                //case (0x00, 0x55):
                //    four shorts?
                //    break;

                default:
                    for (int i = 0; i < objectParameterLength; i++)
                        obj.Parameters.Add(ReadParameter(reader, typeof(byte)));
                    break;
            }
        }

        private static SetParameter ReadParameter(BinaryReaderEx reader, Type type)
        {
            SetParameter parameter = new() { DataType = type };

            switch (type.ToString())
            {
                case "System.Byte": parameter.Data = reader.ReadByte(); break;
                case "System.Boolean": parameter.Data = reader.ReadBoolean(); break;
                case "System.Single": parameter.Data = reader.ReadSingle(); break;
                case "System.UInt32": parameter.Data = reader.ReadUInt32(); break;

                default: throw new NotImplementedException($"Data type of {type} not handled");
            }

            return parameter;
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
                    // Calculate the parameter length for this object.
                    byte parameterLength = 0;
                    foreach (SetParameter parameter in Data.Objects[i].Parameters)
                    {
                        switch (parameter.DataType.ToString())
                        {
                            case "System.Byte": parameterLength++; break;
                            case "System.Boolean": parameterLength++; break;
                            case "System.Single": parameterLength += 4; break;
                            case "System.UInt32": parameterLength += 4; break;

                            default:
                                throw new NotImplementedException($"Data type of {parameter.DataType} not handled");
                        }
                    }

                    writer.Write((byte)0x1);
                    writer.Write((byte)0x0);
                    writer.Write(parameterLength);
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
                    foreach (SetParameter parameter in Data.Objects[i].Parameters)
                    {
                        switch (parameter.DataType.ToString())
                        {
                            case "System.Byte": writer.Write((byte)parameter.Data); break;
                            case "System.Boolean": writer.Write((bool)parameter.Data); break;
                            case "System.Single": writer.Write((float)parameter.Data); break;
                            case "System.UInt32": writer.Write((uint)parameter.Data); break;

                            default:
                                throw new NotImplementedException($"Data type of {parameter.DataType} not handled");
                        }
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
