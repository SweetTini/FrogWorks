﻿using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Scene
    {
        public Color BackgroundColor { get; set; } = Color.CornflowerBlue;

        protected internal LayerManager Layers { get; private set; }

        protected internal EntityManager Entities { get; private set; }

        protected internal Layer MainLayer { get; private set; }

        public bool IsEnabled { get; private set; }

        protected Scene()
        {
            Layers = new LayerManager(this);
            Entities = new EntityManager(this);
            MainLayer = Layers.AddOrGet("Main");
            MainLayer.IsDefault = true;
        }

        public virtual void Begin()
        {
            IsEnabled = true;

            for (int i = 0; i < Entities.Count; i++)
                Entities[i].OnSceneBegan(this);
        }

        public virtual void End()
        {
            for (int i = 0; i < Entities.Count; i++)
                Entities[i].OnSceneEnded(this);

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
