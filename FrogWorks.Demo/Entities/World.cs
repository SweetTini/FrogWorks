using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class World : Entity
    {
        public World(int columns, int rows, int tileWidth, int tileHeight)
            : base()
        {
            var collider = new BitFlagMapCollider(columns, rows, tileWidth, tileHeight);

            collider.Fill(BitFlag.FlagA, 0, 0, columns, 1);
            collider.Fill(BitFlag.FlagA, 0, 0, 1, rows);
            collider.Fill(BitFlag.FlagA, 0, rows - 1, columns, 1);
            collider.Fill(BitFlag.FlagA, columns - 1, 0, 1, rows);

            Collider = collider;
        }

        protected override void BeforeDraw(RendererBatch batch)
        {
            Collider.Draw(batch, Color.DarkGreen, Color.LimeGreen);
        }
    }
}
