namespace FrogWorks
{
    public static class ManagableExtensions
    {
        public static T As<T>(this Scene scene)
            where T : Scene
        {
            return scene is T ? scene as T : null;
        }

        public static T As<T>(this Collider collider)
            where T : Collider
        {
            return collider is T ? collider as T : null;
        }
    }
}
