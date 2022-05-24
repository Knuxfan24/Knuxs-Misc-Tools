using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Encoding.Implementation;
using SkiaSharp;

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

            public List<HGObject_Chunk.SSTEntry>? SST { get; set; }

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
                        HGObject_Chunk.SST sst = new();
                        Data.SST = sst.Read(reader);
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
            if (Data.SST != null)
            {
                HGObject_Chunk.SST sst = new();
                sst.Write(writer, Data.SST);
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
            // Set up our text writer.
            StreamWriter writer = File.CreateText(filepath);

            #region OBJ Writing
            // Write a reference to the MTL.
            writer.Write($"mtllib {Path.GetFileNameWithoutExtension(filepath)}.mtl\n");

            // OBJ is stupid and counts from 1 rather than 0.
            int VertexCount = 1;

            // Loop through all the Geometry entries.
            for (int i1 = 0; i1 < Data.Geometry.Count; i1++)
            {
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
                            writer.WriteLine($"v {Data.Geometry[i1].Meshes[i2].Vertices[i].Position.X} {Data.Geometry[i1].Meshes[i2].Vertices[i].Position.Y} {Data.Geometry[i1].Meshes[i2].Vertices[i].Position.Z}");
                        }

                        writer.WriteLine();
                        for (int i = 0; i < Data.Geometry[i1].Meshes[i2].Vertices.Count; i++)
                        {
                            writer.WriteLine($"vt {Data.Geometry[i1].Meshes[i2].Vertices[i].TextureCoordinates.X} {Data.Geometry[i1].Meshes[i2].Vertices[i].TextureCoordinates.Y}");
                        }

                        writer.WriteLine();
                        for (int i = 0; i < Data.Geometry[i1].Meshes[i2].Vertices.Count; i++)
                        {
                            writer.WriteLine($"vn {Data.Geometry[i1].Meshes[i2].Vertices[i].Normals.X} {Data.Geometry[i1].Meshes[i2].Vertices[i].Normals.Y} {Data.Geometry[i1].Meshes[i2].Vertices[i].Normals.Z}");
                        }

                        // Write the object header.
                        writer.WriteLine();
                        writer.WriteLine($"o geometry{i1}mesh{i2}");
                        writer.WriteLine($"g geometry{i1}mesh{i2}");
                        writer.WriteLine($"usemtl Material{Data.Geometry[i1].Meshes[i2].MaterialIndex}");

                        // Write the Triangle Strips if the type is 6.
                        if (Data.Geometry[i1].Meshes[i2].Primitive.Type == 6)
                        {
                            // TODO: Half the faces here are backwards.
                            for (int i = 0; i < Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips.Count; i++)
                            {
                                for (int v = 0; v < Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i].Count - 2; v++)
                                {
                                    if ((i & 1) > 0)
                                        writer.WriteLine($"f " +
                                                         $"{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v] + VertexCount} " +
                                                         $"{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 1] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 1] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 1] + VertexCount} " +
                                                         $"{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 2] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 2] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 2] + VertexCount}");
                                    else
                                        writer.WriteLine($"f " +
                                                         $"{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 1] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 1] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 1] + VertexCount} " +
                                                         $"{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v] + VertexCount} " +
                                                         $"{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 2] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 2] + VertexCount}/{Data.Geometry[i1].Meshes[i2].Primitive.TriangleStrips[i][v + 2] + VertexCount}");
                                }}
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
            for (int i1 = 0; i1 < Data.Geometry.Count; i1++)
            {
                // Loop through the Meshes in this Geometry entry.
                for (int i2 = 0; i2 < Data.Geometry[i1].Meshes.Count; i2++)
                {
                    // Don't bother writing this material if the Mesh calling it doesn't have any Primitive data.
                    if (Data.Geometry[i1].Meshes[i2].Primitive != null)
                    {
                        // Only write this material if we haven't already.
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
            }

            // Properly close the writer.
            writer.Flush();
            writer.Close();
            #endregion

            #region Bitmap Exporting
            // Not functional, outputs garbage/crashes due to me not knowing how to use the Deswizzler.
            //for (int i = 0; i < Data.Textures.Count; i++)
            //{
            //    if (Data.Textures[i].Type == 0x80)
            //    {
            //        Rainbow.ImgLib.Filters.TileFilter test = new(8, 16, 4, (int)Data.Textures[i].Width, (int)Data.Textures[i].Height);
            //        SKBitmap? decodedBitmap = new ImageDecoderDirectColor(Data.Textures[i].Data, (int)Data.Textures[i].Width, (int)Data.Textures[i].Height, new ColorCodecDXT1(Rainbow.ImgLib.Common.ByteOrder.BigEndian, (int)Data.Textures[i].Width, (int)Data.Textures[i].Height), test).DecodeImage();

            //        using (SKFileWStream? outputStream = new($@"{Path.GetDirectoryName(filepath)}\bitmap{i}.png"))
            //            SKPixmap.Encode(outputStream, decodedBitmap, SKEncodedImageFormat.Png, 100);
            //    }
            //}


            #endregion
        }

        // Literally just to try and see if all the vertices looked like they were in the right positions.
        // Only Blender will accept this OBJ, Max doesn't find anything in it for obvious reasons.
        public void DumpVertexPositions(string filepath)
        {
            // Set up our text writer.
            StreamWriter writer = File.CreateText(filepath);

            // Loop through the Geometry Sets.
            for (int i1 = 0; i1 < Data.Geometry.Count; i1++)
            {
                // Loop through the Meshes in this Geometry entry.
                for (int i2 = 0; i2 < Data.Geometry[i1].Meshes.Count; i2++)
                {
                    // Write each vertex in this Mesh Set as its own object.
                    for (int i = 0; i < Data.Geometry[i1].Meshes[i2].Vertices.Count; i++)
                    {
                        writer.WriteLine($"o geometry{i1}mesh{i2}vertex{i}");
                        writer.WriteLine($"v {Data.Geometry[i1].Meshes[i2].Vertices[i].Position.X} {Data.Geometry[i1].Meshes[i2].Vertices[i].Position.Y} {Data.Geometry[i1].Meshes[i2].Vertices[i].Position.Z}\n");
                    }
                }
            }

            // Properly close the writer.
            writer.Flush();
            writer.Close();
        }
    }
}
