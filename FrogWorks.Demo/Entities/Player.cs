using Microsoft.Xna.Framework;
using System;

namespace FrogWorks.Demo.Entities
{
    public class Player : MoveableEntity
    {
        private const float MaxSpeed = 2f, MaxFallSpeed = 6f;
        private const float Acceleration = .2f, Deceleration = .15f, Gravity = .15f;
        private const float JumpStrength = 5f, JumpRelease = 2f;

        public bool IsJumping { get; private set; }

        public Player(World world) 
            : base(world, 24, 40) { }

        protected override void BeforeUpdatePhysics()
        {
            var movingDirection = UserInput.LeftRightAxis.CurrentValue;

            if (movingDirection != 0f)
            {
                XVelocity += movingDirection * Acceleration;
                XVelocity = XVelocity.Clamp(-MaxSpeed, MaxSpeed);
            }
            else
            {
                if (XVelocity != 0f)
                {
                    var direction = Math.Sign(XVelocity);

                    XVelocity -= Deceleration * direction;
                    XVelocity = direction > 0
                        ? Math.Max(0f, XVelocity)
                        : Math.Min(0f, XVelocity);
                }
            }

            if (IsOnLeftWall && XVelocity < 0f || IsOnRightWall && XVelocity > 0f)
                XVelocity = 0f;

            if (IsJumping)
            {
                if (!UserInput.JumpButton.IsDown && YVelocity < -JumpRelease)
                {
                    YVelocity = -JumpRelease;
                    IsJumping = false;
                }
                else if (YVelocity >= -JumpRelease)
                {
                    IsJumping = false;
                }
            }

            if (IsOnGround)
            {
                if (YVelocity > 0f) YVelocity = 0f;

                if (UserInput.JumpButton.IsPressed)
                {
                    YVelocity = -JumpStrength;
                    IsJumping = true;
                }
            }
            else
            {
                if (YVelocity < MaxFallSpeed)
                {
                    YVelocity += Gravity;
                    YVelocity = Math.Min(YVelocity, MaxFallSpeed);
                }
            }
        }

        protected override void BeforeDraw(RendererBatch batch)
            => Collider.Draw(batch, Color.Yellow);
    }
}
