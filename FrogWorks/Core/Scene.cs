using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class Scene : IManager<Entity>
    {
        Layer _activeLayer;

        internal LayerManager Layers { get; private set; }

        internal EntityManager Entities { get; private set; }

        public Camera Camera { get; private set; }

        public Color ClearColor { get; set; }

        public float TimeActive { get; private set; }

        public bool IsActive { get; private set; }

        protected Scene()
        {
            Layers = new LayerManager(this);
            Entities = new EntityManager(this);
            Camera = new Camera();

            var defaultLayer = new Layer();
            Add(defaultLayer);
        }

        internal void ResetDisplay()
        {
            Camera.UpdateViewport();
            Layers.Reset();
        }

        internal void BeginInternally()
        {
            Begin();

            foreach (var entity in Entities)
                entity.OnSceneBeganInternally();
        }

        internal void EndInternally()
        {
            foreach (var entity in Entities)
                entity.OnSceneEndedInternally();

            End();

            Layers.Reset();
        }

        protected virtual void Begin() { }

        protected virtual void End() { }

        internal void Update(float deltaTime)
        {
            BeforeUpdate(deltaTime);

            if (IsActive)
            {
                Entities.Update(deltaTime);
                Layers.Update(deltaTime);
                TimeActive += deltaTime;
            }

            AfterUpdate(deltaTime);
        }

        protected virtual void BeforeUpdate(float deltaTime) 
        {
        }

        protected virtual void AfterUpdate(float deltaTime) 
        { 
        }

        internal RenderTarget2D Draw(DisplayAdapter display, RendererBatch batch)
        {
            var renderTarget = (RenderTarget2D)null;
            var first = true;

            batch.Configure(camera: Camera);
            batch.Begin();
            BeforeDraw(batch);
            batch.End();

            Layers.State = ManagerState.ThrowError;

            foreach (var layer in Layers)
            {
                var clearColor = first ? ClearColor : Color.Transparent;
                var viewBounds = new Rectangle(Point.Zero, Runner.Application.Display.Size);
                var clearOptions = ClearOptions.Target
                    | ClearOptions.Stencil
                    | ClearOptions.DepthBuffer;

                display.GraphicsDevice.SetRenderTarget(layer.RenderTarget);
                display.GraphicsDevice.Viewport = new Viewport(viewBounds);
                display.GraphicsDevice.Clear(clearOptions, clearColor, 0, 0);

                if (renderTarget != null)
                {
                    batch.Sprite.Begin(samplerState: SamplerState.PointClamp);
                    batch.Sprite.Draw(renderTarget, Vector2.Zero, Color.White);
                    batch.Sprite.End();
                }

                if (layer.IsVisible)
                    layer.DrawInternally(batch);

                renderTarget = layer.RenderTarget;
                first = false;
            }

            Layers.State = ManagerState.Opened;

            batch.Configure(camera: Camera);
            batch.Begin();
            AfterDraw(batch);
            batch.End();

            return renderTarget;
        }

        protected virtual void BeforeDraw(RendererBatch batch)
        { 
        }

        protected virtual void AfterDraw(RendererBatch batch) 
        { 
        }

        #region Layers
        protected void Add(Layer layer)
        {
            var first = Layers.Count == 0;
            
            Layers.Add(layer);

            if (first)
                SetActiveLayer(0);
        }

        protected void Add(params Layer[] layers)
        {
            foreach (var layer in layers)
                Add(layer);
        }

        protected void Add(IEnumerable<Layer> layers)
        {
            foreach (var layer in layers)
                Add(layer);
        }

        protected void Remove(Layer layer)
        {
            if (Layers.Count > 1)
            {
                var index = Layers.IndexOf(layer);
                
                Layers.Remove(layer);

                if (layer == _activeLayer)
                    SetActiveLayer(index);
            }
        }

        protected void Remove(params Layer[] layers)
        {
            foreach (var layer in layers)
                Remove(layer);
        }

        protected void Remove(IEnumerable<Layer> layers)
        {
            foreach (var layer in layers)
                Remove(layer);
        }

        protected void SetActiveLayer(Layer layer)
        {
            if (Layers.Contains(layer))
                _activeLayer = layer;
        }

        public void SetActiveLayer(int index)
        {
            if (Layers.Count > 0)
            {
                index = index.Clamp(0, Layers.Count - 1);
                _activeLayer = Layers[index];
            }
        }
        #endregion

        #region Entities
        protected void Add(Layer layer, Entity entity)
        {
            Entities.Add(entity);

            if (Layers.Contains(layer) && Entities.Contains(entity))
                entity.Layer = layer;
        }

        protected void Add(Layer layer, params Entity[] entities)
        {
            foreach (var entity in entities)
                Add(layer, entity);
        }

        protected void Add(Layer layer, IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
                Add(layer, entity);
        }

        public void Add(Entity entity)
        {
            Add(_activeLayer, entity);
        }

        public void Add(params Entity[] entities)
        {
            Add(_activeLayer, entities);
        }

        public void Add(IEnumerable<Entity> entities)
        {
            Add(_activeLayer, entities);
        }

        public void Remove(Entity entity)
        {
            entity.Layer = null;
            Entities.Remove(entity);
        }

        public void Remove(params Entity[] entities)
        {
            foreach (var entity in entities)
                Remove(entity);
        }

        public void Remove(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
                Remove(entity);
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
