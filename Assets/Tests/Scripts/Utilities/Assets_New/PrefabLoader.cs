using System;
using UnityEngine;

namespace Tests.Utilities.Assets_New
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
            GameObject.Destroy(_instance);
        }

        public override bool Load()
        {
            _instance = GameObject.Instantiate(_prefab, _parent);
            _instance.name = _prefab.name;
            _instance.gameObject.SetActive(true);
            return true;
        }
    }
}
