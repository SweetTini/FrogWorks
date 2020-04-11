namespace FrogWorks
{
    public class VirtualButtonAnalogHoriz : VirtualButtonNode
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
                    case ThresholdMode.GreaterThan:
                        return Analog.CurrentValue.X >= Threshold;
                    case ThresholdMode.LessThan:
                        return Analog.CurrentValue.X <= Threshold;
                    case ThresholdMode.EqualTo:
                        return Analog.CurrentValue.X == Threshold;
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
                    case ThresholdMode.GreaterThan:
                        return Analog.CurrentValue.X >= Threshold
                            && Analog.LastValue.X < Threshold;
                    case ThresholdMode.LessThan:
                        return Analog.CurrentValue.X <= Threshold
                            && Analog.LastValue.X > Threshold;
                    case ThresholdMode.EqualTo:
                        return Analog.CurrentValue.X == Threshold
                            && Analog.LastValue.X != Threshold;
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
                    case ThresholdMode.GreaterThan:
                        return Analog.CurrentValue.X < Threshold
                            && Analog.LastValue.X >= Threshold;
                    case ThresholdMode.LessThan:
                        return Analog.CurrentValue.X > Threshold
                            && Analog.LastValue.X <= Threshold;
                    case ThresholdMode.EqualTo:
                        return Analog.CurrentValue.X != Threshold
                            && Analog.LastValue.X == Threshold;
                }

                return false;
            }
        }

        public VirtualButtonAnalogHoriz(VirtualAnalog analog, float threshold)
            : this(analog, threshold, ThresholdMode.GreaterThan)
        {
        }

        public VirtualButtonAnalogHoriz(
            VirtualAnalog analog,
            float threshold,
            ThresholdMode thresholdMode)
        {
            Analog = analog;
            Threshold = threshold;
            ThresholdMode = thresholdMode;
        }
    }
}
