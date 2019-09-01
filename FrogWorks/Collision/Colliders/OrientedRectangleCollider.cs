namespace FrogWorks
{
    public class OrientedRectangleCollider : PolygonCollider
    {
        public OrientedRectangleCollider(float width, float height, float x = 0, float y = 0) 
            : base(new RectangleF(0, 0, width, height).ToVertices(), x, y)
        {
        }
    }
}
