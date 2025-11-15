using Tests.States;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher.Animations
{
    internal class ArmedLauncherAnimationState : WithCallbackStatemachineState<object>
    {
        ArmedLauncherArmAnimator _animator;
        float _w;
        public ArmedLauncherAnimationState(ArmedLauncherArmAnimator animator, float duration = 0, bool enabled = true) : base(animator.statemachine, "armed_launcher_animation", duration, enabled)
        {
            _animator = animator;
        }
        //FIXME：从其他状态到此状态的过渡开始时，动画会产生意外的扭曲行为
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            statemachine.ChangeStateTo(_animator.AimingTarget == null ? _animator.idle : _animator.aiming);
            base.FromPreviousStateTransitionBegin(currentTransition);
            if (_animator.OutputSetting != null)
                _w = _animator.OutputSetting.Weight;
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            if (_animator.OutputSetting != null)
            {
                var t = currentTransition.Timeline.NormalizedTime;
                _animator.OutputSetting.Weight = Mathf.Lerp(_w, 1, t);
            }
        }
        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            if (_animator.OutputSetting != null)
                _w = _animator.OutputSetting.Weight;
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            if (_animator.OutputSetting != null)
            {
                var t = currentTransition.Timeline.NormalizedTime;
                _animator.OutputSetting.Weight = Mathf.Lerp(_w, 0, t);
            }
        }
        public override void OnEnter()
        {
            statemachine.Enabled = true;
            base.OnEnter();
        }
        public override void OnExit()
        {
            statemachine.Enabled = false;
            base.OnExit();
        }
        public override void OnUpdate()
        {
            base.UpdateAction?.Invoke();
        }
    }
}
