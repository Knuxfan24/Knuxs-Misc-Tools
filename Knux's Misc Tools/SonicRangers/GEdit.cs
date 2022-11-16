// Based on: https://info.sonicretro.org/SCHG:Sonic_Forces/Formats/BINA/GEdit

using HedgeLib.Headers;
using HedgeLib.Sets;

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

            public List<SetParameter> Parameters { get; set; } = new();

            public List<ObjectTag> Tags { get; set; } = new();
        }

        public class SetParameter
        {
            public object? Data { get; set; }

            public Type? DataType { get; set; }
        }

        public class ObjectTag
        {
            public string Type { get; set; } = "";

            public byte[]? Data { get; set; }
        }

        // Set up HedgeLib#'s BINAV2Header.
        public HedgeLib.Headers.BINAHeader Header = new BINAv2Header(210);

        public List<SetObject> Objects = new();

        public void Load(string filepath, Dictionary<string, SetObjectType> templates)
        {
            // Set up our BINAReader and read the BINAV2 header.
            HedgeLib.IO.BINAReader reader = new(File.OpenRead(filepath));
            Header = reader.ReadHeader();

            reader.JumpAhead(0x10); // Always 0? Verify.
            long ObjectTableOffset = reader.ReadInt64(); // Should always be 0x30?
            ulong ObjectCount = reader.ReadUInt64();
            ulong ObjectCount2 = reader.ReadUInt64(); //?
            reader.JumpAhead(0x8); // Always 0? Verify.

            reader.JumpTo(ObjectTableOffset, false);

            for (ulong i = 0; i < ObjectCount; i++)
            {
                SetObject obj = new();

                long ObjectOffset = reader.ReadInt64();
                long pos = reader.BaseStream.Position;

                reader.JumpTo(ObjectOffset, false);

                reader.JumpAhead(0x8);

                long TypeOffset = reader.ReadInt64();
                long NameOffset = reader.ReadInt64();

                obj.ID = reader.ReadUInt16();
                obj.GroupID = reader.ReadUInt16();
                obj.ParentID = reader.ReadUInt16();
                obj.ParentGroupID = reader.ReadUInt16();

                obj.UnknownBytes = reader.ReadBytes(0x20);

                obj.Position = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                obj.Rotation = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                obj.ChildPositionOffset = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                obj.ChildRotationOffset = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                long TagsOffsetTableOffset = reader.ReadInt64();
                ulong TagCount = reader.ReadUInt64();
                ulong TagCount2 = reader.ReadUInt64();
                reader.JumpAhead(0x8); // Always 0? Verify.

                long ParametersOffset = reader.ReadInt64();

                reader.JumpTo(TypeOffset, false);
                obj.Type = reader.ReadNullTerminatedString();

                reader.JumpTo(NameOffset, false);
                obj.Name = reader.ReadNullTerminatedString();

                reader.JumpTo(ParametersOffset, false);

                if (!templates.ContainsKey(obj.Type))
                {
                    reader.JumpTo(pos);
                    continue;
                }

                foreach (var param in templates[obj.Type].Parameters)
                {
                    SetParameter parameter = new();
                    switch (param.DataType.ToString())
                    {
                        case "System.Byte":
                            parameter.DataType = typeof(uint);
                            parameter.Data = reader.ReadUInt32();
                            obj.Parameters.Add(parameter);
                            break;
                        case "System.Single":
                            parameter.DataType = typeof(float);
                            parameter.Data = reader.ReadSingle();
                            obj.Parameters.Add(parameter);
                            break;
                        default: Console.WriteLine(param.DataType); break;
                    }
                }

                reader.JumpTo(TagsOffsetTableOffset, false);
                for (ulong t = 0; t < TagCount; t++)
                {
                    ObjectTag tag = new();

                    long TagOffset = reader.ReadInt64();
                    long tagPos = reader.BaseStream.Position;

                    reader.JumpTo(TagOffset, false);

                    reader.JumpAhead(0x8); // Always 0? Verify.
                    long TagTypeOffset = reader.ReadInt64();
                    ulong DataLength = reader.ReadUInt64();
                    long DataOffset = reader.ReadInt64();

                    reader.JumpTo(TagTypeOffset, false);
                    tag.Type = reader.ReadNullTerminatedString();

                    reader.JumpTo(DataOffset, false);
                    tag.Data = reader.ReadBytes((int)DataLength);

                    obj.Tags.Add(tag);

                    reader.JumpTo(tagPos);
                }

                Objects.Add(obj);


                reader.JumpTo(pos);
            }
        }

        public override void Save(Stream stream)
        {
            // Set up our BINAWriter and write the BINAV2 header.
            HedgeLib.IO.BINAWriter writer = new(stream, Header);
            writer.WriteNulls(0x10);

            writer.AddOffset($"objectTableOffset", 8);
            writer.Write((ulong)Objects.Count);
            writer.Write((ulong)Objects.Count);
            writer.FixPadding(0x10);

            writer.FillInOffsetLong($"objectTableOffset", false, false);
            writer.AddOffsetTable("objectOffset", (uint)Objects.Count, 8);

            for (int i = 0; i < Objects.Count; i++)
            {
                writer.FillInOffsetLong($"objectOffset_{i}", false, false);

                writer.WriteNulls(0x8);
                writer.AddString($"object{i}Type", Objects[i].Type, 8);
                writer.AddString($"object{i}Name", Objects[i].Name, 8);
                writer.Write(Objects[i].ID);
                writer.Write(Objects[i].GroupID);
                writer.Write(Objects[i].ParentID);
                writer.Write(Objects[i].ParentGroupID);
                writer.Write(Objects[i].UnknownBytes);

                writer.Write(Objects[i].Position.X);
                writer.Write(Objects[i].Position.Y);
                writer.Write(Objects[i].Position.Z);

                writer.Write(Objects[i].Rotation.X);
                writer.Write(Objects[i].Rotation.Y);
                writer.Write(Objects[i].Rotation.Z);

                writer.Write(Objects[i].ChildPositionOffset.X);
                writer.Write(Objects[i].ChildPositionOffset.Y);
                writer.Write(Objects[i].ChildPositionOffset.Z);

                writer.Write(Objects[i].ChildRotationOffset.X);
                writer.Write(Objects[i].ChildRotationOffset.Y);
                writer.Write(Objects[i].ChildRotationOffset.Z);

                writer.AddOffset($"object{i}Tags", 8);
                writer.Write((ulong)Objects[i].Tags.Count);
                writer.Write((ulong)Objects[i].Tags.Count);
                writer.FixPadding(0x10);

                writer.AddOffset($"object{i}Parameters", 8);
                writer.FixPadding(0x10);

                // Tags?
                writer.FillInOffsetLong($"object{i}Tags", false, false);

                writer.AddOffsetTable($"object{i}TagTable", (uint)Objects[i].Tags.Count, 8);
                writer.FixPadding(0x10);

                for (int tags = 0; tags < Objects[i].Tags.Count; tags++)
                {
                    writer.FillInOffsetLong($"object{i}TagTable_{tags}", false, false);
                    writer.WriteNulls(0x8);
                    writer.AddString($"object{i}Tag{tags}Offset", Objects[i].Tags[tags].Type, 8);
                    writer.Write((ulong)Objects[i].Tags[tags].Data.Length);
                    writer.AddOffset($"object{i}Tag{tags}Data", 8);
                }
            }

            for (int i = 0; i < Objects.Count; i++)
            {
                writer.FillInOffsetLong($"object{i}Parameters", false, false);
                foreach (var param in Objects[i].Parameters)
                {
                    switch (param.DataType.ToString())
                    {
                        case "System.Byte": writer.Write((uint)param.Data); break;
                        case "System.UInt32": writer.Write((uint)param.Data); break;
                        case "System.Single": writer.Write((float)param.Data); break;
                        default: Console.WriteLine(param.DataType); break;
                    }
                }
                
                for (int tags = 0; tags < Objects[i].Tags.Count; tags++)
                {
                    writer.FillInOffsetLong($"object{i}Tag{tags}Data", false, false);
                    writer.Write(Objects[i].Tags[tags].Data);
                }

                if (i != Objects.Count - 1)
                    writer.FixPadding(0x10);
            }

            // Finish writing the BINA information.
            writer.FinishWrite(Header);
        }
    }
}
