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

            // Read the BINA Data Table.
            reader.JumpAhead(0x10); // Always 0.
            long ObjectTableOffset = reader.ReadInt64(); // Should always be 0x30 if the gedit has any objects.
            ulong ObjectCount = reader.ReadUInt64();
            reader.JumpAhead(0x8); // Always the same as ObjectCount.
            reader.JumpAhead(0x8); // Always 0

            // Jump to the Table of Objects.
            reader.JumpTo(ObjectTableOffset, false);

            // Loop through each object.
            for (ulong i = 0; i < ObjectCount; i++)
            {
                SetObject obj = new();

                // Get the offset to this object's data.
                long ObjectOffset = reader.ReadInt64();

                // Save our current position in the object table.
                long pos = reader.BaseStream.Position;

                // Jump to the data for this object.
                reader.JumpTo(ObjectOffset, false);

                // Skip eight bytes of nulls.
                reader.JumpAhead(0x8);

                // Get the type and name offsets for this object.
                long TypeOffset = reader.ReadInt64();
                long NameOffset = reader.ReadInt64();

                // Skip eight bytes of nulls, these were the IDs in Forces, but seem to have been scrapped in Frontiers?
                reader.JumpAhead(0x8);

                // Read the new data that Frontiers added.
                // TODO: Figure out what this is, I expect the ID stuff was incorporated into here maybe?
                obj.UnknownBytes = reader.ReadBytes(0x20);

                // Read the position and rotation values for this object.
                // Vector3s are done this way as HedgeLib# had a custom Vector3 thing for some reason???
                obj.Position = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                obj.Rotation = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                obj.ChildPositionOffset = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                obj.ChildRotationOffset = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                // Read this object's tag entry information.
                long TagsOffsetTableOffset = reader.ReadInt64();
                ulong TagCount = reader.ReadUInt64();
                reader.JumpAhead(0x8); // Always the same as TagCount.
                reader.JumpAhead(0x8); // Always 0.

                // Read the offset for this object's parameter table.
                long ParametersOffset = reader.ReadInt64();

                // Jump to and read this object's type and name.
                reader.JumpTo(TypeOffset, false);
                obj.Type = reader.ReadNullTerminatedString();

                reader.JumpTo(NameOffset, false);
                obj.Name = reader.ReadNullTerminatedString();

                // Jump to this object's parameters.
                reader.JumpTo(ParametersOffset, false);

                // Skip this object we don't have a template for it.
                if (!templates.ContainsKey(obj.Type))
                {
                    Console.WriteLine($"Skipped '{obj.Name}' with type '{obj.Type}', Parameters located at 0x{ParametersOffset.ToString("X").PadLeft(8, '0')}.");
                    reader.JumpTo(pos);
                    continue;
                }

                // Read each parameter in this object's template.
                // TODO: Finish the data types.
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

                // Jump to this object's tag table.
                reader.JumpTo(TagsOffsetTableOffset, false);

                // Loop through this object's tags.
                for (ulong t = 0; t < TagCount; t++)
                {
                    ObjectTag tag = new();

                    // Get the offset to this tag's data.
                    long TagOffset = reader.ReadInt64();

                    // Save our current position in the tag table.
                    long tagPos = reader.BaseStream.Position;

                    // Jump to the data for this tag.
                    reader.JumpTo(TagOffset, false);

                    // Skip eight bytes of nulls.
                    reader.JumpAhead(0x8);

                    // Get the offset to this tag's type.
                    long TagTypeOffset = reader.ReadInt64();

                    // Read the length and offset for this tag's data.
                    ulong DataLength = reader.ReadUInt64();
                    long DataOffset = reader.ReadInt64();

                    // Jump to and read this tag's type.
                    reader.JumpTo(TagTypeOffset, false);
                    tag.Type = reader.ReadNullTerminatedString();

                    // Jump to and read this tag's data.
                    reader.JumpTo(DataOffset, false);
                    tag.Data = reader.ReadBytes((int)DataLength);

                    // Save this tag to the object.
                    obj.Tags.Add(tag);

                    // Jump back for the next tag in the table.
                    reader.JumpTo(tagPos);
                }

                // Save this object.
                Objects.Add(obj);

                // Jump back for the next object in the table.
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
                writer.WriteNulls(0x8);
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
