namespace FrogWorks
{
    public class VirtualAxisDPadHoriz : VirtualAxisNode
    {
        public int PlayerIndex { get; private set; }

        public override float Value
        {
            get
            {
                return Input.GetAxis(
                    Input.GamePads[PlayerIndex].IsDown(GamePadButton.DPadLeft),
                    Input.GamePads[PlayerIndex].IsDown(GamePadButton.DPadRight));
            }
        }

        public VirtualAxisDPadHoriz(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}
