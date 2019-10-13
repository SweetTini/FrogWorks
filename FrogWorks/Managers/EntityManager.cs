using System.Collections.Generic;
using System.Linq;

namespace FrogWorks
{
    public sealed class EntityManager : DepthSortManager<Entity, Layer>
    {
        private Scene Scene => Container.Parent;

        private Queue<LayerSwitchCommand> SwitchCommands { get; set; }

        internal EntityManager(Layer layer)
            : base(layer)
        {
            SwitchCommands = new Queue<LayerSwitchCommand>();
        }

        protected override void PostProcessQueues()
        {
            base.PostProcessQueues();

            while (SwitchCommands.Count > 0)
                TrySwitchToLayer(SwitchCommands.Dequeue());
        }

        public void SwitchToLayer(Entity item, Layer layer)
        {
            if (!Items.Contains(item) || item.Parent == null || !Scene.Layers.Contains(layer))
                return;

            var command = new LayerSwitchCommand(item, layer);

            if (!SwitchCommands.Contains(command))
                SwitchCommands.Enqueue(command);
        }

        private void TrySwitchToLayer(LayerSwitchCommand command)
        {
            if (!Items.Contains(command.Item) || !Scene.Layers.Contains(command.Target))
                return;

            TryRemove(command.Item);
            command.Target.Entities.TryAdd(command.Item);
        }
    }

    internal struct LayerSwitchCommand : IManagedQueueCommand<Entity>
    {
        public Entity Item { get; }

        public Layer Target { get; }

        public LayerSwitchCommand(Entity item, Layer target) 
            : this()
        {
            Item = item;
            Target = target;
        }
    }
}
