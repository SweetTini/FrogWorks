using System.Collections.Generic;
using System.Linq;

namespace FrogWorks
{
    public sealed class EntityManager : DepthSortManager<Entity, Layer>
    {
        private Scene Scene => Container.Parent;

        private Queue<SwitchLayerCommand> ToSwitchLayer { get; set; }

        internal EntityManager(Layer layer)
            : base(layer)
        {
            ToSwitchLayer = new Queue<SwitchLayerCommand>();
        }

        protected override void PostProcessQueues()
        {
            base.PostProcessQueues();

            while (ToSwitchLayer.Count > 0)
                TrySwitchToLayer(ToSwitchLayer.Dequeue());
        }

        public void SwitchToLayer(Entity item, Layer layer)
        {
            if (!Items.Contains(item) || item.Parent == null || !Scene.Layers.Contains(layer))
                return;

            var command = new SwitchLayerCommand(item, layer);

            if (IsBusy && !ToSwitchLayer.Contains(command))
                ToSwitchLayer.Enqueue(command);
            else if (!IsBusy)
                TrySwitchToLayer(command);
        }

        private void TrySwitchToLayer(SwitchLayerCommand command)
        {
            if (!Items.Contains(command.Entity) || !Scene.Layers.Contains(command.Layer))
                return;

            TryRemove(command.Entity);
            command.Layer.Entities.TryAdd(command.Entity);
        }

        private class SwitchLayerCommand
        {
            public Entity Entity { get; set; }

            public Layer Layer { get; set; }

            public SwitchLayerCommand(Entity entity, Layer layer)
            {
                Entity = entity;
                Layer = layer;
            }
        }
    }
}
