using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks.Demo
{
    public class WaveShader : Shader
    {
        public float Timer
        {
            get { return Effect.Parameters["timer"].GetValueSingle(); }
            set { Effect.Parameters["timer"].SetValue(value); }
        }

        public float Offset
        {
            get { return Effect.Parameters["offset"].GetValueSingle(); }
            set { Effect.Parameters["offset"].SetValue(value); }
        }

        public float Wavelength
        {
            get { return Effect.Parameters["wavelength"].GetValueSingle(); }
            set { Effect.Parameters["wavelength"].SetValue(value); }
        }

        public float Frequency
        {
            get { return Effect.Parameters["frequency"].GetValueSingle(); }
            set { Effect.Parameters["frequency"].SetValue(value); }
        }

        public WaveShader()
            : base()
        {
        }

        public WaveShader(Effect effect)
            : base(effect)
        {
        }

        protected override void Initialize()
        {
            Wavelength = .03f;
            Frequency = 40f;
        }

        public override Shader Clone()
        {
            return new WaveShader(Effect)
            {
                Timer = Timer,
                Offset = Offset,
                Wavelength = Wavelength,
                Frequency = Frequency
            };
        }
    }
}
