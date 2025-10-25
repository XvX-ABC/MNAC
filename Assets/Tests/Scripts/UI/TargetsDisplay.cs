using System;
using Tests.Interaction;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.UI
{
    [ExecuteAlways]
    public class TargetsDisplay : MonoBehaviour
    {
        Vector3 _targetWorldPos;
        Camera _camera;
        RectTransform _rectTransform;
        Image _image;
        RectTransform _imageRectTransform;
        public Camera Camera { get => _camera; set => _camera = value; }
        public Vector3 TargetWorldPos { get => _targetWorldPos; set => _targetWorldPos = value; }
        public bool Activated { get; set; }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>() ?? throw new NullReferenceException(nameof(_rectTransform));
            var obj = GameObject.Find("image") ?? throw new NullReferenceException(nameof(_image));
            var image = obj.GetComponent<Image>() ?? throw new NullReferenceException(nameof(_image));
            _imageRectTransform = obj.GetComponent<RectTransform>();
        }
        private void LateUpdate()
        {
            if (!Activated)
                return;
            var spos = _camera.WorldToScreenPoint(_targetWorldPos);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, spos, _camera, out var localPos))
            {
                _imageRectTransform.anchoredPosition = localPos;
            }
        }
    }
}
