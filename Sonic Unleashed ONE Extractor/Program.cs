namespace Sonic_Unleashed_ONE_Extractor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage:\nSonic_Unleashed_ONE_Extractor.exe \"file.one\"\n\nPress any key to continue...");
                Console.ReadKey();
            }

            else
            {
                foreach (string file in args)
                {
                    Directory.CreateDirectory(@$"{Path.GetDirectoryName(file)}\{Path.GetFileNameWithoutExtension(file)}");
                    Knuxs_Misc_Tools.SWA_Wii.ONE archive = new();
                    archive.Load(file, @$"{Path.GetDirectoryName(file)}\{Path.GetFileNameWithoutExtension(file)}");
                }
            }
        }
    }
}
