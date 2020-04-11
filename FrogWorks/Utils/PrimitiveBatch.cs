using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Linq;
using XnaPrimitiveType = Microsoft.Xna.Framework.Graphics.PrimitiveType;

namespace FrogWorks
{
    public class PrimitiveBatch : IDisposable
    {
        const int VertsPerLine = 2,
            VertsPerTriangle = 3;

        GraphicsDevice _graphicsDevice;
        BasicEffect _basicEffect;
        Effect _effect;
        VertexPositionColor[] _vertices;
        PrimitiveType _type;
        int _bufferIndex;
        bool _hasBegun;

        public PrimitiveType PrimitiveType
        {
            get { return _type; }
            private set
            {
                if (_type != value)
                {
                    Flush();

                    _type = value;
                    _bufferIndex = 0;
                }
            }
        }

        public bool IsDisposed { get; private set; }

        public PrimitiveBatch(GraphicsDevice graphicsDevice, int bufferSize = 512)
        {
            var fixedSize = bufferSize
                   - (bufferSize % VertsPerLine)
                   - (bufferSize % VertsPerTriangle);

            _graphicsDevice = graphicsDevice;
            _basicEffect = new BasicEffect(_graphicsDevice);
            _basicEffect.VertexColorEnabled = true;
            _vertices = new VertexPositionColor[fixedSize];
        }

        public void Begin(
            BlendState blendState = null,
            SamplerState samplerState = null,
            DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null,
            Effect effect = null,
            Matrix? viewMatrix = null)
        {
            CheckAfterDraw();

            var projectionMatrix = Matrix
                .CreateOrthographicOffCenter(
                    new Rectangle(Point.Zero, Runner.Application.ActualSize),
                    -1000f, 1000f);

            _graphicsDevice.BlendState = blendState ?? BlendState.AlphaBlend;
            _graphicsDevice.SamplerStates[0] = samplerState ?? SamplerState.PointClamp;
            _graphicsDevice.RasterizerState = rasterizerState ?? RasterizerState.CullNone;
            _graphicsDevice.DepthStencilState = depthStencilState ?? DepthStencilState.None;
            _effect = effect ?? _basicEffect;

            if (_effect is IEffectMatrices)
            {
                var effectMatrices = _effect as IEffectMatrices;
                effectMatrices.Projection = projectionMatrix;
                effectMatrices.View = viewMatrix ?? Matrix.Identity;
            }

            _hasBegun = true;
        }

        public void End()
        {
            CheckBeforeDraw();
            Flush();

            _hasBegun = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool isDisposing)
        {
            if (isDisposing && !IsDisposed)
            {
                _basicEffect?.Dispose();
                IsDisposed = true;
            }
        }

        void Flush()
        {
            CheckBeforeDraw();

            var perAmount = _type == PrimitiveType.Line
                ? VertsPerLine
                : VertsPerTriangle;

            var primitiveType = _type == PrimitiveType.Line
                ? XnaPrimitiveType.LineList
                : XnaPrimitiveType.TriangleList;

            if (_bufferIndex >= perAmount)
            {
                var count = _bufferIndex / perAmount;

                foreach (var pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphicsDevice.DrawUserPrimitives(primitiveType, _vertices, 0, count);
                }

                _bufferIndex -= count * perAmount;
            }
        }

        #region Validation
        void CheckBeforeDraw()
        {
            CheckIfDisposed();

            if (!_hasBegun)
            {
                var stackTrace = new StackTrace();
                var parentMethod = stackTrace.GetFrame(1).GetMethod();
                throw new Exception($"Begin must be called before {parentMethod.Name}.");
            }
        }

        void CheckAfterDraw()
        {
            CheckIfDisposed();

            if (_hasBegun)
                throw new Exception("End must be called before Begin.");
        }

        void CheckIfDisposed()
        {
            if (IsDisposed)
                throw new Exception("Cannot draw with disposed primitive batch.");
        }
        #endregion

        #region Plotting
        public void AddVertex(float x, float y, float z, Color color, PrimitiveType type)
        {
            AddVertex(new Vector3(x, y, z), color, type);
        }

        public void AddVertex(Vector3 position, Color color, PrimitiveType type)
        {
            CheckBeforeDraw();
            PrimitiveType = type;

            if (_bufferIndex >= _vertices.Length)
                Flush();

            var vertexPositionColor = new VertexPositionColor(position.Round(), color);
            _vertices[_bufferIndex++] = vertexPositionColor;
        }
        #endregion

        #region Dots
        public void DrawDot(float x, float y, Color color)
        {
            DrawDot(new Vector3(x, y, 0f), color);
        }

        public void DrawDot(float x, float y, float z, Color color)
        {
            DrawDot(new Vector3(x, y, z), color);
        }

        public void DrawDot(Vector2 position, Color color)
        {
            DrawDot(new Vector3(position, 0f), color);
        }

        public void DrawDot(Vector3 position, Color color)
        {
            AddVertex(position, color, PrimitiveType.Line);
            AddVertex(position + Vector3.UnitX, color, PrimitiveType.Line);
        }
        #endregion

        #region Lines
        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            DrawLine(new Vector3(x1, y1, 0f), new Vector3(x2, y2, 0f), color);
        }

        public void DrawLine(
            float x1, float y1, float z1,
            float x2, float y2, float z2,
            Color color)
        {
            DrawLine(new Vector3(x1, y1, z1), new Vector3(x2, y2, z2), color);
        }

        public void DrawLine(Vector2 p1, Vector2 p2, Color color)
        {
            DrawLine(new Vector3(p1, 0f), new Vector3(p2, 0f), color);
        }

        public void DrawLine(Vector3 p1, Vector3 p2, Color color)
        {
            if (p1 == p2)
            {
                DrawDot(p1, color);
            }
            else
            {
                AddVertex(p1, color, PrimitiveType.Line);
                AddVertex(p2, color, PrimitiveType.Line);
            }
        }
        #endregion

        #region Triangles
        public void DrawTriangle(
            float x1, float y1,
            float x2, float y2,
            float x3, float y3,
            Color color)
        {
            DrawTriangle(
                new Vector3(x1, y1, 0f),
                new Vector3(x2, y2, 0f),
                new Vector3(x3, y3, 0f),
                color);
        }

        public void DrawTriangle(
            float x1, float y1, float z1,
            float x2, float y2, float z2,
            float x3, float y3, float z3,
            Color color)
        {
            DrawTriangle(
                new Vector3(x1, y1, z1),
                new Vector3(x2, y2, z2),
                new Vector3(x3, y3, z3),
                color);
        }

        public void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
        {
            DrawTriangle(
                new Vector3(p1, 0f),
                new Vector3(p2, 0f),
                new Vector3(p3, 0f),
                color);
        }

        public void DrawTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color)
        {
            if (p1 == p2 && p1 == p3)
            {
                DrawDot(p1, color);
            }
            else
            {
                DrawLine(p1, p2, color);
                DrawLine(p2, p3, color);
                DrawLine(p3, p1, color);
            }
        }

        public void FillTriangle(
            float x1, float y1,
            float x2, float y2,
            float x3, float y3,
            Color color)
        {
            FillTriangle(
                new Vector3(x1, y1, 0f),
                new Vector3(x2, y2, 0f),
                new Vector3(x3, y3, 0f),
                color);
        }

        public void FillTriangle(
            float x1, float y1, float z1,
            float x2, float y2, float z2,
            float x3, float y3, float z3,
            Color color)
        {
            FillTriangle(
                new Vector3(x1, y1, z1),
                new Vector3(x2, y2, z2),
                new Vector3(x3, y3, z3),
                color);
        }

        public void FillTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
        {
            FillTriangle(
                new Vector3(p1, 0f),
                new Vector3(p2, 0f),
                new Vector3(p3, 0f),
                color);
        }

        public void FillTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color)
        {
            if (p1 == p2 && p1 == p3)
            {
                DrawDot(p1, color);
            }
            else if (p2 == p3)
            {
                DrawLine(p1, p2, color);
            }
            else
            {
                AddVertex(p1, color, PrimitiveType.Triangle);
                AddVertex(p2, color, PrimitiveType.Triangle);
                AddVertex(p3, color, PrimitiveType.Triangle);
            }
        }
        #endregion

        #region Polygons
        public void DrawPolygon(Vector2[] vertices, Color color)
        {
            DrawPolygon(vertices.Select(v => new Vector3(v, 0f)).ToArray(), color);
        }

        public void DrawPolygon(Vector3[] vertices, Color color)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                var j = (i + 1) % vertices.Length;
                DrawLine(vertices[i], vertices[j], color);
            }
        }

        public void FillPolygon(Vector2[] vertices, Color color)
        {
            FillPolygon(vertices.Select(v => new Vector3(v, 0f)).ToArray(), color);
        }

        public void FillPolygon(Vector3[] vertices, Color color)
        {
            if (vertices.Length < 3)
            {
                DrawLine(vertices[0], vertices[1 % vertices.Length], color);
            }
            else
            {
                for (int i = 1; i < vertices.Length - 1; i++)
                {
                    var j = (i + 1) % vertices.Length;
                    FillTriangle(vertices[0], vertices[i], vertices[j], color);
                }
            }
        }
        #endregion

        #region Rectangles
        public void DrawRectangle(float x, float y, float width, float height, Color color)
        {
            DrawRectangle(new Vector2(x, y), new Vector2(width, height), color);
        }

        public void DrawRectangle(Vector2 position, Vector2 size, Color color)
        {
            var vertices = new[]
            {
                position,
                position + Vector2.UnitX * size,
                position + size,
                position + Vector2.UnitY * size
            };

            DrawPolygon(vertices, color);
        }

        public void FillRectangle(float x, float y, float width, float height, Color color)
        {
            FillRectangle(new Vector2(x, y), new Vector2(width, height), color);
        }

        public void FillRectangle(Vector2 position, Vector2 size, Color color)
        {
            var vertices = new[]
            {
                position,
                position + Vector2.UnitX * size,
                position + size,
                position + Vector2.UnitY * size
            };

            FillPolygon(vertices, color);
        }
        #endregion

        #region Ellipses
        public void DrawEllipse(
            float cx, float cy,
            float rx, float ry,
            Color color,
            int segments = 16)
        {
            DrawEllipse(new Vector2(cx, cy), new Vector2(rx, ry), color, segments);
        }

        public void DrawEllipse(Vector2 center, Vector2 radii, Color color, int segments = 16)
        {
            var vertices = new Vector2[segments];
            var step = MathHelper.TwoPi / segments;
            var theta = 0.0;

            for (int i = 0; i < segments; i++)
            {
                var angle = new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                vertices[i] = angle * radii + center;
                theta += step;
            }

            DrawPolygon(vertices, color);
        }

        public void FillEllipse(
            float cx, float cy,
            float rx, float ry,
            Color color,
            int segments = 16)
        {
            FillEllipse(new Vector2(cx, cy), new Vector2(rx, ry), color, segments);
        }

        public void FillEllipse(Vector2 center, Vector2 radii, Color color, int segments = 16)
        {
            var vertices = new Vector2[segments + 2];
            var step = MathHelper.TwoPi / segments;
            var theta = 0.0;

            for (int i = 0; i < segments; i++)
            {
                var angle = new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                vertices[i + 1] = angle * radii + center;
                theta += step;
            }

            var lastIndex = vertices.Length - 1;
            vertices[0] = center;
            vertices[lastIndex] = vertices[1];

            FillPolygon(vertices, color);
        }
        #endregion

        #region Circles
        public void DrawCircle(
            float cx, float cy,
            float radius,
            Color color,
            int segments = 16)
        {
            DrawEllipse(new Vector2(cx, cy), Vector2.One * radius, color, segments);
        }

        public void DrawCircle(Vector2 center, float radius, Color color, int segments = 16)
        {
            DrawEllipse(center, Vector2.One * radius, color, segments);
        }

        public void FillCircle(
            float cx, float cy,
            float radius,
            Color color,
            int segments = 16)
        {
            FillEllipse(new Vector2(cx, cy), Vector2.One * radius, color, segments);
        }

        public void FillCircle(Vector2 center, float radius, Color color, int segments = 16)
        {
            FillEllipse(center, Vector2.One * radius, color, segments);
        }
        #endregion
    }

    public enum PrimitiveType
    {
        Line,
        Triangle
    }
}
