using System;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public class LayerManager : IEnumerable<Layer>, IEnumerable
    {
        private Scene _scene;
        private List<Layer> _layers, _layersToRemove;

        internal static Stack<Layer> Cache { get; } = new Stack<Layer>();

        public Layer this[int index]
        {
            get
            {
                if (!_layers.WithinRange(index))
                    throw new IndexOutOfRangeException("Index is outside the range of layers to access.");

                return _layers[index];
            }
        }

        public Layer this[string name]
        {
            get
            {
                Layer layer;

                if (!TryGet(name, out layer))
                    throw new Exception($"Cannot find layer with name \"{name}\".");

                return layer;
            }
        }

        public int Count => _layers.Count;

        internal LayerManager(Scene scene)
        {
            _scene = scene;
            _layers = new List<Layer>();
            _layersToRemove = new List<Layer>();
        }

        internal void ProcessQueues()
        {
            if (_layersToRemove.Count > 0)
            {
                for (int i = 0; i < _layersToRemove.Count; i++)
                {
                    _layers.Remove(_layersToRemove[i]);
                    _layersToRemove[i].OnRemoved();
                    Cache.Push(_layersToRemove[i]);
                }

                _layersToRemove.Clear();
            }
        }

        internal void Update(float deltaTime)
        {
            for (int i = 0; i < _layers.Count; i++)
                if (_layers[i].IsEnabled)
                    _scene.Entities.Update(_layers[i], deltaTime);
        }

        internal void Draw(RendererBatch batch)
        {
            for (int i = 0; i < _layers.Count; i++)
            {
                if (_layers[i].IsVisible)
                {
                    _layers[i].ConfigureBatch(batch);
                    _scene.Entities.Draw(_layers[i], batch);
                }
            }

            batch.Reset();
        }

        public Layer AddOrGet(string name)
        {
            Layer layer;

            if (!TryGet(name, out layer))
            {
                layer = Cache.Count > 0 
                    ? Cache.Pop() 
                    : new Layer();

                _layers.Add(layer);
                layer.Name = name;
                layer.OnAdded(_scene);
            }

            return layer;
        }

        public void Remove(string name)
        {
            Layer layer;

            if (TryGet(name, out layer))
            {
                if (layer.IsDefault)
                    throw new Exception("Cannot remove the default layer.");

                _layersToRemove.Add(layer);
            }
        }

        public bool Exists(string name)
        {
            for (int i = 0; i < _layers.Count; i++)
                if (string.Equals(_layers[i].Name, name, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        public bool TryGet(string name, out Layer layer)
        {
            int index;
            return TryGet(name, out layer, out index);
        }

        public bool TryGet(string name, out Layer layer, out int index)
        {
            layer = null;
            index = -1;

            for (int i = 0; i < _layers.Count; i++)
            {
                if (string.Equals(layer.Name, _layers[i].Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    layer = _layers[i];
                    index = i;
                    return true;
                }
            }

            return false;
        }

        public void MoveToFront(string name)
        {
            Layer layer;
            int index;

            if (TryGet(name, out layer, out index) && index < _layers.Count - 1)
            {
                _layers.Remove(layer);
                _layers.Add(layer);
            }
        }

        public void MoveToFront(string sourceName, string targetName)
        {
            Layer source, target;
            int sourceIndex, targetIndex;

            if (TryGet(sourceName, out source, out sourceIndex) && TryGet(targetName, out target, out targetIndex))
            {
                if (sourceIndex < targetIndex)
                {
                    _layers.Remove(source);
                    _layers.Insert(_layers.IndexOf(target) + 1, source);

                }
            }
        }

        public void MoveToBack(string name)
        {
            Layer layer;
            int index;

            if (TryGet(name, out layer, out index) && index > 0)
            {
                _layers.Remove(layer);
                _layers.Insert(0, layer);
            }
        }

        public void MoveToBack(string sourceName, string targetName)
        {
            Layer source, target;
            int sourceIndex, targetIndex;

            if (TryGet(sourceName, out source, out sourceIndex) && TryGet(targetName, out target, out targetIndex))
            {
                if (sourceIndex > targetIndex)
                {
                    _layers.Remove(source);
                    _layers.Insert(_layers.IndexOf(target), source);

                }
            }
        }

        public Layer[] ToArray()
        {
            var layers = new Layer[_layers.Count];

            for (int i = 0; i < _layers.Count; i++)
                layers[i] = _layers[i];

            return layers;
        }

        public IEnumerator<Layer> GetEnumerator()
        {
            return _layers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
