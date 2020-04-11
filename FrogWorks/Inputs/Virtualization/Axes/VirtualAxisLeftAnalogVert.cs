namespace FrogWorks
{
    public class VirtualAxisLeftAnalogVert : VirtualAxisNode
    {
        public int PlayerIndex { get; private set; }

        public float DeadZone { get; private set; }

        public override float Value
        {
            get
            {
                return Input.GamePads[PlayerIndex]
                    .GetLeftAnalog().Y
                    .SignThreshold(DeadZone);
            }
        }

        public VirtualAxisLeftAnalogVert(int playerIndex, float deadZone)
        {
            PlayerIndex = playerIndex;
            DeadZone = deadZone;
        }
    }
}
