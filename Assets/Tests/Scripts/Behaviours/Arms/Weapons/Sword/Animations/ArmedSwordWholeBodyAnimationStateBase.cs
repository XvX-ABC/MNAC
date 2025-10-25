using System;
using Tests.Animations;
using Tests.States;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    internal abstract class ArmedSwordWholeBodyAnimationStateBase : ArmedSwordAnimationStateBase
    {
        float _w0;
        float _w1;
        MixerPlayable _mixer;

        public ArmedSwordWholeBodyAnimationStateBase(MixerPlayable mixer, ControllerPlayable controller, string name, float duration = 0, bool enabled = true) : base(controller, name, duration, enabled)
        {
            _mixer = mixer ?? throw new ArgumentNullException(nameof(mixer));
        }

        void RecordWeights()
        {
            _w0 = _mixer.GetChildWeight(0);
            _w1 = _mixer.GetChildWeight(1);
        }
        void UpdateWeights(ushort exceptedValue, float t)
        {
            _mixer.SetChildWeight(0, Mathf.Lerp(_w0, 1 - exceptedValue, t));
            _mixer.SetChildWeight(1, Mathf.Lerp(_w1, exceptedValue, t));
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            RecordWeights();
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            UpdateWeights(1, currentTransition.Timeline.NormalizedTime);
        }
        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            RecordWeights();
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            UpdateWeights(0, currentTransition.Timeline.NormalizedTime);
        }
    }
}
