using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks.Components.Logic
{
    public class Shaker : Component
    {
        private bool _isActive;

        internal static Stack<Shaker> Cache { get; } = new Stack<Shaker>();

        public Vector2 Offset { get; private set; }

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
                    Offset = Vector2.Zero;
                    OnShake?.Invoke(Offset);
                }
            }
        }

        public bool RemoveOnCompletion { get; set; }

        public Action<Vector2> OnShake { get; set; }

        private Shaker()
            : base(true, false)
        {
        }

        private void Initialize(float duration, Action<Vector2> onShake, bool removeOnCompletion, bool canActivate = false)
        {
            Offset = Vector2.Zero;
            Duration = Math.Max(Math.Abs(duration), float.Epsilon);
            TimeLeft = 0f;
            OnShake = onShake;
            RemoveOnCompletion = removeOnCompletion;
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
                Offset = Vector2.Zero;
                OnShake?.Invoke(Offset);
                if (RemoveOnCompletion)
                    Destroy();
                return;
            }

            Offset = new Vector2(Randomizer.Current.Next(-1, 1), Randomizer.Current.Next(-1, 1));
            OnShake?.Invoke(Offset);
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
            Cache.Push(this);
        }

        public void Activate()
        {
            TimeLeft = Duration;
            Offset = Vector2.Zero;
            _isActive = true;
        }

        public void Activate(float duration, bool removeOnCompletion)
        {
            Duration = Math.Max(Math.Abs(duration), float.Epsilon);
            RemoveOnCompletion = removeOnCompletion;
            Activate();
        }

        #region Static Methods
        public static Shaker Create(float duration, Action<Vector2> onShake, bool removeOnCompletion, bool canActivate = false)
        {
            var shaker = Cache.Count > 0 ? Cache.Pop() : new Shaker();
            shaker.Initialize(duration, onShake, removeOnCompletion, canActivate);
            return shaker;
        }

        public static Shaker CreateAndApply(Entity entity, float duration, Action<Vector2> onShake, bool removeOnCompletion = true)
        {
            var shaker = Create(duration, onShake, removeOnCompletion, true);
            entity.AddComponents(shaker);
            return shaker;
        }
        #endregion
    }
}
