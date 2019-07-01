namespace FrogWorks.Demo
{
    public class CoreTestScene : Scene
    {
        protected Layer TestLayer { get; set; }

        public CoreTestScene()
            : base()
        {
            BackgroundColor = ColorConvert.FromHsl(0, 0, 59);

            TestLayer = AddLayer("Test");
            TestLayer.MoveToBack();

            CreateApple(160, 120, 0);
            CreateApple(190, 150, -1);
            CreateApple(190, 90, 0);
            CreateApple(130, 150, 1);
            CreateApple(130, 90, -1);
            CreateCheckerBoard(0, GetLayer("Test"));
        }

        void CreateApple(float x, float y, int depth, Layer layer = null)
        {
            var apple = new AppleEntity() { X = x, Y = y, Depth = depth };
            AddEntityToLayer(layer, apple);
        }

        void CreateCheckerBoard(int depth, Layer layer = null)
        {
            var checkerBoard = new CheckerBoardEntity() { Depth = depth };
            AddEntitiesToLayer(layer, checkerBoard);
        }
    }
}
