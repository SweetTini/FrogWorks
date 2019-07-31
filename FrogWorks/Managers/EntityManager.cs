using System;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public class EntityManager : IEnumerable<Entity>, IEnumerable
    {
        private Scene _scene;
        private List<Entity> _entities, _entitiesToAdd, _entitiesToRemove;
        private Dictionary<int, double> _absoluteDepthLookup;
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
            _entitiesToAdd = new List<Entity>();
            _entitiesToRemove = new List<Entity>();
            _absoluteDepthLookup = new Dictionary<int, double>();
        }

        internal void ProcessQueues()
        {
            if (_entitiesToRemove.Count > 0)
            {
                for (int i = 0; i < _entitiesToRemove.Count; i++)
                {
                    _entities.Remove(_entitiesToRemove[i]);
                    _entitiesToRemove[i].OnRemoved();
                }

                _entitiesToRemove.Clear();
            }

            if (_entitiesToAdd.Count > 0)
            {
                for (int i = 0; i < _entitiesToAdd.Count; i++)
                {
                    _entities.Add(_entitiesToAdd[i]);
                    CorrectAbsoluteDepth(_entitiesToAdd[i]);
                }

                _entitiesToAdd.Clear();
            }

            if (_isUnsorted)
            {
                _entities.Sort(Entity.CompareDepth);
                _isUnsorted = false;
            }
        }

        internal void CorrectAbsoluteDepth(Entity entity)
        {
            const double theta = .000001;
            double increment;

            if (_absoluteDepthLookup.TryGetValue(entity.Depth, out increment))
                _absoluteDepthLookup[entity.Depth] += theta;
            else _absoluteDepthLookup.Add(entity.Depth, theta);
            entity.AbsoluteDepth = entity.Depth + increment;

            _isUnsorted = true;
        }

        internal void MarkUnsorted()
        {
            _isUnsorted = true;
        }

        internal void Update(Layer layer, float deltaTime)
        {
            for (int i = 0; i < _entities.Count; i++)
                if (!_entities[i].IsDestroyed && _entities[i].Layer.Equals(layer) && _entities[i].IsEnabled)
                    _entities[i].Update(deltaTime);
        }

        internal void Draw(Layer layer, RendererBatch batch)
        {
            for (int i = 0; i < _entities.Count; i++)
                if (!_entities[i].IsDestroyed && _entities[i].Layer.Equals(layer) && _entities[i].IsVisible)
                    _entities[i].Draw(batch);
        }

        public void Add(Layer layer, Entity entity)
        {
            if (!_entities.Contains(entity) && !_entitiesToAdd.Contains(entity))
            {
                _entitiesToAdd.Add(entity);
                entity.OnAdded(layer);
            }
        }

        public void Add(Layer layer, params Entity[] entities)
        {
            for (int i = 0; i < entities.Length; i++)
                Add(layer, entities[i]);
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
            for (int i = 0; i < entities.Length; i++)
                Remove(entities[i]);
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
