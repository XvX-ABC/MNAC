using System;
using UnityEngine;
using static Tests.UI.ProgressSlider_Slider;

namespace Tests.UI
{
    
    public abstract class ProgressSlider : UIComponent
    {
        [SerializeField]
        protected ProgressSliderMode[] modes;
        protected ProgressSliderMode currentMode;
        public abstract Color Color { get; set; }
        public abstract float Value { get; set; }

        public void ChangeMode(string name)
        {
            var idx = FindModeIndex(name);
            if (idx == -1)
                throw new ProgressSliderModeNotFoundException(name);
            var mode = modes[idx];
            ApplyMode(mode);

        }
        protected abstract void ApplyMode(ProgressSliderMode mode);
        protected int FindModeIndex(string name)
        {
            return Array.FindIndex(modes, e => e.name == name);
        }
        public bool ContainsMode(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (modes.Length == 0)
                return false;
            return FindModeIndex(name) > -1;
        }
    }
}
