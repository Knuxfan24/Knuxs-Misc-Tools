using System;

namespace Knux_s_Misc_Tools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ShadowSET set = new();
            set.Load(@"F:\ROMs & ISOs\Microsoft Xbox\Shadow The Hedgehog\stg0301\stg0301_nrm.dat");
            set.Load(@"F:\ROMs & ISOs\Microsoft Xbox\Shadow The Hedgehog\stg0301\stg0301_cmn.dat");
            set.Load(@"F:\ROMs & ISOs\Microsoft Xbox\Shadow The Hedgehog\stg0301\stg0301_ds1.dat");
            //set.Export06(@"G:\Sonic '06\Mods\Prison Island\xenon\archives\scripts\xenon\placement\wvo\stg0301_nrm.set");
            set.ExportMAXScript(@"C:\Users\Knuxf\Documents\3dsMax\scenes\Prison Island Props\JG0000FOOTRIVERGNFH.ms", 0x30, 0x11);
        }
    }
}
