namespace FrogWorks
{
    public class VirtualAxisLeftAnalogHoriz : VirtualAxisNode
    {
        public int PlayerIndex { get; private set; }

        public float DeadZone { get; private set; }

        public override float Value
        {
            get 
            {
                return Input.GamePads[PlayerIndex]
                    .GetLeftAnalog().X
                    .SignThreshold(DeadZone);
            }
        }

        public VirtualAxisLeftAnalogHoriz(int playerIndex, float deadZone)
        {
            PlayerIndex = playerIndex;
            DeadZone = deadZone;
        }
    }
}
