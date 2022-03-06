namespace Knuxs_Misc_Tools.CarZ
{
    internal class MaterialLibrary
    {
        public class Material
        {
            public string? Name { get; set; }
            
            public string? Flags { get; set; }

            public byte? Opacity { get; set; }

            public string? Texture { get; set; }

            public string? AlphaMask { get; set; }

            public string? NormalMap { get; set; }

            public string? EnvironmentMap { get; set; }

            public byte? EnvironmentMapPower { get; set; }

            public byte[]? Colours { get; set; }

            public override string ToString() => Name;
        }
        
        public List<Material> Materials = new();

        public void Load(string filepath)
        {
            // Load the MAT into a String Array.
            string[] file = File.ReadAllLines(filepath);

            // Check that the start of the MAT is what we're expecting.
            if (file[0] != "[MaterialBegin]")
                throw new Exception($"'{filepath}' does not appear to be a CarZ engine MAT file.");

            // Set up an empty material.
            Material material = new();

            // Loop through all the lines in the MAT file.
            for (int i = 0; i < file.Length; i++)
            {
                // If this line is the [MaterialBegin] tag, then set up a new material.
                if (file[i] == "[MaterialBegin]")
                    material = new();

                // If this line is the [MaterialEnd] tag, then save this material.
                else if (file[i] == "[MaterialEnd]")
                    Materials.Add(material);

                // If this line is neither of those and isn't empty, then assume it's a material parameter.
                else if (file[i] != "")
                {
                    switch (file[i])
                    {
                        case string s when s.StartsWith("Name"): material.Name = file[i][(file[i].IndexOf(' ') + 1)..]; break;
                        case string s when s.StartsWith("Flags"): material.Flags = file[i][(file[i].IndexOf(' ') + 1)..]; break;
                        case string s when s.StartsWith("Opacity"): material.Opacity = byte.Parse(file[i][(file[i].IndexOf(' ') + 1)..]); break;
                        case string s when s.StartsWith("Texture"): material.Texture = file[i][(file[i].IndexOf(' ') + 1)..]; break;
                        case string s when s.StartsWith("AlphaMask"): material.AlphaMask = file[i][(file[i].IndexOf(' ') + 1)..]; break;
                        case string s when s.StartsWith("NormalMap"): material.NormalMap = file[i][(file[i].IndexOf(' ') + 1)..]; break;
                        case string s when s.StartsWith("EnvMap"): material.EnvironmentMap = file[i][(file[i].IndexOf(' ') + 1)..]; break;
                        case string s when s.StartsWith("EnvPower"): material.EnvironmentMapPower = byte.Parse(file[i][(file[i].IndexOf(' ') + 1)..]); break;
                        case string s when s.StartsWith("Color24"):
                            string[] colours = file[i].Split(' ');
                            material.Colours = new byte[3];
                            material.Colours[0] = byte.Parse(colours[1]);
                            material.Colours[1] = byte.Parse(colours[2]);
                            material.Colours[2] = byte.Parse(colours[3]);
                            break;
                        default: throw new NotImplementedException($"Unknown material setting '{file[i].Remove(file[i].IndexOf('='))}'.");
                    }
                }
            }
        }

        public void Save(string filepath)
        {
            StreamWriter mat = new(filepath);

            for (int i = 0; i < Materials.Count; i++)
            {
                mat.WriteLine("[MaterialBegin]");
                if (Materials[i].Name != null) { mat.WriteLine($"Name= {Materials[i].Name}"); }
                if (Materials[i].Flags != null) { mat.WriteLine($"Flags= {Materials[i].Flags}"); }
                if (Materials[i].Opacity != null) { mat.WriteLine($"Opacity= {Materials[i].Opacity}"); }
                if (Materials[i].Texture != null) { mat.WriteLine($"Texture= {Materials[i].Texture}"); }
                if (Materials[i].AlphaMask != null) { mat.WriteLine($"AlphaMask= {Materials[i].AlphaMask}"); }
                if (Materials[i].NormalMap != null) { mat.WriteLine($"NormalMap= {Materials[i].NormalMap}"); }
                if (Materials[i].EnvironmentMap != null) { mat.WriteLine($"EnvMap= {Materials[i].EnvironmentMap}"); }
                if (Materials[i].EnvironmentMapPower != null) { mat.WriteLine($"EnvPower= {Materials[i].EnvironmentMapPower}"); }
                if (Materials[i].Colours != null) { mat.WriteLine($"Color24= {Materials[i].Colours[0]} {Materials[i].Colours[1]} {Materials[i].Colours[2]}"); }
                mat.WriteLine("[MaterialEnd]");
                mat.WriteLine();
            }

            mat.Close();
        }

        public void ExportMtl(string filepath)
        {
            StreamWriter mtl = new(filepath);

            for (int i = 0; i < Materials.Count; i++)
            {
                mtl.WriteLine($"newmtl {Materials[i].Name}");
                if (Materials[i].Opacity != null) { mtl.WriteLine($"\td {Materials[i].Opacity / 255f}"); }
                if (Materials[i].Colours != null) { mtl.WriteLine($"\tKd {Materials[i].Colours[0] / 255f} {Materials[i].Colours[1] / 255f} {Materials[i].Colours[2] / 255f}"); }
                if (Materials[i].Texture != null) { mtl.WriteLine($"\tmap_Kd {Materials[i].Texture}"); }
                if (Materials[i].AlphaMask != null) { mtl.WriteLine($"\tmap_d {Materials[i].AlphaMask}"); }
                if (Materials[i].NormalMap != null) { mtl.WriteLine($"\tmap_bump {Materials[i].NormalMap}"); }
                if (Materials[i].EnvironmentMap != null) { mtl.WriteLine($"\t???"); }
                mtl.WriteLine();
            }

            mtl.Close();
        }
    }
}
