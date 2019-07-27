using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class Entity
    {
        private Collider _collider;
        private Vector2 _position;
        private int _depth;

        internal static Comparison<Entity> CompareDepth
        {
            get { return (entity, other) => Math.Sign(entity.Depth - other.Depth); }
        }

        internal ComponentManager Components { get; private set; }

        protected internal Layer Layer { get; private set; }

        protected internal Scene Scene => Layer?.Scene;

        protected internal Collider Collider
        {
            get { return _collider; }
            set
            {
                if (value == _collider || value.Entity != null) return;
                _collider?.OnRemoved();
                _collider = value;
                _collider?.OnAdded(this);
            }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (value == _position) return;
                var lastPosition = _position;
                _position = value;
                OnTranslated(_position - lastPosition);
            }
        }

        public float X
        {
            get { return _position.X; }
            set
            {
                if (value == _position.X) return;
                var lastPosition = _position;
                _position.X = value;
                OnTranslated(_position - lastPosition);
            }
        }

        public float Y
        {
            get { return _position.Y; }
            set
            {
                if (value == _position.Y) return;
                var lastPosition = _position;
                _position.Y = value;
                OnTranslated(_position - lastPosition);
            }
        }

        public int Depth
        {
            get { return _depth; }
            set
            {
                if (value == _depth) return;
                _depth = value;
                Scene?.Entities.MarkUnsorted();
            }
        }

        public bool IsEnabled { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        public bool IsCollidable { get; set; } = true;

        public bool IsDestroyed { get; internal set; }

        protected Entity()
        {
            Components = new ComponentManager(this);
        }

        public virtual void Update(float deltaTime)
        {
            Components.ProcessQueues();
            Components.Update(deltaTime);
        }

        public virtual void Draw(RendererBatch batch)
        {
            Components.Draw(batch);
        }

        public virtual void OnAdded(Layer layer)
        {
            Layer = layer;
            Collider?.OnEntityAdded(this);

            for (int i = 0; i < Components.Count; i++)
                Components[i].OnEntityAdded(this);

            Scene?.Entities.MarkUnsorted();
        }

        public virtual void OnRemoved()
        {
            for (int i = 0; i < Components.Count; i++)
                Components[i].OnEntityRemoved(this);

            Collider?.OnEntityRemoved(this);
            Layer = null;
        }

        public virtual void OnLayerChanged(Layer layer, Layer lastLayer)
        {
            Collider?.OnLayerChanged(layer, lastLayer);

            for (int i = 0; i < Components.Count; i++)
                Components[i].OnLayerChanged(layer, lastLayer);
        }

        public virtual void OnSceneBegan(Scene scene)
        {
        }

        public virtual void OnSceneEnded(Scene scene)
        {
        }

        public virtual void OnTranslated(Vector2 offset)
        {
            _collider?.OnTranslated(offset);
        }

        public void Destroy()
        {
            Scene?.Entities.Remove(this);
            IsDestroyed = true;
        }

        #region Components
        public void AddComponents(params Component[] components)
        {
            Components.Add(components);
        }

        public void AddComponents(IEnumerable<Component> components)
        {
            Components.Add(components);
        }

        public void RemoveComponents(params Component[] components)
        {
            Components.Remove(components);
        }

        public void RemoveComponents(IEnumerable<Component> components)
        {
            Components.Remove(components);
        }

        public IEnumerable<T> GetComponentsOfType<T>() where T : Component
        {
            for (int i = 0; i < Components.Count; i++)
                if (Components[i] is T)
                    yield return Components[i] as T;
        }

        public int CountComponentsOfType<T>() where T : Component
        {
            var count = 0;

            for (int i = 0; i < Components.Count; i++)
                if (Components[i] is T)
                    count++;

            return count;
        }

        public T FindComponentOfType<T>() where T : Component
        {
            for (int i = 0; i < Components.Count; i++)
                if (Components[i] is T)
                    return Components[i] as T;

            return null;
        }

        public bool HasComponentOfType<T>() where T : Component
        {
            for (int i = 0; i < Components.Count; i++)
                if (Components[i] is T)
                    return true;

            return false;
        }

        public IEnumerable<Component> GetComponents()
        {
            for (int i = 0; i < Components.Count; i++)
                yield return Components[i];
        }

        public int CountComponents()
        {
            return Components.Count;
        }
        #endregion

        #region Layers
        public void MoveToLayer(string name)
        {
            if (Scene == null) return;

            var index = Scene.Layers.IndexOf(name);

            if (index > -1)
            {
                var lastLayer = Layer;
                Layer = Scene.Layers[index];
                OnLayerChanged(Layer, lastLayer);
            }
        }

        public void MoveToLayer(Layer layer)
        {
            if (layer == null || Scene == null) return;

            var index = Scene.Layers.IndexOf(layer.Name);

            if (index > -1)
            {
                var lastLayer = Layer;
                Layer = layer;
                OnLayerChanged(Layer, lastLayer);
            }
        }
        #endregion
    }
}
