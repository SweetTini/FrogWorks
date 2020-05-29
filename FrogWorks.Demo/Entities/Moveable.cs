using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks.Demo
{
    public class Moveable : Entity
    {
        public Playfield Field { get; private set; }

        public Vector2 Velocity { get; set; }

        public float VX
        {
            get { return Velocity.X; }
            set { Velocity = new Vector2(value, Velocity.Y); }
        }

        public float VY
        {
            get { return Velocity.Y; }
            set { Velocity = new Vector2(Velocity.X, value); }
        }

        public float InputXAxis => Input.Keyboard.GetAxis(Keys.LeftArrow, Keys.RightArrow);

        public float InputYAxis => Input.Keyboard.GetAxis(Keys.UpArrow, Keys.DownArrow);

        public Moveable(Playfield field, float x, float y)
            : base()
        {
            Field = field;
            Position = new Vector2(x, y);
            Collider = new BoxCollider(0, 0, 24, 24);
        }

        protected override void BeforeUpdate(float deltaTime)
        {
            Velocity += new Vector2(InputXAxis, InputYAxis) * .2f;
            UpdatePhysics();
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            Collider.Draw(batch, Color.Red);
        }

        public void UpdatePhysics()
        {
            Resolve(false);
            Resolve(true);
        }

        public void Resolve(bool vertical)
        {
            var offset = vertical ? VY : VX;
            if (Field.Check(Collider, offset, vertical, out var amt))
                Position -= amt;
            var unit = vertical ? Vector2.UnitY : Vector2.UnitX;
            Position += Velocity * unit;
        }
    }
}
