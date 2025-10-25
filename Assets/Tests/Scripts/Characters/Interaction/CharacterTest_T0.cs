using System;
using UnityEngine;
using UInput = UnityEngine.Input;
namespace Tests.Characters.Interaction
{
    public class CharacterTest_T0 : MonoBehaviour
    {
        Character_T0 _c;
        [SerializeField, Range(0, 1)]
        float _a;
        private void Awake()
        {
            _c = GetComponent<Character_T0>();
        }
        private void Start()
        {
            _c.HP.ReceivePoint(_a * _c.HP.MaxPoint - _c.HP.Point);
        }
        void OnValidate()
        {
            if (_c?.HP != null)
                _c.HP.ReceivePoint(_a * _c.HP.MaxPoint - _c.HP.Point);
        }
    }
}
