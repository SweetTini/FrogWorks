using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public class Layer : Manageable<Scene>
    {
        protected internal Scene Scene => Parent;

        protected GraphicsDevice GraphicsDevice { get; private set; }

        protected internal RenderTarget2D RenderTarget { get; private set; }

        protected Matrix TransformMatrix { get; private set; }

        protected Rectangle View { get; private set; }

        public BlendState BlendState { get; protected set; }

        public DepthStencilState DepthStencilState { get; protected set; }

        public Effect Effect { get; protected set; }

        public Color Color { get; set; } = Color.White;

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

        public Vector2 Coefficient { get; set; } = Vector2.One;

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

        public float Zoom { get; set; } = 1f;

        public float Angle { get; set; }

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
            UpdateTransformMatrixAndView();
        }

        protected sealed override void Draw(RendererBatch batch)
        {
            if (Scene?.Entities == null) return;

            Scene.Entities.State = ManagerState.ThrowError;
            batch.Configure(BlendState, DepthStencilState, Effect, TransformMatrix);
            batch.Begin();

            foreach (var entity in Scene.Entities.OnLayer(this))
                if (entity.IsVisible)
                    entity.DrawInternally(batch);

            batch.End();
            Scene.Entities.State = ManagerState.Opened;
        }

        protected override void OnAdded()
        {
            ResetRenderTarget();
        }

        protected override void OnRemoved()
        {
            ResetRenderTarget(true);
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

                UpdateTransformMatrixAndView();
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
            return Vector2.Transform(position, Matrix.Invert(TransformMatrix));
        }

        public Vector2 WorldToView(Vector2 position)
        {
            return Vector2.Transform(position, TransformMatrix);
        }

        void UpdateTransformMatrixAndView()
        {
            TransformMatrix = Scene?.Camera?.UpdateTransformMatrix(
                Coefficient, Zoom, Angle) ?? Matrix.Identity;
            
            View = Scene?.Camera?.UpdateView(Coefficient, Zoom, Angle)
                ?? new Rectangle(Point.Zero, Runner.Application.ActualSize);
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
