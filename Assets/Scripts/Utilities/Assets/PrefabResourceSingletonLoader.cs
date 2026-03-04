using UnityEngine;

namespace MNAC.Utilities.Assets
{
    public class PrefabResourceSingletonLoader<T> : ResourceLoader<T> where T : Component
    {
        [SerializeField]
        T _prefab;
        T _instance;

        public override T Resource => _instance;

        public override void Dispose()
        {
        }

        public override bool Load()
        {
            _instance = Object.FindAnyObjectByType<T>();
            if (_instance != null)
                return false;
            _instance = Object.Instantiate(_prefab);
            _instance.name = _prefab.name;
            _instance.gameObject.SetActive(true);
            return true;
        }
    }
}
