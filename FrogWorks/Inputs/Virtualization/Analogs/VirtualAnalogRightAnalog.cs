using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class VirtualAnalogRightAnalog : VirtualAnalogNode
    {
        public int PlayerIndex { get; private set; }

        public float DeadZone { get; private set; }

        public override Vector2 Value => Input.GamePads[PlayerIndex].GetRightAnalog(DeadZone);

        public VirtualAnalogRightAnalog(int playerIndex, float deadZone)
        {
            PlayerIndex = playerIndex;
            DeadZone = deadZone;
        }
    }
}
