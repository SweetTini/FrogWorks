using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public sealed class Layer : Managable<Scene>
    {
        public EntityManager Entities { get; private set; }

        public Camera Camera { get; private set; }

        public BlendState BlendState { get; set; }

        public DepthStencilState DepthStencilState { get; set; }

        public Effect ShaderEffect { get; set; }

        public Layer()
        {
            Entities = new EntityManager(this);
            Camera = new Camera();
        }

        protected override void Update(float deltaTime)
        {
            Entities.ProcessQueues();
            Entities.Update(deltaTime);
        }

        protected override void Draw(RendererBatch batch)
        {
            batch.Configure(BlendState, DepthStencilState, ShaderEffect, Camera.TransformMatrix);
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

        public override void Destroy() => Parent?.Layers.Remove(this);
    }
}
