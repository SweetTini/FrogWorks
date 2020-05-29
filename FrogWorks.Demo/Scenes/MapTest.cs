using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks.Demo.Scenes
{
    public class MapTest : Scene
    {
        Playfield Field { get; set; }

        Moveable Player { get; set; }

        public MapTest()
            : base()
        {
        }

        protected override void Begin()
        {
            ClearColor = Color.Gray;
            Field = new Playfield(8, 7, 32, 32);
            Player = new Moveable(Field, 160, 144);
            Add(Field, Player);
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            batch.Configure(transformMatrix: Camera.Matrix);
            batch.Begin();
            Tools.Font.Draw(batch, Player.Position.ToString(), Vector2.One * 8, new Vector2(32, 8));
            batch.End();
        }
    }
}
