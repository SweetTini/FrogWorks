namespace FrogWorks
{
    public sealed class LayerManager : DepthSortManager<Layer, Scene>
    {
        internal LayerManager(Scene scene)
            : base(scene) { }
    }
}
