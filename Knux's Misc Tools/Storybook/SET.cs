namespace Knuxs_Misc_Tools.Storybook
{
    public class SetObject
    {
        public Vector3 Position { get; set; }
        public uint UnknownUInt32_1 { get; set; }

        public uint UnknownUInt32_2 { get; set; }
        public uint UnknownUInt32_3 { get; set; }
        public uint UnknownUInt32_4 { get; set; }
        public uint UnknownUInt32_5 { get; set; }

        public uint UnknownUInt32_6 { get; set; }
        public uint UnknownUInt32_7 { get; set; }
        public uint UnknownUInt32_8 { get; set; }
        public uint UnknownUInt32_9 { get; set; }
    }

    public class SET : FileBase
    {
        public override void Load(string filepath)
        {
            // TODO: Finish reading this.
            BinaryReaderEx reader = new(File.OpenRead(filepath));

            string sig = reader.ReadNullPaddedString(0x4);

            if (!sig.StartsWith("ST"))
                throw new Exception($"'{filepath}' does not appear to be a Sonic and the Secret Rings or Sonic and the Black Knight SET file.");

            uint objectCount = reader.ReadUInt32();
            uint UnknownUInt32_1 = reader.ReadUInt32();
            uint UnknownUInt32_2 = reader.ReadUInt32();

            for (int i = 0; i < objectCount; i++)
            {
                SetObject obj = new();
                obj.Position = reader.ReadVector3();
                obj.UnknownUInt32_1 = reader.ReadUInt32();
                obj.UnknownUInt32_2 = reader.ReadUInt32();
                obj.UnknownUInt32_3 = reader.ReadUInt32();
                obj.UnknownUInt32_4 = reader.ReadUInt32();
                obj.UnknownUInt32_5 = reader.ReadUInt32();
                obj.UnknownUInt32_6 = reader.ReadUInt32();
                obj.UnknownUInt32_7 = reader.ReadUInt32();
                obj.UnknownUInt32_8 = reader.ReadUInt32();
                obj.UnknownUInt32_9 = reader.ReadUInt32();
            }
        }
    }
}
