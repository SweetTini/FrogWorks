using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public class Tween : Component
    {
        private bool _hasBegunReversed;

        internal static Stack<Tween> Cache { get; } = new Stack<Tween>();

        public Ease Ease { get; set; }

        public float Value { get; private set; }

        public float Duration { get; private set; }

        public float Percent { get; private set; }

        public float TimeLeft { get; private set; }

        public bool IsReversed { get; private set; }

        public TweenMode Mode { get; private set; }

        public Action<Tween> OnStart { get; set; }

        public Action<Tween> OnUpdate { get; set; }

        public Action<Tween> OnFinished { get; set; }

        protected Tween()
            : base(true, false)
        {
        }

        protected void Initialize(Ease ease, float duration, TweenMode mode, bool canStart)
        {
            Ease = ease;
            TimeLeft = Percent = Value = 0f;
            Duration = duration > 0f ? duration : float.Epsilon;
            Mode = mode;
            IsEnabled = false;

            if (canStart) Start();
        }

        public override void Update(float deltaTime)
        {
            TimeLeft -= deltaTime;
            Percent = MathHelper.Clamp(TimeLeft / Duration, 0f, 1f);
            Percent = !IsReversed ? 1f - Percent : Percent;
            Value = Ease?.Invoke(Percent) ?? Percent;

            OnUpdate?.Invoke(this);

            if (TimeLeft <= 0f)
            {
                TimeLeft = 0f;           

                switch (Mode)
                {
                    case TweenMode.Persist:
                        OnFinished?.Invoke(this);
                        IsEnabled = false;
                        break;
                    case TweenMode.PlayOnce:
                        OnFinished?.Invoke(this);
                        IsEnabled = false;
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
                            OnFinished?.Invoke(this);
                            IsEnabled = false;
                            Destroy();
                        }
                        break;
                    case TweenMode.YoyoLoop:
                        Start(!IsReversed);
                        break;
                }
            }
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
            Cache.Push(this);
        }

        public void Start()
        {
            Start(false);
        }

        public void Start(bool reverse)
        {
            IsReversed = _hasBegunReversed = reverse;
            TimeLeft = Duration;
            Value = Percent = IsReversed ? 1f : 0f;

            if (!IsEnabled)
            {
                OnStart?.Invoke(this);
                IsEnabled = true;
            }
        }

        public void Start(float duration, bool reverse)
        {
            if (duration > 0f)
            {
                Duration = duration;
                Start(reverse);
            }
        }

        public void Stop()
        {
            IsEnabled = false;
        }

        public void Reset()
        {
            TimeLeft = Duration;
            Value = Percent = IsReversed ? 1f : 0f;
        }

        public IEnumerator Wait()
        {
            while (IsEnabled) yield return 0;
        }

        #region Static Methods
        public static Tween Create(Ease ease, float duration, TweenMode mode = TweenMode.Persist, bool canStart = false)
        {
            var tween = Cache.Count > 0 ? Cache.Pop() : new Tween();
            tween.OnStart = tween.OnUpdate = tween.OnFinished = null;
            tween.Initialize(ease, duration, mode, canStart);
            return tween;
        }

        public static Tween CreateAndApply(Entity entity, Ease ease, float duration, TweenMode mode, Action<Tween> onUpdate, Action<Tween> onStart = null, Action<Tween> onFinished = null)
        {
            var tween = Create(ease, duration, mode, true);
            tween.OnUpdate += onUpdate;
            tween.OnStart += onStart;
            tween.OnFinished += onFinished;
            entity.AddComponents(tween);
            return tween;
        }

        public static Tween Move(Entity entity, Vector2 target, Ease ease, float duration, TweenMode mode = TweenMode.PlayOnce)
        {
            var position = entity.Position;
            var tween = Create(ease, duration, mode, true);
            tween.OnUpdate = (t) => entity.Position = Vector2.Lerp(position, target, t.Value);
            entity.AddComponents(tween);
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
