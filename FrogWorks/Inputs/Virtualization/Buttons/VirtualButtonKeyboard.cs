namespace FrogWorks
{
    public class VirtualButtonKeyboard : VirtualButtonNode
    {
        public Keys Key { get; private set; }

        public override bool IsDown => Input.Keyboard.IsDown(Key);

        public override bool IsPressed => Input.Keyboard.IsPressed(Key);

        public override bool IsReleased => Input.Keyboard.IsReleased(Key);

        public VirtualButtonKeyboard(Keys key)
        {
            Key = key;
        }
    }
}
