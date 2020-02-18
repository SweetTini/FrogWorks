using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class Alarm : Component
    {
        internal static Stack<Alarm> Cache { get; } = new Stack<Alarm>();

        public float Duration { get; private set; }

        public float TimeLeft { get; private set; }

        public AlarmMode Mode { get; private set; }

        public Action OnFinished { get; set; }

        private Alarm()
            : base(false, false)
        {
        }

        private void Initialize(float duration, AlarmMode mode, Action onFinished, bool canStart)
        {
            IsActive = false;
            Duration = Math.Max(Math.Abs(duration), float.Epsilon);
            TimeLeft = 0f;
            Mode = mode;
            OnFinished = onFinished;

            if (canStart) Start();
        }

        protected override void Update(float deltaTime)
        {
            TimeLeft -= deltaTime;

            if (TimeLeft <= 0f)
            {
                TimeLeft = 0f;
                OnFinished?.Invoke();

                switch (Mode)
                {
                    case AlarmMode.Persist: IsActive = false; break;
                    case AlarmMode.PlayOnce: Destroy(); break;
                    case AlarmMode.Loop: Start(); break;
                }
            }
        }

        protected override void OnRemoved()
        {
            Cache.Push(this);
        }

        public void Start()
        {
            TimeLeft = Duration;
            IsActive = true;
        }

        public void Start(float duration, bool removeOnCompletion = false)
        {
            Duration = Math.Max(Math.Abs(duration), float.Epsilon);
            if (removeOnCompletion)
                Mode = AlarmMode.PlayOnce;
            Start();
        }

        public void Stop()
        {
            IsActive = false;
        }

        #region Static Methods
        public static Alarm Create(Action onFinished, bool canStart = false)
        {
            return Create(1f, AlarmMode.PlayOnce, onFinished, canStart);
        }

        public static Alarm Create(AlarmMode mode, Action onFinished, bool canStart = false)
        {
            return Create(1f, mode, onFinished, canStart);
        }

        public static Alarm Create(float duration, AlarmMode mode, Action onFinished, bool canStart = false)
        {
            var alarm = Cache.Count > 0 ? Cache.Pop() : new Alarm();
            alarm.Initialize(duration, mode, onFinished, canStart);
            return alarm;
        }

        public static Alarm CreateAndApply(Entity entity, Action onFinished)
        {
            return CreateAndApply(entity, 1f, AlarmMode.PlayOnce, onFinished);
        }

        public static Alarm CreateAndApply(Entity entity, float duration, AlarmMode mode, Action onFinished)
        {
            var alarm = Create(duration, mode, onFinished, true);
            if (entity != null)
                entity.Components.Add(alarm);
            return alarm;
        }
        #endregion
    }

    public enum AlarmMode
    {
        Persist,
        PlayOnce,
        Loop
    }
}
