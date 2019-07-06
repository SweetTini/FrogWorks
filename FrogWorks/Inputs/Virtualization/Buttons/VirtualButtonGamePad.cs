namespace FrogWorks
{
    public class VirtualButtonGamePad : VirtualButtonNode
    {
        public int PlayerIndex { get; private set; }

        public GamePadButton Button { get; private set; }

        public override bool IsDown => Input.GamePads[PlayerIndex].IsDown(Button);

        public override bool IsPressed => Input.GamePads[PlayerIndex].IsPressed(Button);

        public override bool IsReleased => Input.GamePads[PlayerIndex].IsReleased(Button);

        public VirtualButtonGamePad(int playerIndex, GamePadButton button)
        {
            PlayerIndex = playerIndex;
            Button = button;
        }
    }
}
