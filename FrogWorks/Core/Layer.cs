using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public abstract class Layer : Managable<Scene>
    {
        protected GraphicsDevice GraphicsDevice { get; private set; }

        protected internal Camera Camera { get; private set; }

        public EntityManager Entities { get; private set; }

        public BlendState BlendState { get; protected set; }

        public DepthStencilState DepthStencilState { get; protected set; }

        public Effect Effect { get; protected set; }

        public Vector2 ScrollRate { get; set; } = Vector2.One;

        public float XScrollRate
        {
            get { return ScrollRate.X; }
            set { ScrollRate = new Vector2(value, ScrollRate.Y); }
        }

        public float YScrollRate
        {
            get { return ScrollRate.Y; }
            set { ScrollRate = new Vector2(ScrollRate.X, value); }
        }

        protected Layer()
        {
            GraphicsDevice = Runner.Application.Game.GraphicsDevice;
            Camera = new Camera();
            Entities = new EntityManager(this);
        }

        protected sealed override void Update(float deltaTime)
        {
            Entities.Update(deltaTime);
            UpdateCamera();
        }

        protected override void Draw(RendererBatch batch)
        {
            batch.Configure(BlendState, DepthStencilState, Effect, Camera);
            batch.Begin();
            Entities.Draw(batch);
            batch.End();
        }

        protected override void OnAdded()
        {
            foreach (var entity in Entities)
                entity.OnInternalLayerAdded();
        }

        protected override void OnRemoved()
        {
            foreach (var entity in Entities)
                entity.OnInternalLayerRemoved();
        }

        internal void UpdateCamera()
        {
            if (Parent?.Camera != null)
            {
                if (XScrollRate != 0f) Camera.X = Parent.Camera.X * XScrollRate;
                if (YScrollRate != 0f) Camera.Y = Parent.Camera.Y * YScrollRate;
            }
        }

        internal void OnInternalSceneBegan()
        {
            foreach (var entity in Entities)
                entity.OnInternalSceneBegan();
        }

        internal void OnInternalSceneEnded()
        {
            foreach (var entity in Entities)
                entity.OnInternalSceneEnded();
        }

        public sealed override void Destroy() => Parent?.Layers.Remove(this);
    }
}
