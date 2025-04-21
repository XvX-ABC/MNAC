using System;
using Assets.Scripts.Utilities;
using Locomotion;
using Tests.Environment;
using UnityEngine;
using JState = Tests.Locomotion.JumpLocomotion.State;
namespace Tests.Locomotion.Animation
{
    public class BoostingLocomotionAnimator : MonoBehaviour, IModule
    {
        IBoostingLocomotionAnimatorDefinitions _definitions;
        BoostingLocomotion _locomotion;
        Animator _animator;
        JumpLocomotion _jumpLocomotion;
        SingleEvent _startEvent;
        SingleEvent _endEvent;
        [Obsolete]
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


            _locomotion = GetComponent<LocomotionCore>()?.boostingLocomotion ?? throw new NullReferenceException(nameof(BoostingLocomotion));



            var ld = _locomotion.Definitions;
            var clipLength = _definitions.BoostingClipLength;


            var plength = ld.Duration * _definitions.PreparatoryProportion;
            var v = plength == 0 ? 0 : clipLength / plength;
            _animator.SetFloat(_definitions.PreparationMultiplierParamName, v);

            var dlength = ld.Duration * (1 - _definitions.PreparatoryProportion);
            v = dlength == 0 ? 0 : clipLength / dlength;
            _animator.SetFloat(_definitions.DurationMultiplierParamName, v);

            _locomotion.StartAction += _ => _startEvent.Enabled = true;
            _locomotion.EndAction += _ => _endEvent.Enabled = true;

            _jumpLocomotion = GetComponent<LocomotionCore>()?.jumpLocomotion ?? throw new NullReferenceException(nameof(_jumpLocomotion));

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