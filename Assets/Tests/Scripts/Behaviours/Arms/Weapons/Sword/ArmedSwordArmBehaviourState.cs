using System;
using Tests.Behaviours.Arms.Weapons.Sword.Animations;
using Tests.States;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword
{
    internal class ArmedSwordArmBehaviourState : WithCallbackStatemachineState<object>
    {
        ArmedSwordArmBehaviour _behaviour;
        ArmedSwordArmAnimator _animator;
        ArmedSwordAnimationState _animationState;
        float _w;
        public ArmedSwordArmBehaviourState(ArmedSwordArmBehaviour behaviour, bool enabled = true) : base(behaviour.statemachine, "armed _Sword", 0, enabled)
        {
            _behaviour = behaviour ?? throw new ArgumentNullException(nameof(behaviour));
            _animator = _behaviour.animator;
            _animationState = _animator.state;
        }

        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            statemachine.ChangeStateTo(_behaviour.idle);
            _animationState.FromPreviousStateTransitionBegin(currentTransition);
            if (_animator.OutputSetting != null)
            {
                _w = _animator.OutputSetting.Weight;
            }
        }

        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            _animationState.FromPreviousStateTransitionEnd(currentTransition);

        }

        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            _animationState.FromPreviousStateTransitionRunning(currentTransition);
            if (_animator.OutputSetting != null)
            {
                var t = currentTransition.Timeline.NormalizedTime;
                _animator.OutputSetting.Weight = Mathf.Lerp(_w, 1, t);
            }

        }

        public override void OnEnter()
        {


            base.OnEnter();
            _animationState.OnEnter();
            if (_animator.OutputSetting != null)
            {
                _animator.OutputSetting.Weight = 1;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            _animationState.OnExit();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            _animationState.ToNextStateTransitionBegin(currentTransition);
            if (_animator.OutputSetting != null)
            {
                _w = _animator.OutputSetting.Weight;
            }

        }

        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            _animationState.ToNextStateTransitionEnd(currentTransition);
        }

        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            _animationState.ToNextStateTransitionRunning(currentTransition);
            if (_animator.OutputSetting != null)
            {
                var t = currentTransition.Timeline.NormalizedTime;
                _animator.OutputSetting.Weight = Mathf.Lerp(_w, 0, t);
            }
        }
    }
}
