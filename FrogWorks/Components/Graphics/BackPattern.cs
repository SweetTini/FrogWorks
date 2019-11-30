namespace FrogWorks
{
    public class BackPattern : TiledGraphicsComponent
    {
        public BackPattern(Texture texture)
            : this(texture, false)
        {
        }

        protected BackPattern(Texture texture, bool isEnabled)
            : base(isEnabled)
        {
            Texture = texture;
            TileSize = texture.Size;
            WrapHorizontally = true;
            WrapVertically = true;
        }

        protected override Texture GetTile(int x, int y)
        {
            if (!WrapHorizontally && x != 0) return null;
            if (!WrapVertically && y != 0) return null;

            return Texture;
        }
    }
}
