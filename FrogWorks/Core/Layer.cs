using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public abstract class Layer : Managable<Scene>
    {
        public EntityManager Entities { get; private set; }

        public Camera Camera { get; private set; }

        public BlendState BlendState { get; protected set; }

        public DepthStencilState DepthStencilState { get; protected set; }

        public Effect ShaderEffect { get; protected set; }

        protected Layer()
        {
            Entities = new EntityManager(this);
            Camera = new Camera();
        }

        protected sealed override void Update(float deltaTime)
        {
            Entities.ProcessQueues();
            Entities.Update(deltaTime);
        }

        protected override void Draw(RendererBatch batch)
        {
            batch.Configure(BlendState, DepthStencilState, ShaderEffect, null, Camera.TransformMatrix);
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
