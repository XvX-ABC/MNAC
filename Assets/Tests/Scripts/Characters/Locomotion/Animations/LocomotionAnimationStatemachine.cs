using System;
using Tests.States;

namespace Tests.Characters.Locomotion.Animations
{
    internal class LocomotionAnimationStatemachine : WithCallbackPlayableStatemachine<LocomotionAnimationStateContext>, IState<LocomotionAnimationStateContext>
    {
        public LocomotionAnimationStatemachine(string name, LocomotionAnimationStateContext context, bool enabled = true) : base($"{name}_locomotion_animation_statemachine", enabled)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        void IState<LocomotionAnimationStateContext>.AddTransition(ITransition<LocomotionAnimationStateContext> transition)
        {
            if (transition == null)
                throw new ArgumentNullException(nameof(transition), "Transition cannot be null.");
            var index = FindTransitionIndex(transition.DestinationState);
            if (index > -1)
                return;
            var transitions = this.transitions;
            if (transitions == null)
            {
                transitions = new ITransition<LocomotionAnimationStateContext>[] { transition };
            }
            else
            {
                Array.Resize(ref transitions, transitions.Length + 1);
                transitions[^1] = transition;
            }
            this.transitions = transitions;
        }
        void IState<LocomotionAnimationStateContext>.RemoveTransition(IState<LocomotionAnimationStateContext> destinationState)
        {
            if (destinationState == null)
                throw new ArgumentNullException(nameof(destinationState), "Destination state cannot be null.");
            var transitions = this.transitions;
            if (transitions == null || transitions.Length == 0)
                return;
            int index = FindTransitionIndex(destinationState);
            if (index == -1)
                return;
            if (transitions.Length == 1)
                transitions = null;
            else
            {
                if (transitions.Length != index)
                    Array.Copy(transitions, index + 1, transitions, index, transitions.Length - index - 1);
                Array.Resize(ref transitions, transitions.Length - 1);
            }
            this.transitions = transitions;
        }
    }
}
