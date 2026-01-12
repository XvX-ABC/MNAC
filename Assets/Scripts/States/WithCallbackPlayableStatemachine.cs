using System;

namespace Tests.States
{
    public class WithCallbackPlayableStatemachine<T> : PlayableStateMachine<T>, IWithCallbackPlayableState<T>
    {
        protected Action entryAction;
        protected Action exitAction;
        protected Action updateAction;

        public WithCallbackPlayableStatemachine(string name, bool enabled = true) : base(name, enabled)
        {
        }

        public Action EntryAction { get => entryAction; set => entryAction = value; }
        public Action ExitAction { get => exitAction; set => exitAction = value; }
        public Action UpdateAction { get => updateAction; set => updateAction = value; }
        public override void OnEnter()
        {
            entryAction?.Invoke();
            base.OnEnter();
        }
        public override void OnExit()
        {
            base.OnExit();
            exitAction?.Invoke();
        }
        public override void OnUpdate()
        {
            updateAction?.Invoke();
            base.OnUpdate();
        }

        void IState<T>.AddTransition(ITransition<T> transition)
        {
            if (transition == null)
                throw new ArgumentNullException(nameof(transition), "Transition cannot be null.");
            var index = FindTransitionIndex(transition.DestinationState);
            if (index > -1)
                return;
            var transitions = this.transitions;
            if (transitions == null)
            {
                transitions = new ITransition<T>[] { transition };
            }
            else
            {
                Array.Resize(ref transitions, transitions.Length + 1);
                transitions[^1] = transition;
            }
            this.transitions = transitions;
        }
        void IState<T>.RemoveTransition(IState<T> destinationState)
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


