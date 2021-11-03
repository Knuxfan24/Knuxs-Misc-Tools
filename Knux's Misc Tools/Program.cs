using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Knux_s_Misc_Tools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] hkxFiles = Directory.GetFiles(@"G:\Sonic '06\Extracted Archives\object\xenon\object", "*.hkx", SearchOption.AllDirectories);
            foreach (string hkxFile in hkxFiles)
            {
                Console.WriteLine(hkxFile);
                S06Havok havok = new();
                havok.Load(hkxFile);

                File.WriteAllText($"{hkxFile}.json", JsonConvert.SerializeObject(havok.Data, Formatting.Indented));
            }
        }
    }
}
