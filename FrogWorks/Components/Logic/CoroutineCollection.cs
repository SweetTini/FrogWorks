using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public class CoroutineCollection : Component
    {
        private List<CoroutineItem> _coroutines, _coroutinesToRemove;
        private bool _isRunning;

        public CoroutineCollection()
            : base(true, false)
        {
            _coroutines = new List<CoroutineItem>();
            _coroutinesToRemove = new List<CoroutineItem>();
        }

        protected override void Update(float deltaTime)
        {
            if (_coroutinesToRemove.Count > 0)
            {
                for (int i = 0; i < _coroutinesToRemove.Count; i++)
                    _coroutines.Remove(_coroutinesToRemove[i]);

                _coroutinesToRemove.Clear();
            }

            _isRunning = true;

            for (int i = 0; i < _coroutines.Count; i++)
            {
                var coroutine = _coroutines[i];
                var callback = coroutine.Enumerators.Peek();

                if (callback != null && callback.MoveNext())
                {
                    if (callback.Current is IEnumerator)
                    {
                        coroutine.Enumerators.Push(
                            callback.Current as IEnumerator);
                    }
                }
                else
                {
                    coroutine.Enumerators.Pop();

                    if (coroutine.IsFinished)
                        _coroutinesToRemove.Add(coroutine);
                }
            }

            _isRunning = false;
        }

        public CoroutineItem StartCoroutine(IEnumerator callback)
        {
            var coroutine = new CoroutineItem(callback);
            _coroutines.Add(coroutine);
            return coroutine;
        }

        public void EndCoroutine(CoroutineItem coroutine)
        {
            if (_coroutines.Contains(coroutine) && !_coroutinesToRemove.Contains(coroutine))
            {
                if (!_isRunning)
                    _coroutines.Remove(coroutine);
                else if (!_coroutinesToRemove.Contains(coroutine))
                    _coroutinesToRemove.Add(coroutine);
            }
        }
    }

    public class CoroutineItem
    {
        internal Stack<IEnumerator> Enumerators { get; private set; }

        public bool IsFinished => Enumerators.Count == 0;

        internal CoroutineItem(IEnumerator callback)
        {
            Enumerators = new Stack<IEnumerator>();
            Enumerators.Push(callback);
        }
    }
}
