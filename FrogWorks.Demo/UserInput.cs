namespace FrogWorks.Demo
{
    public class UserInput
    {
        public static VirtualAxis LeftRightAxis { get; private set; }

        public static VirtualButton JumpButton { get; private set; }

        public static void Initialize()
        {
            LeftRightAxis = new VirtualAxis(
                new VirtualAxisKeyboard(Keys.RightArrow, Keys.LeftArrow),
                new VirtualAxisLeftAnalogHoriz(0, .9f));

            JumpButton = new VirtualButton(
                new VirtualButtonKeyboard(Keys.Z),
                new VirtualButtonGamePad(0, GamePadButton.A));

            Input.VirtualInputs.Add(LeftRightAxis);
            Input.VirtualInputs.Add(JumpButton);
        }
    }
}
