namespace FrogWorks
{
    public class VirtualAxisKeyboard : VirtualAxisNode
    {
        float _value;
        bool _isFlipped;

        public Keys Positive { get; private set; }

        public Keys Negative { get; private set; }

        public OverlapMode OverlapMode { get; private set; }

        public override float Value => _value;

        public VirtualAxisKeyboard(Keys positive, Keys negative)
            : this(positive, negative, OverlapMode.Cancel)
        {
        }

        public VirtualAxisKeyboard(Keys positive, Keys negative, OverlapMode overlapMode)
        {
            Positive = positive;
            Negative = negative;
            OverlapMode = overlapMode;
        }

        public override void Update(float deltaTime)
        {
            var lastValue = _value;
            var posValue = Input.Keyboard.IsDown(Positive) ? 1f : 0f;
            var negValue = Input.Keyboard.IsDown(Negative) ? 1f : 0f;

            if (posValue + negValue > 1f)
            {
                switch (OverlapMode)
                {
                    default:
                    case OverlapMode.Cancel:
                        lastValue = 0f;
                        break;
                    case OverlapMode.TakeLatest:
                        if (!_isFlipped)
                        {
                            lastValue *= -1f;
                            _isFlipped = true;
                        }
                        break;
                    case OverlapMode.TakeOldest:
                        break;
                }

                _value = lastValue;
            }
            else
            {
                _value = posValue - negValue;
                _isFlipped = false;
            }
        }
    }
}
