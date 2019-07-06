namespace FrogWorks
{
    public class VirtualButtonAnalogVert : VirtualButtonNode
    {
        protected VirtualAnalog Analog { get; private set; }

        public float Threshold { get; private set; }

        public ThresholdMode ThresholdMode { get; private set; }

        public override bool IsDown
        {
            get
            {
                switch (ThresholdMode)
                {
                    case ThresholdMode.GreaterThan: return Analog.CurrentValue.Y >= Threshold;
                    case ThresholdMode.LessThan: return Analog.CurrentValue.Y <= Threshold;
                    case ThresholdMode.EqualTo: return Analog.CurrentValue.Y == Threshold;
                }

                return false;
            }
        }

        public override bool IsPressed
        {
            get
            {
                switch (ThresholdMode)
                {
                    case ThresholdMode.GreaterThan: return Analog.CurrentValue.Y >= Threshold && Analog.LastValue.Y < Threshold;
                    case ThresholdMode.LessThan: return Analog.CurrentValue.Y <= Threshold && Analog.LastValue.Y > Threshold;
                    case ThresholdMode.EqualTo: return Analog.CurrentValue.Y == Threshold && Analog.LastValue.Y != Threshold;
                }

                return false;
            }
        }

        public override bool IsReleased
        {
            get
            {
                switch (ThresholdMode)
                {
                    case ThresholdMode.GreaterThan: return Analog.CurrentValue.Y < Threshold && Analog.LastValue.Y >= Threshold;
                    case ThresholdMode.LessThan: return Analog.CurrentValue.Y > Threshold && Analog.LastValue.Y <= Threshold;
                    case ThresholdMode.EqualTo: return Analog.CurrentValue.Y != Threshold && Analog.LastValue.Y == Threshold;
                }

                return false;
            }
        }

        public VirtualButtonAnalogVert(VirtualAnalog analog, float threshold)
            : this(analog, threshold, ThresholdMode.GreaterThan)
        {
        }

        public VirtualButtonAnalogVert(VirtualAnalog analog, float threshold, ThresholdMode thresholdMode)
        {
            Analog = analog;
            Threshold = threshold;
            ThresholdMode = thresholdMode;
        }
    }
}
