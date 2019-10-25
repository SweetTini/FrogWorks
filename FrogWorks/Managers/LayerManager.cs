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

        internal void Reset()
        {
            foreach (var item in Items)
            {
                item.Camera.UpdateViewport();
                item.UpdateBuffer();
            }
        }

        internal void Dispose()
        {
            foreach (var item in Items)
                item.Buffer.Dispose();
        }
    }
}
