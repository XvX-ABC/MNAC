using System;
using UnityEngine;
namespace Tests.Locomotion_Obsolete.Animation.States
{
    public class BoostingLocomotionAnimationState : AnimationStateBase
    {
        IBoostingLocomotionAnimatorDefinitions _definitions;
        internal BoostingLocomotionAnimationState(Animator animator, IBoostingLocomotionAnimatorDefinitions definitions, BoostingLocomotion locomotion) : base("boosting", animator)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            var duration = locomotion.Definitions.Duration;
            var v = duration == 0 ? 1 : _definitions.BoostingClipLength / duration * 20;
            animator.SetFloat(_definitions.DurationMultiplierParamName, v);
        }
        public override void OnEnter()
        {
            animator.SetBool(_definitions.EnterParamName, true);
        }
        public override void OnExit()
        {
            animator.SetBool(_definitions.EnterParamName, false);
        }
    }
}