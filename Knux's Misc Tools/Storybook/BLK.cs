using Marathon.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Knuxs_Misc_Tools.Storybook
{
    public class BlockEntry
    {
        public uint UnknownUInt32_1 { get; set; } // Thought this was like block index or something, apparently not???
        public Vector3 UnknownVector3_1 { get; set; }
        public Vector3 UnknownVector3_2 { get; set; }
        public Vector3 UnknownVector3_3 { get; set; }
        public float UnknownFloat_1 { get; set; }
        public float UnknownFloat_2 { get; set; }
        public byte[]? SectorIndices { get; set; }

    }

    public class BLK : FileBase
    {
        public List<BlockEntry> Blocks = new();

        public override void Load(string filepath)
        {
            BinaryReaderEx reader = new(File.OpenRead(filepath), true);

            reader.ReadSignature(4, "LDBK");
            uint blockCount = reader.ReadUInt32();
            reader.JumpAhead(0x8); // TODO: I assume this is padding? Seems to end up as either all 0xCC or 0x00?

            for (int i = 0; i < blockCount; i++)
            {
                BlockEntry entry = new()
                {
                    UnknownUInt32_1 = reader.ReadUInt32(),
                    UnknownVector3_1 = reader.ReadVector3(),
                    UnknownVector3_2 = reader.ReadVector3(),
                    UnknownVector3_3 = reader.ReadVector3(),
                    UnknownFloat_1 = reader.ReadSingle(),
                    UnknownFloat_2 = reader.ReadSingle(),
                    SectorIndices = reader.ReadBytes(0x10)
                };

                Blocks.Add(entry);
            }
        }
    }
}
