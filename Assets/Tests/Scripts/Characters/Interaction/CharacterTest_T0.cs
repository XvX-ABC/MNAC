using System;
using UnityEngine;
namespace Tests.Characters.Interaction
{
    public class CharacterTest_T0 : MonoBehaviour
    {
        Character_T0 _c;
        [SerializeField, Range(0, 1)]
        float _v = 1;
        private void Awake()
        {
            _c = GetComponent<Character_T0>();
        }
        void Update()
        {
            var mh = _c.HP.MaxPoint;
            _c.HP.ReceivePoint(mh * _v);
        }
    }
}
