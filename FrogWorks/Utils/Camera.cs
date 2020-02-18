using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class Camera
    {
        private Matrix _projectionMatrix, _transformMatrix, _inverseMatrix;
        private Viewport _viewport;
        private Rectangle? _zone;
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

        public Matrix InverseMatrix
        {
            get
            {
                UpdateMatrices();
                return _inverseMatrix;
            }
        }

        public Vector2 Upper
        {
            get
            {
                UpdateMatrices();
                return Vector2.Transform(Vector2.Zero, _inverseMatrix).Round();
            }
        }

        public float Left => Upper.X;

        public float Top => Upper.Y;

        public Vector2 Lower
        {
            get
            {
                UpdateMatrices();
                return Vector2.Transform(_viewport.Bounds.Size.ToVector2(), _inverseMatrix).Round();
            }
        }

        public float Right => Lower.X;

        public float Bottom => Lower.Y;

        public Rectangle View { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_zone.HasValue)
                {
                    var center = Runner.Application.Size.ToVector2() * .5f;
                    value = value.Clamp(center, _zone.Value.Size.ToVector2() - center);
                }

                if (value == _position) return;
                _position = value;
                _isDirty = true;
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

                if (value == _zoom) return;
                _zoom = value;
                _isDirty = true;
            }
        }

        public float Angle
        {
            get { return _angle; }
            set
            {
                if (value == _angle) return;
                _angle = value;
                _isDirty = true;
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

        public Vector2 ViewToWorld(Vector2 position) => Vector2.Transform(position, InverseMatrix);

        public Vector2 WorldToView(Vector2 position) => Vector2.Transform(position, TransformMatrix);

        public void Approach(Vector2 position, float rate) => Position += (position - Position) * rate;

        public void Approach(Vector2 position, float rate, float maxDistance)
        {
            var distanceToMove = (position - Position) * rate;

            Position += distanceToMove.Length() > maxDistance
                ? Vector2.Normalize(distanceToMove) * maxDistance
                : distanceToMove;
        }

        public void SetZone(Point size)
        {
            var application = Runner.Application;
            size = size.Abs().Max(application.Size);
            _zone = new Rectangle(Point.Zero, size);
        }

        public void SetZone(Vector2 size) => SetZone(size.Round().ToPoint());

        public void SetZone(int width, int height) => SetZone(new Point(width, height));

        public void SetZone(float width, float height) => SetZone(new Vector2(width, height).Round().ToPoint());

        public void ResetZone() => _zone = null;

        internal void UpdateViewport()
        {
            var display = Runner.Application.Display;
            var projection = new Rectangle(Point.Zero, display.Size);
            
            _viewport = new Viewport(projection);
            _projectionMatrix = Matrix.CreateOrthographicOffCenter(projection, -1000f, 1000f);
            _origin = _viewport.Bounds.Center.ToVector2();
            _isDirty = true;

            UpdateMatrices();
        }

        protected void UpdateMatrices()
        {
            if (_isDirty)
            {
                _position = _position.Round();
                _transformMatrix = Matrix.CreateTranslation(new Vector3(-_position, 0f)) 
                    * Matrix.CreateRotationZ(_angle) 
                    * Matrix.CreateScale(new Vector3(_zoom * Vector2.One, 1f)) 
                    * Matrix.CreateTranslation(new Vector3(_origin, 0f));

                _inverseMatrix = Matrix.Invert(_transformMatrix);
                _isDirty = false;

                View = _viewport.Bounds.Transform(_position, _origin, Vector2.One / _zoom, _angle);
                OnChanged?.Invoke(this);
            }
        }
    }
}
