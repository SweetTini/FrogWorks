using System.Collections.Generic;
using System.Linq;

namespace FrogWorks
{
    public sealed class EntityManager : DepthSortManager<Entity, Layer>
    {
        private Scene Scene => Container.Parent;

        private Queue<LayerSwitchCommand> ToSwitchLayer { get; set; }

        internal EntityManager(Layer layer)
            : base(layer)
        {
            ToSwitchLayer = new Queue<LayerSwitchCommand>();
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

            var command = new LayerSwitchCommand(item, layer);

            if (!ToSwitchLayer.Contains(command))
                ToSwitchLayer.Enqueue(command);
        }

        private void TrySwitchToLayer(LayerSwitchCommand command)
        {
            if (!Items.Contains(command.Item) || !Scene.Layers.Contains(command.Target))
                return;

            TryRemove(command.Item);
            command.Target.Entities.TryAdd(command.Item);
        }
    }

    internal class LayerSwitchCommand : ManagedQueueCommand<Entity, Layer>
    {
        public Layer Target { get; private set; }

        public LayerSwitchCommand(Entity item, Layer target) 
            : base(item, ManagedQueueAction.Switch)
        {
            Target = target;
        }
    }
}
