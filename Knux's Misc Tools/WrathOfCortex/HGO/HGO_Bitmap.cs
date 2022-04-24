namespace Knuxs_Misc_Tools.WrathOfCortex.HGO
{
    internal class HGO_Bitmap
    {
        public uint Type { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }

        public byte[] Data { get; set; }

        public void ReadBitmap(BinaryReaderEx reader)
        {
            Type = reader.ReadUInt32();
            Width = reader.ReadUInt32();
            Height = reader.ReadUInt32();
            uint bitmapSize = reader.ReadUInt32();
            Data = new byte[bitmapSize];
            for (int b = 0; b < bitmapSize; b++)
                Data[b] = reader.ReadByte();
        }
    }
}
