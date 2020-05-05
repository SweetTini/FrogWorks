using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class Scene : IManager<Entity>
    {
        RenderTarget2D _renderTarget, _backupRenderTarget;
        Layer _currentLayer;

        internal LayerManager Layers { get; private set; }

        internal EntityManager Entities { get; private set; }

        public World World { get; private set; }

        public Camera Camera { get; private set; }

        public Color ClearColor { get; set; }

        public float TimeActive { get; private set; }

        public bool IsActive { get; private set; } = true;

        protected Scene()
        {
            Layers = new LayerManager(this);
            Entities = new EntityManager(this);
            World = new World();
            Camera = new Camera();
        }

        internal void ResetDisplay()
        {
            Camera.UpdateViewport();
            ResetRenderTarget();
            Layers.Reset();
        }

        internal void ResetRenderTarget(bool dispose = false)
        {
            _renderTarget?.Dispose();
            _backupRenderTarget?.Dispose();

            if (!dispose)
            {
                _renderTarget = new RenderTarget2D(
                    Runner.Application.Game.GraphicsDevice,
                    Runner.Application.Display.Width,
                    Runner.Application.Display.Height,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.Depth24Stencil8);

                _backupRenderTarget = new RenderTarget2D(
                    Runner.Application.Game.GraphicsDevice,
                    Runner.Application.Display.Width,
                    Runner.Application.Display.Height,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.Depth24Stencil8);
            }
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
            World.Reset();
        }

        internal void SuspendInternally()
        {
            Suspend();
            IsActive = false;
        }

        internal void ResumeInternally()
        {
            IsActive = true;
            Resume();
        }

        internal void Update(float deltaTime)
        {
            if (IsActive)
            {
                BeforeUpdate(deltaTime);
                Entities.Update(deltaTime);
                TimeActive += deltaTime;
                AfterUpdate(deltaTime);

                Camera.Update();
                Layers.Update(deltaTime);
            }
        }

        internal void Draw(
            DisplayAdapter display,
            RendererBatch batch,
            out RenderTarget2D renderTarget)
        {
            renderTarget = _renderTarget;

            var projection = new Rectangle(Point.Zero, display.Size);
            var clearOptions = ClearOptions.Target
                | ClearOptions.Stencil
                | ClearOptions.DepthBuffer;

            display.GraphicsDevice.SetRenderTarget(renderTarget);
            display.GraphicsDevice.Viewport = new Viewport(projection);
            display.GraphicsDevice.Clear(clearOptions, ClearColor, 0, 0);

            BeforeDraw(batch);

            Entities.State = ManagerState.ThrowError;
            batch.Configure(transformMatrix: Camera.TransformMatrix);
            batch.Begin();

            foreach (var entity in Entities.OnLayer(null))
                if (entity.IsVisible)
                    entity.DrawInternally(batch);

            batch.End();
            Entities.State = ManagerState.Opened;

            Layers.State = ManagerState.ThrowError;

            foreach (var layer in Layers)
            {
                display.GraphicsDevice.SetRenderTarget(layer.RenderTarget);
                display.GraphicsDevice.Clear(clearOptions, Color.Transparent, 0, 0);

                if (!layer.RenderBeforeMerge)
                {
                    batch.Sprite.Begin(samplerState: SamplerState.PointClamp);
                    batch.Sprite.Draw(renderTarget, Vector2.Zero, Color.White);
                    batch.Sprite.End();
                }

                if (layer.IsVisible)
                    layer.DrawInternally(batch);

                display.GraphicsDevice.SetRenderTarget(_backupRenderTarget);
                display.GraphicsDevice.Clear(clearOptions, Color.Transparent, 0, 0);

                if (layer.RenderBeforeMerge)
                {
                    batch.Sprite.Begin(samplerState: SamplerState.PointClamp);
                    batch.Sprite.Draw(renderTarget, Vector2.Zero, Color.White);
                    batch.Sprite.End();
                }

                batch.Sprite.Begin(
                    samplerState: SamplerState.PointClamp,
                    effect: layer.Effect);
                batch.Sprite.Draw(layer.RenderTarget, Vector2.Zero, layer.Color);
                batch.Sprite.End();

                display.GraphicsDevice.SetRenderTarget(layer.RenderTarget);
                display.GraphicsDevice.Clear(clearOptions, Color.Transparent, 0, 0);

                batch.Sprite.Begin(samplerState: SamplerState.PointClamp);
                batch.Sprite.Draw(_backupRenderTarget, Vector2.Zero, Color.White);
                batch.Sprite.End();

                renderTarget = layer.RenderTarget;
            }

            Layers.State = ManagerState.Opened;

            AfterDraw(batch);
        }

        protected virtual void Begin()
        {
        }

        protected virtual void End()
        {
        }

        protected virtual void Suspend()
        {
        }

        protected virtual void Resume()
        {
        }

        protected virtual void BeforeUpdate(float deltaTime)
        {
        }

        protected virtual void AfterUpdate(float deltaTime)
        {
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
            Layers.Add(layer);
        }

        protected void Add(params Layer[] layers)
        {
            Layers.Add(layers);
        }

        protected void Add(IEnumerable<Layer> layers)
        {
            Layers.Add(layers);
        }

        protected void Remove(Layer layer)
        {
            Layers.Remove(layer);
        }

        protected void Remove(params Layer[] layers)
        {
            Layers.Remove(layers);
        }

        protected void Remove(IEnumerable<Layer> layers)
        {
            Layers.Remove(layers);
        }

        protected void SetCurrentLayer(Layer layer)
        {
            if (layer == null || Layers.Contains(layer))
                _currentLayer = layer;
        }

        public void SetCurrentLayer(int index)
        {
            _currentLayer = Layers[index];
        }

        public Layer GetCurrentLayer()
        {
            return _currentLayer;
        }

        public Layer GetLayer(int index)
        {
            return Layers[index];
        }
        #endregion

        #region Entities
        protected void Add(Layer layer, Entity entity)
        {
            Entities.Add(entity);

            if (Layers.Contains(layer))
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
            Add(_currentLayer, entity);
        }

        public void Add(params Entity[] entities)
        {
            Add(_currentLayer, entities);
        }

        public void Add(IEnumerable<Entity> entities)
        {
            Add(_currentLayer, entities);
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

        #region Intervals
        public bool ForEvery(float seconds)
        {
            var deltaTime = Runner.Application.Game.DeltaTime;
            var lastElapsed = (int)((TimeActive - deltaTime) / seconds);
            var elapsed = (int)(TimeActive / seconds);

            return lastElapsed < elapsed;
        }

        public bool ForEvery(float seconds, float offset)
        {
            var deltaTime = Runner.Application.Game.DeltaTime;
            var lastElapsed = (int)((TimeActive - offset - deltaTime) / seconds);
            var elapsed = (int)((TimeActive - offset) / seconds);

            return lastElapsed < elapsed;
        }

        public bool BetweenEvery(float seconds)
        {
            return (TimeActive % (seconds * 2f)) > seconds;
        }
        #endregion
    }
}
