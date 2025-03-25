using System;
using UnityEngine;
using JState = Tests.Locomotion.JumpLocomotion.State;
namespace Tests.Locomotion.Animation
{
    public class AirLocomotionAnimator : MonoBehaviour, IModule
    {
        IAirLocomotionAnimationDefinitions _definition;
        Animator _animator;
        JumpLocomotion _jumpLocomotion;
        void Awake()
        {
            var ldefinition = GetComponent<ILocomotionAnimationDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILocomotionAnimationDefinitions));
            _definition = ldefinition.Air ?? throw new NullReferenceException(nameof(ldefinition.Air));
            _animator = GetComponent<Animator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Animator));
        }
        void Start()
        {
            var controlBase = GetComponent<LocomotionControlBase>() ?? throw new ComponentCantFindException(this.gameObject, typeof(LocomotionControlBase));
            //_jumpLocomotion = controlBase.jumpLocomotion ?? throw new NullReferenceException(nameof(controlBase.jumpLocomotion));
        }
        public void OnUpdate(Context context)
        {
            //var ground = context.Ground;
            //if (ground!=null)
            //{
            //    if (_animator.GetBool(_definition.EnterParamName))
            //        _animator.SetBool(_definition.EnterParamName, false);
            //    return;
            //}
            //if (_jumpLocomotion.CurrentState > JState.Idle && _jumpLocomotion.CurrentState <= JState.Ascending)
            //    return;

            //var input = context.Input;
            //if (!input.IsAscending)
            //    return;


            //_animator.SetBool(_definition.EnterParamName, true);


            //var velocity = context.Velocity * 0.05f;
            //_animator.SetFloat(_definition.XParamName, velocity.x);
            //_animator.SetFloat(_definition.YParamName, velocity.z);
        }
    }
}