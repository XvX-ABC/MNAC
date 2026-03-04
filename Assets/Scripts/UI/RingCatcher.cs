using DG.Tweening;
using System;

using UnityEngine;
using UnityEngine.UI;
namespace MNAC.UI
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
        [SerializeField]
        bool _allowShowCursor;
        Vector3 _cursorPosition;

        public Vector3 CursorPosition
        {
            get => _cursorPosition;
            set
            {
                _cursorPosition = value;
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
        public bool AllowShowCursor
        {
            get => _allowShowCursor;
            set
            {
                if (value)
                    ShowCursor();
                else
                    HideCursor();
                _allowShowCursor = value;
            }
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
        void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        void OnEnable()
        {
            if (Application.isPlaying && !_allowShowCursor)
            {
                HideCursor();
            }
        }
        void OnDisable()
        {
            if (Application.isPlaying && _allowShowCursor)
            {
                ShowCursor();
            }
        }
        private void LateUpdate()
        {
            if (_allowInputPosition)
            {
                CursorPosition = UnityEngine.Input.mousePosition;
            }
            if (!Application.isPlaying)
            {
                CursorPosition = UnityEngine.Input.mousePosition;
            }
        }
        public void OnGUIImpl()
        {
            if (GUILayout.Button("Allow show cursor"))
                AllowShowCursor = !AllowShowCursor;
        }
        void UpdateRingPosition()
        {
            //_ringObj.transform.position = _mousePosition;
            if (_camera != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, _cursorPosition, null, out var localPos))
            {
                    _ringTransform.localPosition = localPos;
            }
        }
    }
}
