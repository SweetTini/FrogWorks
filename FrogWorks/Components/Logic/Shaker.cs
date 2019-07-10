﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class Shaker : Component
    {
        internal static Stack<Shaker> Cache { get; } = new Stack<Shaker>();

        public Vector2 Value { get; private set; }

        public float Duration { get; private set; }

        public float TimeLeft { get; private set; }

        public bool RemoveOnCompletion { get; set; }

        public Action<Vector2> OnShake { get; set; }

        private Shaker()
            : base(true, false)
        {
        }

        private void Initialize(float duration, Action<Vector2> onShake, bool removeOnCompletion, bool canActivate = false)
        {
            Value = Vector2.Zero;
            Duration = Math.Max(Math.Abs(duration), float.Epsilon);
            TimeLeft = 0f;
            OnShake = onShake;
            RemoveOnCompletion = removeOnCompletion;
            IsDestroyed = IsEnabled = false;

            if (canActivate) Start();
        }

        public override void Update(float deltaTime)
        {
            TimeLeft -= deltaTime;

            if (TimeLeft <= 0f)
            {
                TimeLeft = 0f;
                Value = Vector2.Zero;
                OnShake?.Invoke(Value);
                if (RemoveOnCompletion)
                    Destroy();
                return;
            }

            Value = new Vector2(Randomizer.Current.NextFloat(-1f, 1f), Randomizer.Current.NextFloat(-1f, 1f)).Round();
            OnShake?.Invoke(Value);
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
            Cache.Push(this);
        }

        public void Start()
        {
            TimeLeft = Duration;
            IsEnabled = true;
        }

        public void Start(float duration, bool removeOnCompletion = false)
        {
            Duration = Math.Max(Math.Abs(duration), float.Epsilon);
            RemoveOnCompletion = removeOnCompletion;
            Start();
        }

        public void Stop()
        {
            if (IsEnabled)
            {
                Value = Vector2.Zero;
                OnShake?.Invoke(Value);
                IsEnabled = false;
            }
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
            if (entity != null)
                entity.AddComponents(shaker);
            return shaker;
        }
        #endregion
    }
}