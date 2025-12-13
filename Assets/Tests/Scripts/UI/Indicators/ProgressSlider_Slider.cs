using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.UI
{
    [RequireComponent(typeof(Slider))]
    public class ProgressSlider_Slider : ProgressSlider
    {
        public class ProgressSliderModeNotFoundException : Exception
        {
            public ProgressSliderModeNotFoundException(string name) : base($"The mode '{name}' is not found in the ProgressSlider")
            {

            }
        }
        [SerializeField]
        MultiplyImageController _imagesController;



        Slider _slider;
        public override Color Color
        {
            get => _imagesController.Color;
            set => _imagesController.Color = value;
        }
        public override float Value
        {
            get => _slider.value;
            set => _slider.value = value;
        }
        protected override void Awake()
        {
            base.Awake();
            _slider = GetComponent<Slider>();
        }
        protected override void ApplyMode(ProgressSliderMode mode)
        {
            Color = mode.color;
            currentMode = mode;
        }
    }
}
