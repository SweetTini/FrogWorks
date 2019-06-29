using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class Image : GraphicsComponent
    {
        public Texture Texture { get; private set; }

        public Rectangle Bounds
        {
            get { return Texture.Bounds.Transform(DrawPosition, Origin, Scale, Angle); }
        }

        public Image(Texture texture, bool isEnabled)
            : base(isEnabled)
        {
            Texture = texture;
        }

        public override void Draw(RendererBatch batch)
        {
            Texture.Draw(batch, DrawPosition, Origin, Scale, Angle, Color, SpriteEffects);
        }
    }
}
