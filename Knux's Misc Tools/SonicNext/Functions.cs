using Marathon.Formats.Mesh;
using Marathon.Formats.Mesh.Ninja;
using Marathon.Formats.Placement;

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
        /// <param name="srcDir">Directory containing files (or single file) to convert.</param>
        /// <param name="tgtDir">Directory to save the converted XNMs to.</param>
        /// <param name="srcXNO">The name of the XNO to use for getting Node Indices from (defaults to sonic_Root).</param>
        /// <param name="framerate">Framerate for the converted XNMs (defaults to 30 Frames Per Second).</param>
        public static void AnimationImport(string srcDir, string tgtDir, string srcXNO = "sonic_Root", float framerate = 30f)
        {
            string[] fbxFiles;

            if (Path.GetExtension(srcDir.ToLower()) == ".fbx")
                fbxFiles = new[] { srcDir };
            else
                fbxFiles = Directory.GetFiles(srcDir, "*.fbx", SearchOption.TopDirectoryOnly);

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

        /// <summary>
        /// Imports a file as '06 collision.
        /// </summary>
        /// <param name="filepath">File to import.</param>
        /// <param name="target">File to save.</param>
        public static void CollisionImport(string filepath, string target)
        {
            Collision col = new();
            col.ImportAssimp(filepath);
            col.Save(target);
        }

        /// <summary>
        /// Creates an '06 SET Object Parameter so I don't need to keep duplicating shit.
        /// </summary>
        /// <param name="value">The value of this parameter.</param>
        /// <param name="type">The type of this parameter.</param>
        public static SetParameter ParameterCreate(object value, ObjectDataType type)
        {
            SetParameter parameter = new SetParameter()
            {
                Data = value,
                Type = type
            };
            return parameter;
        }

        /// <summary>
        /// Cheaply retargets an animation from one model to another.
        /// </summary>
        /// <param name="xnm">The animation to retarget.</param>
        /// <param name="srcXNO">The model this animation is originally from.</param>
        /// <param name="tgtXNO">The model this animation should be retargeted onto.</param>
        /// <param name="tgt">Where to save the retargeted animation.</param>
        public static void RetargetAnimation(string xnm, string srcXNO, string tgtXNO, string tgt = null)
        {
            // Load the XNM and XNOs.
            NinjaNext anim = new(xnm);
            NinjaNext srcMdl = new(srcXNO);
            NinjaNext tgtMdl = new(tgtXNO);

            // Create a list to store the Sub Motions that use a node that doesn't exist on the target model.
            List<int> Unused = new();

            // Loop through all the Sub Motions in the animation.
            for (int s = 0; s < anim.Data.Motion.SubMotions.Count; s++)
            { 
                // Set a flag to tell if we've retargeted this Sub Motion.
                bool retargeted = false;

                // Get the name of the node this Sub Motion is for.
                string nodeName = srcMdl.Data.NodeNameList.NinjaNodeNames[anim.Data.Motion.SubMotions[s].NodeIndex];

                // Loop through the target model's Ninja Node Name List.
                for (int i = 0; i < tgtMdl.Data.NodeNameList.NinjaNodeNames.Count; i++)
                {
                    // If we've found a Node with the same name as the one from the source model, then update the Sub Motion Node Index and set our flag to true.
                    if (tgtMdl.Data.NodeNameList.NinjaNodeNames[i] == nodeName)
                    {
                        anim.Data.Motion.SubMotions[s].NodeIndex = i;
                        retargeted = true;
                    }
                }

                // If our flag is still false, mark this Sub Motion for removal.
                if (!retargeted)
                    Unused.Add(s);
            }

            // Flip the list of Sub Motions to be removed.
            Unused.Reverse();

            // Remove all the Sub Motions that have a Node Index that has been added to our list.
            foreach (int node in Unused)
                anim.Data.Motion.SubMotions.RemoveAt(node);

            // Save the retargeted animation. If a path wasn't specified, just use the original with a .retargeted extension tacked on.
            if (tgt == null)
                anim.Save($@"{xnm}.retargeted");
            else
                anim.Save(tgt);
        }

        /// <summary>
        /// Sets the values needed (in our system?) to enable translucency on an object.
        /// </summary>
        /// <param name="xnoFile">The XNO to edit.</param>
        /// <param name="matIndex">The material index to edit, if null, do them all.</param>
        public static void EnableTranslucency(string xnoFile, int? matIndex = null)
        {
            // Load the XNO.
            NinjaNext xno = new(xnoFile);

            // If matIndex is not specified, do every material, material logic and sub object.
            if (matIndex == null)
            {
                foreach (var material in xno.Data.Object.Materials)
                    material.Flag = (MaterialType)0x01000036;

                // Alpha Ref gets set to 0 as having it 0x00000096 (which I do a lot for transparent objects) makes this not work.
                foreach (var materialLogic in xno.Data.Object.MaterialLogics)
                {
                    materialLogic.ZUpdate = true;
                    materialLogic.AlphaRef = 0;
                }

                foreach (var subObject in xno.Data.Object.SubObjects)
                    subObject.Type = (SubObjectType)0x00000102;
            }
            
            // If a matIndex is specified, only edit that one.
            else
            {
                xno.Data.Object.Materials[(int)matIndex].Flag = (MaterialType)0x01000036;
                xno.Data.Object.MaterialLogics[(int)matIndex].ZUpdate = true;
                xno.Data.Object.MaterialLogics[(int)matIndex].AlphaRef = 0;
                xno.Data.Object.SubObjects[(int)matIndex].Type = (SubObjectType)0x00000102;
            }

            // Save the edited XNO.
            xno.Save();
        }
    }
}
