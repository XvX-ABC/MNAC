using System;
using Locomotion;
using NUnit.Framework.Constraints;
using UnityEngine;
using JState = Tests.Locomotion.JumpLocomotion.State;
namespace Tests.Locomotion.Animation
{
    public class JumpLocomotionAnimator : MonoBehaviour, IModule
    {
        IJumpLocomotionAnimationDefinitions _definition;
        JumpLocomotion _jumpLocomotion;
        Animator _animator;
        JState _oldState;
        IGroundSampler _sampler;
        float _maxHeight;
        void Awake()
        {
            var ldefinition = GetComponent<ILocomotionAnimationDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILocomotionAnimationDefinitions));
            _definition = ldefinition.Jump ?? throw new NullReferenceException(nameof(ldefinition.Jump));

            _animator = GetComponent<Animator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Animator));
            _sampler = GetComponent<IGroundSampler>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IGroundSampler));

        }
        void Start()
        {
            var controlBase = GetComponent<LocomotionControlBase>() ?? throw new ComponentCantFindException(this.gameObject, typeof(LocomotionControlBase));
            _jumpLocomotion = controlBase.jumpLocomotion ?? throw new NullReferenceException(nameof(controlBase.jumpLocomotion));


            var clipLength = _definition.AscendingClipLength;
            var length = _jumpLocomotion.AscendingDuration;
            _animator.SetFloat(_definition.AscendingMultiplierName, clipLength / length);

            clipLength = _definition.DescendingClipLength;
            length = _jumpLocomotion.Definition.LandingDuration;
            _animator.SetFloat(_definition.LandingMultiplierName, clipLength / length);

        }
        void Landing()
        {
            var point = _sampler.Point;
            var groundHeight = point.y;
            var currentHeight = _sampler.CurrentHeight;
            var v = currentHeight / (_maxHeight - groundHeight);
            _animator.Play(_definition.DescendingClipName, 0, Mathf.Clamp01(1 - v));
        }
        public void OnUpdate(Context context)
        {
            var currentState = _jumpLocomotion.CurrentState;
            if (currentState == _oldState)
                return;


            if (_oldState == JState.Idle)
            {
                var pos = context.Position;
                _animator.SetBool(_definition.EnterParamName, true);
                _maxHeight = pos.y + _jumpLocomotion.Definition.Height;
                _oldState = currentState;
            }
            else if (currentState == JState.Idle)
            {
                _animator.SetBool(_definition.EnterParamName, false);
                _oldState = currentState;
            }

            if (currentState == JState.Descending)
            {
                Landing();
            }
        }
    }
}