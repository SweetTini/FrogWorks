using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class Camera
    {
        Rectangle _view;

        public Matrix TransformMatrix { get; private set; }

        public Rectangle View { get; private set; }

        public Vector2 Min
        {
            get
            {
                return Vector2.Transform(
                    Vector2.Zero,
                    Matrix.Invert(TransformMatrix));
            }
        }

        public float Left => Min.X;

        public float Top => Min.Y;

        public Vector2 Max
        {
            get
            {
                return Vector2.Transform(
                    View.Size.ToVector2(),
                    Matrix.Invert(TransformMatrix));
            }
        }

        public float Right => Max.X;

        public float Bottom => Max.Y;

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

        public float Zoom { get; set; } = 1f;

        public float Angle { get; set; }

        public float AngleInDegrees
        {
            get { return MathHelper.ToDegrees(Angle); }
            set { Angle = MathHelper.ToRadians(value); }
        }

        public Camera()
        {
            Position = Runner.Application.Size.ToVector2() * .5f;
            UpdateViewport();
        }

        public Vector2 ViewToWorld(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(TransformMatrix));
        }

        public Vector2 WorldToView(Vector2 position)
        {
            return Vector2.Transform(position, TransformMatrix);
        }

        public void Approach(Vector2 position, float amount)
        {
            Position = Position.Lerp(position, amount);
        }

        public void Approach(Vector2 position, float amount, float maxDistance)
        {
            Position = Position.Lerp(position, amount, maxDistance);
        }

        internal void UpdateViewport()
        {
            _view = new Rectangle(Point.Zero, Runner.Application.ActualSize);
        }

        internal Matrix UpdateTransformMatrix(
            Vector2? coefficient = null,
            float? zoom = null,
            float? angle = null)
        {
            var position = Position * (coefficient ?? Vector2.One);
            var scale = (zoom ?? Zoom).Clamp(.1f, 5f) * Vector2.One;
            var origin = _view.Size.ToVector2() * .5f;

            return Matrix.CreateTranslation(new Vector3(-position, 0f))
                * Matrix.CreateRotationZ(angle ?? Angle)
                * Matrix.CreateScale(new Vector3(scale, 1f))
                * Matrix.CreateTranslation(new Vector3(origin, 0f));
        }

        internal Rectangle UpdateView(
            Vector2? coefficient = null,
            float? zoom = null,
            float? angle = null)
        {
            var position = Position * (coefficient ?? Vector2.One);
            var inversedZoom = Vector2.One.Divide(zoom ?? Zoom);
            var origin = _view.Size.ToVector2() * .5f;
            return _view.Transform(position, origin, inversedZoom, angle ?? Angle);
        }

        internal void Update()
        {
            TransformMatrix = UpdateTransformMatrix();
            View = UpdateView();
        }
    }
}
