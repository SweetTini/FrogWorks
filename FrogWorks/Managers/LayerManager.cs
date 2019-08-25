namespace FrogWorks
{
    public sealed class LayerManager : AbstractManager<Layer, Scene>
    {
        internal LayerManager(Scene scene)
            : base(scene) { }

        protected override void PostProcessQueues() { }
    }
}
