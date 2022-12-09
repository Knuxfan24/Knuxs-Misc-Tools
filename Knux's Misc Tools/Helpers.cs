using Google.Cloud.Translation.V2;
using HedgeLib.Materials;
using HedgeLib.Textures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Web;

namespace Knuxs_Misc_Tools
{
    /// <summary>
    /// Generic file wrapper for archives.
    /// </summary>
    public class GenericFile
    {
        // The name of this file.
        public string? FileName { get; set; }

        // The bytes that make up this file.
        public byte[]? Data { get; set; }

        // Show the file's name in the debugger rather than Knuxs_Misc_Tools.GenericFile.
        public override string ToString() => FileName;
    }

    internal class Helpers
    {
        public static Quaternion ConvertToQuat(float angle)
        {
            Quaternion quat = new();
            var h = (angle + 90) * Math.PI / 360; //Y
            var a = 0 * Math.PI / 360; //Z
            var b = 0 * Math.PI / 360; //X
            var c1 = Math.Cos(h);
            var c2 = Math.Cos(a);
            var c3 = Math.Cos(b);
            var s1 = Math.Sin(h);
            var s2 = Math.Sin(a);
            var s3 = Math.Sin(b);
            quat.W = ToSingle(Math.Round((c1 * c2 * c3 - s1 * s2 * s3) * 100000) / 100000);
            quat.X = ToSingle(Math.Round((s1 * s2 * c3 + c1 * c2 * s3) * 100000) / 100000);
            quat.Y = ToSingle(Math.Round((s1 * c2 * c3 + c1 * s2 * s3) * 100000) / 100000);
            quat.Z = ToSingle(Math.Round((c1 * s2 * c3 - s1 * c2 * s3) * 100000) / 100000);
            return quat;
        }

        // Ripped from HedgeLib
        public static Vector3 ToEulerAngles(Quaternion quat, bool returnResultInRadians = false)
        {
            // Credit to http://quat.zachbennett.com/
            float qw2 = quat.W * quat.W;
            float qx2 = quat.X * quat.X;
            float qy2 = quat.Y * quat.Y;
            float qz2 = quat.Z * quat.Z;
            float test = quat.X * quat.Y + quat.Z * quat.W;

            if (test > 0.499)
            {
                return GetVect(0,
                    360 / System.Math.PI * System.Math.Atan2(quat.X, quat.W), 90);
            }
            if (test < -0.499)
            {
                return GetVect(0,
                    -360 / System.Math.PI * System.Math.Atan2(quat.X, quat.W), -90);
            }

            double h = System.Math.Atan2(2 * quat.Y * quat.W - 2 * quat.X * quat.Z, 1 - 2 * qy2 - 2 * qz2);
            double a = System.Math.Asin(2 * quat.X * quat.Y + 2 * quat.Z * quat.W);
            double b = System.Math.Atan2(2 * quat.X * quat.W - 2 * quat.Y * quat.Z, 1 - 2 * qx2 - 2 * qz2);

            return GetVect(System.Math.Round(b * 180 / System.Math.PI),
                System.Math.Round(h * 180 / System.Math.PI),
                System.Math.Round(a * 180 / System.Math.PI));

            // Sub-Methods
            Vector3 GetVect(double x, double y, double z)
            {
                float multi = (returnResultInRadians) ? 0.0174533f : 1;
                return new Vector3((float)x * multi,
                    (float)y * multi, (float)z * multi);
            }
        }

        public static float ToSingle(double value)
        {
            return (float)value;
        }

        /// <summary>
        /// Uses the Google Cloud API to translate text into gibberish.
        /// </summary>
        /// <param name="text">Text to translate.</param>
        /// <param name="passes">How many times the text should be translated, defaults to 35.</param>
        public static string GoogleTranslate(string text, int passes = 35)
        {
            // Set Console Encoding so we can actually see non ASCII characters.
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Hardcode the language list so we don't ask Google Translate for it every time.
            List<string> Languages = new() { "af", "am", "ar", "az", "be", "bg", "bn", "bs", "ca", "ceb", "co", "cs", "cy", "da", "de", "el", "eo", "es", "et", "eu", "fa", "fi", "fr", "fy", "ga",
                                             "gd", "gl", "gu", "ha", "haw", "he", "hi", "hmn", "hr", "ht", "hu", "hy", "id", "ig", "is", "it", "iw", "ja", "jw", "ka", "kk", "km", "kn", "ko", "ku",
                                             "ky", "la", "lb", "lo", "lt", "lv", "mg", "mi", "mk", "ml", "mn", "mr", "ms", "mt", "my", "ne", "nl", "no", "ny", "or", "pa", "pl", "ps", "pt", "ro",
                                             "ru", "rw", "sd", "si", "sk", "sl", "sm", "sn", "so", "sq", "sr", "st", "su", "sv", "sw", "ta", "te", "tg", "th", "tk", "tl", "tr", "tt", "ug", "uk",
                                             "ur", "uz", "vi", "xh", "yi", "yo", "zh", "zh-CN", "zh-TW", "zu" };

            // Set up a random number generator to pick languages.
            Random rng = new();

            // Print the original passed in string.
            Console.WriteLine($"Original Line: {text}");

            // Loop through the amount of times the user specified.
            for (int c = 0; c < passes; c++)
            {
                int targetLanguage = rng.Next(Languages.Count);
                text = HttpUtility.HtmlDecode(Translate(text, Languages[targetLanguage]));
                Console.WriteLine($"Translation {c + 1}/{passes}. Language {Languages[targetLanguage]}: {text}");
            }
            
            // Translate back to English at the end.
            text = HttpUtility.HtmlDecode(Translate(text, "en"));

            // Print the final translation.
            Console.WriteLine($"Final Translation: {text}");

            // Return the final translation.
            return text;
        }

        /// <summary>
        /// Actual text translation function, only called by the Google Translate one.
        /// </summary>
        /// <param name="inputString">Text to translate.</param>
        /// <param name="targetLanguage">Target language.</param>
        private static string Translate(string inputString, string targetLanguage)
        {
            TranslationClient client = TranslationClient.Create();
            var response = client.TranslateHtml(inputString, targetLanguage);
            return response.TranslatedText;
        }

        // https://stackoverflow.com/questions/35449339/c-sharp-converting-from-float-to-hexstring-via-ieee-754-and-back-to-float
        /// <summary>
        /// Converts a floating point number to a hex representation.
        /// </summary>
        /// <param name="f">The float to convert.</param>
        public static string ToHexString(float f)
        {
            var bytes = BitConverter.GetBytes(f);
            var i = BitConverter.ToInt32(bytes, 0);
            return i.ToString("X8");
        }

        /// <summary>
        /// Converts a hex string to a floating point number.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        public static float FromHexString(string s)
        {
            var i = Convert.ToInt32(s, 16);
            var bytes = BitConverter.GetBytes(i);
            return BitConverter.ToSingle(bytes, 0);
        }

        #region Taken from LibTWOC
        public static ushort ArrayReadU16(byte[] array, long offset, bool bigEndian = true)
        {
            if (bigEndian)
                return (ushort)(array[offset + 1] | (array[offset] << 8));
            return (ushort)(array[offset] | (array[offset + 1] << 8));
        }
        public static byte Convert3To8(byte v)
        {
            return (byte)((v << 5) | (v << 2) | (v >> 1));
        }
        public static byte Convert4To8(byte v)
        {
            return (byte)((v << 4) | v);
        }
        public static byte Convert5To8(byte v)
        {
            return (byte)((v << 3) | (v >> 2));
        }
        public static byte Convert6To8(byte v)
        {
            return (byte)((v << 2) | (v >> 4));
        }
        public static int DXTBlend(int v1, int v2)
        {
            return ((v1 * 3 + v2 * 5) >> 3);
        }

        public static void DecodeDXTBlock(ref Image<Byte4> dst, byte[] src, int srcOffset, int blockX, int blockY)
        {
            if (srcOffset >= src.Length) return;
            var c1 = ArrayReadU16(src, srcOffset, true);
            var c2 = ArrayReadU16(src, srcOffset + 2, true);
            var lines = new byte[4] { src[srcOffset + 4], src[srcOffset + 5], src[srcOffset + 6], src[srcOffset + 7] };

            byte blue1 = Convert5To8((byte)(c1 & 0x1F));
            byte blue2 = Convert5To8((byte)(c2 & 0x1F));
            byte green1 = Convert6To8((byte)((c1 >> 5) & 0x3F));
            byte green2 = Convert6To8((byte)((c2 >> 5) & 0x3F));
            byte red1 = Convert5To8((byte)((c1 >> 11) & 0x1F));
            byte red2 = Convert5To8((byte)((c2 >> 11) & 0x1F));

            Byte4[] colors = new Byte4[4];
            colors[0] = new Byte4(red1, green1, blue1, 255);
            colors[1] = new Byte4(red2, green2, blue2, 255);
            if (c1 > c2)
            {
                colors[2] = new Byte4((byte)DXTBlend(red2, red1), (byte)DXTBlend(green2, green1), (byte)DXTBlend(blue2, blue1), 255);
                colors[3] = new Byte4((byte)DXTBlend(red1, red2), (byte)DXTBlend(green1, green2), (byte)DXTBlend(blue1, blue2), 255);
            }
            else
            {
                // color[3] is the same as color[2] (average of both colors), but transparent.
                // This differs from DXT1 where color[3] is transparent black.
                colors[2] = new Byte4((byte)((red1 + red2) / 2), (byte)((green1 + green2) / 2), (byte)((blue1 + blue2) / 2), 255);
                colors[3] = new Byte4((byte)((red1 + red2) / 2), (byte)((green1 + green2) / 2), (byte)((blue1 + blue2) / 2), 0);
            }

            for (int y = 0; y < 4; y++)
            {
                int val = lines[y];
                for (int x = 0; x < 4; x++)
                {
                    dst[x + blockX, y + blockY] = colors[(val >> 6) & 3];
                    val <<= 2;
                }
            }
        }

        public static void DecodeRGB5A3Block(ref Image<Byte4> dst, byte[] src, int srcOffset, int x, int y)
        {
            if (srcOffset >= src.Length) return;
            var c = ArrayReadU16(src, srcOffset, true);

            byte r, g, b, a;
            if ((c & 0x8000) != 0)
            {
                r = Convert5To8((byte)((c >> 10) & 0x1F));
                g = Convert5To8((byte)((c >> 5) & 0x1F));
                b = Convert5To8((byte)((c) & 0x1F));
                a = 0xFF;
            }
            else
            {
                a = Convert3To8((byte)((c >> 12) & 0x7));
                b = Convert4To8((byte)((c >> 8) & 0xF));
                g = Convert4To8((byte)((c >> 4) & 0xF));
                r = Convert4To8((byte)((c) & 0xF));
            }

            dst[x, y] = new Byte4(r, g, b, a);
        }
        #endregion

        // https://stackoverflow.com/a/47918790
        public static string ToBinaryString(uint num)
        {
            return Convert.ToString(num, 2).PadLeft(32, '0');
        }

        /// <summary>
        /// Renames the diffuse texture in an '06 material to match my naming scheme, while also adding the specular and emission maps.
        /// </summary>
        /// <param name="Reject">The filenames of materials to reject.</param>
        /// <param name="dir">The directory to process.</param>
        public static void UpdateS06MaterialNaming(List<string> Reject, string dir)
        {
            // Loop through each material.
            foreach (string materialFile in Directory.GetFiles(dir, "*.material"))
            {
                // Only proceed if this material isn't in the reject list.
                if (!Reject.Contains(Path.GetFileNameWithoutExtension(materialFile)))
                {
                    // Back up the material just in case.
                    File.Copy(materialFile, $"{materialFile}.bak");
                    
                    // Print the file path to this material.
                    Console.WriteLine(materialFile);

                    // Load this material.
                    GensMaterial mat = new();
                    mat.Load(materialFile);

                    // Redirect the diffuse texture's name.
                    mat.Texset.Textures[0].TextureName = $"{Path.GetFileNameWithoutExtension(materialFile)}_dif";

                    // Set up for adding textures.
                    int textureIndex = 1;
                    GensTexture texture = new();

                    // Set the base shader to Common_d.
                    string shader = "Common_d";

                    // Add an emission map for this material if it exists, also change the shader to "Emission_d".
                    if (File.Exists($@"{dir}\{Path.GetFileNameWithoutExtension(materialFile)}_ems.dds"))
                    {
                        shader = "Emission_d";
                        shader += "p";
                        texture = new()
                        {
                            Name = $"{Path.GetFileNameWithoutExtension(materialFile)}-{textureIndex.ToString().PadLeft(4, '0')}",
                            TextureName = $"{Path.GetFileNameWithoutExtension(materialFile)}_ems",
                            Type = "emission"
                        };
                        mat.Texset.Textures.Add(texture);

                        textureIndex++;
                    }

                    // Add a specular map for this material if it exists, also add a p to the shader name.
                    if (File.Exists($@"{dir}\{Path.GetFileNameWithoutExtension(materialFile)}_spec.dds"))
                    {
                        shader += "p";
                        texture = new()
                        {
                            Name = $"{Path.GetFileNameWithoutExtension(materialFile)}-{textureIndex.ToString().PadLeft(4, '0')}",
                            TextureName = $"{Path.GetFileNameWithoutExtension(materialFile)}_spec",
                            Type = "specular"
                        };
                        mat.Texset.Textures.Add(texture);

                        textureIndex++;
                    }

                    // Add the E to the shader name if an emission texture exists.
                    if (File.Exists($@"{dir}\{Path.GetFileNameWithoutExtension(materialFile)}_ems.dds"))
                        shader += "E";

                    // Set this material's shader name.
                    mat.ShaderName = mat.SubShaderName = shader;

                    // Save this material.
                    mat.Save(materialFile, true);
                }

                // Print the material's file path with a Skipped message if on the reject list.
                else
                {
                    Console.WriteLine($"Skipped {materialFile}.");
                }
            }
        }
    }
}
