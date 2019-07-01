﻿using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public class Layer
    {
        protected internal Scene Scene { get; private set; }

        public Camera Camera { get; private set; } = new Camera();

        public BlendState BlendState { get; set; }

        public DepthStencilState DepthStencilState { get; set; }

        public Effect ShaderEffect { get; set; }

        public string Name { get; internal set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        internal bool IsDefault { get; set; }

        internal Layer()
        {
        }

        internal void OnAdded(Scene scene)
        {
            Scene = scene;
        }

        internal void OnRemoved()
        {
            for (int i = 0; i < Scene.Entities.Count; i++)
                if (Scene.Entities[i].Layer.Equals(this))
                    Scene.Entities[i].Destroy();

            Scene = null;
        }

        public void ConfigureBatch(RendererBatch batch)
        {
            batch.Configure(BlendState, DepthStencilState, ShaderEffect, null, Camera.TransformMatrix);
        }

        public void MoveToFront()
        {
            Scene?.Layers.MoveToFront(this);
        }

        public void MoveToBack()
        {
            Scene?.Layers.MoveToBack(this);
        }

        public void MoveAheadOfLayer(string name)
        {
            Scene?.Layers.MoveAhead(Name, name);
        }

        public void MoveAheadOfLayer(Layer layer)
        {
            Scene?.Layers.MoveAhead(this, layer);
        }

        public void MoveBehindLayer(string name)
        {
            Scene?.Layers.MoveBehind(Name, name);
        }

        public void MoveBehindLayer(Layer layer)
        {
            Scene?.Layers.MoveBehind(this, layer);
        }
    }
}
