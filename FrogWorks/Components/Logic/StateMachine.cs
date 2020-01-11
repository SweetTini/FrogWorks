using System;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public class StateMachine<T> : Component
        where T : struct
    {
        private T? _key;
        private Dictionary<T, State<T>> _states;
        private Coroutine _coroutine;

        public T? CurrentState
        {
            get { return _key; }
            set
            {
                if (!IsLocked && !value.Equals(_key))
                    if (!value.HasValue || _states.ContainsKey(value.Value))
                        ForceState(value);
            }
        }

        public T? LastState { get; private set; }

        public bool IsLocked { get; set; }

        public StateMachine()
            : base(true, false)
        {
            _states = new Dictionary<T, State<T>>();
            _coroutine = new Coroutine(false);
            _key = default(T);
        }

        protected override void Update(float deltaTime)
        {
            State<T> state;

            if (_key.HasValue && _states.TryGetValue(_key.Value, out state))
            {
                CurrentState = state.Update?.Invoke() ?? _key;

                if (_coroutine.IsEnabled)
                    _coroutine.ForceUpdate(deltaTime);
            }
        }

        protected void AddOrUpdate(T key, Func<T> update = null, Func<IEnumerator> coroutine = null, Action begin = null, Action end = null)
        {
            if (_states.ContainsKey(key))
            {
                _states[key].Update = update ?? _states[key].Update;
                _states[key].Coroutine = coroutine ?? _states[key].Coroutine;
                _states[key].Begin = begin ?? _states[key].Begin;
                _states[key].End = end ?? _states[key].End;
            }
            else
            {
                var isFirst = _states.Count == 0;
                _states.Add(key, new State<T>(update, coroutine, begin, end));

                if (isFirst)
                    CurrentState = key;
            }
        }

        public void SetCallbacks(T key, Func<T> update, Func<IEnumerator> coroutine = null, Action begin = null, Action end = null)
        {
            State<T> state;

            if (_key.HasValue && _states.TryGetValue(_key.Value, out state))
            {
                state.Update = update;
                state.Coroutine = coroutine;
                state.Begin = begin;
                state.End = end;
            }
        }

        public void ForceState(T? key)
        {
            LastState = _key;
            _key = key;

            State<T> lastState = null;
            State<T> currentState = null;
            if (LastState.HasValue) _states.TryGetValue(LastState.Value, out lastState);
            if (_key.HasValue) _states.TryGetValue(_key.Value, out currentState);

            lastState?.End?.Invoke();
            currentState?.Begin?.Invoke();

            if (currentState?.Coroutine != null)
                _coroutine.Replace(currentState.Coroutine.Invoke());
            else _coroutine.Cancel();
        }

        public void ReflectState(T key, string name)
        {
            State<T> state;

            if (Parent != null && _key.HasValue && _states.TryGetValue(_key.Value, out state))
            {
                state.Update += (Func<T>)Extensions.GetMethod<Func<T>>(Parent, $"Update{name}");
                state.Coroutine += (Func<IEnumerator>)Extensions.GetMethod<Func<IEnumerator>>(Parent, $"Coroutine{name}");
                state.Begin += (Action)Extensions.GetMethod<Action>(Parent, $"Begin{name}");
                state.End += (Action)Extensions.GetMethod<Action>(Parent, $"End{name}");
            }
        }

        public static implicit operator T?(StateMachine<T> stateMachine)
        {
            return stateMachine.CurrentState;
        }
    }

    internal class State<T>
        where T : struct
    {
        public Func<T> Update { get; set; }

        public Func<IEnumerator> Coroutine { get; set; }

        public Action Begin { get; set; }

        public Action End { get; set; }

        public State(Func<T> update, Func<IEnumerator> coroutine = null, Action begin = null, Action end = null)
        {
            Update = update;
            Coroutine = coroutine;
            Begin = begin;
            End = end;
        }
    }
}
