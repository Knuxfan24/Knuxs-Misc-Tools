namespace Knuxs_Misc_Tools.WrathOfCortex.HGO_Chunk
{
    // TODO: What the hell is this? Some sort of matrix table? What's the extra data afterwards?
    public class INST
    {
        public void Read(BinaryReaderEx reader)
        {
            // Basic Chunk Header.
            string chunkType = reader.ReadNullPaddedString(4);
            uint chunkSize = reader.ReadUInt32();

            uint matrixCount = reader.ReadUInt32(); // Not sure what this data is, but it looks to have a Matrix as part of it.

            for (int i = 0; i < matrixCount; i++)
            {
                Matrix4x4 UnknownMatrix4x4_1 = reader.Read4x4Matrix();
                uint MatrixIndex = reader.ReadUInt32(); // ?
                uint UnknownUInt32_1 = reader.ReadUInt32();
                uint UnknownUInt32_2 = reader.ReadUInt32();
                reader.JumpAhead(0x4); // Always 0.
            }

            uint UnknownCount = reader.ReadUInt32(); // Count of whatever this is.
            for (int i = 0; i < UnknownCount; i++)
            {
                reader.JumpAhead(0x40); // Literally always 0? Not sure what the hell this is about.
                float UnknownFloat_1 = reader.ReadSingle();
                float UnknownFloat_2 = reader.ReadSingle();
                float UnknownFloat_3 = reader.ReadSingle();
                reader.JumpAhead(0x4); // Always 1.
                float UnknownFloat_4 = reader.ReadSingle();
                reader.JumpAhead(0x8); // Always 0.
                float UnknownFloat_5 = reader.ReadSingle();
            }
        }
    }
}
