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
                var index = IndexOf(name);

                if (index == -1)
                    throw new Exception($"Cannot find layer with name \"{name}\".");

                return _layers[index];
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

                    batch.Begin();
                    _scene.Entities.Draw(_layers[i], batch);
                    batch.End();
                }
            }

            batch.Reset();
        }

        public Layer Add(string name)
        {
            var index = IndexOf(name);

            if (index > -1)
                return _layers[index];

            var layer = Cache.Count > 0 ? Cache.Pop() : new Layer();
            layer.Name = name;

            _layers.Add(layer);
            layer.OnAdded(_scene);

            return layer;
        }

        public void Remove(string name)
        {
            var index = IndexOf(name);

            if (index > -1)
            {
                var layer = _layers[index];

                if (layer.IsDefault)
                    throw new Exception("Cannot remove the default layer.");

                _layersToRemove.Add(layer);
            }
        }

        public int IndexOf(string name)
        {
            for (int i = 0; i < _layers.Count; i++)
                if (string.Equals(name, _layers[i].Name, StringComparison.InvariantCultureIgnoreCase))
                    return i;

            return -1;
        }

        public void MoveToFront(string name)
        {
            var index = IndexOf(name);

            if (index > -1)
            {
                var layer = _layers[index];
                _layers.Remove(layer);
                _layers.Add(layer);
            }
        }

        public void MoveToFront(Layer layer)
        {
            var index = _layers.IndexOf(layer);

            if (index > -1)
            {
                _layers.Remove(layer);
                _layers.Add(layer);
            }
        }

        public void MoveToBack(string name)
        {
            var index = IndexOf(name);

            if (index > -1)
            {
                var layer = _layers[index];
                _layers.Remove(layer);
                _layers.Insert(0, layer);
            }
        }

        public void MoveToBack(Layer layer)
        {
            var index = _layers.IndexOf(layer);

            if (index > -1)
            {
                _layers.Remove(layer);
                _layers.Insert(0, layer);
            }
        }

        public void MoveAhead(string sourceName, string targetName)
        {
            var sourceIndex = IndexOf(sourceName);
            var targetIndex = IndexOf(targetName);

            if (sourceIndex > -1 && targetIndex > -1 && sourceIndex < targetIndex)
            {
                var source = _layers[sourceIndex];
                var target = _layers[targetIndex];

                _layers.Remove(source);
                _layers.Insert(_layers.IndexOf(target) + 1, source);
            }
        }

        public void MoveAhead(Layer source, Layer target)
        {
            var sourceIndex = _layers.IndexOf(source);
            var targetIndex = _layers.IndexOf(target);

            if (sourceIndex > -1 && targetIndex > -1 && sourceIndex < targetIndex)
            {
                _layers.Remove(source);
                _layers.Insert(_layers.IndexOf(target) + 1, source);
            }
        }

        public void MoveBehind(string sourceName, string targetName)
        {
            var sourceIndex = IndexOf(sourceName);
            var targetIndex = IndexOf(targetName);

            if (sourceIndex > -1 && targetIndex > -1 && sourceIndex > targetIndex)
            {
                var source = _layers[sourceIndex];
                var target = _layers[targetIndex];

                _layers.Remove(source);
                _layers.Insert(_layers.IndexOf(target), source);
            }
        }

        public void MoveBehind(Layer source, Layer target)
        {
            var sourceIndex = _layers.IndexOf(source);
            var targetIndex = _layers.IndexOf(target);

            if (sourceIndex > -1 && targetIndex > -1 && sourceIndex > targetIndex)
            {
                _layers.Remove(source);
                _layers.Insert(_layers.IndexOf(target), source);
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
