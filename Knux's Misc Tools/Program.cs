using System;

namespace Knux_s_Misc_Tools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Helpers.Shift06(@"G:\Sonic '06\Mods\Prison Island\xenon\archives\scripts\xenon\placement\stg0301\stg0301_ds1.set", 210000, 0, 0);
            Helpers.Shift06(@"G:\Sonic '06\Mods\Prison Island\xenon\archives\scripts\xenon\placement\stg0301\stg0301_nrm.set", 210000, 0, 0);
            Helpers.Shift06(@"G:\Sonic '06\Mods\Prison Island\xenon\archives\scripts\xenon\placement\stg0301\stg0301_prt.set", 210000, 0, 0);
            //ShadowSET set = new();
            //set.Load(@"F:\ROMs & ISOs\Microsoft Xbox\Shadow The Hedgehog\stg0301\stg0301_nrm.dat");
            //set.Load(@"F:\ROMs & ISOs\Microsoft Xbox\Shadow The Hedgehog\stg0301\stg0301_cmn.dat");
            //set.Load(@"F:\ROMs & ISOs\Microsoft Xbox\Shadow The Hedgehog\stg0301\stg0301_ds1.dat");
            //set.Export06(@"G:\Sonic '06\Mods\Prison Island\xenon\archives\scripts\xenon\placement\stg0301\stg0301_temp.set");
            //set.Dummy06SET(@"G:\Sonic '06\Mods\Prison Island\xenon\archives\scripts\xenon\placement\stg0301\stg0301_temp.set");
            //set.ExportMAXScript(@"C:\Users\Knuxf\Documents\3dsMax\scenes\Prison Island Props\JG0301CAN.ms", 0x89, 0x25);
        }
    }
}
