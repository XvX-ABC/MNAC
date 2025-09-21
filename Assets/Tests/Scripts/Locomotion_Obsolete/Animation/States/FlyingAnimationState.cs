using System;
using UnityEngine;
namespace Tests.Locomotion.Animation.States
{
    public class FlyingAnimationState : AnimationStateBase
    {
        IAirLocomotionAnimationDefinitions _definitions;
        AirLocomotion _locomotion;
        internal FlyingAnimationState(Animator animator, IAirLocomotionAnimationDefinitions definitions, AirLocomotion locomotion) : base("flying", animator)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _locomotion = locomotion ?? throw new ArgumentNullException(nameof(locomotion));
        }
        public override void OnEnter()
        {
            animator.SetBool(_definitions.Flying.EnterParamName, true);
        }
        public override void OnExit()
        {
            animator.SetBool(_definitions.Flying.EnterParamName, false);
        }
    }
}