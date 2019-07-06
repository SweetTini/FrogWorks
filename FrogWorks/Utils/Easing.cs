using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks.Utils
{
    public static class Easing
    {
        #region Linear
        public static Ease Linear => (float t) => t;
        #endregion

        #region Quadratic
        public static Ease QuadIn => (float t) => t * t;

        public static Ease QuadOut => Invert(QuadIn);

        public static Ease QuadInOut => Follow(QuadIn, QuadOut);
        #endregion

        #region Cubic
        public static Ease CubeIn => (float t) => t * t * t;

        public static Ease CubeOut => Invert(CubeIn);

        public static Ease CubeInOut => Follow(CubeIn, CubeOut);
        #endregion

        #region Quartic
        public static Ease QuartIn => (float t) => t * t * t * t;

        public static Ease QuartOut => Invert(QuartIn);

        public static Ease QuartInOut => Follow(QuartIn, QuartOut);
        #endregion

        #region Quintic
        public static Ease QuintIn => (float t) => t * t * t * t;

        public static Ease QuintOut => Invert(QuintIn);

        public static Ease QuintInOut => Follow(QuintIn, QuintOut);
        #endregion

        #region Sinusoidal
        public static Ease SineIn => (float t) => 1f - (float)Math.Cos(t * MathHelper.PiOver2);

        public static Ease SineOut => (float t) => (float)Math.Sin(t * MathHelper.PiOver2);

        public static Ease SineInOut => (float t) => -((float)Math.Cos(MathHelper.Pi * t) - 1f) / 2f;
        #endregion

        #region Exponential
        public static Ease ExpoIn => (float t) => (float)Math.Pow(2f, 10f * (t - 1));

        public static Ease ExpoOut => Invert(ExpoIn);

        public static Ease ExpoInOut => Follow(ExpoIn, ExpoOut);
        #endregion

        #region Circular
        public static Ease CircIn => (float t) => 1f - (float)Math.Sqrt(1f - Math.Pow(t, 2f));

        public static Ease CircOut => (float t) => (float)Math.Sqrt(1f - Math.Pow(t - 1f, 2f));

        public static Ease CircInOut => Follow(CircIn, CircOut);
        #endregion

        #region Back
        public static Ease BackIn => (float t) => t * t * (2.70158f * t - 1.70158f);

        public static Ease BackOut => Invert(BackIn);

        public static Ease BackInOut => Follow(BackIn, BackOut);
        #endregion

        #region Bounce
        public static Ease BounceIn => Invert(BounceOut);

        public static Ease BounceOut => (float t) =>
        {
            const float n = 7.5625f;
            const float d = 2.75f;

            if (t < 1f / d) return n * t * t;
            if (t < 2f / d) return n * (t -= 1.5f / d) * t + .75f;
            if (t < 2.5f / d) return n * (t -= 2.25f / d) * t + .9375f;

            return n * (t -= 2.625f / d) * t + .984375f;
        };

        public static Ease BounceInOut => Follow(BounceIn, BounceOut);
        #endregion

        #region Elastic
        public static Ease ElasticIn => (float t) =>
        {
            const float p = .3f;
            const float s = p / 4f;

            return -(float)(Math.Pow(2f, 10f * (t--)) * Math.Sin((t - s) * MathHelper.TwoPi / p));
        };

        public static Ease ElasticOut => (float t) =>
        {
            const float p = .3f;
            const float s = p / 4f;

            return (float)(Math.Pow(2f, -10f * t) * Math.Sin((t - s) * MathHelper.TwoPi / p));
        };

        public static Ease ElasticInOut => Follow(ElasticIn, ElasticOut);
        #endregion

        private static Ease Invert(Ease ease)
        {
            return (float time) => 1f - ease(1f - time);
        }

        private static Ease Follow(Ease first, Ease last)
        {
            return (float time) =>
                time <= .5f
                    ? first(time * 2f) * .5f
                    : last(time * 2f - 1f) * .5f + .5f;
        }
    }

    public delegate float Ease(float time);
}
