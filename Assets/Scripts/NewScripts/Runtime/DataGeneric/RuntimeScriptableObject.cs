using System;
using System.Collections.Generic;
using UnityEngine;

namespace GDev.Library.ScriptableObjects
{
    public class RuntimeScriptableObject<T> : ScriptableObject, IRuntimeSO
    {
        public event Action<T> OnValueChanged;

        [SerializeField] private T _value = default;

        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value))
                    return;

                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }
}
