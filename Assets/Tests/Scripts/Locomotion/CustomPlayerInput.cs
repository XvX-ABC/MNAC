using UnityEngine;
namespace Tests.Locomotion
{
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
        public Vector3 HorizontalDirection
        {
            get
            {
                var direction = Vector3.zero;
                if (Input.GetKey(_forward))
                    direction = Vector3.forward;
                else if (Input.GetKey(_back))
                    direction = Vector3.back;

                if (Input.GetKey(_left))
                    direction -= Vector3.right;
                else if (Input.GetKey(_right))
                    direction += Vector3.right;
                return direction;
            }
        }

        public bool IsAscending => Input.GetKey(_up);
    }
}