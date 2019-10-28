using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Scene
    {
        protected internal AABBTree Colliders { get; private set; }

        public LayerManager Layers { get; private set; }

        public Camera Camera { get; private set; }

        public Color BackgroundColor { get; set; } = Color.White;

        public float TimeActive { get; private set; }

        public bool IsEnabled { get; set; } = true;

        protected Scene()
        {
            Colliders = new AABBTree(4f);
            Layers = new LayerManager(this);
            Camera = new Camera();
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

            Layers.Reset();
        }

        protected virtual void Begin() { }

        protected virtual void End() { }

        internal void Update(float deltaTime)
        {
            BeforeUpdate();

            if (IsEnabled)
            {
                Layers.Update(deltaTime);
                TimeActive += deltaTime;
            }

            AfterUpdate();
        }

        protected virtual void BeforeUpdate() { }

        protected virtual void AfterUpdate() { }

        internal void Draw(RendererBatch batch, Layer layer)
        {
            BeforeDraw(batch);
            layer.InternalDraw(batch);
            AfterDraw(batch);
        }

        protected virtual void BeforeDraw(RendererBatch batch) { }

        protected virtual void AfterDraw(RendererBatch batch) { }

        internal void OnDisplayReset()
        {
            Camera.UpdateViewport();
            Layers.Reset();
        }
    }
}
