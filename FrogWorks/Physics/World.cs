using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

        internal void Reset()
        {
            _broadphaseTree.Clear();
        }

        public void Draw(RendererBatch batch, Color treeColor, Color leafColor)
        {
            _broadphaseTree.Draw(batch, treeColor, leafColor);
        }

        #region Broadphase Queries
        public IEnumerable<Collider> Query(Vector2 point)
        {
            return _broadphaseTree.Query(new AABB(point, point + Vector2.UnitX));
        }

        public IEnumerable<Collider> Query(Vector2 start, Vector2 end)
        {
            var min = Vector2.Min(start, end);
            var max = Vector2.Max(start, end);

            return _broadphaseTree.Query(new AABB(min, max));
        }

        public IEnumerable<Collider> Query(Vector2 origin, Vector2 normal, float distance)
        {
            return Query(origin, origin + normal * distance);
        }

        public IEnumerable<Collider> Query(Shape shape)
        {
            if (shape != null)
            {
                var min = shape.Bounds.Location.ToVector2();
                var max = min + shape.Bounds.Size.ToVector2();

                return _broadphaseTree.Query(new AABB(min, max));
            }

            return new List<Collider>();
        }

        public IEnumerable<Collider> Query(Shape shape, Vector2 offset)
        {
            if (shape != null)
            {
                var lastPosition = shape.Position;
                var bounds = shape.Bounds;
                shape.Position += offset;
                var nextBounds = shape.Bounds;
                shape.Position = lastPosition;

                var union = bounds.Union(nextBounds);
                var min = union.Location.ToVector2();
                var max = min + union.Size.ToVector2();

                return _broadphaseTree.Query(new AABB(min, max));
            }

            return new List<Collider>();
        }

        public IEnumerable<Collider> Query(Collider collider)
        {
            return collider != null
                ? _broadphaseTree.Query(new AABB(collider.Min, collider.Max))
                : new List<Collider>();
        }

        public IEnumerable<Collider> Query(Collider collider, Vector2 offset)
        {
            if (collider != null)
            {
                var lastPosition = collider.AbsolutePosition;
                var bounds = collider.Bounds;
                collider.AbsolutePosition += offset;
                var nextBounds = collider.Bounds;
                collider.AbsolutePosition = lastPosition;

                var union = bounds.Union(nextBounds);
                var min = union.Location.ToVector2();
                var max = min + union.Size.ToVector2();

                return _broadphaseTree.Query(new AABB(min, max));
            }

            return new List<Collider>();
        }

        public IEnumerable<Collider> Query(Entity entity)
        {
            return Query(entity?.Collider);
        }

        public IEnumerable<Collider> Query(Entity entity, Vector2 offset)
        {
            return Query(entity?.Collider, offset);
        }
        #endregion

        #region Narrowphase Queries
        public IEnumerable<Collider> Contains(Vector2 point)
        {
            return _broadphaseTree.Query(
                new AABB(point, point + Vector2.One),
                c => c.Contains(point));
        }

        public IEnumerable<Collider> Raycast(Vector2 start, Vector2 end)
        {
            var min = Vector2.Min(start, end);
            var max = Vector2.Max(start, end);

            return _broadphaseTree.Query(
                new AABB(min, max),
                c => c.Raycast(start, end, out _));
        }

        public IEnumerable<Collider> Raycast(Vector2 origin, Vector2 normal, float distance)
        {
            return Raycast(origin, origin + normal * distance);
        }

        public IEnumerable<Collider> Overlaps(Shape shape)
        {
            if (shape != null)
            {
                var min = shape.Bounds.Location.ToVector2();
                var max = min + shape.Bounds.Size.ToVector2();

                return _broadphaseTree.Query(
                    new AABB(min, max),
                    c => c.Overlaps(shape));
            }

            return new List<Collider>();
        }

        public IEnumerable<Collider> Overlaps(Collider collider)
        {
            return collider != null
                ? _broadphaseTree.Query(
                    new AABB(collider.Min, collider.Max),
                    c => c.Overlaps(collider))
                : new List<Collider>();
        }

        public IEnumerable<Collider> Overlaps(Entity entity)
        {
            return Overlaps(entity?.Collider);
        }

        public IEnumerable<Raycast> RaycastWithHits(Vector2 start, Vector2 end)
        {
            var hits = new List<Raycast>();
            var min = Vector2.Min(start, end);
            var max = Vector2.Max(start, end);

            _broadphaseTree.Query(new AABB(min, max), c =>
            {
                var gotHit = c.Raycast(start, end, out var hit);
                if (gotHit) hits.Add(hit);
                return gotHit;
            });

            return hits;
        }

        public IEnumerable<Raycast> RaycastWithHits(Vector2 origin, Vector2 normal, float distance)
        {
            return RaycastWithHits(origin, origin + normal * distance);
        }

        public IEnumerable<CollisionResult> OverlapWithHits(Shape shape)
        {
            var hits = new List<CollisionResult>();

            if (shape != null)
            {
                var min = shape.Bounds.Location.ToVector2();
                var max = min + shape.Bounds.Size.ToVector2();

                _broadphaseTree.Query(new AABB(min, max), c =>
                {
                    var gotHit = c.Overlaps(shape, out var hit);
                    if (gotHit) hits.Add(hit);
                    return gotHit;
                });
            }

            return hits;
        }

        public IEnumerable<CollisionResult> OverlapWithHits(Collider collider)
        {
            var hits = new List<CollisionResult>();

            if (collider != null)
            {
                _broadphaseTree.Query(new AABB(collider.Min, collider.Max), c =>
                {
                    var gotHit = c.Overlaps(collider, out var hit);
                    if (gotHit) hits.Add(hit);
                    return gotHit;
                });
            }

            return hits;
        }

        public IEnumerable<CollisionResult> OverlapWithHits(Entity entity)
        {
            return OverlapWithHits(entity?.Collider);
        }
        #endregion
    }

    public static class WorldEX
    {
        public static IEnumerable<Entity> ToEntityList(
            this IEnumerable<Collider> colliders)
        {
            return colliders
                .Where(c => c.Entity != null)
                .Select(c => c.Entity)
                .Distinct()
                .ToList();
        }

        public static IEnumerable<EntityHitPair<Raycast>> ToEntityList(
            this IEnumerable<Raycast> hits)
        {
            return hits
                .Where(h => h.Collider.Entity != null)
                .GroupBy(h => h.Collider.Entity)
                .Select(h => new EntityHitPair<Raycast>(
                    h.Key, h.ToList()))
                .ToList();
        }

        public static IEnumerable<EntityHitPair<CollisionResult>> ToEntityList(
            this IEnumerable<CollisionResult> results)
        {
            return results
                .Where(h => h.Collider.Entity != null)
                .GroupBy(h => h.Collider.Entity)
                .Select(h => new EntityHitPair<CollisionResult>(
                    h.Key, h.ToList()))
                .ToList();
        }

        public static IEnumerable<Entity> ToEntityList(
            this IEnumerable<EntityHitPair<Raycast>> pairs)
        {
            return pairs.Select(p => p.Entity).ToList();
        }

        public static IEnumerable<Entity> ToEntityList(
            this IEnumerable<EntityHitPair<CollisionResult>> pairs)
        {
            return pairs.Select(p => p.Entity).ToList();
        }

        public static IEnumerable<EntityHitPair<Raycast>> WithType<T>(
            this IEnumerable<EntityHitPair<Raycast>> pairs)
            where T : Component
        {
            return pairs
                .Where(p => p.Entity.Components
                    .AnyOfType<T>())
                .ToList();
        }

        public static IEnumerable<EntityHitPair<CollisionResult>> WithType<T>(
            this IEnumerable<EntityHitPair<CollisionResult>> pairs)
            where T : Component
        {
            return pairs
                .Where(p => p.Entity.Components
                    .AnyOfType<T>())
                .ToList();
        }

        public static IEnumerable<EntityHitPair<T, Raycast>> OfType<T>(
            this IEnumerable<EntityHitPair<Raycast>> pairs)
            where T : Entity
        {
            return pairs
                .Where(p => p.Entity is T)
                .Select(p => new EntityHitPair<T, Raycast>(
                    p.Entity as T, p.Results))
                .ToList();
        }

        public static IEnumerable<EntityHitPair<T, CollisionResult>> OfType<T>(
            this IEnumerable<EntityHitPair<CollisionResult>> pairs)
            where T : Entity
        {
            return pairs
                .Where(p => p.Entity is T)
                .Select(p => new EntityHitPair<T, CollisionResult>(
                    p.Entity as T, p.Results))
                .ToList();
        }

        public static EntityHitPair<T, Raycast> FirstOfType<T>(
            this IEnumerable<EntityHitPair<Raycast>> pairs)
            where T : Entity
        {
            return pairs
                .Where(p => p.Entity is T)
                .Select(p => new EntityHitPair<T, Raycast>(
                    p.Entity as T, p.Results))
                .FirstOrDefault();
        }

        public static EntityHitPair<T, CollisionResult> FirstOfType<T>(
            this IEnumerable<EntityHitPair<CollisionResult>> pairs)
            where T : Entity
        {
            return pairs
                .Where(p => p.Entity is T)
                .Select(p => new EntityHitPair<T, CollisionResult>(
                    p.Entity as T, p.Results))
                .FirstOrDefault();
        }

        public static EntityHitPair<T, Raycast> LastOfType<T>(
            this IEnumerable<EntityHitPair<Raycast>> pairs)
            where T : Entity
        {
            return pairs
                .Where(p => p.Entity is T)
                .Select(p => new EntityHitPair<T, Raycast>(
                    p.Entity as T, p.Results))
                .LastOrDefault();
        }

        public static EntityHitPair<T, CollisionResult> LastOfType<T>(
            this IEnumerable<EntityHitPair<CollisionResult>> pairs)
            where T : Entity
        {
            return pairs
                .Where(p => p.Entity is T)
                .Select(p => new EntityHitPair<T, CollisionResult>(
                    p.Entity as T, p.Results))
                .LastOrDefault();
        }

        public static bool AnyOfType<T>(
            this IEnumerable<EntityHitPair<Raycast>> pairs)
            where T : Entity
        {
            return pairs.Any(p => p.Entity is T);
        }

        public static bool AnyOfType<T>(
            this IEnumerable<EntityHitPair<CollisionResult>> pairs)
            where T : Entity
        {
            return pairs.Any(p => p.Entity is T);
        }

        public static int CountType<T>(
            this IEnumerable<EntityHitPair<Raycast>> pairs)
            where T : Entity
        {
            return pairs.Count(p => p.Entity is T);
        }

        public static int CountType<T>(
            this IEnumerable<EntityHitPair<CollisionResult>> pairs)
            where T : Entity
        {
            return pairs.Count(p => p.Entity is T);
        }

        public static void ForEachOfType<T>(
            this IEnumerable<EntityHitPair<Raycast>> pairs,
            Action<T, IEnumerable<Raycast>> action)
            where T : Entity
        {
            pairs.Where(p => p.Entity is T)
                .Select(p => new EntityHitPair<T, Raycast>(
                    p.Entity as T, p.Results))
                .ToList()
                .ForEach(p => action(
                    p.Entity, p.Results));
        }

        public static void ForEachOfType<T>(
            this IEnumerable<EntityHitPair<CollisionResult>> pairs,
            Action<T, IEnumerable<CollisionResult>> action)
            where T : Entity
        {
            pairs.Where(p => p.Entity is T)
                .Select(p => new EntityHitPair<T, CollisionResult>(
                    p.Entity as T, p.Results))
                .ToList()
                .ForEach(p => action(
                    p.Entity, p.Results));
        }

        public static IEnumerable<EntityHitPair<Raycast>> OnLayer(
            this IEnumerable<EntityHitPair<Raycast>> pairs, Layer layer)
        {
            return pairs
                .Where(p => p.Entity.Layer == layer)
                .ToList();
        }

        public static IEnumerable<EntityHitPair<CollisionResult>> OnLayer(
            this IEnumerable<EntityHitPair<CollisionResult>> pairs, Layer layer)
        {
            return pairs
                .Where(p => p.Entity.Layer == layer)
                .ToList();
        }
    }

    public class EntityHitPair<R>
    {
        public Entity Entity { get; private set; }

        public IEnumerable<R> Results { get; private set; }

        internal EntityHitPair(Entity entity, IEnumerable<R> results)
            : base()
        {
            Entity = entity;
            Results = results;
        }
    }

    public class EntityHitPair<T, R>
        where T : Entity
    {
        public T Entity { get; private set; }

        public IEnumerable<R> Results { get; private set; }

        internal EntityHitPair(T entity, IEnumerable<R> results)
            : base()
        {
            Entity = entity;
            Results = results;
        }
    }
}
