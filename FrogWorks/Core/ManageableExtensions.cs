using System;
using System.Collections.Generic;
using System.Linq;

namespace FrogWorks
{
    public static class ManageableExtensions
    {
        public static T As<T>(this Scene scene)
            where T : Scene
        {
            return scene is T ? scene as T : null;
        }

        public static T As<T>(this Layer layer)
            where T : Layer
        {
            return layer is T ? layer as T : null;
        }

        public static T As<T>(this Collider collider)
            where T : Collider
        {
            return collider is T ? collider as T : null;
        }

        public static IEnumerable<T> QueryOfType<T>(this Entity entity)
            where T : Entity
        {
            var isQueryable = entity.Scene != null
                && entity.Collider != null
                && entity.Collider is ShapeCollider;

            if (isQueryable)
            {
                var collidables = entity.Scene.Colliders
                    .Query(entity.Collider as ShapeCollider);

                return collidables
                    .Select(c => c as ShapeCollider)
                    .Where(c => c.Parent is T)
                    .Select(c => c.Parent as T)
                    .ToList();
            }

            return new List<T>();
        }

        public static void QueryForEachType<T>(this Entity entity, Action<T> action)
            where T : Entity
        {
            (entity.QueryOfType<T>() as List<T>).ForEach(action);
        }
    }
}
