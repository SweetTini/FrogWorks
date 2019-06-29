using System;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public class LayerManager : IEnumerable<Layer>, IEnumerable
    {
        private Scene _scene;
        private List<Layer> _layers, _layersToRemove;
        private bool _isUnsorted;

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
                foreach (var layer in _layersToRemove)
                {
                    _layers.Remove(layer);
                    layer.OnRemoved();
                    Cache.Push(layer);
                }

                _layersToRemove.Clear();
            }

            if (_isUnsorted)
            {
                _layers.Sort(Layer.ComparePriority);
                _isUnsorted = false;
            }
        }

        internal void MarkUnsorted()
        {
            _isUnsorted = true;
        }

        internal void Update(float deltaTime)
        {
            foreach (var layer in _layers)
                if (layer.IsEnabled)
                    _scene.Entities.Update(layer, deltaTime);
        }

        internal void Draw(RendererBatch batch)
        {
            foreach (var layer in _layers)
                if (layer.IsVisible)
                    _scene.Entities.Draw(layer, batch);
        }

        public Layer AddOrGet(string name, int priority = 0)
        {
            Layer layer;

            if (!TryGet(name, out layer))
            {
                layer = Cache.Count > 0 
                    ? Cache.Pop() 
                    : new Layer();

                layer.Name = name;
                _layers.Add(layer);
            }

            layer.Priority = priority;
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
            foreach (var layer in _layers)
                if (string.Equals(layer.Name, name, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        public bool TryGet(string name, out Layer layer)
        {
            layer = null;

            foreach (var layerToCheck in _layers)
            {
                if (string.Equals(layer.Name, name, StringComparison.InvariantCultureIgnoreCase))
                {
                    layer = layerToCheck;
                    return true;
                }
            }

            return false;
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
