using System.Collections.Generic;
using System.Linq;

namespace FrogWorks
{
    public sealed class EntityManager : AbstractDepthManager<Entity, Layer>
    {
        private Scene ContainerScene => Container.Parent;

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
                TrySwitchLayer(ToSwitchLayer.Dequeue());
        }

        public void SwitchLayer(Entity item, Layer layer)
        {
            if (!Items.Contains(item) || item.Parent == null || !ContainerScene.Layers.Contains(layer))
                return;

            var command = new SwitchLayerCommand(item, layer);

            if (IsBusy && !ToSwitchLayer.Contains(command))
                ToSwitchLayer.Enqueue(command);
            else if (!IsBusy)
                TrySwitchLayer(command);
        }

        private void TrySwitchLayer(SwitchLayerCommand command)
        {
            if (!Items.Contains(command.Entity) || !ContainerScene.Layers.Contains(command.Layer))
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
