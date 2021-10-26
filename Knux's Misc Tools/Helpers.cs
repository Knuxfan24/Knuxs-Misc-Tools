using Marathon.Formats.Placement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Knux_s_Misc_Tools
{
    internal class Helpers
    {
        // Where the hell did I get this from anyway?
        public static Quaternion ConvertToQuat(float angle, int rotation = 0)
        {
            Quaternion quat = new Quaternion();
            var h = (angle + rotation) * Math.PI / 360; //Y
            var a = 0 * Math.PI / 360; //Z
            var b = 0 * Math.PI / 360; //X
            var c1 = Math.Cos(h);
            var c2 = Math.Cos(a);
            var c3 = Math.Cos(b);
            var s1 = Math.Sin(h);
            var s2 = Math.Sin(a);
            var s3 = Math.Sin(b);
            quat.W = (float)(Math.Round((c1 * c2 * c3 - s1 * s2 * s3) * 100000) / 100000);
            quat.X = (float)(Math.Round((s1 * s2 * c3 + c1 * c2 * s3) * 100000) / 100000);
            quat.Y = (float)(Math.Round((s1 * c2 * c3 + c1 * s2 * s3) * 100000) / 100000);
            quat.Z = (float)(Math.Round((c1 * s2 * c3 - s1 * c2 * s3) * 100000) / 100000);
            return quat;
        }

        // So I'm not constantly duplicating the same five lines over and over.
        public static SetParameter Add06Parameter(object data, Type type)
        {
            SetParameter s06param = new()
            {
                Data = data,
                DataType = type
            };
            return s06param;
        }
    }
}
