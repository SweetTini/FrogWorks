﻿using Microsoft.Xna.Framework;
using XnaGame = Microsoft.Xna.Framework.Game;

namespace FrogWorks
{
    public class Game : XnaGame
    {
        public GraphicsDeviceManager Graphics { get; set; }

        public Display Display { get; set; }

        public RendererBatch RendererBatch { get; set; }

        public Game(int width, int height)
        {
            Graphics = new GraphicsDeviceManager(this);
            Display = new Display(Graphics, width, height);
            RendererBatch = new RendererBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Display.DrawSceneOnBuffer(null);
            Display.DrawScreen(RendererBatch);
            base.Draw(gameTime);
        }

        protected override void EndRun()
        {
            RendererBatch.Dispose();
            base.EndRun();
        }
    }
}
