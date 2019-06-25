using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Scene
    {
        public Color BackgroundColor { get; set; } = Color.CornflowerBlue;

        protected Scene()
        {
        }

        public virtual void Begin()
        {
        }

        public virtual void End()
        {
        }

        public virtual void BeginUpdate()
        {
        }

        public virtual void Update(float deltaTime)
        {
        }

        public virtual void EndUpdate()
        {
        }

        public virtual void Draw(RendererBatch batch)
        {
        }
    }
}
