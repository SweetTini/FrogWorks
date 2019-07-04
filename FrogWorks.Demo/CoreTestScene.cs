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

            CreateCheckerBoard(12, 12, 0, TestLayer);
        }

        public override void Update(float deltaTime)
        {
            TestLayer.Camera.AngleInDegrees += Input.Keyboard.GetAxis(Keys.D, Keys.A) * .5f;
            TestLayer.Camera.Zoom += Input.Keyboard.GetAxis(Keys.S, Keys.W) * .005f;
            TestLayer.Camera.X += Input.Keyboard.GetAxis(Keys.RightArrow, Keys.LeftArrow) * 2f;
            TestLayer.Camera.Y += Input.Keyboard.GetAxis(Keys.DownArrow, Keys.UpArrow) * 2f;

            base.Update(deltaTime);
        }

        void CreateApple(float x, float y, int depth, Layer layer = null)
        {
            var apple = new AppleEntity()
            {
                X = x,
                Y = y,
                Depth = depth
            };

            AddEntityToLayer(layer, apple);
        }

        void CreateCheckerBoard(float x, float y, int depth, Layer layer = null)
        {
            var checkerBoard = new CheckerBoardEntity()
            {
                X = x,
                Y = y,
                Depth = depth
            };

            AddEntitiesToLayer(layer, checkerBoard);
        }
    }
}
