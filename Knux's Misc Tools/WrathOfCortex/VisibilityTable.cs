// TODO: What does this data actually do to make up the visiblity tables?

namespace Knuxs_Misc_Tools.WrathOfCortex
{
    public class VisibilityTable : FileBase
    {
        public class FormatData
        {
            public List<List<Vector3>> VisibilityChunks = new();

            public List<List<uint>> Sectors = new();

            public List<string> Splines = new();
        }
        public FormatData Data = new();

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream, true);
            reader.JumpAhead(0x4); // Always 0xFFFFFFFF.

            uint chunkEntryCount = reader.ReadUInt32();

            for (int i = 0; i < chunkEntryCount; i++)
            {
                uint chunkCount = reader.ReadUInt32();
                List<Vector3> chunk = new();

                for (int c = 0; c < chunkCount; c++)
                    chunk.Add(reader.ReadVector3());

                Data.VisibilityChunks.Add(chunk);
            }

            uint sectorCount = reader.ReadUInt32();

            for (int i = 0; i < sectorCount; i++)
            {
                List<uint> sector = new()
                {
                    reader.ReadUInt32(),
                    reader.ReadUInt32(),
                    reader.ReadUInt32(),
                    reader.ReadUInt32()
                };
                Data.Sectors.Add(sector);
            }

            uint splineCount = reader.ReadUInt32();

            for (int i = 0; i < splineCount; i++)
                Data.Splines.Add(reader.ReadNullTerminatedString());
        }
    }
}
