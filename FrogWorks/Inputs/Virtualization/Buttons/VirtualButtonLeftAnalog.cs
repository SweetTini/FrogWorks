namespace FrogWorks
{
    public class VirtualButtonLeftAnalog : VirtualButtonNode
    {
        public int PlayerIndex { get; private set; }

        public AnalogAxis Axis { get; private set; }

        public float DeadZone { get; private set; }

        public override bool IsDown
        {
            get
            {
                return Input.GamePads[PlayerIndex]
                    .IsLeftAnalogDown(Axis, DeadZone);
            }
        }

        public override bool IsPressed
        {
            get
            {
                return Input.GamePads[PlayerIndex]
                    .IsLeftAnalogPressed(Axis, DeadZone);
            }
        }

        public override bool IsReleased
        {
            get
            {
                return Input.GamePads[PlayerIndex]
                    .IsLeftAnalogReleased(Axis, DeadZone);
            }
        }

        public VirtualButtonLeftAnalog(int playerIndex, AnalogAxis axis, float deadZone)
        {
            PlayerIndex = playerIndex;
            Axis = axis;
            DeadZone = deadZone;
        }
    }
}
