using System;
using System.Collections.Generic;
using UnityEngine;

namespace GDev.Library.ScriptableObjects
{
    public class RuntimeScriptableEvent : ScriptableObject, IRuntimeSE
    {
        public event Action OnRaised;

        public void Raise()
        {
#if UNITY_EDITOR
            RegisterDebugCall();
#endif
            OnRaised?.Invoke();
        }

        public int GetListenerCount() => OnRaised?.GetInvocationList().Length ?? 0;

        // ---------- DEBUG INFO (Editor only) ----------
#if UNITY_EDITOR
        [SerializeField, HideInInspector] private int _debugInvokeCount;
        [SerializeField, HideInInspector] private List<string> _debugLastCalls = new();

        public int DebugInvokeCount => _debugInvokeCount;
        public IReadOnlyList<string> DebugLastCalls => _debugLastCalls;

        private void RegisterDebugCall()
        {
            _debugInvokeCount++;

            string ts = DateTime.Now.ToString("HH:mm:ss.fff");
            _debugLastCalls.Add(ts);

            const int MaxLogs = 10;
            if (_debugLastCalls.Count > MaxLogs)
                _debugLastCalls.RemoveAt(0);
        }

        public void ResetDebug()
        {
            _debugInvokeCount = 0;
            _debugLastCalls.Clear();
        }
#endif
    }

    public class RuntimeScriptableEvent<T> : ScriptableObject, IRuntimeSE
    {
        public event Action<T> OnRaised;

        public void Raise(T t)
        {
#if UNITY_EDITOR
            RegisterDebugCall(t);
#endif
            OnRaised?.Invoke(t);
        }

        public int GetListenerCount() => OnRaised?.GetInvocationList().Length ?? 0;

#if UNITY_EDITOR
        [SerializeField, HideInInspector] private int _debugInvokeCount;
        [SerializeField, HideInInspector] private List<string> _debugLastCalls = new();

        public int DebugInvokeCount => _debugInvokeCount;
        public IReadOnlyList<string> DebugLastCalls => _debugLastCalls;

        private void RegisterDebugCall(T payload)
        {
            _debugInvokeCount++;

            string ts = DateTime.Now.ToString("HH:mm:ss.fff");
            string p = payload != null ? payload.ToString() : "null";

            if (p.Length > 24)
                p = p.Substring(0, 21) + "...";

            _debugLastCalls.Add($"{ts} | {p}");

            if (_debugLastCalls.Count > 10)
                _debugLastCalls.RemoveAt(0);
        }

        public void ResetDebug()
        {
            _debugInvokeCount = 0;
            _debugLastCalls.Clear();
        }
#endif
    }

    public class RuntimeScriptableEvent<T1, T2> : ScriptableObject, IRuntimeSE
    {
        public event Action<T1, T2> OnRaised;

        public void Raise(T1 t1, T2 t2)
        {
#if UNITY_EDITOR
            RegisterDebugCall(t1, t2);
#endif
            OnRaised?.Invoke(t1, t2);
        }

        public int GetListenerCount() => OnRaised?.GetInvocationList().Length ?? 0;

#if UNITY_EDITOR
        [SerializeField, HideInInspector] private int _debugInvokeCount;
        [SerializeField, HideInInspector] private List<string> _debugLastCalls = new();

        public int DebugInvokeCount => _debugInvokeCount;
        public IReadOnlyList<string> DebugLastCalls => _debugLastCalls;

        private void RegisterDebugCall(T1 t1, T2 t2)
        {
            _debugInvokeCount++;

            string ts = DateTime.Now.ToString("HH:mm:ss.fff");

            string p1 = t1?.ToString() ?? "null";
            string p2 = t2?.ToString() ?? "null";

            string payload = $"{Short(p1)}, {Short(p2)}";

            _debugLastCalls.Add($"{ts} | {payload}");

            if (_debugLastCalls.Count > 10)
                _debugLastCalls.RemoveAt(0);
        }

        private string Short(string s) => s.Length > 14 ? s.Substring(0, 11) + "..." : s;

        public void ResetDebug()
        {
            _debugInvokeCount = 0;
            _debugLastCalls.Clear();
        }
#endif
    }

    public class RuntimeScriptableEvent<T1, T2, T3> : ScriptableObject, IRuntimeSE
    {
        public event Action<T1, T2, T3> OnRaised;

        public void Raise(T1 t1, T2 t2, T3 t3)
        {
#if UNITY_EDITOR
            RegisterDebugCall(t1, t2, t3);
#endif
            OnRaised?.Invoke(t1, t2, t3);
        }

        public int GetListenerCount() => OnRaised?.GetInvocationList().Length ?? 0;

#if UNITY_EDITOR
        [SerializeField, HideInInspector] private int _debugInvokeCount;
        [SerializeField, HideInInspector] private List<string> _debugLastCalls = new();

        public int DebugInvokeCount => _debugInvokeCount;
        public IReadOnlyList<string> DebugLastCalls => _debugLastCalls;

        private void RegisterDebugCall(T1 t1, T2 t2, T3 t3)
        {
            _debugInvokeCount++;

            string ts = DateTime.Now.ToString("HH:mm:ss.fff");

            string payload = $"{Short(t1)}, {Short(t2)}, {Short(t3)}";
            _debugLastCalls.Add($"{ts} | {payload}");

            if (_debugLastCalls.Count > 10)
                _debugLastCalls.RemoveAt(0);
        }

        private string Short(object o)
        {
            string s = o?.ToString() ?? "null";
            return s.Length > 14 ? s.Substring(0, 11) + "..." : s;
        }

        public void ResetDebug()
        {
            _debugInvokeCount = 0;
            _debugLastCalls.Clear();
        }
#endif
    }
}