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

        public IEnumerable<Raycast> RaycastEx(Vector2 start, Vector2 end)
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

        public IEnumerable<Collider> Overlaps(Entity entity)
        {
            var aabb = new AABB(entity.Min, entity.Max);
            Func<Collider, bool> onCollide = c => c.Overlaps(entity);
            return _broadphaseTree.Query(aabb, onCollide);
        }

        public IEnumerable<CollisionResult> OverlapsEx(Shape shape)
        {
            var results = new List<CollisionResult>();
            var min = shape.Bounds.Location.ToVector2();
            var max = min + shape.Bounds.Size.ToVector2();
            var aabb = new AABB(min, max);

            Func<Collider, bool> onCollide = c =>
            {
                CollisionResult result;

                if (c.Overlaps(shape, out result))
                {
                    results.Add(result);
                    return true;
                }

                return false;
            };

            _broadphaseTree.Query(aabb, onCollide);

            return results;
        }

        public IEnumerable<CollisionResult> OverlapsEx(Entity entity)
        {
            var results = new List<CollisionResult>();
            var aabb = new AABB(entity.Min, entity.Max);

            Func<Collider, bool> onCollide = c =>
            {
                CollisionResult result;

                if (c.Overlaps(entity, out result))
                {
                    results.Add(result);
                    return true;
                }

                return false;
            };

            _broadphaseTree.Query(aabb, onCollide);

            return results;
        }
        #endregion
    }

    public static class WorldEX
    {
        internal static IEnumerable<Entity> ToEntityList(
            this IEnumerable<Collider> colliders)
        {
            return colliders
                .Where(c => c.Entity != null)
                .Select(c => c.Entity)
                .Distinct()
                .ToList();
        }

        internal static IEnumerable<EntityHitPair<Raycast>> ToEntityList(
            this IEnumerable<Raycast> hits)
        {
            return hits
                .Where(h => h.Collider.Entity != null)
                .GroupBy(h => h.Collider.Entity)
                .Select(h => new EntityHitPair<Raycast>(
                    h.Key, h.ToList()))
                .ToList();
        }

        internal static IEnumerable<EntityHitPair<CollisionResult>> ToEntityList(
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
