using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public abstract class GraphicsComponent : Component
    {
        public Vector2 Position { get; set; }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector2(Position.X, value); }
        }

        public Vector2 DrawPosition
        {
            get { return Position + (Parent?.Position ?? Vector2.Zero); }
            set { Position = value - (Parent?.Position ?? Vector2.Zero); }
        }

        public Vector2 Origin { get; set; }

        public float OriginX
        {
            get { return Origin.X; }
            set { Origin = new Vector2(value, Origin.Y); }
        }

        public float OriginY
        {
            get { return Origin.Y; }
            set { Origin = new Vector2(Origin.X, value); }
        }

        public Vector2 Scale { get; set; } = Vector2.One;

        public float XScale
        {
            get { return Scale.X; }
            set { Scale = new Vector2(value, Scale.Y); }
        }

        public float YScale
        {
            get { return Scale.Y; }
            set { Scale = new Vector2(Scale.X, value); }
        }

        public float Angle { get; set; }

        public float AngleInDegrees
        {
            get { return MathHelper.ToDegrees(Angle); }
            set { Angle = MathHelper.ToRadians(value); }
        }

        public Color Color { get; set; } = Color.White;

        public float Opacity { get; set; } = 1f;

        public SpriteEffects SpriteEffects { get; set; }

        public bool FlipHorizontally
        {
            get { return SpriteEffects.HasFlag(SpriteEffects.FlipHorizontally); }
            set
            {
                SpriteEffects = value
                    ? (SpriteEffects | SpriteEffects.FlipHorizontally)
                    : (SpriteEffects & ~SpriteEffects.FlipHorizontally);
            }
        }

        public bool FlipVertically
        {
            get { return SpriteEffects.HasFlag(SpriteEffects.FlipVertically); }
            set
            {
                SpriteEffects = value
                    ? (SpriteEffects | SpriteEffects.FlipVertically)
                    : (SpriteEffects & ~SpriteEffects.FlipVertically);
            }
        }

        protected GraphicsComponent(bool isEnabled)
            : base(isEnabled, true)
        {
        }
    }
}
