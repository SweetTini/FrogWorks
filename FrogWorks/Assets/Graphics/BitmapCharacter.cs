using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public class BitmapCharacter
    {
        private Dictionary<int, int> _kernings;

        public Texture Texture { get; internal set; }

        public int Ascii { get; internal set; }

        public Point Offset { get; internal set; }

        public int Spacing { get; internal set; }

        public ReadOnlyDictionary<int, int> Kernings { get; }

        internal BitmapCharacter()
        {
            _kernings = new Dictionary<int, int>();
            Kernings = new ReadOnlyDictionary<int, int>(_kernings);
        }

        internal BitmapCharacter(Texture texture, int ascii, Point offset = default(Point), int spacing = 0)
            : this()
        {
            Texture = texture;
            Ascii = ascii;
            Offset = offset;
            Spacing = spacing;
        }

        internal BitmapCharacter(Texture texture, int ascii, int offsetX = 0, int offsetY = 0, int spacing = 0)
            : this(texture, ascii, new Point(offsetX, offsetY), spacing)
        {           
        }

        internal void AddOrUpdateKerning(int ascii, int spacing)
        {
            if (_kernings.ContainsKey(ascii)) _kernings[ascii] = spacing;
            else _kernings.Add(ascii, spacing);
        }
    }
}
