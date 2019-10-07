namespace FrogWorks
{
    public sealed class LayerManager : DepthSortManager<Layer, Scene>
    {
        internal LayerManager(Scene scene)
            : base(scene) { }

        internal override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            foreach (var item in Items)
                item.UpdateCamera();
        }
    }
}
