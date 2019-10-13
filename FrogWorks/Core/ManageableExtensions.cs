namespace FrogWorks
{
    public static class ManageableExtensions
    {
        public static T As<T>(this Scene scene)
            where T : Scene
        {
            return scene is T ? scene as T : null;
        }

        public static T As<T>(this Layer layer)
            where T : Layer
        {
            return layer is T ? layer as T : null;
        }

        public static T As<T>(this Collider collider)
            where T : Collider
        {
            return collider is T ? collider as T : null;
        }
    }
}
