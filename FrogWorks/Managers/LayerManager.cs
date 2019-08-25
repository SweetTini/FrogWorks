namespace FrogWorks
{
    public sealed class LayerManager : AbstractDepthManager<Layer, Scene>
    {
        internal LayerManager(Scene scene)
            : base(scene) { }
    }
}
