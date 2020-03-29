using System;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public class Coroutine : Component
    {
        private Stack<IEnumerator> _enumerators;
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

            if (_enumerators.Count > 0)
            {
                var callback = _enumerators.Peek();

                if (callback != null && callback.MoveNext() && !_hasEnded)
                {
                    if (callback.Current is IEnumerator)
                    {
                        _enumerators.Push(
                            callback.Current as IEnumerator);
                    }
                }
                else if (!_hasEnded)
                {
                    _enumerators.Pop();

                    if (_enumerators.Count == 0)
                    {
                        IsFinished = true;
                        IsActive = false;

                        if (RemoveOnCompletion)
                            Destroy();
                    }
                }
            }
        }

        public void ForceUpdate(float deltaTime) => Update(deltaTime);

        public void Replace(IEnumerator callback)
        {
            IsActive = _hasEnded = true;
            IsFinished = false;
            _enumerators.Clear();
            _enumerators.Push(callback ?? WaitForTicks(0));
        }

        public void Cancel()
        {
            IsActive = false;
            IsFinished = _hasEnded = true;
            _enumerators.Clear();
        }

        #region Static Methods
        public static IEnumerator WaitForTicks(int ticks)
        {
            for (int t = 0; t < ticks; t++)
                yield return null;
        }

        public static IEnumerator WaitForSeconds(float seconds)
        {
            var time = 0f;

            while (time < seconds)
            {
                time += Runner.Application.Game.DeltaTime;
                yield return null;
            }
        }

        public static IEnumerator WaitUntil(Func<bool> predicate)
        {
            while (!predicate()) yield return null;
        }

        public static IEnumerator DoInTicks(int ticks, Action<int, int> action)
        {
            if (ticks > 0)
            {
                for (int t = 0; t <= ticks; t++)
                {
                    action(t, ticks);
                    yield return null;
                }
            }
        }

        public static IEnumerator DoInSeconds(float seconds, Action<float, float> action)
        {
            if (seconds > 0f)
            {
                var time = 0f;

                while (true)
                {
                    action(time, seconds);
                    yield return null;

                    if (time < seconds)
                    {
                        time += Runner.Application.Game.DeltaTime;
                        time = time.Min(seconds);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
