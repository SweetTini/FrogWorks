namespace FrogWorks.Demo.Entities
{
    public class AppleEntity : Entity
    {
        protected static Texture Texture { get; } = Texture.Load(@"Images\Apple.png");

        protected Image Image { get; set; }

        public AppleEntity()
            : base()
        {
            Image = new Image(Texture, true);
            Image.CenterOrigin();
            AddComponents(Image);
        }
    }
}
