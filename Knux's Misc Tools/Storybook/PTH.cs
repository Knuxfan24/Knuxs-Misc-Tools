namespace Knuxs_Misc_Tools.Storybook
{
    public class PTH : FileBase
    {
        public override void Load(string filepath)
        {
            // TODO: Format is different in Black Knight. Handle that.
            BinaryReaderEx reader = new(File.OpenRead(filepath));
            ushort UnknownUShort_1 = reader.ReadUInt16(); // Always 3, except for a few AIRCAR paths, which have it set to 1. Type maybe?
            ushort UnknownUShort_2 = reader.ReadUInt16(); // Count of something?
            float UnknownFloat_1 = reader.ReadSingle();
            uint UnknownUInt32_1 = reader.ReadUInt32(); // Always 0x10. Pointless offset to data maybe?
            uint UnknownUInt32_2 = reader.ReadUInt32(); // Always 0, 1 or 2.

            // Ones with UnknownUShort_1 set to 3 seem to have each entry made up of 0x1C bytes?
            if (UnknownUShort_1 == 3)
            {
                int vertID = 1;
                for (int i = 0; i < UnknownUShort_2; i++)
                {
                    Console.WriteLine(reader.ReadUInt32()); // Flag?
                    Console.WriteLine(reader.ReadVector3());
                    Console.WriteLine(reader.ReadVector3());
                }
            }
        }
    }
}
