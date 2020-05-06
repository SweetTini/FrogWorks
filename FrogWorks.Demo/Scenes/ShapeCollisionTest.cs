using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FrogWorks.Demo.Scenes
{
    public class ShapeCollisionTest : Scene
    {
        List<Shape> _shapes;
        List<Raycast> _raycasts;
        List<Manifold> _manifolds;
        Shape _shapeSelected;
        Vector2 _mouseOffset, 
            _rayStart, 
            _rayEnd;
        bool _isDragging, 
            _isColliding, 
            _isRaycasting,
            _isRayHit;

        public ShapeCollisionTest()
            : base()
        {
            _shapes = new List<Shape>();
            _raycasts = new List<Raycast>();
            _manifolds = new List<Manifold>();
        }

        protected override void Begin()
        {
            var polyVerts = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(48, 0),
                new Vector2(24, 40)
            };

            _shapes.Add(new Box(82, 18, 40, 24));
            _shapes.Add(new Box(20, 60, 48, 60));
            _shapes.Add(new Circle(112, 72, 16));
            _shapes.Add(new Circle(62, 126, 40));
            _shapes.Add(new Polygon(170, 36, polyVerts));
            _shapes.Add(new Polygon(168, 126, 1.5f, 1.5f, .5f, polyVerts));
        }

        protected override void BeforeUpdate(float deltaTime)
        {
            if (_raycasts.Any()) _raycasts.Clear();
            if (_manifolds.Any()) _manifolds.Clear();

            var mouse = Input.Mouse.Position;

            if (!_isRaycasting)
            {
                if (Input.Mouse.IsClicked(MouseButton.Right))
                {
                    _isRaycasting = true;
                    _rayStart = mouse;
                    _rayEnd = _rayStart;
                    return;
                }

                if (!_isDragging)
                {
                    foreach (var shape in _shapes)
                    {
                        if (Input.Mouse.IsClicked(MouseButton.Left) && shape.Contains(mouse))
                        {
                            _shapeSelected = shape;
                            _mouseOffset = mouse - shape.Position;
                            _isDragging = true;
                            break;
                        }
                    }
                }
                else
                {
                    if (!Input.Mouse.IsDown(MouseButton.Left))
                    {
                        _shapeSelected = null;
                        _mouseOffset = Vector2.Zero;
                        _isDragging = false;
                        _isColliding = false;
                        return;
                    }

                    _shapeSelected.Position = mouse - _mouseOffset;
                    _isColliding = false;

                    foreach (var shape in _shapes)
                    {
                        if (shape == _shapeSelected)
                            continue;

                        Manifold hit;
                        if (_shapeSelected.Overlaps(shape, out hit))
                        {
                            _isColliding = true;
                            _manifolds.Add(hit);
                        }
                    }
                }
            }
            else
            {
                if (!Input.Mouse.IsDown(MouseButton.Right))
                {
                    _rayStart = Vector2.Zero;
                    _rayEnd = _rayStart;
                    _isRaycasting = false;
                    _isRayHit = false;
                    return;
                }

                _rayEnd = mouse;
                _isRayHit = false;

                foreach (var shape in _shapes)
                {
                    Raycast hit;
                    if (shape.Raycast(_rayStart, _rayEnd, out hit))
                    {
                        _isRayHit = true;
                        _raycasts.Add(hit);
                    }
                }
            }
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            batch.Configure(transformMatrix: Camera.Matrix);
            batch.Begin();

            foreach (var shape in _shapes)
            {
                var color = Color.Red;

                if (shape == _shapeSelected)
                {
                    if (_isColliding) 
                        color = Color.Yellow;
                    else if (_isDragging) 
                        color = Color.Magenta;

                    if (_isColliding)
                    {
                        var lastPos = shape.Position;

                        foreach (var result in _manifolds)
                        {
                            shape.Position = lastPos + result.Translation;
                            shape.Draw(batch, Color.Blue);
                        }

                        shape.Position = lastPos;
                    }
                }
                
                shape.Draw(batch, color);
            }

            if (_isRaycasting)
            {
                batch.DrawPrimitives(p =>
                {
                    var color = _isRayHit
                        ? Color.Cyan
                        : Color.Blue;

                    p.DrawLine(_rayStart, _rayEnd, color);

                    if (_isRayHit)
                    {
                        foreach (var result in _raycasts)
                        {
                            p.DrawDot(result.Contact, Color.Yellow);
                            p.DrawCircle(result.Contact, 3f, Color.Yellow);
                        }
                    }
                });
            }

            batch.End();
        }
    }
}
