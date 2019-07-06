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
                    Input.GamePads[PlayerIndex].IsDown(GamePadButton.DPadRight),
                    Input.GamePads[PlayerIndex].IsDown(GamePadButton.DPadLeft));

                var yAxis = Input.GetAxis(
                    Input.GamePads[PlayerIndex].IsDown(GamePadButton.DPadDown),
                    Input.GamePads[PlayerIndex].IsDown(GamePadButton.DPadUp));

                return new Vector2(xAxis, yAxis);
            }
        }

        public VirtualAnalogDPad(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}
