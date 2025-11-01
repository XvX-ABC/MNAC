using System;
using System.Collections.Generic;
using System.Text;
using Tests.Utilities.Timeline;
using UnityEngine;

namespace Tests.States
{
    public class PlayableStateMachine : PlayableStateMachine<object>
    {
        public PlayableStateMachine(string name, bool enabled = true) : base(name, enabled)
        {
        }
    }
    public partial class PlayableStateMachine<T> : StateMachineBase<IPlayableState<T>, T>, IPlayableState<T>
    {
        protected const InterruptionSource INTERRUPTION_SOURCE_DEFAULT = PlayableTransition<T>.INTERRUPTION_SOURCE_DEFAULT;
        class TransitionState : PlayableStateBase<T>
        {
            internal IPlayableState<T> srcState;
            internal IPlayableState<T> desState;
            internal IPlayableTransition<T> currentTransition;
            List<ITransition<T>> _list;
            public TransitionState() : base("", 0, true)
            {
                var transition = new PlayableTransition<T>()
                {
                    sourceState = this,
                    destinationState = null,
                };
                this.transitions = new ITransition<T>[] { transition };
                _list = new();
            }
            protected override ITimeline NewTimeline(float duration)
            {
                return null;
            }
            public void Update(IPlayableTransition<T> srcTransition)
            {

                this.currentTransition = srcTransition;
                timeline = srcTransition.Timeline ?? throw new NullReferenceException(nameof(srcTransition.Timeline));
                srcState = (IPlayableState<T>)srcTransition.SourceState;
                desState = (IPlayableState<T>)srcTransition.DestinationState;
                name = $"{srcState.Name} ->  {desState.Name}";


                Array.Resize(ref this.transitions, 1);
                var t = this.transitions[0] as PlayableTransition<T>;
                t.destinationState = desState;
                t.ClearTriggerEvents();
                t.AddTriggerEvent(() => timeline.NormalizedTime >= 1);


                var interruptionSource = srcTransition.InterruptionSource;
                switch (interruptionSource)
                {
                    case InterruptionSource.None:
                        break;
                    case InterruptionSource.Next:
                        SetInterruptionSourceByNextState();
                        break;
                }
            }
            void SetInterruptionSourceByNextState()
            {
                _list.Clear();
                if (desState.Transitions == null || desState.Transitions.Length == 0)
                    return;
                foreach (var ts in desState.Transitions)
                    if (ts.TriggerEvents.Count > 0)
                        _list.Add(ts);
                var dLength = _list.Count;
                if (dLength > 0)
                {
                    Array.Resize(ref this.transitions, dLength + 1);
                    Array.Copy(desState.Transitions, 0, this.transitions, 1, dLength);
                }
            }
            public override void OnEnter()
            {
                timeline.Restart();
            }
            public override void OnExit()
            {
                timeline.End();
            }
            public override void OnUpdate()
            {
                timeline.OnUpdate(Time.deltaTime);
            }

        }
        TransitionState _transitionState;
        public virtual ITimeline Timeline => currentState?.Timeline;

        public bool ExitWhenEnd
        {
            get => false;
            set { }
        }
        IPlayableTransition<T>[] IPlayableState<T>.Transitions => currentState?.Transitions;


        public PlayableStateMachine(string name, bool enabled = true) : base(name, enabled)
        {
            _transitionState = new();
        }
        protected IPlayableTransition<T> NewTransition(IPlayableState<T> sourceState, IPlayableState<T> destinationState, Func<bool> triggerEvent, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent, float duration, InterruptionSource interruptionSource)
        {
            var transition = new PlayableTransition<T>(sourceState, destinationState, triggerEvent, durationEvent, duration, interruptionSource);
            return transition;
        }
        public override void AddTransitionFor(IPlayableState<T> state, IPlayableState<T> destinationState, Func<bool> triggerEvent)
        {
            var transition = NewTransition(state, destinationState, triggerEvent, null, 0, INTERRUPTION_SOURCE_DEFAULT);
            AddTransitionFor(transition);
        }
        public IPlayableTransition<T> AddTransitionFor(IPlayableState<T> state, IPlayableState<T> destinationState, float duration, Func<bool> triggerEvent, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent)
        {
            return AddTransitionFor(state, destinationState, duration, triggerEvent, durationEvent, INTERRUPTION_SOURCE_DEFAULT);
        }

        public IPlayableTransition<T> AddTransitionFor(IPlayableState<T> state, IPlayableState<T> destinationState, float duration, Func<bool> triggerEvent, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent, InterruptionSource interruptionSource)
        {
            var transition = NewTransition(state, destinationState, triggerEvent, durationEvent, duration, interruptionSource);
            AddTransitionFor(transition);
            return transition;
        }

        public IPlayableTransition<T> AddTransitionFor(IPlayableState<T> state, IPlayableState<T> destinationState, float duration, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent)
        {
            return AddTransitionFor(state, destinationState, duration, null, durationEvent);
        }
        [Obsolete]
        public override void AddTransition(ITransition<T> transition)
        {
            AddTransitionFor(transition as IPlayableTransition<T>);
        }
        void IState<T>.AddTransition(ITransition<T> transition)
        {
            base.AddTransition(transition as IPlayableTransition<T>);
        }
        protected override ITransition<T> CheckTransitions()
        {
            var transition = base.CheckTransitions();
            if (transition != null && currentState.ExitWhenEnd)
            {
                var timeline = currentState.Timeline;
                return timeline.NormalizedTime >= 1 ? transition : null;
            }
            return transition;
        }
        protected override void ChangeState(ITransition<T> triggeredTransition)
        {
            var nextState = default(IState<T>);
            var transition = triggeredTransition;
            if (currentState != _transitionState || _transitionState != triggeredTransition.SourceState)
            {
                try
                {
                    _transitionState.Update((IPlayableTransition<T>)transition);
                    nextState = _transitionState;
                }
                catch (NullReferenceException)
                {
                    Debug.Log("Change to destiantion state directly because transition timeline is null.");
                    nextState = transition.DestinationState;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
                nextState = transition.DestinationState;
            ChangeState(nextState);
        }

        public virtual void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
            currentState?.FromPreviousStateTransitionRunning(currentTransition);
        }

        public virtual void ToNextStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
            currentState?.ToNextStateTransitionRunning(currentTransition);
        }

        public virtual void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
            currentState?.FromPreviousStateTransitionBegin(currentTransition);
        }

        public virtual void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
            currentState?.FromPreviousStateTransitionEnd(currentTransition);
        }

        public virtual void ToNextStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
            currentState?.ToNextStateTransitionBegin(currentTransition);
        }

        public virtual void ToNextStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
            currentState?.ToNextStateTransitionEnd(currentTransition);
        }
    }

}
