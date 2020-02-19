using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public static class ColorEX
    {
        public static Color FromRGB(int red, int green, int blue, int alpha)
        {
            return new Color(
                MathHelper.Clamp(red, 0, 255),
                MathHelper.Clamp(green, 0, 255),
                MathHelper.Clamp(blue, 0, 255),
                MathHelper.Clamp(alpha, 0, 255));
        }

        public static Color FromRGB(int red, int green, int blue)
        {
            return FromRGB(red, green, blue, 255);
        }

        public static Color FromHSL(int hue, int saturation, int lightness, int alpha)
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
                (int)Math.Round(b * 255f),
                alpha.Clamp(0, 255));
        }

        public static Color FromHSL(int hue, int saturation, int lightness)
        {
            return FromHSL(hue, saturation, lightness, 255);
        }

        public static Color FromHSV(int hue, int saturation, int value, int alpha)
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
                (int)Math.Round(b * 255f),
                alpha.Clamp(0, 255));
        }

        public static Color FromHSV(int hue, int saturation, int value)
        {
            return FromHSV(hue, saturation, value, 255);
        }

        public static Color FromCMYK(int cyan, int magneta, int yellow, int black, int alpha)
        {
            var c = MathHelper.Clamp(cyan, 0, 100) / 100f;
            var m = MathHelper.Clamp(magneta, 0, 100) / 100f;
            var y = MathHelper.Clamp(yellow, 0, 100) / 100f;
            var k = MathHelper.Clamp(black, 0, 100) / 100f;

            var r = (int)Math.Round(255f * (1f - c) * (1f - k));
            var g = (int)Math.Round(255f * (1f - m) * (1f - k));
            var b = (int)Math.Round(255f * (1f - y) * (1f - k));

            return new Color(r, g, b, alpha.Clamp(0, 255));
        }

        public static Color FromCMYK(int cyan, int magneta, int yellow, int black)
        {
            return FromCMYK(cyan, magneta, yellow, black, 255);
        }

        public static Color FromHex(string hex)
        {
            if (hex.Length >= 6)
            {
                var r = hex[0].HexToByte() * 16 + hex[1].HexToByte();
                var g = hex[2].HexToByte() * 16 + hex[3].HexToByte();
                var b = hex[4].HexToByte() * 16 + hex[5].HexToByte();
                var a = 255;

                if (hex.Length >= 8)
                    a = hex[6].HexToByte() * 16 + hex[7].HexToByte();

                return new Color(r, g, b, a);
            }

            return Color.Black;
        }
    }
}
