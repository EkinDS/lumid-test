using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Infrastructure
{
    public interface IGameEvent { }

    public class EventBus
    {
        readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _handlers[type] = list;
            }
            if (!list.Contains(handler))
                list.Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var list))
            {
                list.Remove(handler);
                if (list.Count == 0)
                    _handlers.Remove(type);
            }
        }

        public void Publish<T>(T evt) where T : IGameEvent
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var list)) return;

            var snapshot = list.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
            {
                if (snapshot[i] is Action<T> action)
                    action(evt);
            }
        }
    }
}