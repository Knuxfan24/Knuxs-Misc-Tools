namespace Knuxs_Misc_Tools.WrathOfCortex.HGO_Chunk
{
    public class MaterialSet
    {
        public List<Material> Read(BinaryReaderEx reader)
        {
            List<Material> Materials = new();

            // Basic Chunk Header.
            string chunkType = reader.ReadNullPaddedString(4);
            uint chunkSize = reader.ReadUInt32();
            uint materialCount = reader.ReadUInt32();

            for (int i = 0; i < materialCount; i++)
            {
                Material material = new()
                {
                    UnknownUInt32_1 = reader.ReadUInt32(),
                    UnknownUInt32_2 = reader.ReadUInt32(),
                    UnknownUInt32_3 = reader.ReadUInt32(),
                    UnknownUInt32_4 = reader.ReadUInt32(),
                    UnknownUInt32_5 = reader.ReadUInt32(),
                    Colours = reader.ReadVector3(),
                    UnknownFloat_1 = reader.ReadSingle(),
                    UnknownFloat_2 = reader.ReadSingle(),
                    UnknownUInt32_6 = reader.ReadUInt32(),
                    UnknownUInt32_7 = reader.ReadUInt32(),
                    UnknownFloat_3 = reader.ReadSingle(),
                    UnknownFloat_4 = reader.ReadSingle(),
                    BitmapIndex = reader.ReadInt32(),
                    UnknownFloat_5 = reader.ReadSingle(),
                    UnknownFloat_6 = reader.ReadSingle(),
                    UnknownFloat_7 = reader.ReadSingle(),
                    UnknownFloat_8 = reader.ReadSingle(),
                    UnknownFloat_9 = reader.ReadSingle(),
                    UnknownFloat_10 = reader.ReadSingle()
                };
                Materials.Add(material);
            }

            // Align to 0x4.
            reader.FixPadding();

            return Materials;
        }

        public void Write(BinaryWriterEx writer, List<Material> materials)
        {
            // Chunk Identifier.
            writer.Write("00SM");

            // Save the position we'll need to write the chunk's size to and add a dummy value in its place.
            long chunkSizePos = writer.BaseStream.Position;
            writer.Write("SIZE");

            // Write the amount of materials in this file.
            writer.Write(materials.Count);

            // Write the materials.
            for (int i = 0; i < materials.Count; i++)
            {
                writer.Write(materials[i].UnknownUInt32_1);
                writer.Write(materials[i].UnknownUInt32_2);
                writer.Write(materials[i].UnknownUInt32_3);
                writer.Write(materials[i].UnknownUInt32_4);
                writer.Write(materials[i].UnknownUInt32_5);
                writer.Write(materials[i].Colours);
                writer.Write(materials[i].UnknownFloat_1);
                writer.Write(materials[i].UnknownFloat_2);
                writer.Write(materials[i].UnknownUInt32_6);
                writer.Write(materials[i].UnknownUInt32_7);
                writer.Write(materials[i].UnknownFloat_3);
                writer.Write(materials[i].UnknownFloat_4);
                writer.Write(materials[i].BitmapIndex);
                writer.Write(materials[i].UnknownFloat_5);
                writer.Write(materials[i].UnknownFloat_6);
                writer.Write(materials[i].UnknownFloat_7);
                writer.Write(materials[i].UnknownFloat_8);
                writer.Write(materials[i].UnknownFloat_9);
                writer.Write(materials[i].UnknownFloat_10);
            }

            // Align to 0x4.
            writer.FixPadding(0x4);

            // Calculate the chunk size.
            uint chunkSize = (uint)(writer.BaseStream.Position - (chunkSizePos - 0x4));

            // Save our current position.
            long pos = writer.BaseStream.Position;

            // Fill in the chunk size.
            writer.BaseStream.Position = chunkSizePos;
            writer.Write(chunkSize);

            // Jump to our saved position so we can continue.
            writer.BaseStream.Position = pos;
        }
    }

    public class Material
    {
        public uint UnknownUInt32_1 { get; set; }

        public uint UnknownUInt32_2 { get; set; }

        public uint UnknownUInt32_3 { get; set; }

        public uint UnknownUInt32_4 { get; set; }

        public uint UnknownUInt32_5 { get; set; }

        public Vector3 Colours { get; set; }

        public float UnknownFloat_1 { get; set; }

        public float UnknownFloat_2 { get; set; }

        public uint UnknownUInt32_6 { get; set; }

        public uint UnknownUInt32_7 { get; set; }

        public float UnknownFloat_3 { get; set; }

        public float UnknownFloat_4 { get; set; }

        public int BitmapIndex { get; set; }

        public float UnknownFloat_5 { get; set; }

        public float UnknownFloat_6 { get; set; }

        public float UnknownFloat_7 { get; set; }

        public float UnknownFloat_8 { get; set; }

        public float UnknownFloat_9 { get; set; }

        public float UnknownFloat_10 { get; set; }
    }
}
