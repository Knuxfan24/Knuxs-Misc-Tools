namespace Sonic_Unleashed_ONE_Tool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage:\nSonic_Unleashed_ONE_Tool.exe \"file.one\"\nSonic_Unleashed_ONE_Tool.exe \"some_directory_path\"\n\nPress any key to continue...");
                Console.ReadKey();
            }

            else
            {
                foreach (string file in args)
                {
                    if (Path.GetExtension(file) == ".one")
                    {
                        Directory.CreateDirectory(@$"{Path.GetDirectoryName(file)}\{Path.GetFileNameWithoutExtension(file)}");
                        Knuxs_Misc_Tools.SWA_Wii.ONE archive = new();
                        archive.Load(file, @$"{Path.GetDirectoryName(file)}\{Path.GetFileNameWithoutExtension(file)}");
                    }

                    if (Directory.Exists(file))
                    {
                        Knuxs_Misc_Tools.SWA_Wii.ONE archive = new();
                        archive.GetFiles(file);
                        archive.Save($"{file}.one");
                    }
                }
            }
        }
    }
}
