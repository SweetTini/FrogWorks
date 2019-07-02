﻿using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class Image : GraphicsComponent
    {
        public Texture Texture { get; protected set; }

        public virtual Rectangle Bounds => new Rectangle(0, 0, Texture.Width, Texture.Height);

        public Rectangle AbsoluteBounds => Bounds.Transform(DrawPosition, Origin, Scale, Angle);

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