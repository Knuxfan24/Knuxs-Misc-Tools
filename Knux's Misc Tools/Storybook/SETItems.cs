namespace Knuxs_Misc_Tools.Storybook
{
    public class SETItems : FileBase
    {
        public override void Load(Stream stream)
        {
            // TODO: Finish reading this.
            BinaryReaderEx reader = new(stream);

            // TODO: RE the Secret Rings version as well.
            uint fileSize = reader.ReadUInt32();
            uint stageCount = reader.ReadUInt32();
            uint stageSubCount = reader.ReadUInt32(); // Feels like the wrong name but I'm not sure what to call this.
            uint entryCount = reader.ReadUInt32();
            uint UnknownUInt32_4 = reader.ReadUInt32();
            uint UnknownUInt32_5 = reader.ReadUInt32();
            uint ObjectTableOffset = reader.ReadUInt32();

            for (int c = 0; c < stageCount; c++)
            {
                for (int i = 0; i < stageSubCount; i++)
                {
                    string stageName = reader.ReadNullPaddedString(0x10); // TODO: How do these factor into whether the object can be loaded?
                    uint index = reader.ReadUInt32();
                }
            }

            reader.JumpTo(ObjectTableOffset); // Seem to already be at it but let's be safe.

            for (int i = 0; i < entryCount; i++)
            {
                string objectName = reader.ReadNullPaddedString(0x20);
                byte objectID = reader.ReadByte();
                byte objectTable = reader.ReadByte();
                reader.JumpAhead(0x2); // Always 0xCD.

                reader.JumpAhead(0x24); // What is this data for? How does it control whether the object can be loaded or not if it does? Does it control parameters in some way maybe?
            }

            reader.Close();
        }
    }
}
