namespace FrogWorks.Demo.Entities
{
    public class TileMapRenderer : Entity
    {
        public TileMapRenderer()
            : base() { }

        public void Add(TileMap tileMap) => Components.Add(tileMap);
    }
}
