using System;
using Tests.Behaviours.Arms.Weapons.Launchers.Animations;
using Tests.States;
using TMPro;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launchers
{
    internal class ArmedLauncherArmBehaviourState : WithCallbackStatemachineState<object>
    {
        ArmedLauncherArmBehaviour _behaviour;
        ArmedLauncherArmAnimator _animator;
        ArmedLauncherAnimationState _animationState;
        float _w;
        public ArmedLauncherArmBehaviourState(ArmedLauncherArmBehaviour behaviour, bool enabled = true) : base(behaviour.statemachine, "armed _launcher", 0, enabled)
        {
            _behaviour = behaviour ?? throw new ArgumentNullException(nameof(behaviour));
            _animator = _behaviour.animator;
            _animationState = _animator.state;
        }

        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _animationState.FromPreviousStateTransitionBegin(currentTransition);
            if (_animator.OutputSetting != null)
            {
                _w = _animator.OutputSetting.Weight;
                Debug.Log("b w: " + _w);
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
                Debug.Log("b weight: " + _animator.OutputSetting.Weight);
                _animator.OutputSetting.Weight = currentTransition.Timeline.NormalizedTime;
            }

        }

        public override void OnEnter()
        {

            statemachine.ChangeStateTo(_behaviour.target == null ? _behaviour.idle : _behaviour.aiming);
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
                Debug.Log("a w: " + _w);
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
                _animator.OutputSetting.Weight = 1 - currentTransition.Timeline.NormalizedTime;
                Debug.Log("a weight: " + _animator.OutputSetting.Weight);
            }
        }
    }
}
