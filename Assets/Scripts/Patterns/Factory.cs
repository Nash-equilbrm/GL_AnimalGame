using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns
{
    public abstract class Factory<T,K>: Singleton<Factory<T, K>> where T : class where K : notnull
    {
        private readonly Dictionary<K, Func<T>> _registeredTypes = new();

        public void FactoryRegister(K key, Func<T> creator)
        {
            if (!_registeredTypes.ContainsKey(key))
            {
                _registeredTypes[key] = creator;
            }
        }

        public void FactoryUnregister(K key)
        {
            if (_registeredTypes.ContainsKey(key))
            {
                _registeredTypes.Remove(key);
            }
        }

        public T FactoryCreate(K key)
        {
            if (_registeredTypes.TryGetValue(key, out var creator))
            {
                return creator();
            }
            throw new ArgumentException($"Type {key} is not registered in the factory.");
        }
    }
}