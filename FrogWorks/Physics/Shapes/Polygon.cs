﻿using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public sealed class Polygon : Shape
    {
        Vector2[] _vertices, _transformed;
        Vector2 _origin, _scale;
        float _angle;

        public Vector2 this[int index] => _transformed[index];

        public int Count => _vertices.Length;

        public override Vector2 Center
        {
            get { return Position + _origin; }
            set { Position = value - _origin; }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    MarkAsDirty();
                    RecalculateVertices();
                }
            }
        }

        public float Angle
        {
            get { return _angle; }
            set
            {
                if (_angle != value)
                {
                    _angle = value;
                    MarkAsDirty();
                    RecalculateVertices();
                }
            }
        }

        public float AngleInDegrees
        {
            get { return MathHelper.ToDegrees(Angle); }
            set { Angle = MathHelper.ToRadians(value); }
        }

        public Polygon(Vector2[] vertices)
            : this(vertices.Min(), Vector2.One, 0f, vertices)
        {
        }

        public Polygon(float x, float y, Vector2[] vertices)
            : this(new Vector2(x, y), Vector2.One, 0f, vertices)
        {
        }

        public Polygon(Vector2 position, Vector2[] vertices)
            : this(position, Vector2.One, 0f, vertices)
        {
        }

        public Polygon(
            float x, float y, float xScale, float yScale, 
            float angle, Vector2[] vertices)
            : this(new Vector2(x, y), new Vector2(xScale, yScale), angle, vertices)
        {
        }

        public Polygon(Vector2 position, Vector2 scale, float angle, Vector2[] vertices)
            : base(position)
        {
            _scale = scale;
            _angle = angle;
            _vertices = vertices.ToOrigin().ToConvexHull();
            _origin = _vertices.Center();
            RecalculateVertices();
        }

        void RecalculateVertices()
        {
            _transformed = _vertices.Transform(Position, _origin, _scale, _angle, false);
        }

        protected override Rectangle RecalculateBounds()
        {
            var min = _transformed.Min();
            var max = _transformed.Max();

            return new Rectangle(min.ToPoint(), (max - min).ToPoint());
        }

        public override bool Contains(Vector2 point)
        {
            var inPoly = false;

            for (int i = 0; i < Count; i++)
            {
                var p1 = this[i];
                var p2 = this[(i + 1).Mod(Count)];
                var edge = p2 - p1;

                if ((p1.Y > point.Y) != (p2.Y > point.Y))
                {
                    var area = edge.X * (point.Y - p1.Y) / edge.Y + p1.X;
                    if (point.X < area) inPoly = !inPoly;
                }
            }

            return inPoly;
        }

        public override Vector2 GetClosestPoint(Vector2 point)
        {
            var minDistSq = float.PositiveInfinity;
            var closest = Vector2.Zero;

            for (int i = 0; i < Count; i++)
            {
                var p1 = this[i];
                var p2 = this[(i + 1).Mod(Count)];

                var next = PlotEX.GetClosestPointOnLine(p1, p2, point);
                var distSq = (point - next).LengthSquared();

                if (minDistSq > distSq)
                {
                    minDistSq = distSq;
                    closest = next;
                }
            }

            return closest;
        }

        public override void Draw(
            RendererBatch batch,
            Color strokeColor,
            Color? fillColor = null)
        {
            batch.DrawPrimitives(p =>
            {
                if (fillColor.HasValue)
                    p.FillPolygon(_transformed, fillColor.Value);

                p.DrawPolygon(_transformed, strokeColor);
                p.DrawDot(Center, strokeColor);
            });
        }

        public override Shape Clone()
        {
            return new Polygon(Position, _scale, _angle, _vertices);
        }

        protected override void OnTranslated()
        {
            RecalculateVertices();
        }

        internal override Vector2[] GetVertices()
        {
            return _transformed;
        }
    }
}
