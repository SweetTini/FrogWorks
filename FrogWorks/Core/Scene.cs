using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Scene
    {
        public Color BackgroundColor { get; set; } = Color.CornflowerBlue;

        protected internal LayerManager Layers { get; private set; }

        protected internal EntityManager Entities { get; private set; }

        protected internal Layer DefaultLayer { get; private set; }

        public bool IsEnabled { get; private set; }

        protected Scene()
        {
            Layers = new LayerManager(this);
            Entities = new EntityManager(this);
            DefaultLayer = Layers.AddOrGet("Default");
        }

        public virtual void Begin()
        {
            IsEnabled = true;

            foreach (var entity in Entities)
                entity.OnSceneBegan(this);
        }

        public virtual void End()
        {
            foreach (var entity in Entities)
                entity.OnSceneEnded(this);

            IsEnabled = false;
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
