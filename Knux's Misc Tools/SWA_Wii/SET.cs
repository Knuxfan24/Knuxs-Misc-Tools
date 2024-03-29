﻿using HedgeLib.Sets;

namespace Knuxs_Misc_Tools.SWA_Wii
{
    public class SET : FileBase
    {
        public class SetObject
        {
            public uint Type { get; set; } // How is this set up? Is it something like Heroes?

            public Vector3 Position { get; set; }
            
            public Quaternion Rotation { get; set; }

            // Is there anyway to identify the parameter data types like in '06?
            public List<int> Parameters_Ints { get; set; } = new();

            public List<float> Parameters_Floats { get; set; } = new();
        }

        // Hardcoded list of objects I've made preliminary GLVL Templates for.
        readonly Dictionary<uint, string> KnownTypes = new()
        {
            { 0x00000009, "checkpoint" },
            { 0x10000001, "superring" },
            { 0x10000002, "ring" },
            { 0x10010002, "itembox" },
            { 0x10010006, "goalring" },
            { 0x1001000D, "hint_collision" },
            { 0x10020002, "jumppanel" },
            { 0x1002000A, "dashpanel" },
            { 0x1002000B, "dashring" },
            { 0x1002000D, "spring" },
            { 0x1002000E, "widespring" },
            { 0x10020010, "updownreel" },
            { 0x10020017, "woodbox" },
            { 0x10020021, "button_hint_volume" },
            { 0x10050004, "camera" },
            { 0x20000001, "enemy_spinner" },
            { 0x20000005, "enemy_eggfighter" }
        };

        public List<SetObject> Objects = new();

        /// <summary>
        /// Loads an Unleashed Wii SET file.
        /// </summary>
        /// <param name="filepath">The file to parse.</param>
        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream);

            byte version = reader.ReadByte(); // Not sure if version's the right name, but it's always 1.
            ushort objectCount = reader.ReadUInt16();
            reader.JumpAhead(0x2); // Always 0, padding probably?
            ushort fileSize = reader.ReadUInt16();
            reader.FixPadding(0x10); // Always 0, padding probably.

            // Everything after the header is Big Endian for some reason???
            reader.IsBigEndian = true;

            for (int i = 0; i < objectCount; i++)
            {
                // Get the offset and length of this object.
                uint objectOffset = reader.ReadUInt32();
                uint objectLength = reader.ReadUInt32();

                // Save our position in the list.
                long pos = reader.BaseStream.Position;

                // Jump to our object.
                reader.JumpTo(objectOffset);

                reader.JumpAhead(0x4); // Copy of the object length?

                SetObject obj = new()
                {
                    Type = reader.ReadUInt32(),
                    Position = reader.ReadVector3(),
                    Rotation = reader.ReadQuaternion()
                };

                // Read the remaining length of the object as parameters (this is probably horribly scuffed).
                uint parameterLength = objectLength - 0x24;
                for (int p = 0; p < parameterLength / 4; p++)
                    obj.Parameters_Ints.Add(reader.ReadInt32());

                reader.JumpBehind(parameterLength);
                for (int p = 0; p < parameterLength / 4; p++)
                    obj.Parameters_Floats.Add(reader.ReadSingle());

                // Save our object.
                Objects.Add(obj);

                // Jump back for the next one.
                reader.JumpTo(pos);
            }

            reader.Close();
        }

        /// <summary>
        /// Saves an Unleashed Wii SET File.
        /// </summary>
        /// <param name="filepath">The filepath to save to.</param>
        public override void Save(Stream stream)
        {
            // Set up the writer.
            BinaryWriterEx writer = new(stream);

            // Header.
            writer.Write((byte)1); // Version.
            writer.Write((ushort)Objects.Count); // Object Count.
            writer.WriteNulls(0x2); // Padding.
            writer.Write("FS"); // Placeholder for the file size.
            writer.FixPadding(0x10); // Padding.

            // Flip the writer's endianness.
            writer.IsBigEndian = true;

            // Write the object offset table.
            for (int i = 0; i < Objects.Count; i++)
            {
                writer.AddOffset($"Object{i}");
                writer.Write(0x24 + (Objects[i].Parameters_Ints.Count * 4));
            }

            // Write the objects.
            for (int i = 0; i < Objects.Count; i++)
            {
                writer.FillOffset($"Object{i}");

                writer.Write(0x24 + (Objects[i].Parameters_Ints.Count * 4));
                writer.Write(Objects[i].Type);
                writer.Write(Objects[i].Position);
                writer.Write(Objects[i].Rotation);

                for (int p = 0; p < Objects[i].Parameters_Ints.Count; p++)
                    writer.Write(Objects[i].Parameters_Ints[p]);
            }

            // Flip the writer's endianness (again).
            writer.IsBigEndian = false;

            // Jump back to the placeholder file size.
            writer.BaseStream.Position = 0x05;

            // Overwrite it with our actual file size.
            writer.Write((ushort)writer.BaseStream.Length);

            // Close the writer.
            writer.Close();
        }

        /// <summary>
        /// Uses HedgeLib to write a Generations SET File.
        /// </summary>
        /// <param name="filepath">The path to the .set.xml to write.</param>
        public void DumpGLVL(string filepath, string templatesPath)
        {
            // Load our templates.
            var templates = SetObjectType.LoadObjectTemplates(templatesPath);

            // Create the Generations SET File.
            GensSetData set = new();

            // Loop through each object.
            for (int i = 0; i < Objects.Count; i++)
            {
                // Create a Generations object.
                HedgeLib.Sets.SetObject gensObj = new();
                gensObj.Transform.Position = new(Objects[i].Position.X, Objects[i].Position.Y, Objects[i].Position.Z);
                gensObj.Transform.Rotation = new(Objects[i].Rotation.X, Objects[i].Rotation.Y, Objects[i].Rotation.Z, Objects[i].Rotation.W);
                gensObj.ObjectID = (uint)i;

                // Check if we have a template for this object.
                var objectType = KnownTypes.FirstOrDefault(x => x.Key == Objects[i].Type);
                var template = templates.FirstOrDefault(x => x.Key == objectType.Value);

                // If we don't have a template, add a dummy Unknown object with the UInt values as parameters.
                if (template.Value == null && template.Key == null)
                {
                    gensObj.ObjectType = $"Unknown_0x{Objects[i].Type:X}";

                    for (int p = 0; p < Objects[i].Parameters_Ints.Count; p++)
                        gensObj.Parameters.Add(new SetObjectParam(typeof(int), Objects[i].Parameters_Ints[p]));
                }

                // If we do have a template, fill things out from it.
                else
                {
                    gensObj.ObjectType = template.Key;
                    for (int p = 0; p < template.Value.Parameters.Count; p++)
                    {
                        if (template.Value.Parameters[p].DataType == typeof(Single))
                            gensObj.Parameters.Add(new SetObjectParam(typeof(float), Objects[i].Parameters_Floats[p]));
                        else
                            gensObj.Parameters.Add(new SetObjectParam(typeof(int), Objects[i].Parameters_Ints[p]));
                    }
                }

                // Save this object.
                set.Objects.Add(gensObj);
            }

            // Save the SET file.
            set.Save(filepath, true);

            // Stupid shit stolen from the '06 GLVL Converter to workaround HedgeLib not writing parameter names god I hate this wtf wtf wtf.
            string[] StupidHedgeLibWorkaround = File.ReadAllLines(filepath);
            bool objectFound = false;
            int objectParam = 0;
            string file;
            int lineNumber = 0;
            string objectName = "";
            List<string> paramNames = new();

            foreach (string line in StupidHedgeLibWorkaround)
            {
                if (line.StartsWith("  <") && !line.StartsWith("  </"))
                {
                    if (!objectFound)
                    {
                        int startIndex = line.IndexOf("<") + 1;
                        int endIndex = line.IndexOf(">") + 1;

                        objectName = line.Substring(startIndex, endIndex - startIndex - 1);
                        file = Directory.GetFiles(templatesPath, line.Substring(startIndex, endIndex - startIndex - 1) + ".xml", SearchOption.AllDirectories)
                        .FirstOrDefault();

                        if (file == null)
                        {
                            lineNumber++;
                            continue;
                        }

                        paramNames.Clear();
                        string[] template = File.ReadAllLines(file);
                        foreach (string param in template)
                            if (param.Contains("type") && !param.Contains("Extra"))
                            {
                                var test = param.IndexOf("<");
                                var test2 = param.Substring(test + 1);
                                var test3 = test2.IndexOf(" ");
                                test2 = test2.Remove(test3);
                                paramNames.Add(test2);
                            }

                        objectParam = 0;
                        objectFound = true;
                    }
                }
                else
                {
                    if (line.StartsWith("    <") && objectFound)
                        if (line == "    <Position>") objectFound = false;
                        else
                        {
                            if (line.Contains("</") && line.Contains("<") && line.Contains(">") && !line.Contains("  </"))
                            {
                                List<string> temp = new List<string> { "    <" };
                                int startIndex = line.IndexOf("<") + 1;
                                int endIndex = line.IndexOf(">") + 1;
                                temp.Add(line.Substring(startIndex, endIndex - startIndex - 1));
                                temp.Add(">");

                                startIndex = line.IndexOf(">") + 1;
                                endIndex = line.IndexOf("</") + 1;
                                temp.Add(line.Substring(startIndex, endIndex - startIndex - 1));

                                temp.Add("</");
                                temp.Add(temp[1]);
                                temp.Add(">");

                                temp[1] = paramNames[objectParam];
                                temp[5] = paramNames[objectParam];
                                objectParam++;
                                StupidHedgeLibWorkaround[lineNumber] = string.Join("", temp);
                            }
                        }
                }
                lineNumber++;
            }

            for (int i = StupidHedgeLibWorkaround.Length - 1; i >= 0; i--)
            {
                if (StupidHedgeLibWorkaround[i] == "    <Range>100</Range>")
                    StupidHedgeLibWorkaround[i] = null;
            }
            StupidHedgeLibWorkaround = StupidHedgeLibWorkaround.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            File.WriteAllLines(filepath, StupidHedgeLibWorkaround);
        }
    
        public void ImportGLVL(string filepath, string templatesPath)
        {
            // Load our templates.
            var templates = SetObjectType.LoadObjectTemplates(templatesPath);

            // Load the Generations SET File.
            GensSetData set = new();
            set.Load(filepath);

            // Loop through each object.
            for (int i = 0; i < set.Objects.Count; i++)
            {
                // Check if we have a template for this object.
                var objectType = KnownTypes.FirstOrDefault(x => x.Value == set.Objects[i].ObjectType);
                var template = templates.FirstOrDefault(x => x.Key == objectType.Value);

                // Create a new Unleashed Wii SET Object.
                SetObject obj = new()
                {
                    Position = new(set.Objects[i].Transform.Position.X, set.Objects[i].Transform.Position.Y, set.Objects[i].Transform.Position.Z),
                    Rotation = new(set.Objects[i].Transform.Rotation.X, set.Objects[i].Transform.Rotation.Y, set.Objects[i].Transform.Rotation.Z, set.Objects[i].Transform.Rotation.W)
                };

                // Check if this type is known, if not, then just take it from its name.
                if (set.Objects[i].ObjectType.StartsWith("Unknown_0x"))
                    obj.Type = uint.Parse(set.Objects[i].ObjectType[10..], System.Globalization.NumberStyles.HexNumber);
                else
                    obj.Type = objectType.Key;

                // Loop through the parameters.
                for (int p = 0; p < set.Objects[i].Parameters.Count; p++)
                {
                    // Use a template for types if we have one.
                    if (template.Value != null)
                    {
                        if (template.Value.Parameters[p].DataType == typeof(int))
                        {
                            // Conver the int to hex so we can add it to the floats list correctly.
                            int data = (int)set.Objects[i].Parameters[p].Data;
                            string hexStringData = data.ToString("X");

                            // Add this parameter to the lists.
                            obj.Parameters_Ints.Add(data);
                            obj.Parameters_Floats.Add(Helpers.FromHexString(hexStringData));
                        }

                        if (template.Value.Parameters[p].DataType == typeof(float))
                        {
                            // Stupid dumb workaround for HedgeLib detecting whole numbers in floats as ints.
                            int wtf = 0;
                            float data = 0f;

                            // Try read this value as an int.
                            try { wtf = (int)set.Objects[i].Parameters[p].Data; }
                            catch { }

                            // Try read this value as a float.
                            try { data = (float)set.Objects[i].Parameters[p].Data; }
                            catch { data = wtf; }

                            // Convert the float to hex so we can add it to the ints list correctly.
                            string hexStringData = Helpers.ToHexString(data);

                            // Add this parameter to the lists.
                            obj.Parameters_Ints.Add(int.Parse(hexStringData, System.Globalization.NumberStyles.HexNumber));
                            obj.Parameters_Floats.Add(data);
                        }
                    }

                    // Fall back on HedgeLib#'s awkward type detection if not.
                    else
                    {
                        if (set.Objects[i].Parameters[p].DataType == typeof(int))
                        {
                            int data = (int)set.Objects[i].Parameters[p].Data;
                            string hexStringData = data.ToString("X");

                            obj.Parameters_Ints.Add(data);
                            obj.Parameters_Floats.Add(Helpers.FromHexString(hexStringData));
                        }

                        if (set.Objects[i].Parameters[p].DataType == typeof(float))
                        {
                            float data = (float)set.Objects[i].Parameters[p].Data;
                            string hexStringData = Helpers.ToHexString(data);

                            obj.Parameters_Ints.Add(int.Parse(hexStringData, System.Globalization.NumberStyles.HexNumber));
                            obj.Parameters_Floats.Add(data);
                        }
                    }
                }

                // Add this object.
                Objects.Add(obj);
            }
        }
    }
}
