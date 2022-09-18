using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Drawing;

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

            public ushort ShopPrice { get; set; }

            public ShopType ShopType { get; set; } // TODO: Enum of names maybe? Will probably require making a huge cube and just going through manually.
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

        [Flags]
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ShopType : ushort
        {
            not_set_10G = 1,
            not_set_20G = 2,
            not_set_30G = 3,
            not_set_40G = 4,
            Scrappaper_shop = 5,
            Wool_shop = 6,
            Bottle_store = 7,
            Secondhand_book_shop = 8,
            Scrapmetal_supplier = 9,
            Stationery_shop = 10,
            General_store = 11,
            Florists = 12,
            Icecream_shop = 13,
            Comicbook_shop = 14,
            Dairy = 15,
            Doughnut_shop = 16,
            Pizza_shack = 17,
            Bakery = 18,
            Grocery_store = 19,
            Pharmacy = 20,
            Fish_market = 21,
            Toy_shop = 22,
            Bookshop = 23,
            Cosmetics_boutique = 24,
            Tshirt_shop = 25,
            Fruit_stall = 26,
            Photography_studio = 27,
            Coffee_shop = 28,
            Butcher_shop = 29,
            Restaurant = 30,
            Barbershop = 31,
            Hat_boutique = 32,
            Hardware_store = 33,
            Gift_shop = 34,
            Launderette = 35,
            Shoe_shop = 36,
            Clothing_store = 37,
            Opticians = 38,
            Clockmakers = 39,
            Furniture_shop = 40,
            Sports_shop = 41,
            Locksmiths = 42,
            Glassmakers = 43,
            Sushi_restaurant = 44,
            Art_gallery = 45,
            Leatherware_boutique = 46,
            Pet_shop = 47,
            Nail_salon = 48,
            Spice_shop = 49,
            Music_shop = 50,
            Surf_shop = 51,
            Boating_shop = 52,
            Cartographers = 53,
            Alloy_rims_shop = 54,
            Fashion_boutique = 55,
            Waxworks = 56,
            Lens_shop = 57,
            Kaleidoscope_shop = 58,
            Crystal_ball_shop = 59,
            not_set_600G = 60,
            Gemstone_supplier = 61,
            Taxidermy_studio = 62,
            not_set_630G = 63,
            not_set_640G = 64,
            Antiques_dealers = 65,
            not_set_660G = 66,
            not_set_670G = 67,
            Goldsmiths = 68,
            not_set_690G = 69,
            Fossil_shop = 70,
            not_set_710G = 71,
            Musicbox_shop = 72,
            not_set_730G = 73,
            not_set_740G = 74,
            Marionette_workshop = 75,
            Health_shop = 76,
            not_set_770G = 77,
            not_set_780G = 78,
            not_set_790G = 79,
            Organic_food_shop = 80,
            Bridal_boutique = 81,
            not_set_820G = 82,
            not_set_830G = 83,
            not_set_840G = 84,
            Autograph_shop = 85,
            not_set_860G = 86,
            not_set_870G = 87,
            not_set_880G = 88,
            not_set_890G = 89,
            Meteorite_shop = 90,
            not_set_910G = 91,
            not_set_920G = 92,
            not_set_930G = 93,
            not_set_940G = 94,
            not_set_950G = 95,
            not_set_960G = 96,
            not_set_970G = 97,
            Department_store = 98,
            SYSTEM_MSG_NAME_shop_name_990A = 99,
            SYSTEM_MSG_NAME_shop_name_1000A = 100
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
                space.ShopPrice = reader.ReadUInt16();
                space.ShopType = (ShopType)reader.ReadUInt16();
                Data.Spaces.Add(space);
            }
        }

        public override void Save(Stream fileStream)
        {
            BinaryWriterEx writer = new(fileStream, true);

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
                writer.Write((ushort)space.ShopType);
            }

            // Fill in size value.
            writer.BaseStream.Position = fileSizePosition;
            writer.Write((uint)writer.BaseStream.Length);
        }

        public void ImportBitmap(string filepath)
        {
            // Load the bitmap.
            Bitmap img = new(filepath);

            // Loop through every column of pixels.
            for (int i = 0; i < img.Height; i++)
            {
                // Loop through every row of pixels.
                for (int j = 0; j < img.Width; j++)
                {
                    // Load this pixel.
                    Color pixel = img.GetPixel(j, i);

                    // Make a basic space on the board if this pixel is full alpha.
                    // TODO: Literally everything 😬
                    if (pixel.A == 255)
                    {
                        Space space = new()
                        {
                            Type = SpaceType.Property,
                            XPosition = (short)(j * 64),
                            YPosition = (short)(i * 64)
                        };

                        // If the pixel is white, set it as the bank and make it the first space in the list.
                        if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                        {
                            space.Type = SpaceType.Bank;
                            Data.Spaces.Insert(0, space);
                            continue;
                        }

                        // Districts
                        if (pixel.R == 255 && pixel.G == 0 && pixel.B == 0)
                            space.DistrictIndex = 0;

                        if (pixel.R == 0 && pixel.G == 255 && pixel.B == 255)
                            space.DistrictIndex = 1;

                        if (pixel.R == 255 && pixel.G == 83 && pixel.B == 0)
                            space.DistrictIndex = 2;

                        if (pixel.R == 0 && pixel.G == 255 && pixel.B == 0)
                            space.DistrictIndex = 3;

                        if (pixel.R == 0 && pixel.G == 0 && pixel.B == 255)
                            space.DistrictIndex = 4;

                        if (pixel.R == 255 && pixel.G == 0 && pixel.B == 255)
                            space.DistrictIndex = 5;

                        if (pixel.R == 99 && pixel.G == 4 && pixel.B == 96)
                            space.DistrictIndex = 6;

                        if (pixel.R == 255 && pixel.G == 255 && pixel.B == 0)
                            space.DistrictIndex = 7;

                        // Suits
                        if (pixel.R == 32 && pixel.G == 32 && pixel.B == 32)
                            space.Type = SpaceType.Spade;

                        if (pixel.R == 64 && pixel.G == 64 && pixel.B == 64)
                            space.Type = SpaceType.Heart;

                        if (pixel.R == 96 && pixel.G == 96 && pixel.B == 96)
                            space.Type = SpaceType.Diamond;

                        if (pixel.R == 128 && pixel.G == 128 && pixel.B == 128)
                            space.Type = SpaceType.Club;

                        // Venture Card
                        if (pixel.R == 237 && pixel.G == 20 && pixel.B == 91)
                            space.Type = SpaceType.VentureCard;

                        // Roll Again
                        if (pixel.R == 196 && pixel.G == 196 && pixel.B == 196)
                            space.Type = SpaceType.RollAgain;

                        // Day Off
                        if (pixel.R == 46 && pixel.G == 49 && pixel.B == 146)
                            space.Type = SpaceType.TakeABreak;

                        // Arcade
                        if (pixel.R == 246 && pixel.G == 150 && pixel.B == 121)
                            space.Type = SpaceType.Arcade;

                        // Comission
                        if (pixel.R == 255 && pixel.G == 242 && pixel.B == 0)
                            space.Type = SpaceType.Comission;

                        Data.Spaces.Add(space);
                    }
                }
            }
        }
    }
}
