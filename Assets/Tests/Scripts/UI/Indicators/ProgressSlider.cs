using UnityEngine;
using UnityEngine.UI;

namespace Tests.UI
{
    [RequireComponent(typeof(Slider))]
    public class ProgressSlider : UIComponent
    {
        [SerializeField]
        MultiplyImageController _imagesController;
        Slider _slider;
        public Color Color
        {
            get => _imagesController.Color;
            set => _imagesController.Color = value;
        }
        public float Value
        {
            get => _slider.value;
            set => _slider.value = value;
        }
        protected override void Awake()
        {
            base.Awake();
            _slider = GetComponent<Slider>();
        }
    }
}
