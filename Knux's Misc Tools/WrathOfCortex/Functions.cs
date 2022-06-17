namespace Knuxs_Misc_Tools.WrathOfCortex
{
    internal class Functions
    {
        /// <summary>
        /// Reads a NuScene and prints any instances that aren't at the default x or y rotation.
        /// </summary>
        /// <param name="nusFile"></param>
        public static void InstanceChecker(string nusFile, int startFrom = 0)
        {
            // Load the NuScene.
            NuScene nus = new();
            nus.Load(nusFile);

            // Loop through all the instances.
            for (int i = startFrom; i < nus.Data.Instances.Instances.Count; i++)
            {
                HGObject_Chunk.InstanceEntry? instance = nus.Data.Instances.Instances[i];

                // If this instance has a non-standard rotation, then display the values.
                if ((instance.Rotation.X != 0 && instance.Rotation.X != -0) ||
                    instance.Rotation.Z != 0 && instance.Rotation.Z != -0)
                {
                    Console.Clear();
                    Console.WriteLine($"Mesh Index: {instance.ModelIndex}\n" +
                                      $"\n" +
                                      $"Instance Index: {i}\n" +
                                      $"\n" +
                                      $"X Translation: {instance.Translation.X}\n" +
                                      $"Y Translation: {-instance.Translation.Z}\n" +
                                      $"Z Translation: {instance.Translation.Y}\n" +
                                      $"\n" +
                                      $"X Rotation: {instance.Rotation.X}\n" +
                                      $"Y Rotation: {-instance.Rotation.Z}\n" +
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
