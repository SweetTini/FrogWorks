namespace FrogWorks.Demo
{
    public class CoreTestScene : Scene
    {
        public CoreTestScene()
            : base()
        {
            BackgroundColor = ColorConvert.FromHsl(0, 100, 80);
            AddLayer("Test");
            CreateApple(160, 144, 0);
            CreateApple(180, 174, -1);
            CreateApple(140, 174, 0);
            CreateApple(160, 114, -1, GetLayer("Test"));
        }

        void CreateApple(float x, float y, int depth, Layer layer = null)
        {
            var apple = new AppleEntity() { X = x, Y = y, Depth = depth };
            AddEntityToLayer(layer, apple);
        }
    }
}
