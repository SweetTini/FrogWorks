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

        public void SetOrigin(Origin origin)
        {
            var bounds = Texture.Bounds;

            switch (origin)
            {
                case FrogWorks.Origin.TopLeft:
                    Origin = new Vector2(bounds.Left, bounds.Top);
                    break;
                case FrogWorks.Origin.Top:
                    Origin = new Vector2(bounds.Center.X, bounds.Top);
                    break;
                case FrogWorks.Origin.TopRight:
                    Origin = new Vector2(bounds.Right, bounds.Top);
                    break;
                case FrogWorks.Origin.Left:
                    Origin = new Vector2(bounds.Left, bounds.Center.Y);
                    break;
                case FrogWorks.Origin.Center:
                    Origin = bounds.Center.ToVector2();
                    break;
                case FrogWorks.Origin.Right:
                    Origin = new Vector2(bounds.Right, bounds.Center.Y);
                    break;
                case FrogWorks.Origin.BottomLeft:
                    Origin = new Vector2(bounds.Left, bounds.Bottom);
                    break;
                case FrogWorks.Origin.Bottom:
                    Origin = new Vector2(bounds.Center.X, bounds.Bottom);
                    break;
                case FrogWorks.Origin.BottomRight:
                    Origin = new Vector2(bounds.Right, bounds.Bottom);
                    break;
            }
        }

        public void CenterOrigin()
        {
            Origin = Texture.Bounds.Center.ToVector2();
        }
    }

    public enum Origin
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }
}
