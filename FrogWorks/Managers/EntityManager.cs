using System;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public class EntityManager : IEnumerable<Entity>, IEnumerable
    {
        private Scene _scene;
        private List<Entity> _entities, _entitiesToRemove;
        private Dictionary<Entity, Layer> _entitiesToAdd;
        private bool _isUnsorted;

        public Entity this[int index]
        {
            get
            {
                if (!_entities.WithinRange(index))
                    throw new IndexOutOfRangeException("Index is outside the range of entities to access.");

                return _entities[index];
            }
        }

        public int Count => _entities.Count;

        internal EntityManager(Scene scene)
        {
            _scene = scene;
            _entities = new List<Entity>();
            _entitiesToRemove = new List<Entity>();
            _entitiesToAdd = new Dictionary<Entity, Layer>();
        }

        internal void ProcessQueues()
        {
            if (_entitiesToRemove.Count > 0)
            {
                foreach (var entity in _entitiesToRemove)
                {
                    _entities.Remove(entity);
                    entity.OnRemoved();
                }

                _entitiesToRemove.Clear();
            }

            if (_entitiesToAdd.Count > 0)
            {
                foreach (var entityLayerPair in _entitiesToAdd)
                {
                    _entities.Add(entityLayerPair.Key);
                    entityLayerPair.Key.OnAdded(entityLayerPair.Value);
                }

                _entitiesToAdd.Clear();
            }

            if (_isUnsorted)
            {
                _entities.Sort(Entity.CompareDepth);
                _isUnsorted = false;
            }
        }

        internal void MarkUnsorted()
        {
            _isUnsorted = true;
        }

        internal void Update(Layer layer, float deltaTime)
        {
            foreach (var entity in _entities)
                if (!entity.IsDestroyed && entity.Layer.Equals(layer) && entity.IsEnabled)
                    entity.Update(deltaTime);
        }

        internal void Draw(Layer layer, RendererBatch batch)
        {
            foreach (var entity in _entities)
                if (!entity.IsDestroyed && entity.Layer.Equals(layer) && entity.IsVisible)
                    entity.Draw(batch);
        }

        public void Add(Layer layer, Entity entity)
        {
            if (!_entities.Contains(entity) && !_entitiesToAdd.ContainsKey(entity))
                _entitiesToAdd.Add(entity, layer ?? _scene.DefaultLayer);
        }

        public void Add(Layer layer, params Entity[] entities)
        {
            foreach (var entity in entities)
                Add(layer, entity);
        }

        public void Add(Layer layer, IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
                Add(layer, entity);
        }

        public void Remove(Entity entity)
        {
            if (_entities.Contains(entity) && !_entitiesToRemove.Contains(entity))
                _entitiesToRemove.Add(entity);
        }

        public void Remove(params Entity[] entities)
        {
            foreach (var entity in entities)
                Remove(entity);
        }

        public void Remove(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
                Remove(entity);
        }

        public Entity[] ToArray()
        {
            var entities = new Entity[_entities.Count];

            for (int i = 0; i < _entities.Count; i++)
                entities[i] = _entities[i];

            return entities;
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
