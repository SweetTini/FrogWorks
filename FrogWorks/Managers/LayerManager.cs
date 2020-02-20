namespace FrogWorks
{
    public sealed class LayerManager : SortingManager<Layer, Scene>
    {
        internal LayerManager(Scene scene)
            : base(scene)
        {
        }

        internal void Reset()
        {
            foreach (var child in Children)
            {
                child.Camera.UpdateViewport();
                child.UpdateRenderTarget();
            }
        }

        internal void Dispose()
        {
            foreach (var child in Children)
                child.RenderTarget.Dispose();
        }
    }
}
