using System;

using UnityEngine;
using UnityEngine.UI;
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
        public bool HIde
        {
            get => this == null ? true : !gameObject.activeSelf;
            set
            {
                if (this == null)
                    return;
                gameObject.SetActive(!value);
            }
        }

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
                MousePosition = UnityEngine.Input.mousePosition;
            }
            if (!Application.isPlaying)
            {
                MousePosition = UnityEngine.Input.mousePosition;
            }
        }
        void UpdateRingPosition()
        {
            //_ringObj.transform.position = _mousePosition;
            if (_camera != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, _mousePosition, null, out var localPos))
            {
                _ringTransform.localPosition = localPos;
            }
        }
    }
}
