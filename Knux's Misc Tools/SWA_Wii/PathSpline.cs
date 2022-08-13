
namespace Knuxs_Misc_Tools.SWA_Wii
{
    internal class PathSpline : FileBase
    {
        public class FormatData
        {
            public List<PathNode> Paths { get; set; } = new();
        }

        public class PathNode
        {
            public string Name { get; set; }
            public float UnknownFloat_1 { get; set; }
            public float UnknownFloat_2 { get; set; }
            public float UnknownFloat_3 { get; set; }
            public float UnknownFloat_4 { get; set; }
            public float UnknownFloat_5 { get; set; }
            public float UnknownFloat_6 { get; set; }
            public float UnknownFloat_7 { get; set; }
            public float UnknownFloat_8 { get; set; }
            public float UnknownFloat_9 { get; set; }
            public float UnknownFloat_10 { get; set; }
            public float UnknownFloat_11 { get; set; }
            public uint UnknownUInt32_1 { get; set; }

            public List<Spline> SplinePoints { get; set; } = new();
        }
        public class Spline
        {
            public uint UnknownUInt32_1 { get; set; }

            public Vector3 UnknownVector3_1 { get; set; }

            public Vector3 UnknownVector3_2 { get; set; }

            public Vector3 UnknownVector3_3 { get; set; }
        }

        public override string Signature { get; } = "sgnt";

        public FormatData Data = new();

        public override void Load(Stream stream)
        {
            BinaryReaderEx reader = new(stream, true);
            reader.ReadSignature(4, Signature);
            uint fileSize = reader.ReadUInt32();
            uint pathCount = reader.ReadUInt32();
            uint pathTableOffset = reader.ReadUInt32(); // ?
            uint pathDataOffset = reader.ReadUInt32(); // ?
            uint pathTableSize = reader.ReadUInt32(); // ?
            reader.JumpAhead(0x4); // Always the same as pathCount.

            for (int i = 0; i < pathCount; i++)
            {
                ushort pathIndex = reader.ReadUInt16();
                ushort pathNameLength = reader.ReadUInt16();
                PathNode node = new()
                {
                    Name = reader.ReadNullPaddedString(pathNameLength)
                };
                Data.Paths.Add(node);
            }

            uint pathDataSize = reader.ReadUInt32();
            reader.JumpAhead(0x4); // Always the same as pathCount.

            for (int i = 0; i < pathCount; i++)
            {
                uint pathLength = reader.ReadUInt32();
                Data.Paths[0].UnknownFloat_1 = reader.ReadSingle();
                Data.Paths[0].UnknownFloat_2 = reader.ReadSingle();
                Data.Paths[0].UnknownFloat_3 = reader.ReadSingle();
                Data.Paths[0].UnknownFloat_4 = reader.ReadSingle();
                Data.Paths[0].UnknownFloat_5 = reader.ReadSingle();
                Data.Paths[0].UnknownFloat_6 = reader.ReadSingle();
                Data.Paths[0].UnknownFloat_7 = reader.ReadSingle();
                Data.Paths[0].UnknownFloat_8 = reader.ReadSingle();
                Data.Paths[0].UnknownFloat_9 = reader.ReadSingle();
                Data.Paths[0].UnknownFloat_10 = reader.ReadSingle();
                Data.Paths[0].UnknownFloat_11 = reader.ReadSingle();

                uint pathLength2 = reader.ReadUInt32();
                uint splineCount = reader.ReadUInt32();
                for (int p = 0; p < splineCount; p++)
                {
                    uint splineLength = reader.ReadUInt32();
                    Data.Paths[0].UnknownUInt32_1 = reader.ReadUInt32();
                    uint splineCount2 = reader.ReadUInt32();
                    for (int p2 = 0; p2 < splineCount2; p2++)
                    {
                        Spline spline = new()
                        {
                            UnknownUInt32_1 = reader.ReadUInt32(),
                            UnknownVector3_1 = reader.ReadVector3(),
                            UnknownVector3_2 = reader.ReadVector3(),
                            UnknownVector3_3 = reader.ReadVector3()
                        };
                        if (spline.UnknownUInt32_1 != 1)
                            Debugger.Break();
                        Data.Paths[0].SplinePoints.Add(spline);
                    }
                }
            }
        }
    }
}
