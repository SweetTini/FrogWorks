namespace FrogWorks
{
    public sealed class ComponentManager : Manager<Component, Entity>
    {
        internal ComponentManager(Entity entity)
            : base(entity) { }
    }
}
