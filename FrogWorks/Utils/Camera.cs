﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class Camera
    {
        private Matrix _transformMatrix, _inverseMatrix;
        private Viewport _viewport;
        private Vector2 _position, _origin, _padding;
        private float _zoom = 1f, _angle;
        private bool _isDirty = true;

        public Action<Camera> OnCameraUpdated { get; set; }

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

        public Vector2 Lower
        {
            get
            {
                UpdateMatrices();
                return Vector2.Transform(new Vector2(_viewport.Width, _viewport.Height), _inverseMatrix).Round();
            }
        }

        public float Left => Upper.X;

        public float Right => Lower.X;

        public float Top => Upper.Y;

        public float Bottom => Lower.Y;

        public Rectangle Bounds { get; private set; }

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
            : this(Engine.Display.Width, Engine.Display.Height)
        {
            Engine.Display.OnBackBufferChanged += OnScreenChanged;
        }

        public Camera(int width, int height)
        {
            _viewport = new Viewport(0, 0, width, height);
            CenterOrigin();
            UpdateMatrices();
        }

        public void RoundPosition()
        {
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
        }

        public void CenterOrigin()
        {
            Origin = new Vector2(_viewport.Width, _viewport.Height) * .5f;
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

        internal void OnScreenChanged()
        {
            var display = Engine.Display;
            _padding = new Vector2(display.ExtendedWidth, display.ExtendedHeight) * .5f;
            _viewport = new Viewport(0, 0, display.Width + display.ExtendedWidth, display.Height + display.ExtendedHeight);
            _isDirty = true;
        }

        protected void UpdateMatrices()
        {
            if (_isDirty)
            {
                var position = _position - _padding;

                _transformMatrix = Matrix.CreateTranslation(new Vector3(-position - _origin, 0f)) 
                    * Matrix.CreateRotationZ(_angle) 
                    * Matrix.CreateScale(new Vector3(_zoom * Vector2.One, 1f)) 
                    * Matrix.CreateTranslation(new Vector3(_origin, 0f));
                _inverseMatrix = Matrix.Invert(_transformMatrix);
                _isDirty = false;

                Bounds = _viewport.Bounds.Transform(position + _origin, _origin, Vector2.One / _zoom, _angle);
                OnCameraUpdated?.Invoke(this);
            }
        }
    }
}
