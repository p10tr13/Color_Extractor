using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GK_Proj_3
{
    public static class Var
    {
        public static BitmapImage? imagebitmap = null;

        public static WriteableBitmap? child1WB = null;

        public static WriteableBitmap? child2WB = null;

        public static WriteableBitmap? child3WB = null;

        public static Dictionary<string, string[]> colorProfiles = new Dictionary<string, string[]>
        {
            {"sRGB", new[]{"0,64", "0,33", "0,3", "0,6", "0,15", "0,06", "0,31273", "0,32902", "2,2", "D65"} },
            {"Adobe RGB", new[]{"0,64", "0,33", "0,21", "0,71", "0,15", "0,06", "0,31273", "0,32902", "2,2", "D65"} },
            {"Apple RGB", new[]{"0,625", "0,34", "0,28", "0,595", "0,155", "0,07", "0,31273", "0,32902", "1,8", "D65"} },
            {"CIE RGB", new[]{"0,735", "0,265", "0,274", "0,717", "0,167", "0,007", "0,333333", "0,333333", "2,2", "E"} },
            {"Wide Gamut", new[]{"0,7347", "0,2653", "0,1152", "0,8264", "0,1566", "0,0177", "0,34567", "0,3585", "1,2", "D50"} }
        };

        public static Dictionary<string, string[]> illuminantDictionary = new Dictionary<string, string[]>
        {
            {"A", new[] {"0,44757", "0,40744"} },
            {"B", new[] {"0,34840", "0,35160"} },
            {"C", new[] {"0,31006", "0,31615"} },
            {"D50", new[] {"0,34567", "0,35850"} },
            {"D55", new[] {"0,33242", "0,34743"} },
            {"D65", new[] {"0,31273", "0,32902"} },
            {"D75", new[] {"0,29902", "0,31485"} },
            {"9300K", new[] {"0,28480", "0,29320"} },
            {"E", new[] {"0,333333", "0,333333"} },
            {"F2", new[] {"0,37207", "0,37512"} },
            {"F7", new[] {"0,31285", "0,32918"} },
            {"F11", new[] {"0,38054", "0,37691"} }
        };
    }
}
