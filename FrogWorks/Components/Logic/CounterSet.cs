using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class CounterSet<T> : Component
        where T : struct
    {
        private Dictionary<T, float> _counters;
        private float _timer;

        public float this[T key]
        {
            get { return _counters.ContainsKey(key) ? Math.Max(_counters[key] - _timer, 0f) : 0f; }
            set { _counters[key] = _timer + value; }
        }

        public CounterSet()
            : base(true, false)
        {
            _counters = new Dictionary<T, float>();
        }

        public override void Update(float deltaTime)
        {
            _timer += deltaTime;
        }

        public bool IsActive(T key)
        {
            return _counters.ContainsKey(key) && _counters[key] - _timer > 0f;
        }
    }
}
