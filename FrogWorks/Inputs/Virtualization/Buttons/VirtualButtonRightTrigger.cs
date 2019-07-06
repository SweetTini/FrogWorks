namespace FrogWorks
{
    public class VirtualButtonRightTrigger : VirtualButtonNode
    {
        public int PlayerIndex { get; private set; }

        public float Threshold { get; private set; }

        public override bool IsDown => Input.GamePads[PlayerIndex].IsRightTriggerDown(Threshold);

        public override bool IsPressed => Input.GamePads[PlayerIndex].IsRightTriggerPressed(Threshold);

        public override bool IsReleased => Input.GamePads[PlayerIndex].IsRightTriggerReleased(Threshold);

        public VirtualButtonRightTrigger(int playerIndex, float threshold)
        {
            PlayerIndex = playerIndex;
            Threshold = threshold;
        }
    }
}
