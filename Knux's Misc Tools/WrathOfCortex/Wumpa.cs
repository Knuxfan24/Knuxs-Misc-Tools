namespace Knuxs_Misc_Tools.WrathOfCortex
{
    internal class Wumpa
    {
        List<Vector3> Coordinates = new();

        public void Load(string filepath, bool bigEndian = false)
        {
            BinaryReaderEx reader = new(File.OpenRead(filepath), bigEndian);

            uint wumpaCount = reader.ReadUInt32();
            for (int i = 0; i < wumpaCount; i++)
                Coordinates.Add(reader.ReadVector3());

            reader.Close();
        }

        public void Save(string filepath, bool bigEndian = false)
        {
            BinaryWriterEx writer = new(File.OpenWrite(filepath), bigEndian);

            writer.Write(Coordinates.Count);
            for (int i = 0; i < Coordinates.Count; i++)
                writer.Write(Coordinates[i]);

            writer.Close();
        }
    }
}
