using Microsoft.Xna.Framework;
using System;

namespace FrogWorks.Demo.Entities
{
    public abstract class MoveableEntity : Entity
    {
        protected World World { get; private set; }

        public Vector2 Velocity { get; set; }

        public float XVelocity
        {
            get { return Velocity.X; }
            set { Velocity = new Vector2(value, Velocity.Y); }
        }

        public float YVelocity
        {
            get { return Velocity.Y; }
            set { Velocity = new Vector2(Velocity.X, value); }
        }

        public bool IsOnLeftWall { get; private set; }

        public bool IsOnRightWall { get; private set; }

        public bool IsOnWall => IsOnLeftWall || IsOnRightWall;

        public bool IsOnCeiling { get; private set; }

        public bool IsOnGround { get; private set; }

        public bool IsOnJumpThru { get; private set; }

        public bool IsOnPlatform => IsOnGround || IsOnJumpThru;

        protected MoveableEntity(World world, int width, int height)
            : base()
        {
            World = world;
            Collider = new RectangleCollider(width, height);
        }

        protected sealed override void BeforeUpdate(float deltaTime)
        {
            BeforeUpdatePhysics();
            UpdatePhysics();
            AfterUpdatePhysics();
        }

        protected virtual void BeforeUpdatePhysics() { }

        protected virtual void AfterUpdatePhysics() { }

        private void UpdatePhysics()
        {
            X += XVelocity;
            CheckHorizontalCollision();
            Y += YVelocity;
            CheckVerticalCollision();
        }

        private void CheckHorizontalCollision()
        {
            if (!IsCollidable) return;

            if (XVelocity > 0)
            {
                if (World.IsSolid(Right - 1, Top, 1, Height))
                {
                    var offset = (World.CellWidth - Width).Mod(World.CellWidth);
                    X = World.Absolute.X + (X / World.CellWidth).Floor() * World.CellWidth + offset;
                    XVelocity = 0f;
                }
            }
            else if (XVelocity < 0)
            {
                if (World.IsSolid(Left, Top, 1, Height))
                {
                    X = World.Absolute.X + (X / World.CellWidth).Ceiling() * World.CellWidth;
                    XVelocity = 0f;
                }
            }

            IsOnRightWall = World.IsSolid(Right, Top, 1, Height);
            IsOnLeftWall = World.IsSolid(Left - 1, Top, 1, Height);
        }

        private void CheckVerticalCollision()
        {
            if (!IsCollidable) return;

            if (YVelocity > 0)
            {
                var isOnPlaform = World.IsSolid(Left, Bottom - 1, Width, 1) 
                    || World.IsAboveJumpThru(Left, Bottom - 1, Width, 1, XVelocity, YVelocity);

                if (isOnPlaform)
                {
                    var offset = (World.CellHeight - Height).Mod(World.CellHeight);
                    Y = World.Absolute.Y + (Y / World.CellHeight).Floor() * World.CellHeight + offset;
                    YVelocity = 0f;
                }
            }
            else if (YVelocity < 0)
            {
                if (World.IsSolid(Left, Top, Width, 1))
                {
                    Y = World.Absolute.Y + (Y / World.CellHeight).Ceiling() * World.CellHeight;
                    YVelocity = 0f;
                }
            }

            IsOnGround = World.IsSolid(Left, Bottom, Width, 1);
            IsOnJumpThru = World.IsJumpThru(Left, Bottom, Width, 1);
            IsOnCeiling = World.IsSolid(Left, Top - 1, Width, 1);
        }

        public override string ToString()
        {
            return $"POS:{X.Round()},{Y.Round()}" + Environment.NewLine
                + $"VEL:{(XVelocity * 100f).Round()},{(YVelocity * 100f).Round()}" + Environment.NewLine
                + $"GR:{Convert.ToInt32(IsOnGround)},CL:{Convert.ToInt32(IsOnCeiling)}," 
                + $"JT:{Convert.ToInt32(IsOnJumpThru)}" + Environment.NewLine
                + $"LW:{Convert.ToInt32(IsOnLeftWall)},RW:{Convert.ToInt32(IsOnRightWall)}";
        }
    }
}
