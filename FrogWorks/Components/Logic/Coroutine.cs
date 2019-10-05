using System;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public class Coroutine : Component
    {
        private Stack<IEnumerator> _enumerators;
        private float _timer;
        private bool _hasEnded;

        public bool IsFinished { get; private set; }

        public bool RemoveOnCompletion { get; set; } = true;

        public Coroutine(bool removeOnCompletion = true)
            : base(false, false)
        {
            _enumerators = new Stack<IEnumerator>();
            RemoveOnCompletion = removeOnCompletion;
        }

        public Coroutine(IEnumerator callback, bool removeOnCompletion = true)
            : base(true, false)
        {
            _enumerators = new Stack<IEnumerator>();
            _enumerators.Push(callback);
            RemoveOnCompletion = removeOnCompletion;
        }

        protected override void Update(float deltaTime)
        {
            _hasEnded = false;
            _timer -= deltaTime;

            if (_timer <= 0f)
            {
                _timer = 0f;

                if (_enumerators.Count > 0)
                {
                    var callback = _enumerators.Peek();

                    if (callback.MoveNext() && !_hasEnded)
                    {
                        if (callback.Current is int)
                            _timer = (int)callback.Current;
                        else if (callback.Current is float)
                            _timer = (float)callback.Current;
                        else if (callback.Current is IEnumerator)
                            _enumerators.Push(callback.Current as IEnumerator);
                    }
                    else if (!_hasEnded)
                    {
                        _enumerators.Pop();

                        if (_enumerators.Count == 0)
                        {
                            IsFinished = true;
                            IsEnabled = false;
                            if (RemoveOnCompletion)
                                Destroy();
                        }
                    }
                }
            }
        }

        public void ForceUpdate(float deltaTime) => Update(deltaTime);

        public void Replace(IEnumerator callback)
        {
            IsEnabled = _hasEnded = true;
            IsFinished = false;
            _timer = 0f;
            _enumerators.Clear();
            _enumerators.Push(callback);
        }

        public void Cancel()
        {
            IsEnabled = false;
            IsFinished = _hasEnded = true;
            _timer = 0f;
            _enumerators.Clear();
        }

        #region Static Methods
        public static IEnumerator WaitForTicks(int ticks)
        {
            for (int i = 0; i < ticks; i++)
                yield return 0;
        }

        public static IEnumerator WaitForSeconds(float seconds)
        {
            var timer = 0f;

            while (timer < seconds)
            {
                timer += Runner.Application.Game.DeltaTime;
                yield return 0;
            }
        }

        public static IEnumerator WaitUntil(Func<bool> predicate)
        {
            while (!predicate()) yield return 0;
        }

        public static IEnumerator DoInTicks(int ticks, Action<int, int> action)
        {
            for (int i = 0; i <= ticks; i++)
            {
                action(i, ticks);
                yield return 0;
            }
        }

        public static IEnumerator DoInSeconds(float seconds, Action<float, float> action)
        {
            var timer = 0f;

            while (timer < seconds)
            {
                timer += Runner.Application.Game.DeltaTime;
                timer = timer.Min(seconds);
                action(timer, seconds);
                yield return 0;
            }
        }
        #endregion
    }
}
