namespace FrogWorks
{
    public class VirtualAxisDPadVert : VirtualAxisNode
    {
        public int PlayerIndex { get; private set; }

        public override float Value
        {
            get
            {
                return Input.GetAxis(
                    Input.GamePads[PlayerIndex].IsDown(GamePadButton.DPadDown),
                    Input.GamePads[PlayerIndex].IsDown(GamePadButton.DPadUp));
            }
        }

        public VirtualAxisDPadVert(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}
