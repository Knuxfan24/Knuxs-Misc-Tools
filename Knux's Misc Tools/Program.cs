namespace Knuxs_Misc_Tools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] matFiles = Directory.GetFiles(@"D:\Standalone Games\Big Rigs Racing\Dum\Data", "*.mat", SearchOption.AllDirectories);
            foreach (string matFile in matFiles)
            {
                CarZ.MaterialLibrary mat = new();
                mat.Load(matFile);
                mat.Save($"{matFile}.knx");
            }
        }
    }
}