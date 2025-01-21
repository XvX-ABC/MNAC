using System;
using Locomotion;
using NUnit.Framework.Constraints;
using UnityEngine;
using JState = Tests.Locomotion.JumpLocomotion.State;
namespace Tests.Locomotion.Animation
{
    public class JumpLocomotionAnimator : MonoBehaviour, IModule
    {
        IJumpLocomotionAnimationDefines _defines;
        JumpLocomotion _jumpLocomotion;
        Animator _animator;
        JState _oldState;
        IGroundSampler _sampler;
        float _maxHeight;
        void Awake()
        {
            var ldefines = GetComponent<ILocomotionAnimationDefines>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILocomotionAnimationDefines));
            _defines = ldefines.Jump ?? throw new NullReferenceException(nameof(ldefines.Jump));

            _animator = GetComponent<Animator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Animator));
            _sampler = GetComponent<IGroundSampler>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IGroundSampler));

        }
        void Start()
        {
            var controlBase = GetComponent<LocomotionControlBase>() ?? throw new ComponentCantFindException(this.gameObject, typeof(LocomotionControlBase));
            _jumpLocomotion = controlBase.jumpLocomotion ?? throw new NullReferenceException(nameof(controlBase.jumpLocomotion));


            var clipLength = _defines.AscendingClipLength;
            var length = _jumpLocomotion.AscendingDuration;
            _animator.SetFloat(_defines.AscendingMultiplierName, clipLength / length);

            clipLength = _defines.DescendingClipLength;
            length = _jumpLocomotion.Defines.LandingDuration;
            _animator.SetFloat(_defines.LandingMultiplierName, clipLength / length);

        }
        void Landing()
        {
            var point = _sampler.Point;
            var groundHeight = point.y;
            var currentHeight = _sampler.CurrentHeight;
            var v = currentHeight / (_maxHeight - groundHeight);
            _animator.Play(_defines.DescendingClipName, 0, Mathf.Clamp01(1 - v));
        }
        public void OnUpdate(Context context)
        {
            var currentState = _jumpLocomotion.CurrentState;
            if (currentState == _oldState)
                return;


            if (_oldState == JState.Idle)
            {
                var pos = context.Position;
                _animator.SetBool(_defines.EnterParamName, true);
                _maxHeight = pos.y + _jumpLocomotion.Defines.Height;
                _oldState = currentState;
            }
            else if (currentState == JState.Idle)
            {
                _animator.SetBool(_defines.EnterParamName, false);
                _oldState = currentState;
            }

            if (currentState == JState.Descending)
            {
                Landing();
            }
        }
    }
}