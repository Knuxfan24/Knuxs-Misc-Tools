using Marathon.IO;
using System.Numerics;

namespace Knuxs_Misc_Tools.WrathOfCortex.HGO
{
    internal class HGO
    {
        public class FormatData
        {
            public List<string>? NodeTable { get; set; }

            public List<HGO_Bitmap>? Bitmaps { get; set; }

            public List<HGO_Material>? Materials { get; set; }

            public uint? UnknownAnimationFlag { get; set; }

            public List<HGO_Animation>? Animations { get; set; }

            // Node Indices?
            public ushort[]? UnknownAnimationShorts { get; set; }
        }

        public FormatData Data = new();

        public void Load(string filepath)
        {
            int chunkCount = 0;
            BinaryReaderEx reader = new(File.OpenRead($"{filepath}.blk"), true);

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                chunkCount++;
                reader.JumpAhead(0x8);
            }

            reader = new(File.OpenRead(filepath), true);

            string fileHeader = reader.ReadNullPaddedString(0x4);
            uint fileSize = reader.ReadUInt32();

            for (int i = 0; i < chunkCount - 1; i++)
            {
                string chunkHeader = reader.ReadNullPaddedString(0x4);
                uint chunkSize = reader.ReadUInt32();

                switch (chunkHeader)
                {
                    case "LBTN":
                        Data.NodeTable = new();
                        uint nodeTableSize = reader.ReadUInt32();
                        long nodeTableEnd = reader.BaseStream.Position + nodeTableSize;

                        while (reader.BaseStream.Position < nodeTableEnd)
                            Data.NodeTable.Add(reader.ReadNullTerminatedString());

                        reader.FixPadding(0x4);

                        break;
                    case "0TST":
                        break;
                    case "0HST":
                        uint bitmapCount = reader.ReadUInt32();
                        break;
                    case "0MXT":
                        if (Data.Bitmaps == null)
                            Data.Bitmaps = new();

                        HGO_Bitmap bitmap = new();
                        bitmap.ReadBitmap(reader);
                        Data.Bitmaps.Add(bitmap);
                        break;
                    case "00SM":
                        if (Data.Materials == null)
                            Data.Materials = new();

                        uint materialCount = reader.ReadUInt32();
                        for (int m = 0; m < materialCount; m++)
                        {
                            HGO_Material material = new();
                            material.ReadMaterial(reader);
                            Data.Materials.Add(material);
                        }

                        break;
                    case "0SAT":
                        if (Data.Animations == null)
                            Data.Animations = new();

                        uint animationCount = reader.ReadUInt32();
                        Data.UnknownAnimationFlag = reader.ReadUInt32();
                        for (int a = 0; a < animationCount; a++)
                        {
                            HGO_Animation animation = new();
                            animation.ReadAnimation(reader);
                            Data.Animations.Add(animation);
                        }

                        uint unknownShortsCount = reader.ReadUInt32();
                        Data.UnknownAnimationShorts = new ushort[unknownShortsCount];
                        for (int a = 0; a < unknownShortsCount; a++)
                            Data.UnknownAnimationShorts[a] = reader.ReadUInt16();

                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
