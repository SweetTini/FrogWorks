namespace FrogWorks
{
    public sealed class ComponentManager : AbstractManager<Component, Entity>
    {
        internal ComponentManager(Entity entity)
            : base(entity) { }
    }
}
