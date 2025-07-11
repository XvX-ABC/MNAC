using Assets.Scripts.Utilities.Timeline;
using System;
using UnityEditor.Timeline;
using UnityEngine;

namespace Tests.States
{
    public class PlayableStateMachine : PlayableStateMachine<object>
    {
        public PlayableStateMachine(string name, bool enabled = true) : base(name, enabled)
        {
        }
    }
    public partial class PlayableStateMachine<T> : StateMachineBase<IPlayableState<T>, T>
    {
        class TransitionState : PlayableStateBase<T>
        {
            internal IPlayableState<T> srcState;
            internal IPlayableState<T> desState;
            internal IPlayableTransition<T> srcTransition;
            public TransitionState() : base("", 0, true)
            {
                var transition = new PlayableTransition()
                {
                    sourceState = this,
                    destinationState = null,
                    triggerEvent = null,
                };
                this.transitions = new ITransition<T>[] { transition };
            }
            protected override ITimeline NewTimeline(float duration)
            {
                return null;
            }
            public void Update(IPlayableTransition<T> srcTransition)
            {
                timeline = srcTransition.Timeline;
                if (timeline == null)
                    throw new NullReferenceException(nameof(srcTransition.Timeline));
                srcState = (IPlayableState<T>)srcTransition.SourceState;
                desState = (IPlayableState<T>)srcTransition.DestinationState;
                name = $"{srcState.Name} ->  {desState.Name}";
                UpdateTransitionWhichToNextState();
            }
            void UpdateTransitionWhichToNextState()
            {
                var t = this.transitions[0] as PlayableTransition;
                t.destinationState = desState;
                t.triggerEvent = () => timeline.NormalizedTime >= 0;

            }
            public override void OnEnter()
            {
                timeline.Start();
            }
            public override void OnExit()
            {
                timeline.Stop();
            }
            public override void OnUpdate()
            {
                timeline.OnUpdate(Time.deltaTime);
            }

        }
        TransitionState _transitionState;
        public PlayableStateMachine(string name, bool enabled = true) : base(name, enabled)
        {
            _transitionState = new();
        }

        protected PlayableTransition NewTransition(IPlayableState<T> sourceState, IPlayableState<T> destinationState, Func<bool> triggerEvent, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent, float duration)
        {
            var transition = new PlayableTransition(sourceState, destinationState, triggerEvent, durationEvent, duration);
            return transition;
        }
        public override void AddTransitionFor(IPlayableState<T> state, IPlayableState<T> destinationState, Func<bool> triggerEvent)
        {
            var transition = NewTransition(state, destinationState, triggerEvent, null, 0);
            AddTransitionFor(transition);
        }
        public void AddTransitionFor<S, D>(S state, D destinationState, float duration, Func<bool> triggerEvent, Action<S, D, float> durationEvent) where S : class, IPlayableState<T> where D : class, IPlayableState<T>
        {
            var transition = NewTransition(state, destinationState, triggerEvent, (Action<IPlayableState<T>, IPlayableState<T>, float>)durationEvent, duration);
            AddTransitionFor(transition);
        }
        public override void AddTransition(ITransition<T> transition)
        {
            base.AddTransitionFor(transition as IPlayableTransition<T>);
        }
        protected override IPlayableState<T> CheckTransitions()
        {
            var state = base.CheckTransitions();
            if (state != null && state.ExitWhenEnd)
            {
                var timeline = state.Timeline;
                return timeline.NormalizedTime >= 1 ? state : null;
            }
            return state;
        }
        protected override void ChangeState(IPlayableState<T> nextState)
        {
            try
            {
                currentState.OnExit();
            }
            catch (Exception e)
            {
                throw new StateExitException(currentState, e, "currentState");
            }


            if (currentState != _transitionState)
            {
                var transition = currentState.FindTransition(nextState);
                try
                {
                    _transitionState.Update((IPlayableTransition<T>)transition);
                    currentState = _transitionState;
                }
                catch (NullReferenceException)
                {
                    currentState = nextState;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                currentState = nextState;
            }


            try
            {
                currentState.OnEnter();
            }
            catch (Exception e)
            {

                throw new StateEntryException(currentState, e, "currentState");
            }
        }
    }

}
