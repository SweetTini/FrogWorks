using System;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public class ComponentManager : IEnumerable<Component>, IEnumerable
    {
        private Entity _entity;
        private List<Component> _components, _componentsToRemove;
        private bool _isLocked;

        public Component this[int index]
        {
            get
            {
                if (!_components.WithinRange(index))
                    throw new IndexOutOfRangeException("Index is outside the range of components to access.");

                return _components[index];
            }
        }

        public int Count => _components.Count;

        internal ComponentManager(Entity entity)
        {
            _entity = entity;
            _components = new List<Component>();
            _componentsToRemove = new List<Component>();
        }

        internal void ProcessQueues()
        {
            if (_componentsToRemove.Count > 0)
            {
                for (int i = 0; i < _componentsToRemove.Count; i++)
                {
                    _components.Remove(_componentsToRemove[i]);
                    _componentsToRemove[i].OnRemoved();
                }

                _componentsToRemove.Clear();
            }
        }

        internal void Update(float deltaTime)
        {
            for (int i = 0; i < _components.Count; i++)
                if (!_components[i].IsDestroyed && _components[i].IsEnabled)
                    _components[i].Update(deltaTime);
        }

        internal void Draw(RendererBatch batch)
        {
            _isLocked = true;

            for (int i = 0; i < _components.Count; i++)
                if (!_components[i].IsDestroyed && _components[i].IsVisible)
                    _components[i].Draw(batch);

            _isLocked = false;
        }

        public void Add(Component component)
        {
            if (_isLocked)
                throw new Exception("Cannot add a component while the manager is in a locked state.");

            if (!_components.Contains(component))
            {
                _components.Add(component);
                component.OnAdded(_entity);
            }
        }

        public void Add(params Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
                Add(components[i]);
        }

        public void Add(IEnumerable<Component> components)
        {
            foreach (var component in components)
                Add(component);
        }

        public void Remove(Component component)
        {
            if (_isLocked)
                throw new Exception("Cannot remove a component while the manager is in a locked state.");

            if (_components.Contains(component) && !_componentsToRemove.Contains(component))
                _componentsToRemove.Add(component);
        }

        public void Remove(params Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
                Remove(components[i]);
        }

        public void Remove(IEnumerable<Component> components)
        {
            foreach (var component in components)
                Remove(component);
        }

        public Component[] ToArray()
        {
            var components = new Component[_components.Count];

            for (int i = 0; i < _components.Count; i++)
                components[i] = _components[i];

            return components;
        }

        public IEnumerator<Component> GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
