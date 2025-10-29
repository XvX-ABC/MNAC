using Assets.Scripts.Utilities;
using Locomotion;
using System;
using Tests.Environment;
using Tests.Weapons.MultiMissileLauncher.Animation;
using UnityEngine;
using JState = Tests.Locomotion_Obsolete.JumpLocomotion.State;
namespace Tests.Locomotion_Obsolete.Animation
{
    [Obsolete]
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



            _startEvent = new(() => { _animator.SetBool(_definitions.EnterParamName, true); });
            _endEvent = new(() => _animator.SetBool(_definitions.EnterParamName, false));
        }
        void Start()
        {


            _locomotion = GetComponent<LocomotionCore>()?.boostingLocomotion ?? throw new NullReferenceException(nameof(BoostingLocomotion));

            var duration = _locomotion.Definitions.Duration;
            var v = duration == 0 ? 1 : _definitions.BoostingClipLength / duration * 50;
            _animator.SetFloat(_definitions.DurationMultiplierParamName, v);

            var ld = _locomotion.Definitions;
            var clipLength = _definitions.BoostingClipLength;


            _locomotion.StartAction += _ => _startEvent.Enabled = true;
            _locomotion.EndAction += _ => _endEvent.Enabled = true;

            _jumpLocomotion = GetComponent<LocomotionCore>()?.jumpLocomotion ?? throw new NullReferenceException(nameof(_jumpLocomotion));

        }
        public void OnFixedUpdate(Context context)
        {
            if (!enabled)
                return;
            var jstate = _jumpLocomotion.CurrentState;
            _toJump = jstate > JState.OnGround;
            _startEvent.TryExecute();
            _endEvent.TryExecute();
        }
    }
}