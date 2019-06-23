using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using XnaPrimitiveType = Microsoft.Xna.Framework.Graphics.PrimitiveType;

namespace FrogWorks
{
    public class PrimitiveBatch : IDisposable
    {
        const int VertsPerLine = 2, VertsPerTriangle = 3;

        private GraphicsDevice _graphicsDevice;
        private BasicEffect _basicEffect;
        private VertexPositionColor[] _lineVertices, _triangleVertices;
        private int _lineBufferIndex, _triangleBufferIndex;
        private bool _hasBegun;

        public bool IsDisposed { get; private set; }

        public PrimitiveBatch(GraphicsDevice graphicsDevice, int bufferSize = 512)
        {
            _graphicsDevice = graphicsDevice;
            _basicEffect = new BasicEffect(_graphicsDevice);
            _lineVertices = new VertexPositionColor[bufferSize - bufferSize % VertsPerLine];
            _triangleVertices = new VertexPositionColor[bufferSize - bufferSize % VertsPerTriangle];
        }

        public void Begin(BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, Matrix? projectionMatrix = null, Matrix? viewMatrix = null)
        {
            ValidateAfterDraw();

            _graphicsDevice.BlendState = blendState ?? BlendState.AlphaBlend;
            _graphicsDevice.SamplerStates[0] = samplerState ?? SamplerState.PointClamp;
            _graphicsDevice.DepthStencilState = depthStencilState ?? DepthStencilState.None;

            var viewport = _graphicsDevice.Viewport;
            _basicEffect.Projection = projectionMatrix ?? Matrix.CreateOrthographicOffCenter(0f, viewport.Width, viewport.Height, 0f, -1000f, 1000f);
            _basicEffect.View = viewMatrix ?? Matrix.Identity;
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.CurrentTechnique.Passes[0].Apply();

            _hasBegun = true;
        }

        public void End()
        {
            ValidateBeforeDraw();
            FlushLines();
            FlushTriangles();

            _hasBegun = false;
        }

        #region Plotting
        public void AddVertex(Vector2 position, Color color, PrimitiveType type)
        {
            AddVertex(new Vector3(position, 0f), color, type);
        }

        public void AddVertex(float x, float y, Color color, PrimitiveType type)
        {
            AddVertex(new Vector3(x, y, 0f), color, type);
        }

        public void AddVertex(Vector3 position, Color color, PrimitiveType type)
        {
            ValidateBeforeDraw();

            if (type == PrimitiveType.Line)
            {
                if (_lineBufferIndex >= _lineVertices.Length)
                    FlushLines();

                _lineVertices[_lineBufferIndex++] = new VertexPositionColor(position, color);
            }

            if (type == PrimitiveType.Triangle)
            {
                if (_triangleBufferIndex >= _triangleVertices.Length)
                    FlushTriangles();

                _triangleVertices[_triangleBufferIndex++] = new VertexPositionColor(position, color);
            }
        }

        public void AddVertex(float x, float y, float z, Color color, PrimitiveType type)
        {
            AddVertex(new Vector3(x, y, z), color, type);
        }
        #endregion

        #region Dots & Lines
        public void DrawDot(Vector2 position, Color color)
        {
            AddVertex(position, color, PrimitiveType.Line);
            AddVertex(position + Vector2.UnitX, color, PrimitiveType.Line);
        }

        public void DrawDot(float x, float y, Color color)
        {
            DrawDot(new Vector2(x, y), color);
        }

        public void DrawLine(Vector2 p1, Vector2 p2, Color color)
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

        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color);
        }
        #endregion

        #region Triangles
        public void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
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

        public void DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            DrawTriangle(new Vector2(x1, y1), new Vector2(x2, y2), new Vector2(x3, y3), color);
        }

        public void FillTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
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

        public void FillTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            FillTriangle(new Vector2(x1, y1), new Vector2(x2, y2), new Vector2(x3, y3), color);
        }
        #endregion

        #region Polygons
        public void DrawPolygon(Vector2[] vertices, Color color)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                var j = (i + 1) % vertices.Length;
                DrawLine(vertices[i], vertices[j], color);
            }
        }

        public void DrawPolygon(float[] coords, Color color)
        {
            var count = coords.Length / 2;

            if (count > 0)
            {
                var vertices = new Vector2[count];

                for (int i = 0; i < count; i++)
                {
                    var j = i * 2;
                    vertices[i] = new Vector2(coords[j], coords[j + 1]);
                }

                DrawPolygon(vertices, color);
            }
        }

        public void FillPolygon(Vector2[] vertices, Color color)
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

        public void FillPolygon(float[] coords, Color color)
        {
            var count = coords.Length / 2;

            if (count > 0)
            {
                var vertices = new Vector2[count];

                for (int i = 0; i < count; i++)
                {
                    var j = i * 2;
                    vertices[i] = new Vector2(coords[j], coords[j + 1]);
                }

                FillPolygon(vertices, color);
            }
        }
        #endregion

        #region Rectangles
        public void DrawRectangle(Vector2 location, Vector2 size, Color color)
        {
            var vertices = new[]
            {
                location,
                new Vector2(location.X + size.X, location.Y),
                location + size,
                new  Vector2(location.X, location.Y + size.Y)
            };

            DrawPolygon(vertices, color);
        }

        public void DrawRectangle(float x, float y, float width, float height, Color color)
        {
            DrawRectangle(new Vector2(x, y), new Vector2(width, height), color);
        }

        public void FillRectangle(Vector2 location, Vector2 size, Color color)
        {
            var vertices = new[] 
            {
                location,
                new Vector2(location.X + size.X, location.Y),
                location + size,
                new  Vector2(location.X, location.Y + size.Y)
            };

            FillPolygon(vertices, color);
        }

        public void FillRectangle(float x, float y, float width, float height, Color color)
        {
            FillRectangle(new Vector2(x, y), new Vector2(width, height), color);
        }
        #endregion

        #region Ellipses
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

        public void DrawEllipse(float x, float y, float rx, float ry, Color color, int segments = 16)
        {
            DrawEllipse(new Vector2(x, y), new Vector2(rx, ry), color, segments);
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

        public void FillEllipse(float x, float y, float rx, float ry, Color color, int segments = 16)
        {
            FillEllipse(new Vector2(x, y), new Vector2(rx, ry), color, segments);
        }
        #endregion

        #region Circles
        public void DrawCircle(Vector2 center, float radius, Color color, int segments = 16)
        {
            DrawEllipse(center, radius * Vector2.One, color, segments);
        }

        public void DrawCircle(float x, float y, float radius, Color color, int segments = 16)
        {
            DrawEllipse(new Vector2(x, y), radius * Vector2.One, color, segments);
        }

        public void FillCircle(Vector2 center, float radius, Color color, int segments = 16)
        {
            FillEllipse(center, radius * Vector2.One, color, segments);
        }

        public void FillCircle(float x, float y, float radius, Color color, int segments = 16)
        {
            FillEllipse(new Vector2(x, y), radius * Vector2.One, color, segments);
        }
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing && !IsDisposed)
            {
                _basicEffect?.Dispose();
                IsDisposed = true;
            }
        }

        private void FlushLines()
        {
            ValidateBeforeDraw();

            if (_lineBufferIndex >= VertsPerLine)
            {
                var count = _lineBufferIndex / VertsPerLine;
                _graphicsDevice.DrawUserPrimitives(XnaPrimitiveType.LineList, _lineVertices, 0, count);
                _lineBufferIndex -= count * VertsPerLine;
            }
        }

        private void FlushTriangles()
        {
            ValidateBeforeDraw();

            if (_triangleBufferIndex >= VertsPerTriangle)
            {
                var count = _triangleBufferIndex / VertsPerTriangle;
                _graphicsDevice.DrawUserPrimitives(XnaPrimitiveType.TriangleList, _triangleVertices, 0, count);
                _triangleBufferIndex -= count * VertsPerTriangle;
            }
        }

        private void ValidateBeforeDraw()
        {
            ValidateDisposure();

            if (!_hasBegun)
            {
                var stackTrace = new StackTrace();
                var parentMethod = stackTrace.GetFrame(1).GetMethod();
                throw new Exception($"Begin must be called before {parentMethod.Name}.");
            }
        }

        private void ValidateAfterDraw()
        {
            ValidateDisposure();

            if (_hasBegun)
                throw new Exception("End must be called before Begin.");
        }

        private void ValidateDisposure()
        {
            if (IsDisposed)
                throw new Exception("Cannot draw with disposed primitive batch.");
        }
    }

    public enum PrimitiveType
    {
        Line,
        Triangle
    }
}
