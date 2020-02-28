using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks.Demo
{
    public class ShapeCollisionScene : Scene
    {
        List<Shape> _shapes;
        List<Manifold> _results;
        Shape _shapeSelected;
        Vector2 _mouseOffset;
        bool _isDragging, _isColliding;

        public ShapeCollisionScene()
            : base()
        {
            _shapes = new List<Shape>();
            _results = new List<Manifold>();
        }

        protected override void Begin()
        {
            _shapes.Add(new Box(10, 10, 40, 24));
            _shapes.Add(new Box(60, 40, 48, 60));
            _shapes.Add(new Circle(120, 24, 16));
            _shapes.Add(new Circle(80, 108, 40));
        }

        protected override void BeforeUpdate(float deltaTime)
        {
            _results.Clear();

            var mouse = Input.Mouse.Position;

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
                        _results.Add(hit);
                    }
                }
            }
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            batch.Configure(camera: Camera);
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

                        foreach (var result in _results)
                        {
                            shape.Position = lastPos + result.Translation;
                            shape.Draw(batch, Color.Lime);
                        }

                        shape.Position = lastPos;
                    }
                }
                
                shape.Draw(batch, color);
            }

            batch.End();
        }
    }
}
