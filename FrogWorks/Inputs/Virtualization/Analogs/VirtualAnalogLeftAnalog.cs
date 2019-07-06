using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class VirtualAnalogLeftAnalog : VirtualAnalogNode
    {
        public int PlayerIndex { get; private set; }

        public float DeadZone { get; private set; }

        public override Vector2 Value => Input.GamePads[PlayerIndex].GetLeftAnalog(DeadZone);

        public VirtualAnalogLeftAnalog(int playerIndex, float deadZone)
        {
            PlayerIndex = playerIndex;
            DeadZone = deadZone;
        }
    }
}
