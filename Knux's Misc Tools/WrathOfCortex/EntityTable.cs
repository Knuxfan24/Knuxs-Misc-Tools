namespace Knuxs_Misc_Tools.WrathOfCortex
{
    public class EntityTable : FileBase
    {
        public List<Entity> Entities = new();
        
        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream, true);

            uint entityCount = reader.ReadUInt32();

            for (int i = 0; i < entityCount; i++)
            {
                Entity entry = new();
                entry.Type = reader.ReadNullPaddedString(0x10);
                uint positionCount = reader.ReadUInt32();

                for (int e = 0; e < positionCount; e++)
                    entry.Positions.Add(reader.ReadVector3());

                Entities.Add(entry);
            }
        }

        public override void Save(Stream stream)
        {
            BinaryWriterEx writer = new(stream, true);

            writer.Write(Entities.Count);

            foreach (Entity? entityGroup in Entities)
            {
                writer.WriteNullPaddedString(entityGroup.Type, 0x10);
                writer.Write(entityGroup.Positions.Count);

                foreach (var entity in entityGroup.Positions)
                    writer.Write(entity);
            }
        }
    }

    public class Entity
    {
        public string Type { get; set; }

        public List<Vector3> Positions { get; set; } = new();
    }
}
