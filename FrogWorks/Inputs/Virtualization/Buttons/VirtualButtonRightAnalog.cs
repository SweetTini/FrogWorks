namespace FrogWorks
{
    public class VirtualButtonRightAnalog : VirtualButtonNode
    {
        public int PlayerIndex { get; private set; }

        public AnalogAxis Axis { get; private set; }

        public float DeadZone { get; private set; }

        public override bool IsDown => Input.GamePads[PlayerIndex].IsRightAnalogDown(Axis, DeadZone);

        public override bool IsPressed => Input.GamePads[PlayerIndex].IsRightAnalogPressed(Axis, DeadZone);

        public override bool IsReleased => Input.GamePads[PlayerIndex].IsRightAnalogReleased(Axis, DeadZone);

        public VirtualButtonRightAnalog(int playerIndex, AnalogAxis axis, float deadZone)
        {
            PlayerIndex = playerIndex;
            Axis = axis;
            DeadZone = deadZone;
        }
    }
}
