using System;
using System.Collections.Generic;
using System.Linq;

namespace FrogWorks
{
    public static class ManagerEX
    {
        #region Entities
        public static IEnumerable<Entity> WithType<T>(this IEnumerable<Entity> entities)
            where T : Component
        {
            return entities.Where(e => e.Components.AnyOfType<T>()).ToList();
        }

        public static IEnumerable<T> OfType<T>(this IEnumerable<Entity> entities)
            where T : Entity
        {
            return entities.Where(e => e is T).Select(e => e as T).ToList();
        }

        public static T FirstOfType<T>(this IEnumerable<Entity> entities)
            where T : Entity
        {
            return entities.FirstOrDefault(e => e is T) as T;
        }

        public static T LastOfType<T>(this IEnumerable<Entity> entities)
            where T : Entity
        {
            return entities.LastOrDefault(e => e is T) as T;
        }

        public static bool AnyOfType<T>(this IEnumerable<Entity> entities)
            where T : Entity
        {
            return entities.Any(e => e is T);
        }

        public static int CountType<T>(this IEnumerable<Entity> entities)
            where T : Entity
        {
            return entities.Count(e => e is T);
        }

        public static void ForEachOfType<T>(
            this IEnumerable<Entity> entities, Action<T> action)
            where T : Entity
        {
            entities.Where(e => e is T).Select(e => e as T).ToList().ForEach(action);
        }

        public static IEnumerable<Entity> OnLayer(
            this IEnumerable<Entity> entities, Layer layer)
        {
            return entities.Where(e => e.Layer == layer).ToList();
        }
        #endregion

        #region Components
        public static IEnumerable<T> OfType<T>(this IEnumerable<Component> components)
            where T : Component
        {
            return components.Where(c => c is T).Select(c => c as T).ToList();
        }

        public static T FirstOfType<T>(this IEnumerable<Component> components)
            where T : Component
        {
            return components.FirstOrDefault(c => c is T) as T;
        }

        public static bool AnyOfType<T>(this IEnumerable<Component> components)
            where T : Component
        {
            return components.Any(c => c is T);
        }
        #endregion
    }
}
