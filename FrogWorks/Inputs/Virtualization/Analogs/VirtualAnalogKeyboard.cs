using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class VirtualAnalogKeyboard : VirtualAnalogNode
    {
        private Vector2 _value;
        private bool _isFlippedHorizontally, _isFlippedVertically;

        public Keys Left { get; private set; }

        public Keys Right { get; private set; }

        public Keys Up { get; private set; }

        public Keys Down { get; private set; }

        public OverlapMode OverlapMode { get; private set; }

        public override Vector2 Value => _value;

        public VirtualAnalogKeyboard(Keys left, Keys right, Keys up, Keys down)
                : this(left, right, up, down, OverlapMode.Cancel)
            {
        }

        public VirtualAnalogKeyboard(Keys left, Keys right, Keys up, Keys down, OverlapMode overlapMode)
        {
            Left = left;
            Right = right;
            Up = up;
            Down = down;
            OverlapMode = overlapMode;
        }

        public override void Update(float deltaTime)
        {
            _value = new Vector2(
                GetAxis(Right, Left, ref _value.X, ref _isFlippedHorizontally),
                GetAxis(Down, Up, ref _value.Y, ref _isFlippedVertically));
        }

        private float GetAxis(Keys positive, Keys negative, ref float nextValue, ref bool isFlipped)
        {
            var lastValue = nextValue;
            var posValue = Input.Keyboard.IsDown(positive) ? 1f : 0f;
            var negValue = Input.Keyboard.IsDown(negative) ? 1f : 0f;

            if (posValue + negValue > 1f)
            {
                switch (OverlapMode)
                {
                    default:
                    case OverlapMode.Cancel:
                        lastValue = 0f;
                        break;
                    case OverlapMode.TakeLatest:
                        if (!isFlipped)
                        {
                            lastValue *= -1f;
                            isFlipped = true;
                        }
                        break;
                    case OverlapMode.TakeOldest:
                        break;
                }

                nextValue = lastValue;
            }
            else
            {
                nextValue = posValue - negValue;
                isFlipped = false;
            }

            return nextValue;
        }
    }
}
