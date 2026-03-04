using System;
using MNAC.UI;
using MNAC.Utilities.Blackboards;

namespace MNAC.Characters.UI
{
    public class HealthBar_QuantityBhv : HealthBar
    {
        internal new ProgressSlider_QuantityBhv slider;
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            slider = base.slider as ProgressSlider_QuantityBhv ?? throw new InvalidCastException("The slider must be type " + typeof(ProgressSlider_QuantityBhv).Name);
        }

    }
}
