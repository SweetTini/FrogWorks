namespace FrogWorks
{
    public class VirtualButtonRightTrigger : VirtualButtonNode
    {
        public int PlayerIndex { get; private set; }

        public float Threshold { get; private set; }

        public override bool IsDown
        {
            get
            {
                return Input.GamePads[PlayerIndex]
                    .IsRightTriggerDown(Threshold);
            }
        }

        public override bool IsPressed
        {
            get
            {
                return Input.GamePads[PlayerIndex]
                    .IsRightTriggerPressed(Threshold);
            }
        }

        public override bool IsReleased
        {
            get
            {
                return Input.GamePads[PlayerIndex]
                    .IsRightTriggerReleased(Threshold);
            }
        }

        public VirtualButtonRightTrigger(int playerIndex, float threshold)
        {
            PlayerIndex = playerIndex;
            Threshold = threshold;
        }
    }
}
