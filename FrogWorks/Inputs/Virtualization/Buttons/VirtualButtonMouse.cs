namespace FrogWorks
{
    public class VirtualButtonMouse : VirtualButtonNode
    {
        public MouseButton Button { get; private set; }

        public override bool IsDown => Input.Mouse.IsDown(Button);

        public override bool IsPressed => Input.Mouse.IsClicked(Button);

        public override bool IsReleased => Input.Mouse.IsReleased(Button);

        public VirtualButtonMouse(MouseButton button)
        {
            Button = button;
        }
    }
}
