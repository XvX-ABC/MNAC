using Assets.Scripts.Utilities;
using System;
using Tests.Environment;
using Unity.VisualScripting;
using UnityEngine;
using JState = Tests.Locomotion.JumpLocomotion.State;
namespace Tests.Locomotion.Animation
{
    public class AirLocomotionAnimator : MonoBehaviour, IModule
    {
        IAirLocomotionAnimationDefinitions _definitions;
        Animator _animator;
        AirLocomotion _locomotion;
        JumpLocomotion _jumpLocomotion;
        SingleEvent<Context> _startEvent;
        SingleEvent<Context> _endEvent;
        void Awake()
        {
            var ldefinition = GetComponent<ILocomotionAnimationDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILocomotionAnimationDefinitions));
            _definitions = ldefinition.Air ?? throw new NullReferenceException(nameof(ldefinition.Air));
            _animator = GetComponent<Animator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Animator));
        }
        void Start()
        {
            var controlBase = GetComponent<LocomotionCore>() ?? throw new ComponentCantFindException(this.gameObject, typeof(LocomotionCore));
            _jumpLocomotion = GetComponent<LocomotionCore>()?.jumpLocomotion ?? throw new NullReferenceException(nameof(_jumpLocomotion));
            _locomotion = GetComponent<LocomotionCore>()?.airLocomotion ?? throw new NullReferenceException(nameof(_locomotion));

            _startEvent = new(_ => _animator.SetBool(_definitions.EnterParamName, true));
            _endEvent = new(_ => _animator.SetBool(_definitions.EnterParamName, false));

            _locomotion.StartAction += _ => _startEvent.Enabled = true;
            _locomotion.EndAction += _ => _endEvent.Enabled = true;
        }
        void UpdateVelocity(Context context)
        {
            var rotation = context.OriginalLocomotion.Rotation;
            var velocity = context.Velocity;
            var v = Quaternion.Inverse(rotation) * velocity * 0.05f;
            //var v = velocity * 0.05f;
            _animator.SetFloat(_definitions.XParamName, v.x);
            _animator.SetFloat(_definitions.YParamName, v.z);
        }
        public void OnFixedUpdate(Context context)
        {
            if (!enabled)
                return;
            var ground = context.Ground;
            _startEvent.TryExecute(context);
            _endEvent.TryExecute(context);
            if (ground == null)
                UpdateVelocity(context);

        }
    }
}