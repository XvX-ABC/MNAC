using System;
using System.Threading;
using MNAC.Animations;
using MNAC.States;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons.Sword.Animations
{
    internal abstract class ArmedSwordWholeBodyAnimationStateBase : ArmedSwordAnimationStateBase
    {
        float _w0;
        float _w1;
        MixerPlayable _mixer;
        ControllerPlayable _baseController;
        protected WholeBodyControllerPlayable wholeBodyController;
        float baseWeight { get => _baseController.OutputSetting.Weight; set => _baseController.OutputSetting.Weight = value; }
        float wholeBodyWeight { get => controller.OutputSetting.Weight; set => controller.OutputSetting.Weight = value; }

        public ArmedSwordWholeBodyAnimationStateBase(ControllerPlayable baseWholeBodyController, WholeBodyControllerPlayable wholeBodyController, string name, float duration = 0, bool enabled = true) : base(wholeBodyController, name, duration, enabled)
        {
            _baseController = baseWholeBodyController ?? throw new ArgumentNullException(nameof(baseWholeBodyController));
            this.wholeBodyController = wholeBodyController ?? throw new ArgumentNullException(nameof(wholeBodyController));
        }

        void RecordWeights()
        {
            _w0 = baseWeight;
            _w1 = wholeBodyWeight;
        }
        void UpdateWeights(ushort exceptedWeight, float t)
        {
            baseWeight = Mathf.Lerp(_w0, 1 - exceptedWeight, t);
            wholeBodyWeight = Mathf.Lerp(_w1, exceptedWeight, t);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            UpdateWeights(1, 1);
        }
        public override void OnExit()
        {
            //UpdateWeights(0, 1);
            base.OnExit();
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
