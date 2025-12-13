using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.UI
{
    public class PlayerCursorIndicator : CursorIndicator
    {
        [SerializeField]
        ProgressSlider _slider_lt;
        [SerializeField]
        ProgressSlider _slider_lb;
        [SerializeField]
        ProgressSlider _slider_rt;
        [SerializeField]
        ProgressSlider _slider_rb;
        [SerializeField]
        ProgressSlider _slider_lm;
        [SerializeField]
        ProgressSlider _slider_rm;

        public ProgressSlider Slider_lt { get => _slider_lt; }
        public ProgressSlider Slider_lb { get => _slider_lb; }
        public ProgressSlider Slider_rt { get => _slider_rt; }
        public ProgressSlider Slider_rb { get => _slider_rb; }
        public ProgressSlider Slider_rm { get => _slider_rm; }
        public ProgressSlider Slider_lm { get => _slider_lm; set => _slider_lm = value; }

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(UIBlackboardFields.Character_Actor_Cursor_Indicator, this);
        }
        public override void Dispose()
        {
            base.Dispose();
            blackboard.TryUnregisterField(UIBlackboardFields.Character_Actor_Cursor_Indicator);
        }
    }
}
