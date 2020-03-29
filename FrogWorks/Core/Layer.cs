using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public class Layer : Manageable<Scene>
    {
        protected internal Scene Scene => Parent;

        protected GraphicsDevice GraphicsDevice { get; private set; }

        protected internal RenderTarget2D RenderTarget { get; private set; }

        protected internal Camera Camera { get; private set; }

        public BlendState BlendState { get; protected set; }

        public DepthStencilState DepthStencilState { get; protected set; }

        public Effect Effect { get; protected set; }

        public Color Color { get; set; } = Color.White;

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

        public float Zoom
        {
            get { return Camera.Zoom; }
            set { Camera.Zoom = value; }
        }

        public float Angle
        {
            get { return Camera.Angle; }
            set { Camera.Angle = value; }
        }

        public float AngleInDegrees
        {
            get { return Camera.AngleInDegrees; }
            set { Camera.AngleInDegrees = value; }
        }

        public bool RenderBeforeMerge { get; set; } = true;

        public Layer()
        {
            GraphicsDevice = Runner.Application.Game.GraphicsDevice;
            Camera = new Camera();
            Position = Camera.Position;
        }

        protected sealed override void Update(float deltaTime)
        {
            var offset = Parent?.Camera.Min ?? Vector2.Zero;
            Camera.Position = Position + offset * Coefficient;
        }

        protected sealed override void Draw(RendererBatch batch)
        {
            if (Scene?.Entities == null) return;

            Scene.Entities.State = ManagerState.ThrowError;
            batch.Configure(BlendState, DepthStencilState, Effect, Camera);
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
