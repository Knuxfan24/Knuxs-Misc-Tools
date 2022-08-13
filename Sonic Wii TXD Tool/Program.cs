namespace Sonic_Wii_TXD_Tool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage:\nSonic_Wii_TXD_Tool.exe \"file.txd\"\nSonic_Wii_TXD_Tool.exe \"some_directory_path\"\n\nPress any key to continue...");
                Console.ReadKey();
            }

            else
            {
                foreach (string file in args)
                {
                    if (Path.GetExtension(file).ToLower() is ".txd")
                    {
                        Directory.CreateDirectory(@$"{Path.GetDirectoryName(file)}\{Path.GetFileNameWithoutExtension(file)}");
                        Knuxs_Misc_Tools.Storybook.TXD textures = new();
                        textures.Load(file);
                        textures.Extract(@$"{Path.GetDirectoryName(file)}\{Path.GetFileNameWithoutExtension(file)}");
                    }

                    if (Directory.Exists(file))
                    {
                        Knuxs_Misc_Tools.Storybook.TXD textures = new();
                        textures.GetFiles(file);
                        textures.Save($"{file}.txd");
                    }
                }
            }
        }
    }
}