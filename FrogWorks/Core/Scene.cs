﻿using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Scene
    {
        protected internal LayerManager Layers { get; private set; }

        protected internal Color BackgroundColor { get; set; } = Color.White;

        protected float TimeActived { get; private set; }

        protected bool IsEnabled { get; set; } = true;

        protected Scene()
        {
            Layers = new LayerManager(this);
        }

        internal void InternalBegin()
        {
            Begin();
        }

        internal void InternalEnd()
        {
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
                TimeActived += deltaTime;
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
