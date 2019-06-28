using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Scene
    {
        public Color BackgroundColor { get; set; } = Color.CornflowerBlue;

        internal LayerManager Layers { get; private set; }

        internal EntityManager Entities { get; private set; }

        protected Layer DefaultLayer { get; private set; }

        protected Scene()
        {
            Layers = new LayerManager(this);
            Entities = new EntityManager(this);
            DefaultLayer = Layers.AddOrGet("Default");
        }

        public virtual void Begin()
        {
        }

        public virtual void End()
        {
        }

        public virtual void BeginUpdate()
        {
            Entities.ProcessQueues();
            Layers.ProcessQueues();
        }

        public virtual void Update(float deltaTime)
        {
            Layers.Update(deltaTime);
        }

        public virtual void EndUpdate()
        {
        }

        public virtual void Draw(RendererBatch batch)
        {
            Layers.Draw(batch);
        }
    }
}
