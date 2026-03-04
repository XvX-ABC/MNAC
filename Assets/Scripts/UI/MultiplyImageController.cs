using System;
using UnityEngine;
using UnityEngine.UI;

namespace MNAC.UI
{
    [Serializable]
    public class MultiplyImageController
    {
        [SerializeField]
        Image[] _images;
        Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                foreach (var image in _images)
                {
                    image.color = value;
                }
                _color = value;
            }
        }
    }
}
