using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class Scene
    {
        internal LayerManager Layers { get; private set; }

        internal EntityManager Entities { get; private set; }

        protected Layer MainLayer { get; private set; }

        public Color BackgroundColor { get; set; } = Color.CornflowerBlue;

        public bool IsEnabled { get; private set; }

        protected Scene()
        {
            Layers = new LayerManager(this);
            Entities = new EntityManager(this);
            MainLayer = Layers.Add("Main");
            MainLayer.IsDefault = true;
        }

        public virtual void Begin()
        {
            IsEnabled = true;

            for (int i = 0; i < Entities.Count; i++)
                Entities[i].OnSceneBegan(this);
        }

        public virtual void End()
        {
            for (int i = 0; i < Entities.Count; i++)
                Entities[i].OnSceneEnded(this);

            IsEnabled = false;
        }

        public virtual void BeginUpdate()
        {
            Entities.ProcessQueues();
            Layers.ProcessQueues();
        }

        public virtual void Update(float deltaTime)
        {
            Layers.Update(deltaTime);
        }

        public virtual void EndUpdate()
        {
        }

        public virtual void Draw(RendererBatch batch)
        {
            Layers.Draw(batch);
        }

        #region Layers
        public Layer AddLayer(string name)
        {
            return Layers.Add(name);
        }

        public void RemoveLayer(string name)
        {
            Layers.Remove(name);
        }

        public bool HasLayer(string name)
        {
            return Layers.IndexOf(name) > -1;
        }

        public Layer GetLayer(string name)
        {
            var index = Layers.IndexOf(name);
            return index > -1 ? Layers[index] : null;
        }
        #endregion

        #region Entities
        public void AddEntities(params Entity[] entities)
        {
            Entities.Add(MainLayer, entities);
        }

        public void AddEntities(IEnumerable<Entity> entities)
        {
            Entities.Add(MainLayer, entities);
        }

        public void AddEntitiesToLayer(string name, params Entity[] entities)
        {
            AddEntitiesToLayer(GetLayer(name), entities);
        }

        public void AddEntitiesToLayer(string name, IEnumerable<Entity> entities)
        {
            AddEntitiesToLayer(GetLayer(name), entities);
        }

        public void AddEntitiesToLayer(Layer layer, params Entity[] entities)
        {
            Entities.Add(layer ?? MainLayer, entities);
        }

        public void AddEntitiesToLayer(Layer layer, IEnumerable<Entity> entities)
        {
            Entities.Add(layer ?? MainLayer, entities);
        }

        public void RemoveEntities(params Entity[] entities)
        {
            Entities.Remove(entities);
        }

        public void RemoveEntities(IEnumerable<Entity> entities)
        {
            Entities.Remove(entities);
        }

        public IEnumerable<T> GetEntitiesOfType<T>() where T : Entity
        {
            for (int i = 0; i < Entities.Count; i++)
                if (Entities[i] is T)
                    yield return Entities[i] as T;
        }

        public int CountEntitiesOfType<T>() where T : Entity
        {
            var count = 0;

            for (int i = 0; i < Entities.Count; i++)
                if (Entities[i] is T)
                    count++;

            return count;
        }

        public T FindEntityOfType<T>() where T : Entity
        {
            for (int i = 0; i < Entities.Count; i++)
                if (Entities[i] is T)
                    return Entities[i] as T;

            return null;
        }

        public bool HasEntityOfType<T>() where T : Entity
        {
            for (int i = 0; i < Entities.Count; i++)
                if (Entities[i] is T)
                    return true;

            return false;
        }

        public IEnumerable<Entity> GetEntitiesByComponent<T>() where T : Component
        {
            for (int i = 0; i < Entities.Count; i++)
                if (Entities[i].HasComponentOfType<T>())
                    yield return Entities[i];
        }

        public int CountEntitiesByComponent<T>() where T : Component
        {
            var count = 0;

            for (int i = 0; i < Entities.Count; i++)
                if (Entities[i].HasComponentOfType<T>())
                    count++;

            return count;
        }

        public Entity FindEntityByComponent<T>() where T : Component
        {
            for (int i = 0; i < Entities.Count; i++)
                if (Entities[i].HasComponentOfType<T>())
                    return Entities[i];

            return null;
        }

        public IEnumerable<Entity> GetEntities()
        {
            for (int i = 0; i < Entities.Count; i++)
                yield return Entities[i];
        }

        public int CountEntities()
        {
            return Entities.Count;
        }
        #endregion
    }
}
