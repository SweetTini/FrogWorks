using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class ShakerSet : Component
    {
        private bool _isActive;

        internal static Stack<ShakerSet> Cache { get; } = new Stack<ShakerSet>();

        public Vector2[] Values { get; private set; }

        public float Duration { get; private set; }

        public float TimeLeft { get; private set; }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (value == _isActive) return;
                _isActive = value;

                if (!_isActive)
                {
                    ResetValues();
                    OnShake?.Invoke(Values);
                }
            }
        }

        public bool RemoveOnCompletion { get; set; }

        public Action<Vector2[]> OnShake { get; set; }

        private ShakerSet()
            : base(true, false)
        {
        }

        private void Initialize(int length, float duration, Action<Vector2[]> onShake, bool removeOnCompletion, bool canActivate = false)
        {
            Values = new Vector2[Math.Max(Math.Abs(length), 1)];
            ResetValues();

            Duration = Math.Max(Math.Abs(duration), float.Epsilon);
            TimeLeft = 0f;
            OnShake = onShake;
            RemoveOnCompletion = removeOnCompletion;
            IsDestroyed = false;
            _isActive = false;

            if (canActivate) Activate();
        }

        public override void Update(float deltaTime)
        {
            if (!_isActive) return;

            TimeLeft -= deltaTime;

            if (TimeLeft <= 0f)
            {
                TimeLeft = 0f;
                ResetValues();
                OnShake?.Invoke(Values);
                if (RemoveOnCompletion)
                    Destroy();
                return;
            }

            for (int i = 0; i < Values.Length; i++)
                Values[i] = new Vector2(Randomizer.Current.NextFloat(-1f, 1f), Randomizer.Current.NextFloat(-1f, 1f)).Round();
            OnShake?.Invoke(Values);
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
            Cache.Push(this);
        }

        public void Activate()
        {
            TimeLeft = Duration;
            _isActive = true;
        }

        public void Activate(float duration, bool removeOnCompletion)
        {
            Duration = Math.Max(Math.Abs(duration), float.Epsilon);
            RemoveOnCompletion = removeOnCompletion;
            Activate();
        }

        private void ResetValues()
        {
            for (int i = 0; i < Values.Length; i++)
                Values[i] = Vector2.Zero;
        }

        #region Static Methods
        public static ShakerSet Create(int length, float duration, Action<Vector2[]> onShake, bool removeOnCompletion, bool canActivate = false)
        {
            var shaker = Cache.Count > 0 ? Cache.Pop() : new ShakerSet();
            shaker.Initialize(length, duration, onShake, removeOnCompletion, canActivate);
            return shaker;
        }

        public static ShakerSet CreateAndApply(Entity entity, int length, float duration, Action<Vector2[]> onShake, bool removeOnCompletion = true)
        {
            var shaker = Create(length, duration, onShake, removeOnCompletion, true);
            entity.AddComponents(shaker);
            return shaker;
        }
        #endregion
    }
}
