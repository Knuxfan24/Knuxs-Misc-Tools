using HedgeLib.Headers;

namespace Knuxs_Misc_Tools.SonicRangers
{
    internal class PathSpline : FileBase
    {
        public class Spline
        {
            public string Name { get; set; } = "";

            public float UnknownFloat_1 { get; set; }

            public byte[]? UnknownData_1 { get; set; }

            public float[]? UnknownData_2 { get; set; }

            public Vector3[]? SplineKnots { get; set; }

            public Vector3[]? DoubleSplineKnotsA { get; set; }

            public Vector3[]? DoubleSplineKnotsB { get; set; }

            public Vector3 BoundingBox_Min { get; set; }

            public Vector3 BoundingBox_Max { get; set; }

            public string Type { get; set; } = "";

            public ulong UnknownULong_1 { get; set; }

            public ulong UnknownULong_2 { get; set; }

            public uint[]? UnknownData_3 { get; set; }

            public uint[]? UnknownData_4 { get; set; }

            public override string ToString() => Name;
        }

        // Set up HedgeLib#'s BINAV2Header.
        public HedgeLib.Headers.BINAHeader Header = new BINAv2Header(210);

        // Set up the Signature we expect.
        public new const string Signature = "HTAP";

        public List<Spline> Splines = new();

        public override void Load(Stream stream)
        {
            // Set up our BINAReader and read the BINAV2 header.
            HedgeLib.IO.BINAReader reader = new(stream);
            Header = reader.ReadHeader();

            // Check this file's signature.
            string signature = reader.ReadSignature();
            if (signature != Signature)
                throw new Exception($"Invalid signature, got '{signature}', expected '{Signature}'.");

            reader.JumpAhead(0x4); // Always 00 02 00 00.
            ulong PathCount = reader.ReadUInt64();
            long PathTableOffset = reader.ReadInt64(); // Always 0x18

            // Jump to the Path Table.
            reader.JumpTo(PathTableOffset, false); // Should already be here but just to be safe.

            // Read each spline.
            for (ulong i = 0; i < PathCount; i++)
            {
                // Set up our spline entry.
                Spline spline = new();

                // Read this spline's name.
                long PathNameOffset = reader.ReadInt64();

                // Save our current position.
                long stringPos = reader.BaseStream.Position;

                // Jump to and read the name of this path.
                reader.JumpTo(PathNameOffset, false);
                spline.Name = reader.ReadNullTerminatedString();

                // Jump back.
                reader.JumpTo(stringPos);

                reader.JumpAhead(0x2); // Always 1.
                ushort KnotCount = reader.ReadUInt16();
                
                // Skipped by Forces MaxScript.
                spline.UnknownFloat_1 = reader.ReadSingle();
                long UnknownOffset_1 = reader.ReadInt64();
                long UnknownOffset_2 = reader.ReadInt64();
                
                long KnotsOffset = reader.ReadInt64();

                // Skipped by Forces MaxScript.
                long UnknownOffset_3 = reader.ReadInt64();
                long UnknownOffset_4 = reader.ReadInt64();

                ulong DoubleKnotCount = reader.ReadUInt64();
                long DoubleKnotOffset = reader.ReadInt64();

                // Skipped by Forces MaxScript.
                spline.BoundingBox_Min = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                spline.BoundingBox_Max = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                reader.JumpAhead(0x8); // Looks to be acount for UnknownOffset_5, but is always 1.
                long UnknownOffset_5 = reader.ReadInt64(); // Something like the tag system that GEdits use?
                reader.JumpAhead(0x8); // Always 0.
                long UnknownOffset_6 = reader.ReadInt64();

                // Save our current position.
                long pos = reader.BaseStream.Position;

                // Read UnknownOffset_1's data.
                // TODO: What is this data? Seems to always be 1s? Check other files.
                reader.JumpTo(UnknownOffset_1, false);
                spline.UnknownData_1 = reader.ReadBytes(KnotCount);

                // Read UnknownOffset_2's data.
                // TODO: What is this data? Looks like a load of floats that seem to count up from 0, but not always by 1???
                reader.JumpTo(UnknownOffset_2, false);
                spline.UnknownData_2 = new float[KnotCount];

                // Read each float.
                for (ulong knot = 0; knot < KnotCount; knot++)
                    spline.UnknownData_2[knot] = reader.ReadSingle();

                // Read Knot Data
                if (KnotCount != 0)
                {
                    // Set up this spline's knot list.
                    spline.SplineKnots = new Vector3[KnotCount];

                    // Jump to this knot's offset.
                    reader.JumpTo(KnotsOffset, false);

                    // Read each vertex position.
                    for (ulong knot = 0; knot < KnotCount; knot++)
                        spline.SplineKnots[knot] = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                }

                // TODO: Read UnknownOffset_3's data.
                reader.JumpTo(UnknownOffset_3, false);

                // TODO: Read UnknownOffset_4's data.
                reader.JumpTo(UnknownOffset_4, false);

                // Read Double Knot Data
                if (DoubleKnotCount != 0)
                {
                    spline.DoubleSplineKnotsA = new Vector3[DoubleKnotCount / 2];
                    spline.DoubleSplineKnotsB = new Vector3[DoubleKnotCount / 2];

                    reader.JumpTo(DoubleKnotOffset, false);

                    for (ulong knot = 0; knot < DoubleKnotCount / 2; knot++)
                    {
                        spline.DoubleSplineKnotsA[knot] = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        spline.DoubleSplineKnotsB[knot] = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    }
                }

                // Read UnknownOffset_5's data.
                reader.JumpTo(UnknownOffset_5, false);
                long PathTypeOffset = reader.ReadInt64();

                // Save our current position.
                stringPos = reader.BaseStream.Position;

                // Jump to and read the name of this path.
                reader.JumpTo(PathTypeOffset, false);
                spline.Type = reader.ReadNullTerminatedString();

                // Jump back.
                reader.JumpTo(stringPos);

                spline.UnknownULong_1 = reader.ReadUInt64();
                spline.UnknownULong_2 = reader.ReadUInt64();

                // TODO: Read UnknownOffset_6's data.
                reader.JumpTo(UnknownOffset_6, false);
                uint UnknownCount_1 = reader.ReadUInt32();
                uint UnknownCount_2 = reader.ReadUInt32();
                long UnknownOffset_7 = reader.ReadInt64();
                ulong UnknownCount_3 = reader.ReadUInt64();
                long UnknownOffset_8 = reader.ReadInt64();
                ulong UnknownCount_4 = reader.ReadUInt64();
                long UnknownOffset_9 = reader.ReadInt64();

                // TODO: Actually read this data.
                reader.JumpTo(UnknownOffset_7, false);

                // TODO: What is this data?
                reader.JumpTo(UnknownOffset_8, false);
                spline.UnknownData_3 = new uint[UnknownCount_3 * 2];
                for (ulong index = 0; index < UnknownCount_3 * 2; index++)
                    spline.UnknownData_3[index] = reader.ReadUInt32();

                // TODO: Is this just a linear sequence of uints???
                reader.JumpTo(UnknownOffset_9, false);
                spline.UnknownData_4 = new uint[UnknownCount_4];
                for (ulong index = 0; index < UnknownCount_4; index++)
                    spline.UnknownData_4[index] = reader.ReadUInt32();

                Splines.Add(spline);

                reader.JumpTo(pos);
            }
        }
    }
}
