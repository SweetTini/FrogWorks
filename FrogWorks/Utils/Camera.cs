using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public class Camera
    {
        Matrix _transformMatrix;
        Rectangle _view, _transformView;
        Vector2 _position;
        float _zoom = 1f, _angle;
        bool _isDirty = true;

        public Matrix Matrix
        {
            get
            {
                Update();
                return _transformMatrix;
            }
        }

        public Rectangle View
        {
            get
            {
                Update();
                return _transformView;
            }
        }

        public Vector2 Min => Vector2.Transform(Vector2.Zero, Matrix.Invert(Matrix));

        public float Left => Min.X;

        public float Top => Min.Y;

        public Vector2 Max => Vector2.Transform(View.Size.ToVector2(), Matrix.Invert(Matrix));

        public float Right => Max.X;

        public float Bottom => Max.Y;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                var changed = _position != value;
                _isDirty = _isDirty || changed;
                _position = value;

                if (changed)
                    OnTranslated?.Invoke();
            }
        }

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

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                value = value.Clamp(.1f, 5f);
                _isDirty = _isDirty || _zoom != value;
                _zoom = value;
            }
        }

        public float Angle
        {
            get { return _angle; }
            set
            {
                _isDirty = _isDirty || _angle != value;
                _angle = value;
            }
        }

        public float AngleInDegrees
        {
            get { return MathHelper.ToDegrees(Angle); }
            set { Angle = MathHelper.ToRadians(value); }
        }

        internal Action OnTranslated { get; set; }

        public Camera()
        {
            Position = Runner.Application.Size.ToVector2() * .5f;
            UpdateViewport();
        }

        public Vector2 ViewToWorld(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(Matrix));
        }

        public Vector2 WorldToView(Vector2 position)
        {
            return Vector2.Transform(position, Matrix);
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
            _view = new Rectangle(
                Point.Zero,
                Runner.Application.ActualSize);

            Update(true);
        }

        internal Matrix UpdateMatrix(Vector2 coefficient, float zoom, float angle)
        {
            var position = (Position * coefficient).Round();
            var scale = zoom * Vector2.One;
            var origin = _view.Size.ToVector2() * .5f;

            return Matrix.CreateTranslation(new Vector3(-position, 0f))
                * Matrix.CreateRotationZ(angle)
                * Matrix.CreateScale(new Vector3(scale, 1f))
                * Matrix.CreateTranslation(new Vector3(origin, 0f));
        }

        internal Rectangle UpdateView(Vector2 coefficient, float zoom, float angle)
        {
            var position = (Position * coefficient).Round();
            var inversedZoom = Vector2.One.Divide(zoom);
            var origin = _view.Size.ToVector2() * .5f;

            return _view.Transform(position, origin, inversedZoom, angle);
        }

        void Update(bool forceUpdate = false)
        {
            if (_isDirty || forceUpdate)
            {
                _transformMatrix = UpdateMatrix(Vector2.One, Zoom, Angle);
                _transformView = UpdateView(Vector2.One, Zoom, Angle);
                _isDirty = false;
            }
        }
    }
}
