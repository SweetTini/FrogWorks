namespace FrogWorks.Demo
{
    public class CoreTestScene : Scene
    {
        public CoreTestScene()
            : base()
        {
            BackgroundColor = ColorConvert.FromHsl(0, 100, 80);
            CreateApple(160, 144, 0);
            CreateApple(200, 174, -1);
            CreateApple(120, 174, 1);
        }

        void CreateApple(float x, float y, int depth, Layer layer = null)
        {
            var apple = new AppleEntity() { X = x, Y = y, Depth = depth };
            AddEntityToLayer(layer, apple);
        }
    }
}
