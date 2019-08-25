namespace FrogWorks
{
    public sealed class LayerManager : AbstractSortingManager<Layer, Scene>
    {
        internal LayerManager(Scene scene)
            : base(scene) { }
    }
}
