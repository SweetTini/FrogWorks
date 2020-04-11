namespace FrogWorks
{
    public class VirtualButtonAxisTrigger : VirtualButtonNode
    {
        protected VirtualAxis Axis { get; private set; }

        public float Threshold { get; private set; }

        public ThresholdMode ThresholdMode { get; private set; }

        public override bool IsDown
        {
            get
            {
                switch (ThresholdMode)
                {
                    case ThresholdMode.GreaterThan: 
                        return Axis.CurrentValue >= Threshold;
                    case ThresholdMode.LessThan: 
                        return Axis.CurrentValue <= Threshold;
                    case ThresholdMode.EqualTo: 
                        return Axis.CurrentValue == Threshold;
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
                        return Axis.CurrentValue >= Threshold 
                            && Axis.LastValue < Threshold;
                    case ThresholdMode.LessThan: 
                        return Axis.CurrentValue <= Threshold 
                            && Axis.LastValue > Threshold;
                    case ThresholdMode.EqualTo: 
                        return Axis.CurrentValue == Threshold 
                            && Axis.LastValue != Threshold;
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
                        return Axis.CurrentValue < Threshold 
                            && Axis.LastValue >= Threshold;
                    case ThresholdMode.LessThan: 
                        return Axis.CurrentValue > Threshold 
                            && Axis.LastValue <= Threshold;
                    case ThresholdMode.EqualTo: 
                        return Axis.CurrentValue != Threshold 
                            && Axis.LastValue == Threshold;
                }

                return false;
            }
        }

        public VirtualButtonAxisTrigger(VirtualAxis axis, float threshold)
            : this(axis, threshold, ThresholdMode.GreaterThan)
        {
        }

        public VirtualButtonAxisTrigger(
            VirtualAxis axis, 
            float threshold, 
            ThresholdMode thresholdMode)
        {
            Axis = axis;
            Threshold = threshold;
            ThresholdMode = thresholdMode;
        }
    }
}
