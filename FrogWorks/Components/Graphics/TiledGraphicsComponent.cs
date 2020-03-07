﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public abstract class TiledGraphicsComponent : Component
    {
        Camera _camera;
        Rectangle _drawRegion;
        Vector2 _position;

        public Texture Texture { get; protected set; }

        public Point TileSize { get; protected set; }

        public int TileWidth => TileSize.X;

        public int TileHeight => TileSize.Y;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnTransformedInternally();
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

        public Vector2 DrawPosition
        {
            get { return Position + (Parent?.Position ?? Vector2.Zero); }
            set { Position = value - (Parent?.Position ?? Vector2.Zero); }
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

        public bool WrapHorizontally { get; set; }

        public bool WrapVertically { get; set; }

        protected TiledGraphicsComponent(bool isEnabled)
            : base(isEnabled, true)
        {
        }

        protected sealed override void Draw(RendererBatch batch)
        {
            for (int i = 0; i < _drawRegion.Width * _drawRegion.Height; i++)
            {
                var x = _drawRegion.Left + (i % _drawRegion.Width);
                var y = _drawRegion.Top + (i / _drawRegion.Width);
                var position = DrawPosition + new Vector2(x * TileWidth, y * TileHeight);

                GetTile(x, y)?.Draw(batch, position, Vector2.Zero, Vector2.One, 0f,
                                    Color * Opacity.Clamp(0f, 1f), SpriteEffects);
            }
        }

        protected abstract Texture GetTile(int x, int y);

        protected sealed override void OnAdded()
        {
            var camera = Layer?.Camera ?? Scene?.Camera;
            AddLinkToCamera(camera);
        }

        protected sealed override void OnRemoved()
        {
            RemoveLinkToCamera();

            var camera = Layer?.Camera ?? Scene?.Camera;
            AddLinkToCamera(camera);
        }

        protected sealed override void OnEntityAdded()
        {
            var camera = Layer?.Camera ?? Scene?.Camera;
            AddLinkToCamera(camera);
        }

        protected sealed override void OnEntityRemoved()
        {
            RemoveLinkToCamera();

            var camera = Layer?.Camera ?? Scene?.Camera;
            AddLinkToCamera(camera);
        }

        protected sealed override void OnLayerAdded()
        {
            var camera = Layer?.Camera ?? Scene?.Camera;
            AddLinkToCamera(camera);
        }

        protected sealed override void OnLayerRemoved()
        {
            RemoveLinkToCamera();

            var camera = Layer?.Camera ?? Scene?.Camera;
            AddLinkToCamera(camera);
        }

        protected sealed override void OnTransformed()
        {
            UpdateDrawRegion(_camera);
        }

        void AddLinkToCamera(Camera camera)
        {
            if (camera != null)
            {
                if (_camera == null)
                {
                    _camera = camera;
                    _camera.OnChanged += UpdateDrawRegion;
                    UpdateDrawRegion(_camera);
                }
                else if (_camera != camera)
                {
                    RemoveLinkToCamera();
                    AddLinkToCamera(camera);
                }
            }
        }

        void RemoveLinkToCamera()
        {
            if (_camera != null)
            {
                _camera.OnChanged -= UpdateDrawRegion;
                _camera = null;
            }
        }

        void UpdateDrawRegion(Camera camera)
        {
            if (camera != null)
            {
                var cameraMin = camera.View.Location.ToVector2();
                var cameraMax = cameraMin + camera.View.Size.ToVector2();
                var tileSize = TileSize.ToVector2();

                var min = (cameraMin - DrawPosition).Divide(tileSize).Floor().ToPoint();
                var max = (cameraMax + DrawPosition).Divide(tileSize).Ceiling().ToPoint();

                _drawRegion = new Rectangle(min, max - min);
            }
        }
    }
}
