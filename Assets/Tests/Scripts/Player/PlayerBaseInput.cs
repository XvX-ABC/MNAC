using System;
using Tests.Behaviours.Input;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Tests.Player
{
    [Serializable]
    public class PlayerBaseInput : IBaseInput
    {
        [SerializeField]
        KeyCode _forward;
        [SerializeField]
        KeyCode _back;
        [SerializeField]
        KeyCode _left;
        [SerializeField]
        KeyCode _right;

        public Vector3 MousePosition => UInput.mousePosition;
        public Vector3 MousePositionDelta => UInput.mousePositionDelta;

        public Vector3 HorizontalVector
        {
            get
            {
                var direction = Vector3.zero;
                if (UInput.GetKey(_forward))
                    direction = Vector3.forward;
                else if (UInput.GetKey(_back))
                    direction = Vector3.back;



                if (UInput.GetKey(_left))
                    direction -= Vector3.right;
                else if (UInput.GetKey(_right))
                    direction += Vector3.right;
                return direction.normalized;
            }
        }
    }
}
