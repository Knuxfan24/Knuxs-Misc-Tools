using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Knuxs_Misc_Tools.WrathOfCortex
{
    public class NuScene : FileBase
    {
        public class FormatData
        {
            public List<string>? Names { get; set; }

            public List<HGObject_Chunk.Texture>? Textures { get; set; }

            public List<HGObject_Chunk.Material>? Materials { get; set; }

            public List<HGObject_Chunk.Geometry>? Geometry { get; set; }

            public HGObject_Chunk.Instance? Instances { get; set; }

            public List<HGObject_Chunk.SPECEntry>? SPEC { get; set; }

            public List<HGObject_Chunk.SplineEntry>? Splines { get; set; }

            public uint? LDirSize { get; set; }

            public HGObject_Chunk.TextureAnimation? TextureAnimations { get; set; }
        }

        public override string Signature { get; } = "0CSG";

        public FormatData Data = new();

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
                Console.WriteLine($"'{chunkType}' at '0x{reader.BaseStream.Position.ToString("X").PadLeft(8, '0')}' with a size of '0x{chunkSize.ToString("X").PadLeft(8, '0')}'.");

                switch (chunkType)
                {
                    case "LBTN":
                        HGObject_Chunk.NameTable? nameTable = new();
                        Data.Names = nameTable.Read(reader);
                        break;

                    case "0TST":
                        HGObject_Chunk.TextureSet textureSet = new();
                        Data.Textures = textureSet.Read(reader);
                        break;

                    case "00SM":
                        HGObject_Chunk.MaterialSet materialSet = new();
                        Data.Materials = materialSet.Read(reader);
                        break;

                    case "0TSG":
                        HGObject_Chunk.GeometrySet geometrySet = new();
                        Data.Geometry = geometrySet.Read(reader);
                        break;

                    case "TSNI":
                        Data.Instances = new();
                        Data.Instances.Read(reader);
                        break;

                    case "CEPS":
                        HGObject_Chunk.SPEC spec = new();
                        Data.SPEC = spec.Read(reader);
                        break;

                    case "0TSS":
                        HGObject_Chunk.Spline sst = new();
                        Data.Splines = sst.Read(reader);
                        break;

                    case "RIDL":
                        Data.LDirSize = chunkSize; // Literally just a large chunk made entirely of nulls because yes?
                        reader.JumpAhead(chunkSize);
                        break;

                    case "0SAT":
                        Data.TextureAnimations = new();
                        Data.TextureAnimations.Read(reader);
                        break;

                    default:
                        Console.WriteLine($"NUS Chunk Type not yet handled.");
                        reader.JumpAhead(chunkSize);
                        break;
                }
            }
        }

        public override void Save(Stream stream)
        {
            // Set up the writer.
            BinaryWriterEx writer = new(stream, true);

            // Write the 0CSG chunk identifier.
            writer.Write(Signature);

            // Save the position we'll need to write the file's size to and add a dummy value in its place.
            long sizePosition = writer.BaseStream.Position;
            writer.Write("SIZE");

            // Save the chunks that exist in this file.
            // TODO: Missing chunks and order.
            if (Data.Names != null)
            {
                HGObject_Chunk.NameTable nameTable = new();
                nameTable.Write(writer, Data.Names);
            }
            if (Data.Textures != null)
            {
                HGObject_Chunk.TextureSet textureSet = new();
                textureSet.Write(writer, Data.Textures);
            }
            if (Data.Materials != null)
            {
                HGObject_Chunk.MaterialSet materialSet = new();
                materialSet.Write(writer, Data.Materials);
            }
            if (Data.LDirSize != null)
            {
                WriteLDir(writer);
            }
            if (Data.Geometry != null)
            {
                HGObject_Chunk.GeometrySet geometrySet = new();
                geometrySet.Write(writer, Data.Geometry);
            }
            if (Data.Instances != null)
            {
                Data.Instances.Write(writer);
            }
            if (Data.SPEC != null)
            {
                HGObject_Chunk.SPEC spec = new();
                spec.Write(writer, Data.SPEC);
            }
            if (Data.Splines != null)
            {
                HGObject_Chunk.Spline sst = new();
                sst.Write(writer, Data.Splines);
            }

            // Write the file size.
            writer.BaseStream.Position = sizePosition;
            writer.Write((uint)writer.BaseStream.Length);

            // Properly close the writer.
            writer.Flush();
            writer.Close();
        }

        private void WriteLDir(BinaryWriterEx writer)
        {
            // Chunk Identifier.
            writer.Write("RIDL");

            // Write this odd chunk length thing.
            writer.Write((uint)Data.LDirSize);

            for (int i = 0; i < Data.LDirSize - 0x8; i++)
                writer.Write((byte)0);
        }

        public void ExportOBJ(string filepath)
        {
            // Initalise a writer variable.
            StreamWriter writer;

            #region OBJ and MTL Writing
            // Loop through all the Geometry entries.
            for (int i1 = 0; i1 < Data.Geometry.Count; i1++)
            {
                // OBJ is stupid and counts from 1 rather than 0.
                // Plus we need this count so that we don't mix up vertex indices.
                int VertexCount = 1;

                // Set up our text writer.
                writer = File.CreateText($@"{Path.GetDirectoryName(filepath)}\{Path.GetFileNameWithoutExtension(filepath)}_geometry{i1}.obj");

                // Write a reference to the MTL.
                writer.Write($"mtllib {Path.GetFileNameWithoutExtension(filepath)}_geometry{i1}.mtl\n");

                // Add a comment numbering this Geometry entry to make the OBJ a tiny bit more organised.
                writer.WriteLine($"\n# geometry{i1}\n");

                // Loop through the Meshes in this Geometry entry.
                for (int i2 = 0; i2 < Data.Geometry[i1].Meshes.Count; i2++)
                {
                    // If there isn't a Primitive here, don't bother writing it.
                    if (Data.Geometry[i1].Meshes[i2].Primitive != null)
                    {
                        // Write the Vertex Positions, Texture Coordinates and Normals.
                        for (int i = 0; i < Data.Geometry[i1].Meshes[i2].Vertices.Count; i++)
                        {
                            writer.WriteLine($"v {Data.Geometry[i1].Meshes[i2].Vertices[i].Position.X:F8} {Data.Geometry[i1].Meshes[i2].Vertices[i].Position.Y:F8} {Data.Geometry[i1].Meshes[i2].Vertices[i].Position.Z:F8}");
                        }

                        writer.WriteLine();
                        for (int i = 0; i < Data.Geometry[i1].Meshes[i2].Vertices.Count; i++)
                        {
                            writer.WriteLine($"vt {Data.Geometry[i1].Meshes[i2].Vertices[i].TextureCoordinates.X:F8} {Data.Geometry[i1].Meshes[i2].Vertices[i].TextureCoordinates.Y:F8}");
                        }

                        writer.WriteLine();
                        for (int i = 0; i < Data.Geometry[i1].Meshes[i2].Vertices.Count; i++)
                        {
                            writer.WriteLine($"vn {Data.Geometry[i1].Meshes[i2].Vertices[i].Normals.X:F8} {Data.Geometry[i1].Meshes[i2].Vertices[i].Normals.Y:F8} {Data.Geometry[i1].Meshes[i2].Vertices[i].Normals.Z:F8}");
                        }

                        writer.WriteLine();
                        for (int i = 0; i < Data.Geometry[i1].Meshes[i2].Vertices.Count; i++)
                        {
                            writer.WriteLine($"vc {Data.Geometry[i1].Meshes[i2].Vertices[i].Colours[1]} {Data.Geometry[i1].Meshes[i2].Vertices[i].Colours[2]} {Data.Geometry[i1].Meshes[i2].Vertices[i].Colours[3]} {Data.Geometry[i1].Meshes[i2].Vertices[i].Colours[0]}");
                        }
                    }
                }

                // Write the object header.
                writer.WriteLine();
                writer.WriteLine($"o geometry{i1}");
                writer.WriteLine($"g geometry{i1}");

                // Loop through by Mesh so we can split it by material.
                for (int i2 = 0; i2 < Data.Geometry[i1].Meshes.Count; i2++)
                {
                    // If there isn't a Primitive here, don't bother writing it.
                    if (Data.Geometry[i1].Meshes[i2].Primitive != null)
                    {
                        writer.WriteLine($"usemtl Material{Data.Geometry[i1].Meshes[i2].MaterialIndex}");

                        // Write the Triangle Strips if the type is 6.
                        if (Data.Geometry[i1].Meshes[i2].Primitive.Type == 6)
                        {
                            for (int i = 0; i < Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips.Count; i++)
                            {
                                for (int v = 0; v < Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i].Count - 2; v++)
                                {
                                    if ((v & 1) == 0)
                                        writer.WriteLine($"f " +
                                                         $"{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 0] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 0] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 0] + VertexCount} " +
                                                         $"{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 1] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 1] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 1] + VertexCount} " +
                                                         $"{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 2] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 2] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 2] + VertexCount}");
                                    else
                                        writer.WriteLine($"f " +
                                                         $"{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 1] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 1] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 1] + VertexCount} " +
                                                         $"{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 0] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 0] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 0] + VertexCount} " +
                                                         $"{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 2] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 2] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 2] + VertexCount}");
                                }
                            }
                            writer.WriteLine();
                        }

                        // Write the standard Triangle List if the type is 5.
                        else
                        {
                            // TODO: This doesn't work right.
                            for (int i = 0; i < Data.Geometry[i1].Meshes[i2].Primitive.FaceIndices.Count - 2; i++)
                            {
                                writer.WriteLine($"f " +
                                                        $"{Data.Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.FaceIndices[i] + VertexCount} " +
                                                        $"{Data.Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 1] + VertexCount} " +
                                                        $"{Data.Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.FaceIndices[i + 2] + VertexCount}");
                            }
                            writer.WriteLine();
                        }

                        // Increment VertexCount so we don't accidentally use Vertices from the wrong mesh.
                        VertexCount += Data.Geometry[i1].Meshes[i2].Vertices.Count;
                    }
                }

                // Properly close the writer.
                writer.Flush();
                writer.Close();

                // Set up a list of Materials Indices we've already written so we don't dupe them.
                List<uint> writtenMats = new();

                // Recreate the text writer with the same name as the filepath, but with .obj replaced with .mtl.
                writer = File.CreateText($@"{Path.GetDirectoryName(filepath)}\{Path.GetFileNameWithoutExtension(filepath)}_geometry{i1}.mtl");

                // Loop through the Meshes in this Geometry entry.
                for (int i2 = 0; i2 < Data.Geometry[i1].Meshes.Count; i2++)
                {
                    // Don't bother writing this material if the Mesh calling it doesn't have any Primitive data.
                    if (Data.Geometry[i1].Meshes[i2].Primitive != null)
                    {
                        // Only write this material if we haven't already and if we need to.
                        if (!writtenMats.Contains(Data.Geometry[i1].Meshes[i2].MaterialIndex))
                        {
                            // Mark this index as having been written.
                            writtenMats.Add(Data.Geometry[i1].Meshes[i2].MaterialIndex);

                            // Write a newmtl header.
                            writer.WriteLine($"newmtl Material{Data.Geometry[i1].Meshes[i2].MaterialIndex}");

                            // Write the material colours.
                            writer.WriteLine($"\tKd {Data.Materials[(int)Data.Geometry[i1].Meshes[i2].MaterialIndex].Colours.X} {Data.Materials[(int)Data.Geometry[i1].Meshes[i2].MaterialIndex].Colours.Y} {Data.Materials[(int)Data.Geometry[i1].Meshes[i2].MaterialIndex].Colours.Z}");

                            // Write a placeholder diffuse map entry if this material has one.
                            // Also includes a comment with the type.
                            if (Data.Materials[(int)Data.Geometry[i1].Meshes[i2].MaterialIndex].BitmapIndex != -1)
                            {
                                writer.WriteLine($"\t#Bitmap Type = 0x{Data.Textures[Data.Materials[(int)Data.Geometry[i1].Meshes[i2].MaterialIndex].BitmapIndex].Type:X}");
                                writer.WriteLine($"\tmap_Kd bitmap{Data.Materials[(int)Data.Geometry[i1].Meshes[i2].MaterialIndex].BitmapIndex}.png");

                            }

                            writer.WriteLine();
                        }
                    }
                }

                // Properly close the writer.
                writer.Flush();
                writer.Close();
            }

            // Set up our text writer again for the splines.
            writer = File.CreateText($@"{Path.GetDirectoryName(filepath)}\{Path.GetFileNameWithoutExtension(filepath)}_splines.obj");

            // Write spline data to the file.
            for (int i = 0; i < Data.Splines.Count; i++)
            {
                // OBJ is stupid and counts from 1 rather than 0. Same as above.
                int VertexCount = 1;

                // Add a comment numbering this SST entry to make the OBJ a tiny bit more organised.
                writer.WriteLine($"\n# spline{i}_0x{Data.Splines[i].UnknownUInt32_1.ToString("X").PadLeft(8, '0')}\n");

                // Write each point on this spline.
                for (int v = 0; v < Data.Splines[i].SplinePoints.Count; v++)
                    writer.WriteLine($"v {Data.Splines[i].SplinePoints[v].X} {Data.Splines[i].SplinePoints[v].Y} {Data.Splines[i].SplinePoints[v].Z}");

                // Write the object header.
                writer.WriteLine();
                writer.WriteLine($"o spline{i}_0x{Data.Splines[i].UnknownUInt32_1.ToString("X").PadLeft(8, '0')}");
                writer.WriteLine($"g spline{i}_0x{Data.Splines[i].UnknownUInt32_1.ToString("X").PadLeft(8, '0')}");

                // Write the line header.
                writer.Write("l ");

                // Write each point as a vertex index.
                for (int v = 0; v < Data.Splines[i].SplinePoints.Count; v++)
                    writer.Write($"{v + VertexCount} ");

                writer.WriteLine();

                // Increment VertexCount so we don't accidentally use Vertices from the wrong mesh or spline.
                VertexCount += Data.Splines[i].SplinePoints.Count;
            }

            // Properly close the writer.
            writer.Flush();
            writer.Close();
            #endregion

            #region Bitmap Exporting
            // Half functional, fails on certain textures, breaks others and doesn't yet handle any non DXT1 types.
            for (int i = 0; i < Data.Textures.Count; i++)
            {
                try
                {
                    switch (Data.Textures[i].Type)
                    {
                        case 0x80:
                            var image = new Image<Byte4>((int)Data.Textures[i].Width, (int)Data.Textures[i].Height);
                            int index = 0;
                            for (int y = 0; y < Data.Textures[i].Height; y += 8)
                            {
                                for (var x = 0; x < Data.Textures[i].Height; x += 8)
                                {
                                    Helpers.DecodeDXTBlock(ref image, Data.Textures[i].Data, index, x, y);
                                    index += 8;
                                    Helpers.DecodeDXTBlock(ref image, Data.Textures[i].Data, index, x + 4, y);
                                    index += 8;
                                    Helpers.DecodeDXTBlock(ref image, Data.Textures[i].Data, index, x, y + 4);
                                    index += 8;
                                    Helpers.DecodeDXTBlock(ref image, Data.Textures[i].Data, index, x + 4, y + 4);
                                    index += 8;
                                }
                            }
                            image.SaveAsPng($@"{Path.GetDirectoryName(filepath)}\bitmap{i}.png");
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine($"Failed on {i}.");
                }
            }


            #endregion
        }
    }
}
