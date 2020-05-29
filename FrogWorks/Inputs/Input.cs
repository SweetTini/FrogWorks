using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public static class Input
    {
        static GamePadReader[] _gamePads;

        internal static List<VirtualInput> VirtualInputs { get; private set; }

        public static KeyboardReader Keyboard { get; private set; }

        public static MouseReader Mouse { get; private set; }

        public static ReadOnlyCollection<GamePadReader> GamePads { get; private set; }

        public static bool IsDisabled { get; set; }

        internal static void Initialize()
        {
            _gamePads = new GamePadReader[4];

            VirtualInputs = new List<VirtualInput>();
            Keyboard = new KeyboardReader();
            Mouse = new MouseReader();
            GamePads = new ReadOnlyCollection<GamePadReader>(_gamePads);

            for (int i = 0; i < _gamePads.Length; i++)
                _gamePads[i] = new GamePadReader(i);
        }

        internal static void Dispose()
        {
        }

        internal static void Update(bool isActive, float deltaTime)
        {
            Keyboard.Update(isActive);
            Mouse.Update(isActive);

            for (int i = 0; i < _gamePads.Length; i++)
                _gamePads[i].Update(isActive);

            for (int i = 0; i < VirtualInputs.Count; i++)
                VirtualInputs[i].Update(deltaTime);
        }

        public static int GetAxis(bool negative, bool positive, int both = 0)
        {
            var negValue = negative ? 1 : 0;
            var posValue = positive ? 1 : 0;
            return (negValue + posValue) > 1 ? both : posValue - negValue;
        }

        public static int GetAxis(float analog, float deadZone)
        {
            return Math.Abs(analog) >= deadZone ? Math.Sign(analog) : 0;
        }

        public static int GetAxis(
            bool negative,
            bool positive,
            float analog,
            float deadZone,
            int both = 0)
        {
            var axis = GetAxis(analog, deadZone);
            return axis == 0 ? GetAxis(negative, positive, both) : axis;
        }

        internal static int SignThreshold(this float amount, float threshold)
        {
            return Math.Abs(amount) >= threshold
                ? Math.Sign(amount) : 0;
        }

        internal static Vector2 SnapAngle(this Vector2 vec, float segments)
        {
            segments = Math.Abs(segments);

            if (segments <= float.Epsilon)
                segments = 1f;

            var divider = MathHelper.Pi / segments;
            var angle = (float)Math.Floor(
                (vec.ToAngle() + divider / 2f) / divider) * divider;

            return angle.FromAngle(vec.Length());
        }

        internal static Vector2 SnapAndNormalizeAngle(this Vector2 vec, float segments)
        {
            segments = Math.Abs(segments);

            if (segments <= float.Epsilon)
                segments = 1f;

            var divider = MathHelper.Pi / (segments > 0f ? segments : 1f);
            var angle = (float)Math.Floor(
                (vec.ToAngle() + divider / 2f) / divider) * divider;

            return angle.FromAngle(1f);
        }
    }
}
