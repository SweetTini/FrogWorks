using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public class Tween : Component
    {
        bool _hasBegunReversed;

        internal static Stack<Tween> Cache { get; } = new Stack<Tween>();

        public Ease Ease { get; set; }

        public float Value { get; private set; }

        public float Duration { get; private set; }

        public float Percent { get; private set; }

        public float TimeLeft { get; private set; }

        public bool IsReversed { get; private set; }

        public TweenMode Mode { get; private set; }

        public Action<Tween> OnUpdate { get; set; }

        public Action<Tween> OnBegin { get; set; }

        public Action<Tween> OnEnd { get; set; }

        Tween()
            : base(true, false)
        {
        }

        void Initialize(
            Ease ease,
            float duration,
            TweenMode mode,
            bool canStart)
        {
            Ease = ease;
            Duration = Math.Max(Math.Abs(duration), float.Epsilon);
            TimeLeft = Percent = Value = 0f;
            Mode = mode;
            IsActive = false;

            if (canStart) Start();
        }

        protected override void Update(float deltaTime)
        {
            TimeLeft -= deltaTime;
            Percent = MathHelper.Clamp(TimeLeft / Duration, 0f, 1f);
            Percent = !IsReversed ? 1f - Percent : Percent;
            Value = Ease?.Invoke(Percent) ?? Percent;
            OnUpdate?.Invoke(this);

            if (TimeLeft <= 0f)
            {
                TimeLeft = 0f;
                OnEnd?.Invoke(this);

                switch (Mode)
                {
                    case TweenMode.Persist:
                        IsActive = false;
                        break;
                    case TweenMode.PlayOnce:
                        IsActive = false;
                        Destroy();
                        break;
                    case TweenMode.Loop:
                        Start(IsReversed);
                        break;
                    case TweenMode.YoyoOnce:
                        if (IsReversed == _hasBegunReversed)
                        {
                            Start(!IsReversed);
                            _hasBegunReversed = !IsReversed;
                        }
                        else
                        {
                            IsActive = false;
                            Destroy();
                        }
                        break;
                    case TweenMode.YoyoLoop:
                        Start(!IsReversed);
                        break;
                }
            }
        }

        protected override void OnRemoved() => Cache.Push(this);

        public void Start()
        {
            Start(false);
        }

        public void Start(bool reverse)
        {
            IsReversed = _hasBegunReversed = reverse;
            TimeLeft = Duration;
            Value = Percent = IsReversed ? 1f : 0f;
            IsActive = true;
            OnBegin?.Invoke(this);
        }

        public void Start(float duration, bool reverse)
        {
            Duration = Math.Max(Math.Abs(duration), float.Epsilon);
            Start(reverse);
        }

        public void Stop()
        {
            IsActive = false;
        }

        public void Reset()
        {
            TimeLeft = Duration;
            Value = Percent = IsReversed ? 1f : 0f;
        }

        public IEnumerator Wait()
        {
            while (IsActive) yield return 0;
        }

        #region Static Methods
        public static Tween Create(
            Ease ease,
            float duration,
            TweenMode mode = TweenMode.Persist,
            bool canStart = false)
        {
            var tween = Cache.Count > 0 ? Cache.Pop() : new Tween();
            tween.OnUpdate = tween.OnBegin = tween.OnEnd = null;
            tween.Initialize(ease, duration, mode, canStart);
            return tween;
        }

        public static Tween CreateAndApply(
            Entity entity,
            Ease ease,
            float duration,
            TweenMode mode,
            Action<Tween> onUpdate,
            Action<Tween> onBegin = null,
            Action<Tween> onEnd = null)
        {
            var tween = Create(ease, duration, mode, true);
            tween.OnUpdate += onUpdate;
            tween.OnBegin += onBegin;
            tween.OnEnd += onEnd;
            if (entity != null)
                entity.Components.Add(tween);
            return tween;
        }

        public static Tween Move(
            Entity entity,
            Vector2 target,
            Ease ease,
            float duration,
            TweenMode mode = TweenMode.PlayOnce)
        {
            var tween = Create(ease, duration, mode, true);

            if (entity != null)
            {
                var position = entity.Position;

                tween.OnUpdate = (t) =>
                {
                    entity.Position = Vector2.Lerp(position, target, t.Value);
                };

                entity.Components.Add(tween);
            }

            return tween;
        }
        #endregion
    }

    public enum TweenMode
    {
        Persist,
        PlayOnce,
        Loop,
        YoyoOnce,
        YoyoLoop
    }
}
