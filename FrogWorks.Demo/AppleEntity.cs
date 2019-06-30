namespace FrogWorks.Demo
{
    public class AppleEntity : Entity
    {
        public Image Image { get; private set; }

        public AppleEntity()
            : base()
        {
            Image = new Image(Texture.Load("Images\\Apple.png"), false);
            Image.CenterOrigin();
            AddComponent(Image);
        }
    }
}
