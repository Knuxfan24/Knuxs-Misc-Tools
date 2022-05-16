namespace Knuxs_Misc_Tools.WrathOfCortex
{
    public class NUS : FileBase
    {
        public override string Signature { get; } = "0CSG";

        public List<string>? Names { get; set; }

        public List<HGO_Chunk.Texture>? Textures { get; set; }

        public List<HGO_Chunk.Material>? Materials { get; set; }

        public List<HGO_Chunk.Geometry>? Geometry { get; set; }

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream, true);

            // Basic Chunk Header.
            reader.ReadSignature(4, Signature);
            uint fileSize = reader.ReadUInt32();

            while (reader.BaseStream.Position < fileSize)
            {
                string chunkType = reader.ReadNullPaddedString(4);
                uint chunkSize = reader.ReadUInt32();
                reader.JumpBehind(8);

                switch (chunkType)
                {
                    case "LBTN":
                        HGO_Chunk.NameTable? nameTable = new();
                        Names = nameTable.Read(reader);
                        break;

                    case "0TST":
                        HGO_Chunk.TextureSet textureSet = new();
                        Textures = textureSet.Read(reader);
                        break;

                    case "00SM":
                        HGO_Chunk.MaterialSet materialSet = new();
                        Materials = materialSet.Read(reader);
                        break;

                    case "0TSG":
                        HGO_Chunk.GeometrySet geometrySet = new();
                        Geometry = geometrySet.Read(reader);
                        break;

                    case "TSNI":
                        HGO_Chunk.INST inst = new();
                        inst.Read(reader);
                        break;

                    default:
                        Console.WriteLine($"NUS Chunk Type '{chunkType}' not yet handled.");
                        reader.JumpAhead(chunkSize);
                        break;
                }
            }
        }
    
        public void ExportOBJ(string filepath)
        {
            #region OBJ Writing
            // OBJ is stupid and counts from 1 rather than 0.
            int VertexCount = 1;

            // Set up our text writer.
            StreamWriter writer = File.CreateText(filepath);

            // Loop through all the Geometry entries.
            for (int i1 = 0; i1 < Geometry.Count; i1++)
            {
                // Add a comment numbering this Geometry entry to make the OBJ a tiny bit more organised.
                writer.WriteLine($"\n# geometry{i1}\n");

                // Loop through the Meshes in this Geometry entry.
                for (int i2 = 0; i2 < Geometry[i1].Meshes.Count; i2++)
                {
                    // If there isn't a Primitive here, don't bother writing it.
                    if (Geometry[i1].Meshes[i2].Primitive != null)
                    {
                        // Write the Vertex Positions, Texture Coordinates and Normals.
                        for (int i = 0; i < Geometry[i1].Meshes[i2].Vertices.Count; i++)
                        {
                            writer.WriteLine($"v {Geometry[i1].Meshes[i2].Vertices[i].Position.X} {Geometry[i1].Meshes[i2].Vertices[i].Position.Y} {Geometry[i1].Meshes[i2].Vertices[i].Position.Z}");
                        }

                        writer.WriteLine();
                        for (int i = 0; i < Geometry[i1].Meshes[i2].Vertices.Count; i++)
                        {
                            writer.WriteLine($"vt {Geometry[i1].Meshes[i2].Vertices[i].TextureCoordinates.X} {Geometry[i1].Meshes[i2].Vertices[i].TextureCoordinates.Y}");
                        }

                        writer.WriteLine();
                        for (int i = 0; i < Geometry[i1].Meshes[i2].Vertices.Count; i++)
                        {
                            writer.WriteLine($"vn {Geometry[i1].Meshes[i2].Vertices[i].Normals.X} {Geometry[i1].Meshes[i2].Vertices[i].Normals.Y} {Geometry[i1].Meshes[i2].Vertices[i].Normals.Z}");
                        }

                        // Write the object header.
                        writer.WriteLine();
                        writer.WriteLine($"o geometry{i1}mesh{i2}");
                        writer.WriteLine($"g geometry{i1}mesh{i2}");
                        writer.WriteLine($"usemtl Material{Geometry[i1].Meshes[i2].MaterialIndex}");

                        // Write the Triangle Strips if the type is 6.
                        // TODO: This conversion is fucked.
                        if (Geometry[i1].Meshes[i2].Primitive.Type == 6)
                        {
                            for (int i = 0; i < Geometry[i1].Meshes[i2].Primitive.FaceIndices.Count - 2; i++)
                            {
                                if ((i & 1) > 0)
                                    writer.WriteLine($"f " +
                                                     $"{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + VertexCount} " +
                                                     $"{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + VertexCount} " +
                                                     $"{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + VertexCount}");
                                else
                                    writer.WriteLine($"f " +
                                                     $"{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + VertexCount} " +
                                                     $"{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + VertexCount} " +
                                                     $"{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + VertexCount}");
                            }
                        }

                        // Write the standard Triangle List if the type is 5.
                        else
                        {
                            for (int i = 0; i < Geometry[i1].Meshes[i2].Primitive.FaceIndices.Count - 2; i++)
                            {
                                writer.WriteLine($"f " +
                                                     $"{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + VertexCount} " +
                                                     $"{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + VertexCount} " +
                                                     $"{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + VertexCount}/{Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + VertexCount}");
                            }
                        }

                        // Increment VertexCount so we don't accidentally use Vertices from the wrong mesh.
                        VertexCount += Geometry[i1].Meshes[i2].Vertices.Count;
                    }
                }
            }

            // Properly close the writer.
            writer.Flush();
            writer.Close();
            #endregion

            #region MTL Writing
            // Set up a list of Materials Indices we've already written so we don't dupe them.
            List<uint> writtenMats = new();

            // Recreate the text writer with the same name as the filepath, but with .obj replaced with .mtl.
            writer = File.CreateText($@"{Path.GetDirectoryName(filepath)}\{Path.GetFileNameWithoutExtension(filepath)}.mtl");

            // Loop through all the Geometry entries.
            for (int i1 = 0; i1 < Geometry.Count; i1++)
            {
                // Loop through the Meshes in this Geometry entry.
                for (int i2 = 0; i2 < Geometry[i1].Meshes.Count; i2++)
                {
                    // Don't bother writing this material if the Mesh calling it doesn't have any Primitive data.
                    if (Geometry[i1].Meshes[i2].Primitive != null)
                    {
                        // Only write this material if we haven't already.
                        if (!writtenMats.Contains(Geometry[i1].Meshes[i2].MaterialIndex))
                        {
                            // Mark this index as having been written.
                            writtenMats.Add(Geometry[i1].Meshes[i2].MaterialIndex);

                            // Write a newmtl header.
                            writer.WriteLine($"newmtl Material{Geometry[i1].Meshes[i2].MaterialIndex}");

                            // Write the material colours.
                            writer.WriteLine($"\tKd {Materials[(int)Geometry[i1].Meshes[i2].MaterialIndex].Colours.X} {Materials[(int)Geometry[i1].Meshes[i2].MaterialIndex].Colours.Y} {Materials[(int)Geometry[i1].Meshes[i2].MaterialIndex].Colours.Z}");
                            
                            // Write a placeholder diffuse map entry if this material has one.
                            // Also includes a comment with the type.
                            if (Materials[(int)Geometry[i1].Meshes[i2].MaterialIndex].BitmapIndex != -1)
                            {
                                writer.WriteLine($"\t#Bitmap Type = 0x{Textures[Materials[(int)Geometry[i1].Meshes[i2].MaterialIndex].BitmapIndex].Type:X}");
                                writer.WriteLine($"\tmap_Kd bitmap{Materials[(int)Geometry[i1].Meshes[i2].MaterialIndex].BitmapIndex}");
                                    
                            }

                            writer.WriteLine();
                        }
                    }
                }
            }

            // Properly close the writer.
            writer.Flush();
            writer.Close();
            #endregion

            #region Bitmap Exporting
            // Not functional, outputs garbage, hardcoded path.
            //for (int i = 0; i < Textures.Count; i++)
            //{
            //    if (Textures[i].Type == 0x80)
            //    {
            //        DXT1Decompressor.DXT1Decompressor d = new((int)Textures[i].Width, (int)Textures[i].Height, Textures[i].Data);
            //        var bitmap = d.ToBitmap();
            //        bitmap.Save($@"Y:\test{i}.bmp");
            //    }
            //}
            #endregion
        }
    }
}
