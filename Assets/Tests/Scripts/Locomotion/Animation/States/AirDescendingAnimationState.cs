using System;
using UnityEngine;
namespace Tests.Locomotion.Animation.States
{
    public class AirDescendingAnimationState : AnimationStateBase
    {
        IAirLocomotionAnimationDefinitions _definitions;
        AirLocomotion _locomotion;
        internal AirDescendingAnimationState(Animator animator, IAirLocomotionAnimationDefinitions definitions, AirLocomotion locomotion) : base("air_descending", animator)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _locomotion = locomotion ?? throw new ArgumentNullException(nameof(locomotion));
        }
        public override void OnEnter()
        {
            animator.SetBool(_definitions.Descending.EnterParamName, true);
        }
        public override void OnExit()
        {
            animator.SetBool(_definitions.Descending.EnterParamName, false);
        }
    }
}