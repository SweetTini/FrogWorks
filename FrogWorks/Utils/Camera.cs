using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public class Camera
    {
        private Matrix _projectionMatrix, _transformMatrix;
        private Rectangle _bounds;
        private Vector2 _position, _origin;
        private float _zoom = 1f, _angle;
        private bool _isDirty = true;

        public Action<Camera> OnChanged { get; set; }

        public Matrix ProjectionMatrix => _projectionMatrix;

        public Matrix TransformMatrix
        {
            get
            {
                UpdateMatrices();
                return _transformMatrix;
            }
        }

        public Vector2 Min
        {
            get
            {
                return Vector2.Transform(
                    Vector2.Zero,
                    Matrix.Invert(TransformMatrix)).Floor();
            }
        }

        public float Left => Min.X;

        public float Top => Min.Y;

        public Vector2 Max
        {
            get
            {
                UpdateMatrices();
                return Vector2.Transform(
                        _bounds.Size.ToVector2(),
                        Matrix.Invert(TransformMatrix)).Ceiling();
            }
        }

        public float Right => Max.X;

        public float Bottom => Max.Y;

        public Rectangle View { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    _isDirty = true;
                }
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
                value = MathHelper.Clamp(value, .1f, 5f);

                if (_zoom != value)
                {
                    _zoom = value;
                    _isDirty = true;
                }
            }
        }

        public float Angle
        {
            get { return _angle; }
            set
            {
                if (_angle != value)
                {
                    _angle = value;
                    _isDirty = true;
                }
            }
        }

        public float AngleInDegrees
        {
            get { return MathHelper.ToDegrees(_angle); }
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
            _bounds = new Rectangle(Point.Zero, Runner.Application.ActualSize);
            _projectionMatrix = Matrix.CreateOrthographicOffCenter(_bounds, -1000f, 1000f);
            _origin = _bounds.Center.ToVector2();
            _isDirty = true;
        }

        protected void UpdateMatrices()
        {
            if (_isDirty)
            {
                _transformMatrix = Matrix.CreateTranslation(new Vector3(-_position, 0f))
                    * Matrix.CreateRotationZ(_angle)
                    * Matrix.CreateScale(new Vector3(_zoom * Vector2.One, 1f))
                    * Matrix.CreateTranslation(new Vector3(_origin, 0f));
                _isDirty = false;

                View = _bounds.Transform(
                    _position,
                    _origin,
                    Vector2.One.Divide(_zoom),
                    _angle);
                OnChanged?.Invoke(this);
            }
        }
    }
}
