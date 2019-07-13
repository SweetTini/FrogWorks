namespace FrogWorks.Demo
{
    public class DefaultFont
    {
        public const string CharacterSet = " !\"\'*+,-./0123456789:;ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static Texture Texture { get; } = Texture.Load(@"Images\MonoFont.png");

        public static BitmapFont Font { get; } = new BitmapFont(Texture, 10, 16, CharacterSet);
    }
}
