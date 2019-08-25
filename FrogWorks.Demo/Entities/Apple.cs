namespace FrogWorks.Demo.Entities
{
    public class Apple : Entity
    {
        protected static Texture Texture { get; } = Texture.Load(@"Images\Apple.png");

        protected Image Image { get; set; }

        public Apple()
            : base()
        {
            Image = new Image(Texture, true);
            Image.CenterOrigin();
            Components.Add(Image);            
        }
    }
}
