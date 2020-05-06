using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public class Layer : Manageable<Scene>
    {
        Matrix _transformMatrix;
        Rectangle _transformView;
        Vector2 _coefficient = Vector2.One;
        float _zoom = 1f, _angle;
        bool _isDirty = true;

        protected internal Scene Scene => Parent;

        protected GraphicsDevice GraphicsDevice { get; private set; }

        protected internal RenderTarget2D RenderTarget { get; private set; }

        public Matrix Matrix
        {
            get
            {
                UpdateMatrixAndView();
                return _transformMatrix;
            }
        }

        public Rectangle View
        {
            get
            {
                UpdateMatrixAndView();
                return _transformView;
            }
        }

        public BlendState BlendState { get; protected set; }

        public DepthStencilState DepthStencilState { get; protected set; }

        public Effect Effect { get; protected set; }

        public Color Color { get; set; } = Color.White;

        public Vector2 Min => Vector2.Transform(Vector2.Zero, Matrix.Invert(Matrix));

        public float Left => Min.X;

        public float Top => Min.Y;

        public Vector2 Max => Vector2.Transform(View.Size.ToVector2(), Matrix.Invert(Matrix));

        public float Right => Max.X;

        public float Bottom => Max.Y;

        public Vector2 Coefficient
        {
            get { return _coefficient; }
            set
            {
                _isDirty = _isDirty || _coefficient != value;
                _coefficient = value;
            }
        }

        public float XCoefficient
        {
            get { return Coefficient.X; }
            set { Coefficient = new Vector2(value, Coefficient.Y); }
        }

        public float YCoefficient
        {
            get { return Coefficient.Y; }
            set { Coefficient = new Vector2(Coefficient.X, value); }
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

        public bool RenderBeforeMerge { get; set; } = true;

        public Layer()
        {
            GraphicsDevice = Runner.Application.Game.GraphicsDevice;
        }

        protected sealed override void Update(float deltaTime)
        {
        }

        protected sealed override void Draw(RendererBatch batch)
        {
            if (Scene?.Entities == null) return;

            Scene.Entities.State = ManagerState.ThrowError;
            batch.Configure(BlendState, DepthStencilState, Effect, Matrix);
            batch.Begin();

            foreach (var entity in Scene.Entities.OnLayer(this))
                if (entity.IsVisible)
                    entity.DrawInternally(batch);

            batch.End();
            Scene.Entities.State = ManagerState.Opened;
        }

        protected override void OnAdded()
        {
            Scene.Camera.OnTranslated += MarkAsDirty;
            ResetRenderTarget();
        }

        protected override void OnRemoved()
        {
            ResetRenderTarget(true);
            Scene.Camera.OnTranslated -= MarkAsDirty;
        }

        protected void ResetRenderTarget(bool dispose = false)
        {
            RenderTarget?.Dispose();

            if (!dispose)
            {
                RenderTarget = new RenderTarget2D(
                    GraphicsDevice,
                    Runner.Application.Display.Width,
                    Runner.Application.Display.Height,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.Depth24Stencil8);

                UpdateMatrixAndView(true);
            }
        }

        internal void UpdateRenderTarget()
        {
            ResetRenderTarget();
        }

        public sealed override void Destroy()
        {
            Parent?.Layers.Remove(this);
        }

        public Vector2 ViewToWorld(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(Matrix));
        }

        public Vector2 WorldToView(Vector2 position)
        {
            return Vector2.Transform(position, Matrix);
        }

        void UpdateMatrixAndView(bool forceUpdate = false)
        {
            if (_isDirty || forceUpdate)
            {
                var camera = Scene?.Camera;

                if (camera != null)
                {
                    _transformMatrix = camera.UpdateMatrix(Coefficient, Zoom, Angle);
                    _transformView = camera.UpdateView(Coefficient, Zoom, Angle);
                }
                else
                {
                    _transformMatrix = Matrix.Identity;
                    _transformView = new Rectangle(Point.Zero, Runner.Application.ActualSize);
                }

                _isDirty = false;
            }
        }

        void MarkAsDirty()
        {
            _isDirty = true;
        }

        #region Ordering
        public void MoveToTop()
        {
            Scene?.Layers.MoveToTop(this);
        }

        public void MoveToBottom()
        {
            Scene?.Layers.MoveToBottom(this);
        }

        public void MoveAbove(Layer layer)
        {
            Scene?.Layers.MoveAbove(this, layer);
        }

        public void MoveBelow(Layer layer)
        {
            Scene?.Layers.MoveBelow(this, layer);
        }
        #endregion
    }
}
