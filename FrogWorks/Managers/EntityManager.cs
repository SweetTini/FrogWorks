namespace FrogWorks
{
    public sealed class EntityManager : AbstractManager<Entity, Layer>
    {
        private Scene ContainerParent => Container.Parent;

        internal EntityManager(Layer layer)
            : base(layer) { }

        protected override void PostProcessQueues() { }
    }
}
