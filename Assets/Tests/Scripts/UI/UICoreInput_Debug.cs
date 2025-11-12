using UnityEngine;

namespace Tests.UI
{
    [SerializeField]
    public class UICoreInput_Debug : MonoBehaviour, IInput
    {
        public Vector3 MousePosition => UnityEngine.Input.mousePosition;
    }
}
