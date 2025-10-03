using System;
using UnityEngine;

namespace Tests.UI
{

    [ExecuteAlways]
    public class RingCatcher : MonoBehaviour
    {

        RectTransform _rectTransform;
        GameObject _ringObj;
        internal Ring ring;
        RectTransform _ringTransform;

        [SerializeField]
        Camera _camera;
        [SerializeField]
        bool _allowInputPosition;
        Vector3 _mousePosition;

        public Vector3 MousePosition
        {
            get => _mousePosition;
            set
            {
                if (!_allowInputPosition)
                    _mousePosition = value;
                UpdateRingPosition();
            }
        }
        public Camera Camera
        {
            get => _camera;
            set
            {
                if (value == null)
                    throw new NullReferenceException(nameof(_camera));
                _camera = value;
            }
        }

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            _rectTransform = GetComponent<RectTransform>() ?? throw new NullReferenceException(nameof(_rectTransform));
            _ringObj = GameObject.Find("ring") ?? throw new NullReferenceException(nameof(_ringObj));
            ring = _ringObj.GetComponent<Ring>() ?? throw new NullReferenceException(nameof(ring));
            _ringTransform = _ringObj.GetComponent<RectTransform>() ?? throw new NullReferenceException(nameof(_ringTransform));
        }
        private void LateUpdate()
        {
            if (_allowInputPosition)
            {
                _mousePosition = UnityEngine.Input.mousePosition;
                UpdateRingPosition();
            }
            if (!Application.isPlaying)
            {
                UpdateRingPosition();
            }
        }
        void UpdateRingPosition()
        {
            if (_camera != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, _mousePosition, _camera, out var localPos))
            {
                _ringTransform.anchoredPosition = localPos;
            }
        }
    }
}
