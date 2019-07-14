using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class Wiggler : Component
    {
        private float _sineCounter, _wavelength, _increment;

        internal static Stack<Wiggler> Cache { get; } = new Stack<Wiggler>();

        public float Value { get; private set; }

        public float Counter { get; private set; }

        public WigglerMode Mode { get; private set; }

        public bool StartAtZero { get; set; }

        public bool RemoveOnCompletion { get; set; }

        public Action<float> OnUpdate { get; set; }

        public Action<float> OnFinished { get; set; }

        private Wiggler()
            : base(true, false)
        {
        }

        private void Initialize(float duration, float frequency, WigglerMode mode, Action<float> onUpdate, Action<float> onFinished, bool removeOnCompletion, bool canStart = false)
        {
            _wavelength = MathHelper.TwoPi * frequency;
            _increment = 1f / Math.Max(Math.Abs(duration), float.Epsilon);
            Counter = _sineCounter = 0f;
            Mode = mode;
            OnUpdate = onUpdate;
            OnFinished = onFinished;
            RemoveOnCompletion = removeOnCompletion;
            IsDestroyed = IsEnabled = false;

            if (canStart) Start();
        }

        public override void Update(float deltaTime)
        {
            _sineCounter += _wavelength * deltaTime;
            Counter -= _increment * deltaTime;

            switch (Mode)
            {
                case WigglerMode.Linear:
                    Value = (float)Math.Cos(_sineCounter);
                    break;
                case WigglerMode.EaseIn:
                    Value = (float)Math.Cos(_sineCounter) * (1f - Counter);
                    break;
                case WigglerMode.EaseOut:
                    Value = (float)Math.Cos(_sineCounter) * Counter;
                    break;
                case WigglerMode.EaseInOut:
                    Value = Counter <= .5f
                        ? (float)Math.Cos(_sineCounter) * Counter
                        : (float)Math.Cos(_sineCounter) * (1f - Counter);
                    break;
            }

            OnUpdate?.Invoke(Value);

            if (Counter <= 0f)
            {
                Counter = 0f;
                OnFinished?.Invoke(Counter);
                IsEnabled = false;
                if (RemoveOnCompletion)
                    Destroy();
            }
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
            Cache.Push(this);
        }

        public void Start()
        {
            _sineCounter = StartAtZero ? MathHelper.PiOver2 : 0f;
            Counter = 1f;
            Value = StartAtZero ? 0f : 1f;
            OnUpdate?.Invoke(Value);
            IsEnabled = true;
        }

        public void Start(float duration, float frequency, WigglerMode mode = WigglerMode.Linear, bool removeOnCompletion = false)
        {
            _wavelength = MathHelper.TwoPi * frequency;
            _increment = 1f / Math.Max(Math.Abs(duration), float.Epsilon);
            Mode = mode;
            RemoveOnCompletion = removeOnCompletion;
            Start();
        }

        public void Stop()
        {
            IsEnabled = false;
        }

        #region Static Methods
        public static Wiggler Create(float duration, float frequency, WigglerMode mode, Action<float> onUpdate, Action<float> onFinished, bool removeOnCompletion, bool canStart = false)
        {
            var wiggler = Cache.Count > 0 ? Cache.Pop() : new Wiggler();
            wiggler.Initialize(duration, frequency, mode, onUpdate, onFinished, removeOnCompletion, canStart);
            return wiggler;
        }

        public static Wiggler CreateAndApply(Entity entity, float duration, float frequency, Action<float> onUpdate, Action<float> onFinished = null, bool removeOnCompletion = true)
        {
            return CreateAndApply(entity, duration, frequency, WigglerMode.Linear, onUpdate, onFinished, removeOnCompletion);
        }

        public static Wiggler CreateAndApply(Entity entity, float duration, float frequency, WigglerMode mode, Action<float> onUpdate, Action<float> onFinished = null, bool removeOnCompletion = true)
        {
            var wiggler = Create(duration, frequency, mode, onUpdate, onFinished, removeOnCompletion, true);
            if (entity != null)
                entity.AddComponents(wiggler);
            return wiggler;
        }
        #endregion
    }

    public enum WigglerMode
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }
}
