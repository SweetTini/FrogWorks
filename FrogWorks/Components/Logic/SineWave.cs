using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public class SineWave : Component
    {
        float _counter;

        public float Counter
        {
            get { return _counter; }
            set { _counter = value.Mod(MathHelper.TwoPi * 4f); }
        }

        public float Frequency { get; set; } = 1f;

        public float Rate { get; set; } = 1f;

        public float Value => (float)Math.Sin(_counter);

        public float TwoValue => (float)Math.Sin(_counter * 2f);

        public float ValueOverTwo => (float)Math.Sin(_counter / 2f);

        public Action<float> OnUpdate { get; set; }

        public SineWave()
            : base(true, false) { }

        public SineWave(float frequency)
            : this()
        {
            Frequency = frequency;
        }

        protected override void Update(float deltaTime)
        {
            Counter += MathHelper.TwoPi * Frequency * Rate * deltaTime;
            OnUpdate?.Invoke(Value);
        }

        public float OffsetValue(float offset)
        {
            return (float)Math.Sin(_counter + offset);
        }

        public void StartUpwards()
        {
            Counter = MathHelper.PiOver2;
        }

        public void StartDownwards()
        {
            Counter = MathHelper.PiOver2 * 3f;
        }

        public void Reset()
        {
            Counter = 0f;
        }

        public SineWave Randomize()
        {
            Counter = RandomEX.Current.NextFloat() * MathHelper.TwoPi * 2f;
            return this;
        }
    }
}
