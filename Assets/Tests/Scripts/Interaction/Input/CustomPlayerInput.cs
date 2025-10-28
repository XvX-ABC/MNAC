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
    public class CustomPlayerInput : MonoBehaviour, IInput_Obsolete
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
        KeyCode _boost;
        [SerializeField]
        KeyCode _quickBoost;
        [SerializeField]
        KeyCode _fire;
        [SerializeField]
        KeyCode _reload;
        [SerializeField]
        KeyCode _supply;
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

        public bool Jump => UInput.GetKey(_up);

        public bool Boost
        {
            get
            {
                var direction = HorizontalVector;
                return UInput.GetKey(_boost) && direction != Vector3.zero;
            }
        }
        public bool QuickBoost
        {
            get
            {
                var direction = HorizontalVector;
                return UInput.GetKeyDown(_quickBoost) && direction != Vector3.zero;
            }
        }
        bool IInput_Obsolete.Fire => UInput.GetKey(_fire);

        bool IInput_Obsolete.Reload => UInput.GetKeyDown(_reload);

        bool IInput_Obsolete.Supply => UInput.GetKeyDown(_supply);
    }
}