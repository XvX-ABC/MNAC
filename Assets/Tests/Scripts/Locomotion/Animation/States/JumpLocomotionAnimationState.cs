using System;
using Tests.Weapons.MultiMissileLauncher.Animation;
using UnityEngine;
namespace Tests.Locomotion.Animation.States
{
    public class JumpLocomotionAnimationState : AnimationStateBase
    {
        IJumpLocomotionAnimationDefinitions _definitions;
        internal JumpLocomotionAnimationState(Animator animator, IJumpLocomotionAnimationDefinitions definitions, JumpLocomotion locomotion) : base("jump", animator)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            var length = locomotion.ascendingDurationTime;
            var clipLength = _definitions.AscendingClipLength;
            var v = length == 0 ? 0 : clipLength / length;
            animator.SetFloat(_definitions.AscendingMultiplierName, v);
        }

        public override void OnEnter()
        {
            //animator.SetBool(_definitions.EnterParamName, true);
            animator.SetTrigger(_definitions.EnterParamName);

        }


    }
}