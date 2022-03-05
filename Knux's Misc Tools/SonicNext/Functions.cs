using Marathon.Formats.Mesh.Ninja;

namespace Knuxs_Misc_Tools.SonicNext
{
    internal class Functions
    {
        /// <summary>
        /// Converts a file to an XNO.
        /// </summary>
        /// <param name="src">Filepath of source model.</param>
        /// <param name="tgt">Filepath to save to.</param>
        /// <param name="removeVertexColours">Whether or not to automatically remove the Vertex Colours on the model.</param>
        public static void ModelImport(string src, string tgt, bool removeVertexColours = false)
        {
            NinjaNext import = new();
            import.ImportModel(src);
            import.Save(tgt);

            if (removeVertexColours)
                RemoveVertexColours(tgt);
        }

        /// <summary>
        /// Remove the vertex colours from an XNO.
        /// </summary>
        /// <param name="xno">Filepath of XNO to remove colours from.</param>
        public static void RemoveVertexColours(string xno)
        {
            NinjaNext vertexColours = new(xno);
            foreach (var vertexList in vertexColours.Data.Object.VertexLists)
            {
                foreach (var vertex in vertexList.Vertices)
                {
                    vertex.VertexColours[0] = 255;
                    vertex.VertexColours[1] = 255;
                    vertex.VertexColours[2] = 255;
                    vertex.VertexColours[3] = 255;
                }
            }
            vertexColours.Save();
        }

        /// <summary>
        /// Converts a directory of files to an XNM.
        /// </summary>
        /// <param name="srcDir">Directory containing files to convert.</param>
        /// <param name="tgtDir">Directory to save the converted XNMs to.</param>
        /// <param name="srcXNO">The name of the XNO to use for getting Node Indices from (defaults to sonic_Root).</param>
        /// <param name="framerate">Framerate for the converted XNMs (defaults to 30 Frames Per Second).</param>
        public static void AnimationImport(string srcDir, string tgtDir, string srcXNO = "sonic_Root", float framerate = 30f)
        {
            string[] fbxFiles = Directory.GetFiles(srcDir, "*.fbx", SearchOption.TopDirectoryOnly);
            foreach (string fbxFile in fbxFiles)
            {
                if (!File.Exists($@"{tgtDir}\{Path.GetFileNameWithoutExtension(fbxFile)}.xnm"))
                {
                    Console.WriteLine($"Converting '{fbxFile}'.");
                    NinjaNext xnm = new();
                    xnm.ImportAnimation(fbxFile, $@"{tgtDir}\{srcXNO}.xno");
                    xnm.Data.Motion.Framerate = framerate;
                    xnm.Save($@"{tgtDir}\{Path.GetFileNameWithoutExtension(fbxFile)}.xnm");
                }
            }
        }

        /// <summary>
        /// Adds the keyframes from an XNM to another XNM.
        /// </summary>
        /// <param name="srcXNM">The XNM to add keyframes to.</param>
        /// <param name="addXNM">The XNM to get the keyframes from.</param>
        /// <param name="saveXNM">Filepath to save the edited XNM to, if null, just overwrite the original.</param>
        public static void MergeAnimations(string srcXNM, string addXNM, string saveXNM = null)
        {
            NinjaNext source = new(srcXNM);
            NinjaNext target = new(addXNM);

            for (int i = 0; i < source.Data.Motion.SubMotions.Count; i++)
            {
                source.Data.Motion.SubMotions[i].EndFrame += target.Data.Motion.EndFrame + 1;
                source.Data.Motion.SubMotions[i].EndKeyframe += target.Data.Motion.EndFrame + 1;

                for (int i2 = 0; i2 < target.Data.Motion.SubMotions[i].Keyframes.Count; i2++)
                {
                    if (target.Data.Motion.SubMotions[i].Type.HasFlag(SubMotionType.NND_SMOTTYPE_FRAME_FLOAT))
                        (target.Data.Motion.SubMotions[i].Keyframes[i2] as NinjaKeyframe.NNS_MOTION_KEY_VECTOR).Frame += source.Data.Motion.EndFrame + 1;

                    if (target.Data.Motion.SubMotions[i].Type.HasFlag(SubMotionType.NND_SMOTTYPE_ANGLE_ANGLE16))
                        (target.Data.Motion.SubMotions[i].Keyframes[i2] as NinjaKeyframe.NNS_MOTION_KEY_ROTATE_A16).Frame += (short)(source.Data.Motion.EndFrame + 1);
                }

                source.Data.Motion.SubMotions[i].Keyframes = source.Data.Motion.SubMotions[i].Keyframes.Concat(target.Data.Motion.SubMotions[i].Keyframes).ToList();
            }

            source.Data.Motion.EndFrame += target.Data.Motion.EndFrame + 1;

            if (saveXNM != null)
                source.Save(saveXNM);
            else
                source.Save();
        }

        /// <summary>
        /// Changes the framerate value in an XNM to a new value.
        /// </summary>
        /// <param name="xnm">The XNM to edit.</param>
        /// <param name="framerate">The new framerate value to use.</param>
        public static void ChangeFramerate(string xnm, float framerate)
        {
            NinjaNext fps = new(xnm);
            fps.Data.Motion.Framerate = framerate;
            fps.Save();
        }
    }
}
