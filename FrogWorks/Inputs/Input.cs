namespace FrogWorks
{
    public class Input
    {
        public static KeyboardReader Keyboard { get; private set; }

        public static MouseReader Mouse { get; private set; }

        public static GamePadReader[] GamePads { get; private set; }

        public static bool IsDisabled { get; set; }

        internal static void Initialize()
        {
            Keyboard = new KeyboardReader();
            Mouse = new MouseReader();
            GamePads = new GamePadReader[4];

            for (int i = 0; i < GamePads.Length; i++)
                GamePads[i] = new GamePadReader(i);
        }

        internal static void Close()
        {
        }

        internal static void Update(bool isActive)
        {
            Keyboard.Update(isActive);
            Mouse.Update(isActive);

            for (int i = 0; i < GamePads.Length; i++)
                GamePads[i].Update(isActive);
        }
    }
}
