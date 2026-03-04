using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MNAC.Utilities.Assets
{
    [Serializable]
    public class PrefabLoader<T> : ResourceLoader<T> where T : Component
    {
        [SerializeField]
        T _prefab;
        T _instance;
        Transform _parent;

        public override T Resource => _instance;

        public Transform Parent { get => _parent; set => _parent = value; }

        public override void Dispose()
        {
            Object.Destroy(_instance);
        }

        public override bool Load()
        {
            _instance = Object.Instantiate(_prefab, _parent);
            _instance.name = _prefab.name;
            _instance.gameObject.SetActive(true);
            return true;
        }
    }
}
