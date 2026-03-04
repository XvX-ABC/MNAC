using UnityEngine;

namespace MNAC.UI
{
    public class CursorIndicator_Test : MonoBehaviour
    {
        [SerializeField]
        CursorIndicator _indicator;
        [SerializeField]
        float _width;
        [SerializeField]
        bool _enabled;
        private void LateUpdate()
        {
            _indicator.Width = _width;
            //_indicator.CursorPosition = UnityEngine.Input.mousePosition;
            _indicator.Enabled = _enabled;
        }
    }
}