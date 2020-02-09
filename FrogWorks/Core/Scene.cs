using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public abstract class Scene : IManagerAccessor<Layer>
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
            BeforeUpdate(deltaTime);

            if (IsEnabled)
            {
                Layers.Update(deltaTime);
                TimeActive += deltaTime;
            }

            AfterUpdate(deltaTime);
        }

        protected virtual void BeforeUpdate(float deltaTime) { }

        protected virtual void AfterUpdate(float deltaTime) { }

        internal RenderTarget2D Draw(DisplayAdapter display, RendererBatch batch)
        {
            batch.Begin();
            BeforeDraw(batch);
            batch.End();

            var buffer = null as RenderTarget2D;
            var first = true;

            foreach (var layer in Layers)
            {
                var clearColor = first ? BackgroundColor : Color.TransparentBlack;

                display.GraphicsDevice.SetRenderTarget(layer.Buffer);
                display.GraphicsDevice.Viewport = new Viewport(0, 0, display.Width, display.Height);
                display.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, clearColor, 0, 0);

                if (buffer != null)
                {
                    batch.Sprite.Begin(samplerState: SamplerState.PointClamp);
                    batch.Sprite.Draw(buffer, Vector2.Zero, Color.White);
                    batch.Sprite.End();
                }

                layer.InternalDraw(batch);
                buffer = layer.Buffer;
                first = false;
            }

            batch.Begin();
            AfterDraw(batch);
            batch.End();

            return buffer;
        }

        protected virtual void BeforeDraw(RendererBatch batch) { }

        protected virtual void AfterDraw(RendererBatch batch) { }

        internal void OnDisplayReset()
        {
            Camera.UpdateViewport();
            Layers.Reset();
        }

        #region Manager Shortcuts
        public void Add(Layer item) => Layers.Add(item);

        public void Add(params Layer[] items) => Layers.Add(items);

        public void Add(IEnumerable<Layer> items) => Layers.Add(items);

        public void Remove(Layer item) => Layers.Remove(item);

        public void Remove(params Layer[] items) => Layers.Remove(items);

        public void Remove(IEnumerable<Layer> items) => Layers.Remove(items);

        public void MoveToTop(Layer item) => Layers.MoveToTop(item);

        public void MoveToBottom(Layer item) => Layers.MoveToBottom(item);

        public void MoveAbove(Layer item, Layer other) => Layers.MoveAbove(item, other);

        public void MoveBelow(Layer item, Layer other) => Layers.MoveBelow(item, other);

        public IEnumerator<Layer> GetEnumerator() => Layers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
