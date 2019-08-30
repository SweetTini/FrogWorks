using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Scene
    {
        protected internal AABBTree ColliderTree { get; private set; }

        public LayerManager Layers { get; private set; }

        public Color BackgroundColor { get; set; } = Color.White;

        public float TimeActive { get; private set; }

        public bool IsEnabled { get; set; } = true;

        protected Scene()
        {
            ColliderTree = new AABBTree(4f);
            Layers = new LayerManager(this);
        }

        internal void InternalBegin()
        {
            Begin();

            foreach (var layer in Layers)
                layer.OnInternalSceneBegan();
        }

        internal void InternalEnd()
        {
            foreach (var layer in Layers)
                layer.OnInternalSceneEnded();

            End();
        }

        protected virtual void Begin() { }

        protected virtual void End() { }

        internal void Update(float deltaTime)
        {
            BeforeUpdate();
            Layers.ProcessQueues();

            if (!IsEnabled)
            {
                Layers.Update(deltaTime);
                TimeActive += deltaTime;
            }

            AfterUpdate();
        }

        protected virtual void BeforeUpdate() { }

        protected virtual void AfterUpdate() { }

        internal void Draw(RendererBatch batch)
        {
            BeforeDraw(batch);
            Layers.Draw(batch);
            AfterDraw(batch);
        }

        protected virtual void BeforeDraw(RendererBatch batch) { }

        protected virtual void AfterDraw(RendererBatch batch) { }
    }
}
