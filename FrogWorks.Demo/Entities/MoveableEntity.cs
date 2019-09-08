using Microsoft.Xna.Framework;
using System;

namespace FrogWorks.Demo.Entities
{
    public abstract class MoveableEntity : Entity
    {
        protected World World { get; private set; }

        protected BitFlagMapCollider WorldCollider => World.Collider.As<BitFlagMapCollider>();

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

        public bool IsOnGround { get; private set; }

        public bool IsOnCeiling { get; private set; }

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
                var edge = new RectangleF(Right - 1, Top, 1, Height);

                if (WorldCollider.Collide(edge, BitFlag.FlagA))
                {
                    var offset = (WorldCollider.CellWidth - Width).Mod(WorldCollider.CellWidth);
                    X = WorldCollider.AbsoluteX + (X / WorldCollider.CellWidth).Floor()
                        * WorldCollider.CellWidth + offset;
                    XVelocity = 0f;
                }
            }
            else if (XVelocity < 0)
            {
                var edge = new RectangleF(Left, Top, 1, Height);

                if (WorldCollider.Collide(edge, BitFlag.FlagA))
                {
                    X = WorldCollider.AbsoluteX + (X / WorldCollider.CellWidth).Ceiling()
                        * WorldCollider.CellWidth;
                    XVelocity = 0f;
                }
            }

            IsOnRightWall = WorldCollider.Collide(new RectangleF(Right, Top, 1, Height), BitFlag.FlagA);
            IsOnLeftWall = WorldCollider.Collide(new RectangleF(Left - 1, Top, 1, Height), BitFlag.FlagA);
        }

        private void CheckVerticalCollision()
        {
            if (!IsCollidable) return;

            if (YVelocity > 0)
            {
                var edge = new RectangleF(Left, Bottom - 1, Width, 1);

                if (WorldCollider.Collide(edge, BitFlag.FlagA))
                {
                    var offset = (WorldCollider.CellHeight - Height).Mod(WorldCollider.CellHeight);
                    Y = WorldCollider.AbsoluteY + (Y / WorldCollider.CellHeight).Floor()
                        * WorldCollider.CellHeight + offset;
                    YVelocity = 0f;
                }
            }
            else if (YVelocity < 0)
            {
                var edge = new RectangleF(Left, Top, Width, 1);

                if (WorldCollider.Collide(edge, BitFlag.FlagA))
                {
                    Y = WorldCollider.AbsoluteY + (Y / WorldCollider.CellHeight).Ceiling()
                        * WorldCollider.CellHeight;
                    YVelocity = 0f;
                }
            }

            IsOnGround = WorldCollider.Collide(new RectangleF(Left, Bottom, Width, 1), BitFlag.FlagA);
            IsOnCeiling = WorldCollider.Collide(new RectangleF(Left, Top - 1, Width, 1), BitFlag.FlagA);
        }

        public override string ToString()
        {
            return $"POS:{X.Round()},{Y.Round()}" + Environment.NewLine
                + $"VEL:{(XVelocity * 100f).Round()},{(YVelocity * 100f).Round()}" + Environment.NewLine
                + $"GR:{Convert.ToInt32(IsOnGround)},CL:{Convert.ToInt32(IsOnCeiling)}" + Environment.NewLine
                + $"LW:{Convert.ToInt32(IsOnLeftWall)},RW:{Convert.ToInt32(IsOnRightWall)}";
        }
    }
}
