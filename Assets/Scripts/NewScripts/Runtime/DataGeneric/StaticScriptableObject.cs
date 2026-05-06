using UnityEngine;

namespace GDev.Library.ScriptableObjects
{
    public class StaticScriptableObject<T> : ScriptableObject, IStaticSO
    {
        [SerializeField] private T _value = default;

        public T Value
        {
            get => _value;
        }
    }

    public interface IStaticSO { }
    public interface IRuntimeSO { }
    public interface IRuntimeSE { }
}
