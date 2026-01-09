using UnityEngine;

namespace Assets.Scripts
{
    internal class ValidDirectionAndKeyOfKeyBoardPressInputEvent : ValidDirectionInputEvent
    {
        [SerializeField]
        internal KeyCode key;
        public override bool Invoke(Vector3 direction)
        {
            return base.Invoke(direction) && Input.GetKey(key);
        }
    }
}
