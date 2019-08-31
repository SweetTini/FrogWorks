﻿using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public sealed class PolygonCollider : ShapeCollider
    {
        private Vector2[] _vertices;
        private Vector2 _size, _origin, _scale;
        private float _angle;

        protected internal override Shape Shape
            => new Polygon(_vertices.Transform(AbsolutePosition, _origin, _scale, _angle));

        public sealed override Vector2 Size
        {
            get { return (_size * _scale).Round(); }
            set { Scale = _size.Divide(value); }
        }

        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                if (value == _origin) return;
                _origin = value;
                OnInternalTransformed();
            }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                if (value == _scale) return;
                _scale = value;
                OnInternalTransformed();
            }
        }

        public float Angle
        {
            get { return _angle; }
            set
            {
                if (value == _angle) return;
                _angle = value;
                OnInternalTransformed();
            }
        }

        public float AngleInDegrees
        {
            get { return MathHelper.ToDegrees(Angle); }
            set { Angle = MathHelper.ToRadians(value); }
        }

        public PolygonCollider(Vector2[] vertices, float x = 0f, float y = 0f)
            : base(new Vector2(x, y))
        {
            _vertices = vertices;
            _size = _vertices.Max() - _vertices.Min();
            _origin = _size / 2f;
            _scale = Vector2.One;
        }

        public sealed override Collider Clone() => new PolygonCollider(_vertices, X, Y)
        {
            Origin = Origin,
            Scale = Scale,
            Angle = Angle
        };
    }
}
