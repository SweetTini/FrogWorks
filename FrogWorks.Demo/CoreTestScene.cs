namespace FrogWorks.Demo
{
    public class CoreTestScene : Scene
    {
        protected BitmapFont Font { get; set; }

        protected Layer TestLayer { get; set; }

        public CoreTestScene()
            : base()
        {
            BackgroundColor = ColorConvert.FromHsl(0, 0, 59);

            var charSet = " !\"\'*+,-./0123456789:;ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Font = new BitmapFont(Texture.Load("Images\\MonoFont.png"), 10, 16, charSet);

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

        public override void Draw(RendererBatch batch)
        {
            base.Draw(batch);

            MainLayer.ConfigureBatch(batch);
            batch.Begin();

            Font.Draw(batch, "Arrow keys - Move\nWASD - Rotate and zoom", 8, 8, 320, 16, HorizontalAlignment.Center);

            batch.End();
            batch.Reset();
        }

        void CreateApple(float x, float y, int depth, Layer layer = null)
        {
            var apple = new AppleEntity()
            {
                X = x,
                Y = y,
                Depth = depth
            };

            AddEntitiesToLayer(layer, apple);
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
