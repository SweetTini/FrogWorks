using System;

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

        public static int GetAxis(bool positive, bool negative, int both = 0)
        {
            var posValue = positive ? 1 : 0;
            var negValue = negative ? 1 : 0;

            return (posValue + negValue) > 1 ? both : posValue - negValue;
        }

        public static int GetAxis(float analog, float deadZone)
        {
            return Math.Abs(analog) >= deadZone ? Math.Sign(analog) : 0;
        }

        public static int GetAxis(bool positive, bool negative, float analog, float deadZone, int both = 0)
        {
            var axis = GetAxis(analog, deadZone);
            return axis == 0 ? GetAxis(positive, negative, both) : axis;
        }
    }
}
