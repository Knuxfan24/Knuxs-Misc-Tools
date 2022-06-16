namespace Sonic_Unleashed_ArcInfo_Tool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Check if the user has given a file. If not, tell them how to use this rather than just having the window blink in and out for a frame.
            if (args.Length < 1)
            {
                Console.WriteLine("Usage:\nSonic_Unleashed_ArcInfo_Tool.exe \"swa.arcinfo\"\nSonic_Unleashed_ArcInfo_Tool.exe \"swa.arcinfo.json\"\n\nPress any key to continue...");
                Console.ReadKey();
            }

            else
            {
                // Set up an ArcInfo var.
                Knuxs_Misc_Tools.SWA.ArcInfo arc = new();

                // Check the extension of the file given.
                switch (Path.GetExtension(args[0].ToLower()))
                {
                    // If it's an .arcinfo file, then try to load and export it as a JSON.
                    case ".arcinfo":
                        arc = new();
                        arc.Load(args[0]);
                        arc.JsonSerialise($"{args[0]}.json", arc.entries);
                        break;

                    // If it's a .json file, then try to deseralise it and save it.
                    case ".json":
                        arc = new();
                        arc.entries = arc.JsonDeserialise<List<Knuxs_Misc_Tools.SWA.ArchiveEntry>>(args[0]);
                        arc.Save(args[0].Replace(".json", ""));
                        break;

                    // If it's neither, repeat the usage instructions.
                    default:
                        Console.WriteLine("Usage:\nSonic_Unleashed_ArcInfo_Tool.exe \"swa.arcinfo\"\nSonic_Unleashed_ArcInfo_Tool.exe \"swa.arcinfo.json\"\n\nPress any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
