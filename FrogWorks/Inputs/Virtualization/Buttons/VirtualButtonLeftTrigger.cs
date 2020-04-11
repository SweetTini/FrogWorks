namespace FrogWorks
{
    public class VirtualButtonLeftTrigger : VirtualButtonNode
    {
        public int PlayerIndex { get; private set; }

        public float Threshold { get; private set; }

        public override bool IsDown
        {
            get
            {
                return Input.GamePads[PlayerIndex]
                    .IsLeftTriggerDown(Threshold);
            }
        }

        public override bool IsPressed
        {
            get
            {
                return Input.GamePads[PlayerIndex]
                    .IsLeftTriggerPressed(Threshold);
            }
        }

        public override bool IsReleased
        {
            get
            {
                return Input.GamePads[PlayerIndex]
                    .IsLeftTriggerReleased(Threshold);
            }
        }

        public VirtualButtonLeftTrigger(int playerIndex, float threshold)
        {
            PlayerIndex = playerIndex;
            Threshold = threshold;
        }
    }
}
