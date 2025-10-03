using System;
using UnityEngine;
using UInput = UnityEngine.Input;
namespace Tests.Characters.Interaction
{
    public class CharacterTest_T0 : MonoBehaviour
    {
        Character_T0 _c;
        [SerializeField]
        float _v;
        [SerializeField]
        float _point;
        private void Awake()
        {
            _c = GetComponent<Character_T0>();
        }
        void Update()
        {
            var mh = _c.HP.MaxPoint;
            if (UInput.GetKeyDown(KeyCode.Mouse1))
                _c.HP.ReceivePoint(-_v);
            _point = _c.HP.Point;
        }
    }
}
