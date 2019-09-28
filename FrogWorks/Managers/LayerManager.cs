namespace FrogWorks
{
    public sealed class LayerManager : DepthSortManager<Layer, Scene>
    {
        internal LayerManager(Scene scene)
            : base(scene) { }

        internal override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            IsBusy = true;

            foreach (var item in Items)
                item.UpdateCamera();

            IsBusy = false;
        }
    }
}
