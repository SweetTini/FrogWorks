using Microsoft.Xna.Framework;
using XnaGame = Microsoft.Xna.Framework.Game;

namespace FrogWorks
{
    public class Game : XnaGame
    {
        public GraphicsDeviceManager Graphics { get; set; }

        public Display Display { get; set; }

        public RendererBatch RenderBatch { get; set; }

        public Game(int width, int height)
        {
            Graphics = new GraphicsDeviceManager(this);
            Display = new Display(Graphics, width, height);
            RenderBatch = new RendererBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Display.DrawSceneOnBuffer(null);
            Display.DrawScreen(RenderBatch);
            base.Draw(gameTime);
        }

        protected override void EndRun()
        {
            RenderBatch.Dispose();
            base.EndRun();
        }
    }
}
