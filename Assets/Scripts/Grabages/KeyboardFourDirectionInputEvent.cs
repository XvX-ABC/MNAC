using UnityEngine;

namespace Assets.Scripts
{
    internal class KeyboardFourDirectionInputEvent : FourDirectionInputEvent
    {
        public KeyCode ForwardKey;
        public KeyCode BackKey;
        public KeyCode LeftKey;
        public KeyCode RightKey;

        void Awake()
        {
            ForwardTrigger = () => Input.GetKey(ForwardKey);
            BackTrigger = () => Input.GetKey(BackKey);
            LeftTrigger = () => Input.GetKey(LeftKey);
            RightTrigger = () => Input.GetKey(RightKey);
        }
    }
}
