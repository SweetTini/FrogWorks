using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public sealed class World
    {
        DynamicAABBTree _broadphaseTree;

        internal World()
        {
            _broadphaseTree = new DynamicAABBTree(8f);
        }

        internal void AddCollider(Collider collider)
        {
            _broadphaseTree.Add(collider);
        }

        internal void UpdateCollider(Collider collider)
        {
            _broadphaseTree.Update(collider);
        }

        internal void RemoveCollider(Collider collider)
        {
            _broadphaseTree.Remove(collider);
        }

        internal void DrawBroadphase(RendererBatch batch, Color treeColor, Color leafColor)
        {
            _broadphaseTree.Draw(batch, treeColor, leafColor);
        }

        internal void Reset()
        {
            _broadphaseTree.Clear();
        }

        #region Queries
        public IEnumerable<Collider> Contains(Vector2 point)
        {
            var aabb = new AABB(point, point + Vector2.One);
            Func<Collider, bool> onCollide = c => c.Contains(point);
            return _broadphaseTree.Query(aabb, onCollide);
        }

        public IEnumerable<Collider> Raycast(Vector2 start, Vector2 end)
        {
            var min = Vector2.Min(start, end);
            var max = Vector2.Max(start, end);
            var aabb = new AABB(min, max);

            Func<Collider, bool> onCollide = c => c.Raycast(start, end, out _);

            return _broadphaseTree.Query(aabb, onCollide);
        }

        public IEnumerable<Raycast> RaycastWithHits(Vector2 start, Vector2 end)
        {
            var results = new List<Raycast>();
            var min = Vector2.Min(start, end);
            var max = Vector2.Max(start, end);
            var aabb = new AABB(min, max);

            Func<Collider, bool> onCollide = c =>
            {
                Raycast hit;

                if (c.Raycast(start, end, out hit))
                {
                    hit.Collider = c;
                    results.Add(hit);
                    return true;
                }
                
                return false;
            };

            _broadphaseTree.Query(aabb, onCollide);

            return results;
        }

        public IEnumerable<Collider> Overlaps(Shape shape)
        {
            var min = shape.Bounds.Location.ToVector2();
            var max = min + shape.Bounds.Size.ToVector2();
            var aabb = new AABB(min, max);

            Func<Collider, bool> onCollide = c => c.Overlaps(shape);

            return _broadphaseTree.Query(aabb, onCollide);
        }

        public IEnumerable<Manifold> OverlapsWithHits(Shape shape)
        {
            var results = new List<Manifold>();
            var min = shape.Bounds.Location.ToVector2();
            var max = min + shape.Bounds.Size.ToVector2();
            var aabb = new AABB(min, max);

            Func<Collider, bool> onCollide = c =>
            {
                Manifold hit;
                
                if (c.Overlaps(shape, out hit))
                {
                    hit.Collider = c;
                    results.Add(hit);
                    return true;
                }

                return false;
            };

            _broadphaseTree.Query(aabb, onCollide);

            return results;
        }
        #endregion
    }
}
