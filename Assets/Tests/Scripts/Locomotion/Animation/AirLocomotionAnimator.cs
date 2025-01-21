using System;
using UnityEngine;
using JState = Tests.Locomotion.JumpLocomotion.State;
namespace Tests.Locomotion.Animation
{
    public class AirLocomotionAnimator : MonoBehaviour, IModule
    {
        IAirLocomotionAnimationDefines _defines;
        Animator _animator;
        JumpLocomotion _jumpLocomotion;
        void Awake()
        {
            var ldefines = GetComponent<ILocomotionAnimationDefines>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILocomotionAnimationDefines));
            _defines = ldefines.Air ?? throw new NullReferenceException(nameof(ldefines.Air));
            _animator = GetComponent<Animator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Animator));
        }
        void Start()
        {
            var controlBase = GetComponent<LocomotionControlBase>() ?? throw new ComponentCantFindException(this.gameObject, typeof(LocomotionControlBase));
            _jumpLocomotion = controlBase.jumpLocomotion ?? throw new NullReferenceException(nameof(controlBase.jumpLocomotion));
        }
        public void OnUpdate(Context context)
        {
            var ground = context.Ground;
            if (ground.Touched)
            {
                if (_animator.GetBool(_defines.EnterParamName))
                    _animator.SetBool(_defines.EnterParamName, false);
                return;
            }
            if (_jumpLocomotion.CurrentState > JState.Idle && _jumpLocomotion.CurrentState <= JState.Ascending)
                return;

            var input = context.Input;
            if (!input.IsAscending)
                return;


            _animator.SetBool(_defines.EnterParamName, true);


            var velocity = context.Velocity * 0.05f;
            _animator.SetFloat(_defines.XParamName, velocity.x);
            _animator.SetFloat(_defines.YParamName, velocity.z);
        }
    }
}