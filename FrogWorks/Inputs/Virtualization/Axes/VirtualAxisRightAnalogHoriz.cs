namespace FrogWorks
{
    public class VirtualAxisRightAnalogHoriz : VirtualAxisNode
    {
        public int PlayerIndex { get; private set; }

        public float DeadZone { get; private set; }

        public override float Value => Input.GamePads[PlayerIndex].GetRightAnalog().X.SignThreshold(DeadZone);

        public VirtualAxisRightAnalogHoriz(int playerIndex, float deadZone)
        {
            PlayerIndex = playerIndex;
            DeadZone = deadZone;
        }
    }
}
