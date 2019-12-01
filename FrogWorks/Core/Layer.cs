using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public abstract class Layer : Manageable<Scene>, IManagerAccessor<Entity>
    {
        protected GraphicsDevice GraphicsDevice { get; private set; }

        protected internal RenderTarget2D Buffer { get; private set; }

        protected internal Camera Camera { get; private set; }

        public EntityManager Entities { get; private set; }

        public BlendState BlendState { get; protected set; }

        public DepthStencilState DepthStencilState { get; protected set; }

        public Effect Effect { get; protected set; }

        public Vector2 ScrollRate { get; set; } = Vector2.One;

        public float XScrollRate
        {
            get { return ScrollRate.X; }
            set { ScrollRate = new Vector2(value, ScrollRate.Y); }
        }

        public float YScrollRate
        {
            get { return ScrollRate.Y; }
            set { ScrollRate = new Vector2(ScrollRate.X, value); }
        }

        public float Zoom
        {
            get { return Camera.Zoom; }
            set { Camera.Zoom = value; }
        }

        public float Angle
        {
            get { return Camera.Angle; }
            set { Camera.Angle = value; }
        }

        public float AngleInDegrees
        {
            get { return Camera.AngleInDegrees; }
            set { Camera.AngleInDegrees = value; }
        }        

        protected Layer()
        {
            GraphicsDevice = Runner.Application.Game.GraphicsDevice;
            Camera = new Camera();
            Entities = new EntityManager(this);
        }

        protected sealed override void Update(float deltaTime)
        {
            Entities.Update(deltaTime);
            UpdateCamera();
        }

        protected override void Draw(RendererBatch batch)
        {
            batch.Configure(BlendState, DepthStencilState, Effect, Camera);
            batch.Begin();
            Entities.Draw(batch);
            batch.End();
        }

        protected override void OnAdded()
        {
            ResetBuffer();

            foreach (var entity in Entities)
                entity.OnInternalLayerAdded();
        }

        protected override void OnRemoved()
        {
            foreach (var entity in Entities)
                entity.OnInternalLayerRemoved();

            ResetBuffer(true);
        }

        protected void ResetBuffer(bool dispose = false)
        {
            Buffer?.Dispose();
            
            if (!dispose)
            {
                var display = Runner.Application.Display;

                Buffer = new RenderTarget2D(GraphicsDevice, display.Width, display.Height, false, 
                                            SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            }
        }

        internal void UpdateBuffer() => ResetBuffer();

        internal void UpdateCamera()
        {
            if (Parent?.Camera != null)
            {
                if (XScrollRate != 0f) Camera.X = Parent.Camera.X * XScrollRate;
                if (YScrollRate != 0f) Camera.Y = Parent.Camera.Y * YScrollRate;
            }
        }

        internal void OnInternalSceneBegan()
        {
            foreach (var entity in Entities)
                entity.OnInternalSceneBegan();
        }

        internal void OnInternalSceneEnded()
        {
            foreach (var entity in Entities)
                entity.OnInternalSceneEnded();
        }

        public sealed override void Destroy() => Parent?.Layers.Remove(this);

        #region Manager Shortcuts
        public void Add(Entity item) => Entities.Add(item);

        public void Add(params Entity[] items) => Entities.Add(items);

        public void Add(IEnumerable<Entity> items) => Entities.Add(items);

        public void Remove(Entity item) => Entities.Remove(item);

        public void Remove(params Entity[] items) => Entities.Remove(items);

        public void Remove(IEnumerable<Entity> items) => Entities.Remove(items);

        public void MoveToTop(Entity item) => Entities.MoveToTop(item);

        public void MoveToBottom(Entity item) => Entities.MoveToBottom(item);

        public void MoveAbove(Entity item, Entity other) => Entities.MoveAbove(item, other);

        public void MoveBelow(Entity item, Entity other) => Entities.MoveBelow(item, other);

        public void SwitchToLayer(Entity item, Layer layer) => Entities.SwitchToLayer(item, layer);

        public IEnumerator<Entity> GetEnumerator() => Entities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
