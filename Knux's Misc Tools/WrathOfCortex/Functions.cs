namespace Knuxs_Misc_Tools.WrathOfCortex
{
    internal class Functions
    {
        /// <summary>
        /// Reads a NuScene and prints any instances that aren't at the default position.
        /// </summary>
        /// <param name="nusFile"></param>
        public static void InstanceChecker(string nusFile)
        {
            // Load the NuScene.
            NuScene nus = new();
            nus.Load(nusFile);

            // Loop through all the instances.
            foreach (var instance in nus.Data.Instances.Instances)
            {
                // If this instance has a non-standard translation, then display the values.
                if (instance.Translation.X != 0 && instance.Translation.X != -0 &&
                    instance.Translation.Y != 0 && instance.Translation.Y != -0 &&
                    instance.Translation.Z != 0 && instance.Translation.Z != -0)
                {
                    Console.Clear();
                    Console.WriteLine($"Mesh Index: {instance.ModelIndex}\n" +
                                      $"\n" +
                                      $"X Translation: {instance.Translation.X}\n" +
                                      $"Y Translation: {-instance.Translation.Z}\n" +
                                      $"Z Translation: {instance.Translation.Y}\n" +
                                      $"\n" +
                                      $"X Rotation: {instance.Rotation.Z}\n" +
                                      $"Y Rotation: {-instance.Rotation.X}\n" +
                                      $"Z Rotation: {instance.Rotation.Y}\n" +
                                      $"\n" +
                                      $"X Scale: {instance.Scale.X * 100}\n" +
                                      $"Y Scale: {instance.Scale.Z * 100}\n" +
                                      $"Z Scale: {instance.Scale.Y * 100}");
                    Console.ReadKey();
                }
            }
        }
    }
}
