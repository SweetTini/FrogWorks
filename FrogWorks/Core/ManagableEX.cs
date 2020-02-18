namespace FrogWorks
{
    public static class ManagableEX
    {
        public static T As<T>(this Scene scene)
            where T : Scene
        {
            return scene as T;
        }

        public static T As<T>(this Collider collider)
            where T : Collider
        {
            return collider as T;
        }
    }
}
