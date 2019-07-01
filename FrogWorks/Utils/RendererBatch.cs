using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class RendererBatch : IDisposable
    {
        protected internal SpriteBatch Sprite { get; private set; }

        protected internal PrimitiveBatch Primitive { get; private set; }

        protected BlendState BlendState { get; set; }

        protected DepthStencilState DepthStencilState { get; set; }

        protected Effect ShaderEffect { get; set; }

        protected Matrix? ProjectionMatrix { get; set; }

        protected Matrix? TransformMatrix { get; set; }

        protected RenderingMode RenderingMode { get; private set; } = RenderingMode.None;

        protected bool WrapTexture { get; set; }

        public bool IsDrawing { get; private set; }

        public bool IsDisposed { get; private set; }

        internal RendererBatch(GraphicsDevice graphicsDevice)
        {
            Sprite = new SpriteBatch(graphicsDevice);
            Primitive = new PrimitiveBatch(graphicsDevice);
        }

        public void Configure(BlendState blendState = null, DepthStencilState depthStencilState = null, Effect shaderEffect = null, Matrix? projectionMatrix = null, Matrix? transformMatrix = null)
        {
            BlendState = blendState ?? BlendState.AlphaBlend;
            DepthStencilState = depthStencilState ?? DepthStencilState.None;
            ShaderEffect = shaderEffect;
            ProjectionMatrix = projectionMatrix;
            TransformMatrix = transformMatrix;
        }

        public void Reset()
        {
            BlendState = BlendState.AlphaBlend;
            DepthStencilState = DepthStencilState.None;
            ShaderEffect = null;
            ProjectionMatrix = null;
            TransformMatrix = null;
        }

        public void Begin()
        {
            if (IsDrawing)
                throw new Exception("End must be called before Begin.");

            Sprite.Begin(SpriteSortMode.Deferred, BlendState, SamplerState.PointClamp, DepthStencilState, RasterizerState.CullCounterClockwise, ShaderEffect, TransformMatrix);
            RenderingMode = RenderingMode.Sprites;
            IsDrawing = true;
        }

        public void End()
        {
            if (!IsDrawing)
                throw new Exception("End cannot be called before Begin.");

            if (RenderingMode == RenderingMode.Sprites)
                Sprite.End();
            else Primitive.End();

            RenderingMode = RenderingMode.None;
            WrapTexture = IsDrawing = false;
        }

        public void DrawSprites(Action<SpriteBatch> drawAction, bool wrapTexture = false)
        {
            if (!IsDrawing)
                throw new Exception("Begin must be called before drawing sprites.");

            if (RenderingMode != RenderingMode.Sprites)
            {
                Primitive.End();
                Sprite.Begin(SpriteSortMode.Deferred, BlendState, wrapTexture ? SamplerState.PointWrap : SamplerState.PointClamp, DepthStencilState, RasterizerState.CullCounterClockwise, ShaderEffect, TransformMatrix);
                WrapTexture = wrapTexture;
                RenderingMode = RenderingMode.Sprites;
            }
            else if (WrapTexture != wrapTexture)
            {
                Sprite.End();
                Sprite.Begin(SpriteSortMode.Deferred, BlendState, wrapTexture ? SamplerState.PointWrap : SamplerState.PointClamp, DepthStencilState, RasterizerState.CullCounterClockwise, ShaderEffect, TransformMatrix);
                WrapTexture = wrapTexture;
            }

            drawAction(Sprite);
        }

        public void DrawPrimitives(Action<PrimitiveBatch> drawAction)
        {
            if (!IsDrawing)
                throw new Exception("Begin must be called before drawing primitives.");

            if (RenderingMode != RenderingMode.Primitives)
            {
                Sprite.End();
                Primitive.Begin(BlendState, SamplerState.PointClamp, DepthStencilState, ProjectionMatrix, TransformMatrix);
                RenderingMode = RenderingMode.Primitives;
            }

            drawAction(Primitive);
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

    public enum RenderingMode
    {
        None,
        Sprites,
        Primitives
    }
}
