using System;
using UnityEngine;

namespace Tests.UI
{
    [ExecuteAlways]
    public class RingCatcher : MonoBehaviour, ICursor
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
        public float RingRadius
        {
            get => ring.Radius;
            set => ring.Radius = value;
        }
        public bool HIde { get => !gameObject.activeSelf; set => gameObject.SetActive(!value); }

        private void Awake()
        {

            _rectTransform = GetComponent<RectTransform>() ?? throw new NullReferenceException(nameof(_rectTransform));
            _ringObj = GameObject.Find("ring") ?? throw new NullReferenceException(nameof(_ringObj));
            ring = _ringObj.GetComponent<Ring>() ?? throw new NullReferenceException(nameof(ring));
            _ringTransform = _ringObj.GetComponent<RectTransform>() ?? throw new NullReferenceException(nameof(_ringTransform));
        }
        void OnEnable()
        {
            if (Application.isPlaying)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
        void OnDisable()
        {
            if (Application.isPlaying)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
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
