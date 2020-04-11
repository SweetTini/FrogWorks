namespace FrogWorks
{
    public class VirtualAxisRightAnalogVert : VirtualAxisNode
    {
        public int PlayerIndex { get; private set; }

        public float DeadZone { get; private set; }

        public override float Value
        {
            get
            {
                return Input.GamePads[PlayerIndex]
                    .GetRightAnalog().Y
                    .SignThreshold(DeadZone);
            }
        }

        public VirtualAxisRightAnalogVert(int playerIndex, float deadZone)
        {
            PlayerIndex = playerIndex;
            DeadZone = deadZone;
        }
    }
}
