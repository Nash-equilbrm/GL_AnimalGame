using Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Patterns
{
    public class PubSub : Singleton<PubSub>
    {
        private Dictionary<EventID, Action<object>> listeners = new Dictionary<EventID, Action<object>>();

        #region Register, Unregister, Broadcast
        public void PSRegister(EventID id, Action<object> action)
        {
            if (action == null) { return; }
            if (listeners.ContainsKey(id))
            {
                listeners[id] += action;
            }
            else
            {
                listeners.Add(id, action);
            }
        }
        public void PSUnregister(EventID id, Action<object> action)
        {
            if (listeners.ContainsKey(id) && action != null)
            {
                listeners[id] -= action;
            }
        }
        public void PSubUnregisterAll(EventID id)
        {
            if (listeners.ContainsKey(id))
            {
                listeners.Remove(id);
            }
        }
        public void PSBroadcast(EventID id, object data = null)
        {
            if (listeners.ContainsKey(id))
            {
                listeners[id].Invoke(data);
            }
        }
        #endregion
    }
    public static class PubSubExtension
    {
        public static void PubSubRegister(this MonoBehaviour listener, EventID id, Action<object> action)
        {
            if (PubSub.HasInstance)
            {
                PubSub.Instance.PSRegister(id, action);
            }
        }
        public static void PubSubUnregister(this MonoBehaviour listener, EventID id, Action<object> action)
        {
            if (PubSub.HasInstance)
            {
                PubSub.Instance.PSUnregister(id, action);
            }
        }
        public static void PubSubUnregisterAll(this MonoBehaviour listener, EventID id)
        {
            if (PubSub.HasInstance)
            {
                PubSub.Instance.PSubUnregisterAll(id);
            }
        }
        public static void PubSubBroadcast(this MonoBehaviour listener, EventID id)
        {
            if (PubSub.HasInstance)
            {
                PubSub.Instance.PSBroadcast(id, null);
                LogUtility.ValidInfo("PubSubBroadcast", id.ToString());
            }
            else
            {
                LogUtility.InvalidInfo("PubSubBroadcast Fail", id.ToString());
            }
        }
        public static void PubSubBroadcast(this MonoBehaviour listener, EventID id, object data)
        {
            if (PubSub.HasInstance)
            {
                PubSub.Instance.PSBroadcast(id, data);
            }
        }
    }
}