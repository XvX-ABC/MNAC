using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.UI
{
    [ExecuteAlways]
    public class Ring : MonoBehaviour
    {
        GameObject _mask;
        GameObject _background;
        RectTransform _rectTransform;
        RectTransform _maskTransform;
        Image _maskImage;


        RectTransform _backgroundTransform;
        Image _backgroundImage;


        [SerializeField, Range(0, 100)]
        float _borderProportion;
        [SerializeField]
        Sprite _backgroundSprite;
        [SerializeField]
        Color _color;
        [SerializeField, Min(0)]
        float _radius;

        public float BorderProportion
        {
            get => _borderProportion;
            set
            {
                _borderProportion = Mathf.Clamp(value, 0, 100);
                UpdateChildrenLayout();
            }
        }

        public float Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                UpdateRadius();
            }
        }

        public Sprite BackgroundSprite
        {
            get => _backgroundSprite;
            set
            {
                _backgroundSprite = value;
                UpdateImageAndColor();
            }
        }
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                UpdateImageAndColor();
            }
        }

        private void Awake()
        {
            _mask = GameObject.Find("mask") ?? throw new NullReferenceException(nameof(_mask));
            _background = GameObject.Find("mask/background") ?? throw new NullReferenceException(nameof(_background));
            _rectTransform = GetComponent<RectTransform>() ?? throw new NullReferenceException(nameof(_rectTransform));
            _maskTransform = _mask.GetComponent<RectTransform>();
            _backgroundTransform = _background.GetComponent<RectTransform>();
            _backgroundImage = _background.GetComponent<Image>() ?? throw new NullReferenceException(nameof(_backgroundImage));
            _maskImage = _mask.GetComponent<Image>() ?? throw new NullReferenceException(nameof(_maskImage));
        }
        private void Update()
        {
            if (!Application.isPlaying)
            {
                UpdateChildrenLayout();
                UpdateImageAndColor();
                UpdateRadius();
            }
        }
        void UpdateImageAndColor()
        {
            var existingSprite = _backgroundImage.sprite;
            var color = _backgroundImage.color;
            if (existingSprite != _backgroundSprite)
            {
                _backgroundImage.sprite = _backgroundSprite;
                _maskImage.sprite = _backgroundSprite;
            }
            if (color != _color)
                _backgroundImage.color = _color;
        }
        void UpdateChildrenLayout()
        {
            var wh = new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);

            var insert = wh * (_borderProportion * 0.01f);
            _backgroundTransform.anchorMin = new Vector2(0, 0);
            _backgroundTransform.anchorMax = new Vector2(1, 1);
            _backgroundTransform.sizeDelta = new Vector2(insert.x, insert.y);

            _maskTransform.anchorMin = new Vector2(0, 0);
            _maskTransform.anchorMax = new Vector2(1, 1);

            _maskTransform.sizeDelta = -1 * new Vector2(insert.x, insert.y);
        }
        void UpdateRadius()
        {
            var d = _radius * 2;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, d);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, d);
        }
    }
}
