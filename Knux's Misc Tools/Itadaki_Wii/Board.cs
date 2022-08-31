namespace Knuxs_Misc_Tools.Itadaki_Wii
{
    internal class Board : FileBase
    {
        public class FormatData
        {
            public ushort StartingCash { get; set; }

            public ushort NetWorthTarget { get; set; }

            public ushort BasicSalary { get; set; }

            public ushort PromotionBonus { get; set; }

            public ushort MaxDiceValue { get; set; } // TODO: Confirm, tutorial board is rigged.

            public ushort GravityType { get; set; } // TODO: Is 2 on colony and mooncity, 1 on galaxy1.

            public List<Space> Spaces { get; set; } = new();
        }

        public class Space
        {
            public ushort Type { get; set; } // TODO: Enum for types?

            public short XPosition { get; set; }

            public short YPosition { get; set; }

            public short UnknownUShort_2 { get; set; } // Controls something to do with adjacent spaces???

            public short UnknownUShort_3 { get; set; } // Controls something to do with adjacent spaces???

            public short UnknownUShort_4 { get; set; } // Controls something to do with adjacent spaces???

            public short UnknownUShort_5 { get; set; } // Controls something to do with adjacent spaces???

            public short UnknownUShort_6 { get; set; } // Controls something to do with adjacent spaces???

            public short UnknownUShort_7 { get; set; } // Controls something to do with adjacent spaces???

            public short UnknownUShort_8 { get; set; } // Controls something to do with adjacent spaces???

            public short UnknownUShort_9 { get; set; } // Usually -1, very rarely not.

            public short UnknownUShort_10 { get; set; }

            public ushort PropertyValue { get; set; }

            public short UnknownUShort_12 { get; set; }

            public short UnknownUShort_13 { get; set; }
        }

        public FormatData Data = new();

        public override void Load(Stream fileStream)
        {
            BinaryReaderEx reader = new(fileStream, true);

            // Header I4DT Chunk
            reader.ReadSignature(4, "I4DT");
            uint fileSize = reader.ReadUInt32();
            reader.JumpAhead(0x8); // Always 0.

            // Other I4DT Chunk
            reader.ReadSignature(4, "I4DT");
            reader.JumpAhead(0x4); // Always 0x20.
            reader.JumpAhead(0x8); // Always 0.
            Data.StartingCash = reader.ReadUInt16();
            Data.NetWorthTarget = reader.ReadUInt16();
            Data.BasicSalary = reader.ReadUInt16();
            Data.PromotionBonus = reader.ReadUInt16();
            Data.MaxDiceValue = reader.ReadUInt16();
            Data.GravityType = reader.ReadUInt16();
            reader.JumpAhead(0x4); // Always 0.

            // I4PL Chunk Header
            reader.ReadSignature(4, "I4PL");
            uint chunkSize = reader.ReadUInt32();
            reader.JumpAhead(0x4); // Always 0.
            ushort spaceCount = reader.ReadUInt16();
            reader.FixPadding(0x4);

            // Spaces
            for (int i = 0; i < spaceCount; i++)
            {
                Space space = new();
                space.Type = reader.ReadUInt16();
                space.XPosition = reader.ReadInt16();
                space.YPosition = reader.ReadInt16();
                reader.JumpAhead(0x2); // Always 0.
                space.UnknownUShort_2 = reader.ReadInt16();
                space.UnknownUShort_3 = reader.ReadInt16();
                space.UnknownUShort_4 = reader.ReadInt16();
                space.UnknownUShort_5 = reader.ReadInt16();
                space.UnknownUShort_6 = reader.ReadInt16();
                space.UnknownUShort_7 = reader.ReadInt16();
                space.UnknownUShort_8 = reader.ReadInt16();
                space.UnknownUShort_9 = reader.ReadInt16();
                space.UnknownUShort_10 = reader.ReadInt16();
                space.PropertyValue = reader.ReadUInt16();
                space.UnknownUShort_12 = reader.ReadInt16();
                space.UnknownUShort_13 = reader.ReadInt16();
                Data.Spaces.Add(space);
            }
        }

        public override void Save(Stream fileStream)
        {
            BinaryWriterEx writer = new BinaryWriterEx(fileStream, true);

            // Header I4DT Chunk
            writer.Write("I4DT");
            long fileSizePosition = writer.BaseStream.Position;
            writer.Write("SIZE"); // Placeholder to fill in later.
            writer.WriteNulls(0x8);

            // Other I4DT Chunk
            writer.Write("I4DT");
            writer.Write(0x20);
            writer.WriteNulls(0x8);
            writer.Write(Data.StartingCash);
            writer.Write(Data.NetWorthTarget);
            writer.Write(Data.BasicSalary);
            writer.Write(Data.PromotionBonus);
            writer.Write(Data.MaxDiceValue);
            writer.Write(Data.GravityType);
            writer.WriteNulls(0x4);

            // I4PL Chunk Header
            writer.Write("I4PL");
            writer.Write((uint)(Data.Spaces.Count * 0x20) + 0x10);
            writer.WriteNulls(0x4);
            writer.Write((ushort)Data.Spaces.Count);
            writer.FixPadding(0x4);

            // Spaces
            foreach (Space space in Data.Spaces)
            {
                writer.Write(space.Type);
                writer.Write(space.XPosition);
                writer.Write(space.YPosition);
                writer.WriteNulls(0x2);
                writer.Write(space.UnknownUShort_2);
                writer.Write(space.UnknownUShort_3);
                writer.Write(space.UnknownUShort_4);
                writer.Write(space.UnknownUShort_5);
                writer.Write(space.UnknownUShort_6);
                writer.Write(space.UnknownUShort_7);
                writer.Write(space.UnknownUShort_8);
                writer.Write(space.UnknownUShort_9);
                writer.Write(space.UnknownUShort_10);
                writer.Write(space.PropertyValue);
                writer.Write(space.UnknownUShort_12);
                writer.Write(space.UnknownUShort_13);
            }

            // Fill in size value.
            writer.BaseStream.Position = fileSizePosition;
            writer.Write((uint)writer.BaseStream.Length);
        }
    }
}
