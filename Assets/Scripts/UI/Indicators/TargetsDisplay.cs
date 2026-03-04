using System;
using UnityEngine;
using UnityEngine.UI;

namespace MNAC.UI
{
    
    [ExecuteAlways]
    public class TargetsDisplay : MonoBehaviour
    {
        [SerializeField]
        Camera _camera;
        [SerializeField]
        Transform _targetTransform;
        Vector3 _targetWorldPos;
        RectTransform _rectTransform;
        Image _image;
        RectTransform _imageRectTransform;
        public Camera Camera { get => _camera; set => _camera = value; }
        public Vector3 TargetWorldPos { get => _targetWorldPos; set => _targetWorldPos = value; }
        public bool Activated { get => this.enabled; set => this.enabled = value; }

        private void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>() ?? throw new NullReferenceException(nameof(_rectTransform));
            var obj = GameObject.Find("image") ?? throw new NullReferenceException(nameof(_image));
            _image = obj.GetComponent<Image>() ?? throw new NullReferenceException(nameof(_image));
            _imageRectTransform = obj.GetComponent<RectTransform>();
        }
        private void LateUpdate()
        {
            if (!Activated)
                return;
            _targetWorldPos = _targetTransform.position;
            var spos = _camera.WorldToScreenPoint(_targetWorldPos);
            //_image.transform.position = spos;
            //_imageRectTransform.transform.position = spos;



            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, spos, null, out var localPos))
            {
                Debug.Log("localpos: " + localPos);
                _imageRectTransform.localPosition = localPos;
            }
        }
    }
}
