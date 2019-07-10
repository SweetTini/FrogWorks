using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public interface IShape
    {
        Vector2 Position { get; set; }

        float X { get; set; }

        float Y { get; set; }

        Rectangle Bounds { get; }

        void Draw(RendererBatch batch, Color color, bool fill = false);

        bool Contains(Vector2 point);

        bool Contains(float x, float y);
    }
}
