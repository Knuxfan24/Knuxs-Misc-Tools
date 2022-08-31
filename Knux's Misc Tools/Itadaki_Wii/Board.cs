using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

            public ushort MaxDiceValue { get; set; }

            public ushort GravityType { get; set; } // TODO: Is 2 on colony and mooncity, 1 on galaxy1.

            public List<Space> Spaces { get; set; } = new();
        }

        public class Space
        {
            public SpaceType Type { get; set; }

            public short XPosition { get; set; }

            public short YPosition { get; set; }

            public byte[] Adjacent1 { get; set; } = new byte[4] {0xFF, 0xFF, 0xFF, 0xFF};

            public byte[] Adjacent2 { get; set; } = new byte[4] {0xFF, 0xFF, 0xFF, 0xFF};

            public byte[] Adjacent3 { get; set; } = new byte[4] {0xFF, 0xFF, 0xFF, 0xFF};

            public byte[] Adjacent4 { get; set; } = new byte[4] {0xFF, 0xFF, 0xFF, 0xFF};

            public byte DistrictIndex { get; set; }

            public byte UnknownByte1 { get; set; }

            public ushort PropertyValue { get; set; }

            public short ShopPrice { get; set; }

            public short UnknownUShort_1 { get; set; }
        }

        [Flags]
        [JsonConverter(typeof(StringEnumConverter))]
        public enum SpaceType : ushort
        {
            Property = 0,
            Bank = 1,
            VentureCard = 2,
            Spade = 3,
            Heart = 4,
            Diamond = 5,
            Club = 6,
            SpadeShuffle = 7,
            HeartShuffle = 8,
            DiamondShuffle = 9,
            ClubShuffle = 10,
            TakeABreak = 11,
            Comission = 12,
            BigComission = 13,
            Stocks = 14,
            RollAgain = 16,
            Arcade = 17,
            Button = 18,
            Cannon = 19,
            TelporterA_Blue = 20,
            TelporterA_Red = 21,
            TelporterA_Green = 22,
            TelporterA_Yellow = 23,
            TelporterA_Purple = 24,
            TelporterB_Blue = 25,
            TelporterB_Green = 26,
            TelporterB_Purple = 27,
            Tunnel_Blue = 28,
            Tunnel_Purple = 29,
            Tunnel_Yellow = 30,
            Tunnel_Green = 31,
            LiftSquare = 32,
            LiftSquareInactive = 33,
            Tunnel_Destination = 34,
            LiftDestination = 35,
            Vacant = 48,
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
                space.Type = (SpaceType)reader.ReadUInt16();
                space.XPosition = reader.ReadInt16();
                space.YPosition = reader.ReadInt16();
                reader.JumpAhead(0x2); // Always 0.
                space.Adjacent1 = reader.ReadBytes(4);
                space.Adjacent2 = reader.ReadBytes(4);
                space.Adjacent3 = reader.ReadBytes(4);
                space.Adjacent4 = reader.ReadBytes(4);
                space.DistrictIndex = reader.ReadByte();
                space.UnknownByte1 = reader.ReadByte();
                space.PropertyValue = reader.ReadUInt16();
                space.ShopPrice = reader.ReadInt16();
                space.UnknownUShort_1 = reader.ReadInt16();
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
                writer.Write((ushort)space.Type);
                writer.Write(space.XPosition);
                writer.Write(space.YPosition);
                writer.WriteNulls(0x2);
                writer.Write(space.Adjacent1);
                writer.Write(space.Adjacent2);
                writer.Write(space.Adjacent3);
                writer.Write(space.Adjacent4);
                writer.Write(space.DistrictIndex);
                writer.Write(space.UnknownByte1);
                writer.Write(space.PropertyValue);
                writer.Write(space.ShopPrice);
                writer.Write(space.UnknownUShort_1);
            }

            // Fill in size value.
            writer.BaseStream.Position = fileSizePosition;
            writer.Write((uint)writer.BaseStream.Length);
        }
    }
}
