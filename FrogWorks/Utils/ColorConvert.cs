using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public static class ColorConvert
    {
        public static Color FromRgb(int red, int green, int blue)
        {
            return new Color(
                MathHelper.Clamp(red, 0, 255),
                MathHelper.Clamp(green, 0, 255),
                MathHelper.Clamp(blue, 0, 255));
        }

        public static Color FromHsl(int hue, int saturation, int lightness)
        {
            hue = hue.Mod(360);

            var s = MathHelper.Clamp(saturation, 0, 100) / 100f;
            var l = MathHelper.Clamp(lightness, 0, 100) / 100f;
            var c = s * (1f - Math.Abs(2f * l - 1f));
            var x = c * (1f - Math.Abs(((hue / 60f) % 2f) - 1f));
            var m = l - c / 2f;
            float r, g, b;

            switch (hue / 60)
            {
                default: r = c + m; g = x + m; b = m; break;
                case 1: r = x + m; g = c + m; b = m; break;
                case 2: r = m; g = c + m; b = x + m; break;
                case 3: r = m; g = x + m; b = c + m; break;
                case 4: r = x + m; g = m; b = c + m; break;
                case 5: r = c + m; g = m; b = x + m; break;
            }

            return new Color(
                (int)Math.Round(r * 255f),
                (int)Math.Round(g * 255f),
                (int)Math.Round(b * 255f));
        }

        public static Color FromHsv(int hue, int saturation, int value)
        {
            hue = hue.Mod(360);

            var s = MathHelper.Clamp(saturation, 0, 100) / 100f;
            var v = MathHelper.Clamp(value, 0, 100) / 100f;
            var c = s * v;
            var x = c * (1f - Math.Abs(((hue / 60f) % 2f) - 1f));
            var m = v - c;

            float r, g, b;

            switch (hue / 60)
            {
                default: r = c + m; g = x + m; b = m; break;
                case 1: r = x + m; g = c + m; b = m; break;
                case 2: r = m; g = c + m; b = x + m; break;
                case 3: r = m; g = x + m; b = c + m; break;
                case 4: r = x + m; g = m; b = c + m; break;
                case 5: r = c + m; g = m; b = x + m; break;
            }

            return new Color(
                (int)Math.Round(r * 255f),
                (int)Math.Round(g * 255f),
                (int)Math.Round(b * 255f));
        }

        public static Color FromCmyk(int cyan, int magneta, int yellow, int black)
        {
            var c = MathHelper.Clamp(cyan, 0, 100) / 100f;
            var m = MathHelper.Clamp(magneta, 0, 100) / 100f;
            var y = MathHelper.Clamp(yellow, 0, 100) / 100f;
            var k = MathHelper.Clamp(black, 0, 100) / 100f;

            var r = (int)Math.Round(255f * (1f - c) * (1f - k));
            var g = (int)Math.Round(255f * (1f - m) * (1f - k));
            var b = (int)Math.Round(255f * (1f - y) * (1f - k));

            return new Color(r, g, b);
        }
    }
}
