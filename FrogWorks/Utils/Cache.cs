﻿using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class Cache<U>
    {
        private Dictionary<Type, Stack<U>> _cache;

        public Cache()
        {
            _cache = new Dictionary<Type, Stack<U>>();
        }

        public T Create<T>() where T : U, new()
        {
            Initialize<T>();

            return _cache[typeof(T)].Count > 0
                ? (T)_cache[typeof(T)].Pop()
                : new T();
        }

        public void Store(U instance)
        {
            if (instance != null)
            {
                var type = instance.GetType();
                Initialize(type);

                if (!_cache[type].Contains(instance))
                    _cache[type].Push(instance);
            }
        }

        public void Store<T>(T instance) where T : U, new()
        {
            if (instance != null)
            {
                Initialize<T>();

                if (!_cache[typeof(T)].Contains(instance))
                    _cache[typeof(T)].Push(instance);
            }
        }

        public void Clear()
        {
            foreach (var stack in _cache.Values)
                stack.Clear();
        }

        public void Clear<T>() where T : U, new()
        {
            if (_cache.ContainsKey(typeof(T)))
                _cache[typeof(T)].Clear();
        }

        private void Initialize(Type type)
        {
            if (!_cache.ContainsKey(type))
                _cache.Add(type, new Stack<U>());
        }

        private void Initialize<T>() where T : U, new()
        {
            if (!_cache.ContainsKey(typeof(T)))
                _cache.Add(typeof(T), new Stack<U>());
        }
    }
}
