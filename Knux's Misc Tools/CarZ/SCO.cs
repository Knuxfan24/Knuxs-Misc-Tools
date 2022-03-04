using System.Numerics;

namespace Knuxs_Misc_Tools.CarZ
{
    internal class SCO
    {
        public class FormatData
        {
            public string Name { get; set; }

            public Vector3 CentralPoint { get; set; }

            public List<Vector3> Vertices { get; set; } = new();

            public List<Face> Faces { get; set; } = new();

            public override string ToString() => Name;
        }

        public class Face
        {
            public int UnknownInt32_1 { get; set; } // Always 3?

            public short[] VertexIndices { get; set; } = new short[3];

            public string MaterialName { get; set; }

            public Vector2[] TextureCoordinates { get; set; } = new Vector2[3];

            public override string ToString() => $"<{VertexIndices[0]}, {VertexIndices[1]}, {VertexIndices[2]}>";
        }

        public FormatData Data = new();

        public void Load(string filepath)
        {
            // Load the SCO into a String Array.
            string[] file = File.ReadAllLines(filepath);

            // Check that the start of the SCO is what we're expecting.
            if (file[0] != "[ObjectBegin]")
                throw new Exception($"'{filepath}' does not appear to be a CarZ engine SCO file.");

            // Read this SCO's Model Name.
            Data.Name = file[1][(file[1].IndexOf(' ') + 1)..];

            // Get this SCO's Central Point.
            string[] CentralPoint = file[2].Split(' ');
            Data.CentralPoint = new(float.Parse(CentralPoint[1]), float.Parse(CentralPoint[2]), float.Parse(CentralPoint[3]));

            // Read this SCO's Vertex Coordinates.
            int VertexCount = int.Parse(file[3][(file[3].IndexOf(' ') + 1)..]);
            int FacePosition = 4 + VertexCount;
            for (int i = 4; i < FacePosition; i++)
            {
                string[] Vertex = file[i].Split(' ');
                Data.Vertices.Add(new Vector3(float.Parse(Vertex[0]), float.Parse(Vertex[1]), float.Parse(Vertex[2])));
            }

            // Read this SCO's Faces.
            int FaceCount = int.Parse(file[FacePosition][(file[FacePosition].IndexOf(' ') + 1)..]);
            for (int i = FacePosition + 1; i < FacePosition + 1 + FaceCount; i++)
            {
                char[] delimiters = { '\t', ' ' };
                string[] line = file[i].Split(delimiters).Where(x => !string.IsNullOrEmpty(x)).ToArray();

                Face face = new();
                face.UnknownInt32_1 = int.Parse(line[0]);
                face.VertexIndices[0] = short.Parse(line[1]);
                face.VertexIndices[1] = short.Parse(line[2]);
                face.VertexIndices[2] = short.Parse(line[3]);
                face.MaterialName = line[4];
                face.TextureCoordinates[0] = new(float.Parse(line[5]), float.Parse(line[6]));
                face.TextureCoordinates[1] = new(float.Parse(line[7]), float.Parse(line[8]));
                face.TextureCoordinates[2] = new(float.Parse(line[9]), float.Parse(line[10]));
                Data.Faces.Add(face);
            }
        }

        public void Save(string filepath)
        {
            StreamWriter sco = new(filepath);

            sco.WriteLine("[ObjectBegin]");
            sco.WriteLine($"Name= {Data.Name}");
            sco.WriteLine($"CentralPoint= {Data.CentralPoint.X.ToString("n4").Replace(",", "")} {Data.CentralPoint.Y.ToString("n4").Replace(",", "")} {Data.CentralPoint.Z.ToString("n4").Replace(",", "")}");
            sco.WriteLine($"Verts= {Data.Vertices.Count}");

            for (int i = 0; i < Data.Vertices.Count; i++)
                sco.WriteLine($"{Data.Vertices[i].X.ToString("n4").Replace(",", "")} {Data.Vertices[i].Y.ToString("n4").Replace(",", "")} {Data.Vertices[i].Z.ToString("n4").Replace(",", "")}");

            sco.WriteLine($"Faces= {Data.Faces.Count}");

            for (int i = 0; i < Data.Faces.Count; i++)
                sco.WriteLine($"{Data.Faces[i].UnknownInt32_1}\t" +
                              $"{Data.Faces[i].VertexIndices[0],4} {Data.Faces[i].VertexIndices[1],4} {Data.Faces[i].VertexIndices[2],4}\t" +
                              $"{Data.Faces[i].MaterialName,-20}\t" +
                              $"{Data.Faces[i].TextureCoordinates[0].X.ToString("n12").Replace(",", "")} {Data.Faces[i].TextureCoordinates[0].Y.ToString("n12").Replace(",", "")} " +
                              $"{Data.Faces[i].TextureCoordinates[1].X.ToString("n12").Replace(",", "")} {Data.Faces[i].TextureCoordinates[1].Y.ToString("n12").Replace(",", "")} " +
                              $"{Data.Faces[i].TextureCoordinates[2].X.ToString("n12").Replace(",", "")} {Data.Faces[i].TextureCoordinates[2].Y.ToString("n12").Replace(",", "")}");

            sco.WriteLine("[ObjectEnd]\n");
            sco.Close();
        }
    }
}
