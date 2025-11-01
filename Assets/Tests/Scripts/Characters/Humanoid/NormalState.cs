using Tests.Characters.Humanoid.Arms;
using Tests.Characters.Humanoid.Locomotion;
using Tests.States;
using Tests.Utilities.Timeline;

namespace Tests.Characters.Humanoid
{
    internal class NormalState : WithCallbackPlayableStatemachine<object>
    {
        ArmCore _armCore;
        LocomotionCore _core;
        PlayableStateMachine _astatemachine;
        LocomotionStatemachine _lstatemachine;

        public NormalState(LocomotionCore core, ArmCore armCore, bool enabled = true) : base("locomotion", enabled)
        {

            _armCore = armCore;
            _core = core;
            _astatemachine = _armCore?.stateMachine;
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
            if (_armCore != null)
                _armCore.enabled = true;
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
            _core.enabled = false;
            if (_armCore != null)
                _armCore.enabled = false;
            base.OnExit();
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _lstatemachine.FromPreviousStateTransitionBegin(currentTransition);
            _astatemachine?.FromPreviousStateTransitionBegin(currentTransition);
        }
        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            _lstatemachine.FromPreviousStateTransitionEnd(currentTransition);
            _astatemachine?.FromPreviousStateTransitionEnd(currentTransition);
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            _lstatemachine.FromPreviousStateTransitionRunning(currentTransition);
            _astatemachine?.FromPreviousStateTransitionRunning(currentTransition);
        }
        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            _lstatemachine.ToNextStateTransitionBegin(currentTransition);
            _astatemachine?.ToNextStateTransitionBegin(currentTransition);
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            _lstatemachine.ToNextStateTransitionEnd(currentTransition);
            _astatemachine?.ToNextStateTransitionEnd(currentTransition);
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            _lstatemachine.ToNextStateTransitionRunning(currentTransition);
            _astatemachine?.ToNextStateTransitionRunning(currentTransition);
        }

    }
}
