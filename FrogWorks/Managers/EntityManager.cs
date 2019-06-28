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

        public Entity this[int index] => _entities.WithinRange(index) ? _entities[index] : null;

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
                _entities.Sort(Entity.Compare);
                _isUnsorted = false;
            }
        }

        internal void Update(Layer layer, float deltaTime)
        {
            foreach (var entity in _entities)
                if (entity.Layer.Equals(layer) && entity.IsEnabled)
                    entity.Update(deltaTime);
        }

        internal void Draw(Layer layer, RendererBatch batch)
        {
            foreach (var entity in _entities)
                if (entity.Layer.Equals(layer) && entity.IsVisible)
                    entity.Draw(batch);
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
