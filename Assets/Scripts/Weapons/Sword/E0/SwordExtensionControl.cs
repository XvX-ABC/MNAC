using UnityEngine;

namespace MNAC.Weapons.Sword
{
    [RequireComponent(typeof(Collider))]
    internal class SwordExtensionControl : SwordComponent
    {
        [SerializeField]
        SkinnedMeshRenderer _meshRenderer;
        [SerializeField]
        ushort _materialIndex;
        [SerializeField]
        string _paramName;
        [SerializeField, Range(0, 1)]
        float _value;
        Material _material;

        public float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, 0, 1);
                _material?.SetFloat(_paramName, _value);
            }
        }

        private void OnEnable()
        {
            _material = _meshRenderer.materials[_materialIndex];
        }
        private void Start()
        {
            Value = _value;
        }
        void Update()
        {
            Value = _value;
        }
        private void OnValidate()
        {
            if (_material != null)
                Value = _value;
        }

    }
}
