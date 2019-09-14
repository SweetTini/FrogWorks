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

        public bool IsOnCeiling { get; private set; }

        public bool IsOnGround { get; private set; }

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
            MoveAndResolveCollisionHorizontally();
            MoveAndResolveCollisionVertically();
            DetectEdges();
        }

        private void MoveAndResolveCollisionHorizontally()
        {
            X += XVelocity;

            if (!IsCollidable || XVelocity == 0f) return;

            var direction = XVelocity.Sign();
            var offset = direction > 0f ? Right - 1 : Left;

            if (World.IsSolid(offset, Top, 1, Height))
            {
                var gridOffset = direction > 0f ? (World.CellWidth - Width).Mod(World.CellWidth) : 0f;
                var gridPosition = direction > 0f ? (X / World.CellWidth).Floor() : (X / World.CellWidth).Ceiling();
                X = World.Absolute.X + gridPosition * World.CellWidth + gridOffset;
                XVelocity = 0f;
            }
        }

        private void MoveAndResolveCollisionVertically()
        {
            Y += YVelocity;

            if (!IsCollidable || YVelocity == 0f) return;

            var direction = YVelocity.Sign();
            var offset = direction > 0f ? Bottom - 1 : Top;
            var isAboveJumpThru = false;

            if (direction > 0f)
                isAboveJumpThru = World.IsAboveJumpThru(Left, offset, Width, 1, XVelocity, YVelocity);

            if (World.IsSolid(Left, offset, Width, 1) || isAboveJumpThru)
            {
                var gridOffset = direction > 0f ? (World.CellHeight - Height).Mod(World.CellHeight) : 0f;
                var gridPosition = direction > 0f ? (Y / World.CellHeight).Floor() : (Y / World.CellHeight).Ceiling();
                Y = World.Absolute.Y + gridPosition * World.CellHeight + gridOffset;
                YVelocity = 0f;
            }
        }

        private void DetectEdges()
        {
            if (!IsCollidable) return;

            IsOnLeftWall = World.IsSolid(Left - 1, Top, 1, Height);
            IsOnRightWall = World.IsSolid(Right, Top, 1, Height);
            IsOnCeiling = World.IsSolid(Left, Top - 1, Width, 1);
            IsOnGround = World.IsSolid(Left, Bottom, Width, 1) || (YVelocity >= 0f 
                && World.IsJumpThru(Left, Bottom, Width, 1));
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
