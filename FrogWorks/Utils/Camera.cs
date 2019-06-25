using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class Camera
    {
        private Matrix _transformMatrix, _inverseMatrix;
        private Vector2 _position, _origin;
        private float _scale = 1f, _angle;
        private bool _isDirty = true;

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

        public Vector2 UpperBounds
        {
            get
            {
                UpdateMatrices();
                return Vector2.Transform(Vector2.Zero, _inverseMatrix);
            }
        }

        public Vector2 LowerBounds
        {
            get
            {
                UpdateMatrices();
                return Vector2.Transform(new Vector2(Viewport.Width, Viewport.Height), _inverseMatrix);
            }
        }

        public float Left => UpperBounds.X;

        public float Right => LowerBounds.X;

        public float Top => UpperBounds.Y;

        public float Bottom => LowerBounds.Y;

        public Viewport Viewport { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (value == _position) return;
                _position = value;
                _isDirty = true;
            }
        }

        public float X
        {
            get { return _position.X; }
            set
            {
                if (value == _position.X) return;
                _position.X = value;
                _isDirty = true;
            }
        }

        public float Y
        {
            get { return _position.Y; }
            set
            {
                if (value == _position.Y) return;
                _position.Y = value;
                _isDirty = true;
            }
        }

        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                if (value == _origin) return;
                _origin = value;
                _isDirty = true;
            }
        }

        public float Scale
        {
            get { return _scale; }
            set
            {
                if (value == _scale) return;
                _scale = value;
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
            set { Angle = MathHelper.ToRadians(_angle); }
        }

        public Camera()
            : this(Game.Instance.Display.Width, Game.Instance.Display.Height)
        {
        }

        public Camera(int width, int height)
        {
            Viewport = new Viewport(0, 0, width, height);
            CenterOrigin();
            UpdateMatrices();
        }

        public void RoundPosition()
        {
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
        }

        public void CenterOrigin()
        {
            Origin = new Vector2(Viewport.Width, Viewport.Height) * .5f;
        }

        public Vector2 ViewToWorld(Vector2 position)
        {
            return Vector2.Transform(position, InverseMatrix);
        }

        public Vector2 WorldToView(Vector2 position)
        {
            return Vector2.Transform(position, TransformMatrix);
        }

        public void Approach(Vector2 position, float rate)
        {
            Position += (position - Position) * rate;
        }

        public void Approach(Vector2 position, float rate, float maxDistance)
        {
            var distanceToMove = (position - Position) * rate;

            Position += distanceToMove.Length() > maxDistance
                ? Vector2.Normalize(distanceToMove) * maxDistance
                : distanceToMove;
        }

        protected void UpdateMatrices()
        {
            if (_isDirty)
            {
                _transformMatrix = Matrix.CreateTranslation(new Vector3(-_position - _origin, 0f)) 
                    * Matrix.CreateRotationZ(_angle) 
                    * Matrix.CreateScale(new Vector3(_scale * Vector2.One, 1f)) 
                    * Matrix.CreateTranslation(new Vector3(_origin, 0f));
                _inverseMatrix = Matrix.Invert(_transformMatrix);
                _isDirty = false;
            }
        }
    }
}
