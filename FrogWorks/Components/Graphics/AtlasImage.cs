using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class AtlasImage : GraphicsComponent
    {
        public TextureAltasTexture Texture { get; protected set; }

        public virtual Rectangle Bounds => new Rectangle(Point.Zero, Texture.Size.ToPoint());

        public Rectangle AbsoluteBounds => Bounds.Transform(DrawPosition, Origin, Scale, Angle);

        public AtlasImage(TextureAltasTexture texture, bool isEnabled)
            : base(isEnabled)
        {
            Texture = texture;
        }

        protected override void Draw(RendererBatch batch)
        {
            Texture.Draw(batch, DrawPosition, Origin, Scale, Angle, Color * Opacity.Clamp(0f, 1f), SpriteEffects);
        }

        public void SetOrigin(Origin origin)
        {
            switch (origin)
            {
                case FrogWorks.Origin.TopLeft:
                    Origin = new Vector2(Bounds.Left, Bounds.Top);
                    break;
                case FrogWorks.Origin.Top:
                    Origin = new Vector2(Bounds.Center.X, Bounds.Top);
                    break;
                case FrogWorks.Origin.TopRight:
                    Origin = new Vector2(Bounds.Right, Bounds.Top);
                    break;
                case FrogWorks.Origin.Left:
                    Origin = new Vector2(Bounds.Left, Bounds.Center.Y);
                    break;
                case FrogWorks.Origin.Center:
                    Origin = Bounds.Center.ToVector2();
                    break;
                case FrogWorks.Origin.Right:
                    Origin = new Vector2(Bounds.Right, Bounds.Center.Y);
                    break;
                case FrogWorks.Origin.BottomLeft:
                    Origin = new Vector2(Bounds.Left, Bounds.Bottom);
                    break;
                case FrogWorks.Origin.Bottom:
                    Origin = new Vector2(Bounds.Center.X, Bounds.Bottom);
                    break;
                case FrogWorks.Origin.BottomRight:
                    Origin = new Vector2(Bounds.Right, Bounds.Bottom);
                    break;
            }
        }

        public void CenterOrigin()
        {
            Origin = Bounds.Center.ToVector2();
        }
    }
}
