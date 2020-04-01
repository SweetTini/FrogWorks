using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class VirtualAnalogDPad : VirtualAnalogNode
    {
        public int PlayerIndex { get; private set; }

        public override Vector2 Value
        {
            get
            {
                var xAxis = Input.GetAxis(
                    Input.GamePads[PlayerIndex].IsDown(GamePadButton.DPadLeft),
                    Input.GamePads[PlayerIndex].IsDown(GamePadButton.DPadRight));

                var yAxis = Input.GetAxis(
                    Input.GamePads[PlayerIndex].IsDown(GamePadButton.DPadUp),
                    Input.GamePads[PlayerIndex].IsDown(GamePadButton.DPadDown));

                return new Vector2(xAxis, yAxis);
            }
        }

        public VirtualAnalogDPad(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}
