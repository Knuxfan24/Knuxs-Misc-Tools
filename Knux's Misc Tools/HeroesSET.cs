using Marathon.IO;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Knux_s_Misc_Tools
{
    public class HeroesSET
    {
        public class SETObject
        {
            public Vector3 Position { get; set; }

            public Vector3 Rotation { get; set; }

            public short UnknownShort_1 { get; set; }

            public byte TeamIdentifier { get; set; }

            public byte ObjectLoader { get; set; }

            public byte[] UnknownBytes_1 { get; set; }

            public byte[] UnknownBytes_2 { get; set; }

            public byte ObjectList { get; set; }

            public byte ObjectType { get; set; }

            public byte LinkID { get; set; }

            public byte RenderDistance { get; set; }

            public short UnknownShort_2 { get; set; }

            public short MiscEntryID { get; set; }

            public List<float> MiscData = new();
        }

        // List of Objects.
        public List<SETObject> Objects = new();

        /// <summary>
        /// Loads a Sonic Heroes SET file.
        /// </summary>
        /// <param name="filepath">File to load.</param>
        public void Load(string filepath)
        {
            // Set up the Extended Binary Reader.
            BinaryReaderEx reader = new(File.OpenRead(filepath), true);

            // Heroes SETs always have space for 2048 objects, so read that many.
            for (int i = 0; i < 2048; i++)
            {
                // Read this object.
                SETObject obj = new()
                {
                    Position = reader.ReadVector3(),
                    Rotation = new(reader.ReadInt32() * 360 / 65536, reader.ReadInt32() * 360 / 65536, reader.ReadInt32() * 360 / 65536),
                    UnknownShort_1 = reader.ReadInt16(),
                    TeamIdentifier = reader.ReadByte(),
                    ObjectLoader = reader.ReadByte(),
                    UnknownBytes_1 = reader.ReadBytes(0x4),
                    UnknownBytes_2 = reader.ReadBytes(0x8),
                    ObjectList = reader.ReadByte(),
                    ObjectType = reader.ReadByte(),
                    LinkID = reader.ReadByte(),
                    RenderDistance = reader.ReadByte(),
                    UnknownShort_2 = reader.ReadInt16(),
                    MiscEntryID = reader.ReadInt16()
                };

                // Sloppy implemntation of Misc Data.
                // Save current position so we can jump back.
                long pos = reader.BaseStream.Position;

                // Jump to the Misc. Data table.
                reader.JumpTo(0x18000 + (obj.MiscEntryID * 0x24));

                // Ignore the first four bytes.
                reader.JumpAhead(0x4);

                // Read the misc data as floats because a lot of shit seems to use floats.
                for (int p = 0; p < 8; p++)
                    obj.MiscData.Add(reader.ReadSingle());

                // Jump back for the next object.
                reader.JumpTo(pos);

                // Heroes Objects only load if this value is nine, therefore, ignore it if it's not.
                if (obj.ObjectLoader == 09)
                    Objects.Add(obj);
            }
        }
    }
}
