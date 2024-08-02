using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFusionsRevitCore.Servers3dContext.Graphics
{
    //https://htmlcolorcodes.com/color-names/
    //https://www.computerhope.com/htmcolor.htm


    public class ColorFactory
    {
        private List<Color> Colors = new List<Color>();
        private int ColorsInt = 0;
        private Random random = new Random();
        public ColorFactory()
        {
            Colors.Add(ColorsGreen.Olive.GetColor());
            Colors.Add(ColorsGreen.GreenYellow.GetColor());
            Colors.Add(ColorsRed.Red.GetColor());
            Colors.Add(ColorsOrange.Orange.GetColor());
            Colors.Add(ColorsPurple.Magenta.GetColor());
            Colors.Add(ColorsBrown.DarkGoldenrod.GetColor());
            Colors.Add(ColorsPink.Pink.GetColor());
            Colors.Add(ColorsYellow.Yellow.GetColor());
        }

        public Color GetAColor()
        {
            if (ColorsInt < Colors.Count)
            {
                ColorsInt++;
                return Colors[ColorsInt - 1];
            }
            else
            {
                ColorsInt = 1;
                return Colors[0];
            }

        }

    }

    public class SimpleColors
    {
        public static ColorWithTransparency Red = new ColorWithTransparency(255, 0, 0, 0);
        public static ColorWithTransparency Green = new ColorWithTransparency(0, 255, 0, 0);
        public static ColorWithTransparency Blue = new ColorWithTransparency(0, 0, 255, 0);
        public static ColorWithTransparency Black = new ColorWithTransparency(0, 0, 0, 0);
        public static ColorWithTransparency White = new ColorWithTransparency(255, 255, 255, 0);
        public static ColorWithTransparency Magenta = new ColorWithTransparency(255, 0, 255, 0);
        public static ColorWithTransparency Cyan = new ColorWithTransparency(0, 255, 255, 0);
        public static ColorWithTransparency Orange = new ColorWithTransparency(255, 165, 0, 0);
    }

    public class ColorsScreen
    {
        public static ColorWithTransparency GreenScreen = new ColorWithTransparency(0, 128, 0, 0);
    }



    public class ColorsGreen
    {
        public static ColorWithTransparency Olive = new ColorWithTransparency(128, 128, 0, 0);
        public static ColorWithTransparency DarkSeaGreen = new ColorWithTransparency(143, 188, 139, 0);
        public static ColorWithTransparency ForestGreen = new ColorWithTransparency(34, 139, 34, 0);
        public static ColorWithTransparency SpringGreen = new ColorWithTransparency(0, 255, 127, 0);
        public static ColorWithTransparency GreenYellow = new ColorWithTransparency(173, 255, 47, 0);
    }

    public class ColorsRed
    {
        public static ColorWithTransparency DarkRed = new ColorWithTransparency(139, 0, 0, 0);
        public static ColorWithTransparency Red = new ColorWithTransparency(255, 0, 0, 0);
        public static ColorWithTransparency IndianRed = new ColorWithTransparency(205, 92, 92, 0);
        public static ColorWithTransparency RedLight = new ColorWithTransparency(255, 128, 128, 0);
        public static ColorWithTransparency Crimson = new ColorWithTransparency(220, 20, 60, 0);
    }

    public class ColorsPink
    {
        public static ColorWithTransparency DeepPink = new ColorWithTransparency(255, 20, 147, 0);
        public static ColorWithTransparency Pink = new ColorWithTransparency(255, 192, 203, 0);
    }

    public class ColorsOrange
    {
        public static ColorWithTransparency Orange = new ColorWithTransparency(255, 165, 0, 0);
        public static ColorWithTransparency OrangeRed = new ColorWithTransparency(255, 69, 0, 0);
        public static ColorWithTransparency LightSalmon = new ColorWithTransparency(255, 160, 122, 0);
        public static ColorWithTransparency Coral = new ColorWithTransparency(255, 127, 80, 0);
    }

    public class ColorsYellow
    {
        public static ColorWithTransparency Yellow = new ColorWithTransparency(255, 255, 0, 0);
        public static ColorWithTransparency Gold = new ColorWithTransparency(255, 215, 0, 0);
        public static ColorWithTransparency LemonChiffon = new ColorWithTransparency(255, 250, 205, 0);
        public static ColorWithTransparency Khaki = new ColorWithTransparency(240, 230, 140, 0);
    }

    public class ColorsPurple
    {
        public static ColorWithTransparency Magenta = new ColorWithTransparency(255, 0, 255, 0);
        public static ColorWithTransparency Indigo = new ColorWithTransparency(75, 0, 130, 0);
        public static ColorWithTransparency Orchid = new ColorWithTransparency(218, 112, 214, 0);
        public static ColorWithTransparency Thistle = new ColorWithTransparency(216, 191, 216, 0);
        public static ColorWithTransparency Lavender = new ColorWithTransparency(230, 230, 250, 0);
        public static ColorWithTransparency Purple = new ColorWithTransparency(128, 0, 128, 0);
    }
    //32
    public class ColorsBlue
    {
        public static ColorWithTransparency Blue = new ColorWithTransparency(0, 0, 255, 0);
        public static ColorWithTransparency DarkBlue = new ColorWithTransparency(0, 0, 139, 0);
        public static ColorWithTransparency DodgerBlue = new ColorWithTransparency(30, 144, 255, 0);
        public static ColorWithTransparency DeepSkyValue = new ColorWithTransparency(0, 191, 255, 0);
        public static ColorWithTransparency LightBlue = new ColorWithTransparency(173, 216, 230, 0);
        public static ColorWithTransparency Cyan = new ColorWithTransparency(0, 255, 255, 0);
        public static ColorWithTransparency Turquoise = new ColorWithTransparency(64, 224, 208, 0);
        public static ColorWithTransparency SteelBlue = new ColorWithTransparency(70, 130, 180, 0);
    }

    public class ColorsBrown
    {
        public static ColorWithTransparency SaddleBrown = new ColorWithTransparency(0, 0, 255, 0);
        public static ColorWithTransparency DarkGoldenrod = new ColorWithTransparency(184, 134, 11, 0);
    }

    public class ColorsWhite
    {
        public static ColorWithTransparency HoneyDew = new ColorWithTransparency(240, 255, 240, 0);
        public static ColorWithTransparency SeaShell = new ColorWithTransparency(255, 245, 238, 0);
        public static ColorWithTransparency Linen = new ColorWithTransparency(250, 240, 230, 0);
        public static ColorWithTransparency Beige = new ColorWithTransparency(245, 245, 220, 0);
    }

    public class ColorsGray
    {
        public static ColorWithTransparency Black = new ColorWithTransparency(0, 0, 0, 0);
        public static ColorWithTransparency DarkSlateGray = new ColorWithTransparency(47, 79, 79, 0);
        public static ColorWithTransparency DimGray = new ColorWithTransparency(105, 105, 105, 0);
        public static ColorWithTransparency Silver = new ColorWithTransparency(192, 192, 192, 0);
        public static ColorWithTransparency Gainsboro = new ColorWithTransparency(220, 220, 220, 0);
    }
}
