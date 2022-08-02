﻿namespace Knuxs_Misc_Tools.Storybook
{
    public class SETItems : FileBase
    {
        public class FormatData
        {
            public List<StageEntry> Stages { get; set; } = new();

            public List<ObjectEntry> Objects { get; set; } = new();
        }

        public class StageEntry
        {
            public string Name { get; set; } // TODO: How do these factor into whether the object can be loaded?

            public uint? Index { get; set; }

            public override string ToString() => Name;
        }

        public class ObjectEntry
        {
            public string Name { get; set; }

            public byte ID { get; set; }

            public byte Table { get; set; }

            // TODO: How do these control stuff???
            public byte[] UnknownBytes { get; set; }
            public List<byte> JsonBytes;
                      
            public override string ToString() => Name;
        }

        public FormatData Data = new();

        public void Load(string filepath, bool isBlackKnight = true)
        {
            BinaryReaderEx reader = new(File.OpenRead(filepath));

            uint fileSize = reader.ReadUInt32();
            uint stageCount = reader.ReadUInt32();
            uint? stageSubCount = reader.ReadUInt32(); // Feels like the wrong name but I'm not sure what to call this.

            // Secret Rings doesn't seem to have the stageSubCount value.
            if (!isBlackKnight)
            {
                stageSubCount = null;
                reader.JumpBehind(0x4);
            }

            uint entryCount = reader.ReadUInt32();
            uint UnknownUInt32_4 = reader.ReadUInt32(); // TODO: Change these values and see if anything gets screwed by it. Might be the object parameter(?) count?
            uint UnknownUInt32_5 = reader.ReadUInt32(); // TODO: Change these values and see if anything gets screwed by it. Changing this stopped objects from loading in Misty Lake at all.
            uint ObjectTableOffset = reader.ReadUInt32();

            for (int c = 0; c < stageCount; c++)
            {
                // Secret Rings doesn't seem to have the stageSubCount value.
                if (isBlackKnight)
                {
                    for (int i = 0; i < stageSubCount; i++)
                    {
                        StageEntry stage = new()
                        {
                            Name = reader.ReadNullPaddedString(0x10),
                            Index = reader.ReadUInt32()
                        };
                        Data.Stages.Add(stage);
                    }
                }
                else
                {
                    StageEntry stage = new()
                    {
                        Name = reader.ReadNullPaddedString(0x10)
                    };
                    reader.JumpAhead(0x4); // The Index value is always CC CC CC CC in Secret Rings.
                    Data.Stages.Add(stage);
                }
            }

            reader.JumpTo(ObjectTableOffset); // Seem to already be at it but let's be safe.

            for (int i = 0; i < entryCount; i++)
            {
                ObjectEntry obj = new();

                obj.Name = reader.ReadNullPaddedString(0x20);
                obj.ID = reader.ReadByte();
                obj.Table = reader.ReadByte();
                reader.JumpAhead(0x2); // Always 0xCD.
                if (isBlackKnight)
                    obj.UnknownBytes = reader.ReadBytes(0x24);
                else
                    obj.UnknownBytes = reader.ReadBytes(0xC);

                // Because Newtonsoft is stupid and outputs arrays in Base64.
                obj.JsonBytes = obj.UnknownBytes.ToList();

                Data.Objects.Add(obj);
            }

            reader.Close();
        }

        public void Save(string filepath, bool isBlackKnight = true)
        {
            BinaryWriterEx writer = new(File.OpenWrite(filepath));

            // Write the file header.
            // Placeholder size value to be filled in later.
            writer.Write("SIZE");

            // TODO: Unhardcode the Black Knight stuff.
            if (isBlackKnight)
            {
                writer.Write(Data.Stages.Count / 0x6);
                writer.Write(0x6);
            }
            else
            {
                writer.Write(Data.Stages.Count);
            }

            writer.Write(Data.Objects.Count);

            // TODO: Unhardcode these values, might have something to do with the UnknownInts in the objects? As 9 and 3 match the parameter(?) count.
            if (isBlackKnight)
            {
                writer.Write(0x9);
                writer.Write(0x1C);
            }
            else
            {
                writer.Write(0x3);
                writer.Write(0x18);
            }
            writer.AddOffset("ObjectTableOffset");

            // Write the stage table.
            for (int i = 0; i < Data.Stages.Count; i++)
            {
                writer.WriteNullPaddedString(Data.Stages[i].Name, 0x10);

                // Write the stage index if this is a Black Knight file. If not, just write the CC CC CC CC padding.
                if (isBlackKnight)
                    writer.Write((uint)Data.Stages[i].Index);
                else
                    writer.Write(0xCCCCCCCC);
            }

            // Fill in the offset to the object table.
            writer.FillOffset("ObjectTableOffset");

            // Write the object table.
            for (int i = 0; i < Data.Objects.Count; i++)
            {
                writer.WriteNullPaddedString(Data.Objects[i].Name, 0x20);
                writer.Write(Data.Objects[i].ID);
                writer.Write(Data.Objects[i].Table);
                writer.Write((byte)0xCD);
                writer.Write((byte)0xCD);
                writer.Write(Data.Objects[i].UnknownBytes);
            }
            
            // Go back and fill in the file size.
            writer.BaseStream.Position = 0;
            writer.Write((uint)writer.BaseStream.Length);

            // Close the writer.
            writer.Flush();
            writer.Close();
        }
    }
}
