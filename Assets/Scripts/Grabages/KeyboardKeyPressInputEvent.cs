using UnityEngine;

namespace Assets.Scripts
{
    internal class KeyboardKeyPressInputEvent : CustomEvent<bool>
    {
        [SerializeField]
        KeyCode _key;
        public override bool Invoke()
        {
            return Input.GetKey(_key);
        }
    }
}
