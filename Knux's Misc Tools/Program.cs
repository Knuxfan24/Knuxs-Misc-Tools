using System;

namespace Knux_s_Misc_Tools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HeroesSET set = new();
            set.Load(@"D:\Standalone Games\SONICHEROES\dvdroot\s01_P2.bin");
        }
    }
}
