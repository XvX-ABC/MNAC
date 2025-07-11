using System;
using UnityEngine;
using UInput = UnityEngine.Input;
namespace Tests.Input
{
    public enum InputTypes
    {
        HorizontalDirection,    
        IsAscending,
        IsBoosting,
        Fire,
        Reload,
        Supply,
    }
    [Serializable]
    public class CustomPlayerInput : MonoBehaviour, IInput
    {
        [SerializeField]
        KeyCode _forward;
        [SerializeField]
        KeyCode _back;
        [SerializeField]
        KeyCode _left;
        [SerializeField]
        KeyCode _right;
        [SerializeField]
        KeyCode _up;
        [SerializeField]
        KeyCode _qb;
        [SerializeField]
        KeyCode _fire;
        [SerializeField]
        KeyCode _reload;
        [SerializeField]
        KeyCode _supply;
        public Vector3 HorizontalDirection
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

        public bool IsAscending => UInput.GetKey(_up);

        public bool IsBoosting
        {
            get
            {
                var direction = HorizontalDirection;
                return UInput.GetKeyDown(_qb) && direction != Vector3.zero;
            }
        }

        bool IInput.Fire => UInput.GetKey(_fire);

        bool IInput.Reload => UInput.GetKeyDown(_reload);

        bool IInput.Supply => UInput.GetKeyDown(_supply);
    }
}