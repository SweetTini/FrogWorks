using System;

namespace FrogWorks
{
    public class Layer
    {
        private int _priority;

        internal static Comparison<Layer> ComparePriority
        {
            get { return (layer, other) => Math.Sign(layer.Priority - other.Priority); }
        }

        public Scene Scene { get; private set; }

        public Camera Camera { get; private set; } = new Camera();

        public string Name { get; internal set; }

        public int Priority
        {
            get { return _priority; }
            set
            {
                if (value == _priority) return;
                _priority = value;
                Scene?.Layers.MarkUnsorted();
            }
        }

        public bool IsEnabled { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        internal bool IsDefault { get; set; }

        internal Layer()
        {
        }

        internal void OnAdded(Scene scene)
        {
            Scene = scene;
        }

        internal void OnRemoved()
        {
            foreach (var entity in Scene.Entities)
                if (entity.Layer.Equals(this))
                    entity.Destroy();

            Scene = null;
        }
    }
}
