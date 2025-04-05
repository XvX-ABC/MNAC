using System;
using Assets.Scripts.Utilities;
using Locomotion;
using UnityEngine;
using JState = Tests.Locomotion.JumpLocomotion_New.State;
namespace Tests.Locomotion.Animation
{
    public class BoostingLocomotionAnimator : MonoBehaviour, IModule
    {
        IBoostingLocomotionAnimatorDefinitions _definitions;
        QuickBoostLocomotion _locomotion;
        Animator _animator;
        JumpLocomotion_New _jumpLocomotion;
        SingleEvent _startEvent;
        SingleEvent _endEvent;
        bool _toJump;
        void Awake()
        {
            _definitions = GetComponent<ILocomotionAnimationDefinitions>()?.Boosting ?? throw new ComponentCantFindException(this.gameObject, typeof(IBoostingLocomotionAnimatorDefinitions));
       
            _animator = GetComponent<Animator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Animator));

            _startEvent = new(() => _animator.SetTrigger(_definitions.EnterParamName));
            _endEvent = new(() => _animator.SetBool(_definitions.ToJumpParamName, _toJump));

        }
        void Start()
        {


            _locomotion = GetComponent<LocomotionControlBase>()?.quickBoostLocomotion ?? throw new NullReferenceException(nameof(QuickBoostLocomotion));



            var ld = _locomotion.Definitions;
            var length = ld.Duration;
            var clipLength = _definitions.BoostingClipLength;
            var v = length == 0 ? 0 : clipLength / length;
            _animator.SetFloat(_definitions.SpeedMultiplierParamName, v);

            _locomotion.StartAction += _ => _startEvent.Enabled = true;
            _locomotion.EndAction += _ => _endEvent.Enabled = true;

            _jumpLocomotion = GetComponent<LocomotionControlBase>()?.jumpLocomotion_New ?? throw new NullReferenceException(nameof(_jumpLocomotion));

        }
        public void OnFixedUpdate(Context context)
        {
            var jstate = _jumpLocomotion.CurrentState;
            _toJump = jstate > JState.OnGround;
            _startEvent.TryExecute();
            _endEvent.TryExecute();
        }
    }
}