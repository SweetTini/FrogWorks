using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class TileMapScene : DefaultScene
    {
        private TileMapRenderer Renderer { get; set; }

        private SplashPattern Pattern { get; set; }

        private BasicLayer ParallaxLayer { get; set; }

        public TileMapScene()
            : base()
        {
        }

        protected override void Begin()
        {
            ParallaxLayer = new BasicLayer() { ScrollRate = Vector2.One * .3f };
            Add(ParallaxLayer);
            MoveToBottom(ParallaxLayer);

            Pattern = new SplashPattern(Color.DarkCyan);
            Renderer = new TileMapRenderer();
            ParallaxLayer.Add(Pattern);
            DefaultLayer.Add(Renderer);

            var collection = Tiled.Load(@"Maps\TestMap");
            BackgroundColor = collection.BackgroundColor;
            Renderer.Add(collection.TileMaps[0].Component, collection.TileMaps[1].Component);
            Camera.SetZone(collection.Size * collection.TileSize); 
        }

        protected override void BeforeUpdate(float deltaTime)
        {
            Camera.X += Input.Keyboard.GetAxis(Keys.RightArrow, Keys.LeftArrow) * 2f;
            Camera.Y += Input.Keyboard.GetAxis(Keys.DownArrow, Keys.UpArrow) * 2f;
        }
    }
}
