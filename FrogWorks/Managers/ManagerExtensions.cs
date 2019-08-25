using System;
using System.Collections.Generic;
using System.Linq;

namespace FrogWorks
{
    public static class ManagerExtensions
    {
        #region Layers
        public static Layer With(this IEnumerable<Layer> layers, Entity entity)
            => layers.FirstOrDefault(l => entity.Parent == l);

        public static IEnumerable<Layer> With(this IEnumerable<Layer> layers, IEnumerable<Entity> entities)
            => layers.Where(l => entities.Any(e => e.Parent == l));

        public static IEnumerable<Layer> WithType<T>(this IEnumerable<Layer> layers)
            where T : Entity
            => layers.Where(l => l.Entities.AnyOfType<T>());
        #endregion

        #region Entities
        public static IEnumerable<Entity> WithType<T>(this IEnumerable<Entity> entities)
            where T : Component
            => entities.Where(e => e.Components.AnyOfType<T>());

        public static IEnumerable<T> OfType<T>(this IEnumerable<Entity> entities)
            where T : Entity
            => entities.Where(e => e is T).Select(e => e as T);

        public static T FirstOfType<T>(this IEnumerable<Entity> entities)
            where T : Entity
            => entities.FirstOrDefault(e => e is T) as T;

        public static T LastOfType<T>(this IEnumerable<Entity> entities)
            where T : Entity
            => entities.LastOrDefault(e => e is T) as T;

        public static bool AnyOfType<T>(this IEnumerable<Entity> entities)
            where T : Entity
            => entities.Any(e => e is T);

        public static int CountType<T>(this IEnumerable<Entity> entities)
            where T : Entity
            => entities.Count(e => e is T);

        public static void ForEachOfType<T>(this IEnumerable<Entity> entities, Action<T> action)
            where T : Entity
            => entities.Where(e => e is T).Select(e => e as T).ToList().ForEach(action);
        #endregion

        #region Components
        public static IEnumerable<T> OfType<T>(this IEnumerable<Component> components)
            where T : Component
            => components.Where(c => c is T).Select(c => c as T);

        public static T FirstOfType<T>(this IEnumerable<Component> components)
            where T : Component
            => components.FirstOrDefault(c => c is T) as T;

        public static bool AnyOfType<T>(this IEnumerable<Component> components)
            where T : Component
            => components.Any(c => c is T);
        #endregion
    }
}
