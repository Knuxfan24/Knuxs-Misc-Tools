using Google.Cloud.Translation.V2;
using System.Web;

namespace Knuxs_Misc_Tools
{
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
    }
}
