using Tests.Characters.Humanoid.Arms;
using Tests.Characters.Humanoid.Locomotion;
using Tests.States;
using Tests.Utilities.Timeline;

namespace Tests.Characters.Humanoid
{
    internal class NormalState : WithCallbackPlayableStatemachine<object>
    {
        ArmController _leftArmCore;
        PlayableStateMachine _leftArmStatemachine;
        ArmController _rightArmCore;
        PlayableStateMachine _rightArmStatemachine;
        LocomotionCore _core;
        LocomotionStatemachine _lstatemachine;

        public NormalState(LocomotionCore core, ArmController leftArmCore, ArmController rightArmCore, bool enabled = true) : base("locomotion", enabled)
        {

            _leftArmCore = leftArmCore;
            _rightArmCore = rightArmCore;
            _leftArmStatemachine = _leftArmCore?.stateMachine;
            _rightArmStatemachine = _rightArmCore?.stateMachine;
            _core = core;
            _lstatemachine = _core.statemachine;
        }
        public override ITimeline Timeline => _lstatemachine.Timeline;
        public override void ChangeStateTo(IPlayableState<object> state)
        {
            _lstatemachine.ChangeStateTo(state);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            _core.enabled = true;
            if (_leftArmCore != null && !_leftArmCore.enabled)
                _leftArmCore.enabled = true;
            if (_rightArmCore != null && !_rightArmCore.enabled)
                _rightArmCore.enabled = true;
            _lstatemachine.OnEnter();
            _leftArmStatemachine?.OnEnter();
            _rightArmStatemachine?.OnEnter();
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
            _core.enabled = false;
            if (_leftArmCore != null)
                _leftArmCore.enabled = false;
            if (_rightArmCore != null)
                _rightArmCore.enabled = false;
            _lstatemachine.OnExit();
            _leftArmStatemachine?.OnExit();
            _rightArmStatemachine?.OnExit();
            base.OnExit();
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _core.enabled = true;
            if (_leftArmCore != null)
                _leftArmCore.enabled = true;
            if (_rightArmCore != null)
                _rightArmCore.enabled = true;
            _lstatemachine.FromPreviousStateTransitionBegin(currentTransition);
            _leftArmStatemachine?.FromPreviousStateTransitionBegin(currentTransition);
            _rightArmStatemachine?.FromPreviousStateTransitionBegin(currentTransition);
        }
        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            _lstatemachine.FromPreviousStateTransitionEnd(currentTransition);
            _leftArmStatemachine?.FromPreviousStateTransitionEnd(currentTransition);
            _rightArmStatemachine?.FromPreviousStateTransitionEnd(currentTransition);
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            _lstatemachine.FromPreviousStateTransitionRunning(currentTransition);
            _leftArmStatemachine?.FromPreviousStateTransitionRunning(currentTransition);
            _rightArmStatemachine?.FromPreviousStateTransitionRunning(currentTransition);
        }
        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            _lstatemachine.ToNextStateTransitionBegin(currentTransition);
            _leftArmStatemachine?.ToNextStateTransitionBegin(currentTransition);
            _rightArmStatemachine?.ToNextStateTransitionBegin(currentTransition);
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            _lstatemachine.ToNextStateTransitionEnd(currentTransition);
            _leftArmStatemachine?.ToNextStateTransitionEnd(currentTransition);
            _rightArmStatemachine?.ToNextStateTransitionEnd(currentTransition);
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            _lstatemachine.ToNextStateTransitionRunning(currentTransition);
            _leftArmStatemachine?.ToNextStateTransitionRunning(currentTransition);
            _rightArmStatemachine?.ToNextStateTransitionRunning(currentTransition);
        }

    }
}
