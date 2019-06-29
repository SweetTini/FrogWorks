using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class RendererBatch : IDisposable
    {
        public SpriteBatch Sprite { get; private set; }

        public PrimitiveBatch Primitive { get; private set; }

        protected BlendState BlendState { get; set; }

        protected DepthStencilState DepthStencilState { get; set; }

        protected Effect ShaderEffect { get; set; }

        protected Matrix? ProjectionMatrix { get; set; }

        protected Matrix? TransformMatrix { get; set; }

        public bool IsDisposed { get; private set; }

        internal RendererBatch(GraphicsDevice graphicsDevice)
        {
            Sprite = new SpriteBatch(graphicsDevice);
            Primitive = new PrimitiveBatch(graphicsDevice);
        }

        internal void Configure(BlendState blendState = null, DepthStencilState depthStencilState = null, Effect shaderEffect = null, Matrix? projectionMatrix = null, Matrix ? transformMatrix = null)
        {
            BlendState = blendState ?? BlendState.AlphaBlend;
            DepthStencilState = depthStencilState ?? DepthStencilState.None;
            ShaderEffect = shaderEffect;
            ProjectionMatrix = projectionMatrix;
            TransformMatrix = transformMatrix;
        }

        internal void Reset()
        {
            BlendState = BlendState.AlphaBlend;
            DepthStencilState = DepthStencilState.None;
            ShaderEffect = null;
            ProjectionMatrix = null;
            TransformMatrix = null;
        }

        public void DrawSprite(Action<SpriteBatch> drawAction)
        {
            Sprite.Begin(SpriteSortMode.Deferred, BlendState,SamplerState.PointClamp, DepthStencilState, RasterizerState.CullCounterClockwise, ShaderEffect, TransformMatrix);
            drawAction(Sprite);
            Sprite.End();
        }

        public void DrawPrimitive(Action<PrimitiveBatch> drawAction)
        {
            Primitive.Begin(BlendState, SamplerState.PointClamp, DepthStencilState, ProjectionMatrix, TransformMatrix);
            drawAction(Primitive);
            Primitive.End();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing && !IsDisposed)
            {
                Sprite.Dispose();
                Primitive.Dispose();
                IsDisposed = true;
            }
        }
    }
}
