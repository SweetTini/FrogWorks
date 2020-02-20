using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public class Layer : Manageable<Scene>
    {
        protected Scene Scene => Parent;

        protected GraphicsDevice GraphicsDevice { get; private set; }

        protected internal RenderTarget2D RenderTarget { get; private set; }

        protected internal Camera Camera { get; private set; }

        public BlendState BlendState { get; protected set; }

        public DepthStencilState DepthStencilState { get; protected set; }

        public Effect Effect { get; protected set; }

        public Color Color { get; set; } = Color.White;

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
        }

        protected sealed override void Update(float deltaTime)
        {
            if (Parent?.Camera != null)
                Camera.Position = Parent.Camera.Position;
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
    }
}
