// Based on: https://info.sonicretro.org/SCHG:Sonic_Forces/Formats/BINA/GEdit

namespace Knuxs_Misc_Tools.SonicRangers
{
    internal class GEdit : FileBase
    {
        public class SetObject
        {
            public string Type { get; set; } = "";

            public string Name { get; set; } = "";

            public ushort ID { get; set; }

            public ushort GroupID { get; set; }

            public ushort ParentID { get; set; }

            public ushort ParentGroupID { get; set; }

            public byte[] UnknownBytes { get; set; } = new byte[0x20];

            public Vector3 Position { get; set; }

            public Vector3 Rotation { get; set; }

            public Vector3 ChildPositionOffset { get; set; }

            public Vector3 ChildRotationOffset { get; set; }

            public List<ObjectTag> Tags { get; set; } = new();
        }

        public class ObjectTag
        {
            public string Type { get; set; } = "";

            public byte[]? Data { get; set; }
        }

        public List<SetObject> Objects = new();

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream)
            {
                Offset = 0x40
            };

            // Skip the BINA header.
            reader.JumpTo(0x40);

            reader.JumpAhead(0x10); // Always 0? Verify.
            long ObjectTableOffset = reader.ReadInt64(); // Should always be 0x30?
            ulong ObjectCount = reader.ReadUInt64();
            ulong ObjectCount2 = reader.ReadUInt64(); //?
            reader.JumpAhead(0x8); // Always 0? Verify.

            for (ulong i = 0; i < ObjectCount; i++)
            {
                SetObject obj = new();

                long ObjectOffset = reader.ReadInt64();
                long pos = reader.BaseStream.Position;

                reader.JumpTo(ObjectOffset, true);

                reader.JumpAhead(0x8);

                long TypeOffset = reader.ReadInt64();
                long NameOffset = reader.ReadInt64();

                obj.ID = reader.ReadUInt16();
                obj.GroupID = reader.ReadUInt16();
                obj.ParentID = reader.ReadUInt16();
                obj.ParentGroupID = reader.ReadUInt16();

                obj.UnknownBytes = reader.ReadBytes(0x20);

                obj.Position = reader.ReadVector3();
                obj.Rotation = reader.ReadVector3();
                obj.ChildPositionOffset = reader.ReadVector3();
                obj.ChildRotationOffset = reader.ReadVector3();

                long TagsOffsetTableOffset = reader.ReadInt64();
                ulong TagCount = reader.ReadUInt64();
                ulong TagCount2 = reader.ReadUInt64();
                reader.JumpAhead(0x8); // Always 0? Verify.

                long ParametersOffset = reader.ReadInt64();

                reader.JumpTo(TypeOffset, true);
                obj.Type = reader.ReadNullTerminatedString();

                reader.JumpTo(NameOffset, true);
                obj.Name = reader.ReadNullTerminatedString();

                reader.JumpTo(TagsOffsetTableOffset, true);
                for (ulong t = 0; t < TagCount; t++)
                {
                    ObjectTag tag = new();

                    long TagOffset = reader.ReadInt64();
                    long tagPos = reader.BaseStream.Position;

                    reader.JumpTo(TagOffset, true);

                    reader.JumpAhead(0x8); // Always 0? Verify.
                    long TagTypeOffset = reader.ReadInt64();
                    ulong DataLength = reader.ReadUInt64();
                    long DataOffset = reader.ReadInt64();

                    reader.JumpTo(TagTypeOffset, true);
                    tag.Type = reader.ReadNullTerminatedString();

                    reader.JumpTo(DataOffset, true);
                    tag.Data = reader.ReadBytes((int)DataLength);

                    obj.Tags.Add(tag);

                    reader.JumpTo(tagPos);
                }

                Objects.Add(obj);


                reader.JumpTo(pos);
            }
        }
    }
}
