using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
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
    public partial class PlayableStateMachine<T> : StateMachineBase<IPlayableState<T>, T>, IPlayableState<T>
    {
        static byte s_defaultInterruptionSource = 1;
        class TransitionState : PlayableStateBase<T>
        {
            internal IPlayableState<T> srcState;
            internal IPlayableState<T> desState;
            internal IPlayableTransition<T> currentTransition;
            List<ITransition<T>> _list;
            public TransitionState() : base("", 0, true)
            {
                var transition = new PlayableTransition()
                {
                    sourceState = this,
                    destinationState = null,
                    triggerEvent = null,
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
                var t = this.transitions[0] as PlayableTransition;
                t.destinationState = desState;
                t.triggerEvent = () => timeline.NormalizedTime >= 1;


                var interruptionSource = srcTransition.InterruptionSource;
                if (interruptionSource > 0)
                    SetInterruptionSourceByNextState();
            }
            void SetInterruptionSourceByNextState()
            {
                _list.Clear();
                foreach (var ts in desState.Transitions)
                    if (ts.TriggerEvent != null)
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
        protected bool _exitWhenEnd;
        public ITimeline Timeline => currentState?.Timeline;

        public bool ExitWhenEnd { get => _exitWhenEnd; set => _exitWhenEnd = value; }

        IPlayableTransition<T>[] IPlayableState<T>.Transitions => currentState?.Transitions;

        public PlayableStateMachine(string name, bool enabled = true) : base(name, enabled)
        {
            _transitionState = new();
        }

        protected PlayableTransition NewTransition(IPlayableState<T> sourceState, IPlayableState<T> destinationState, Func<bool> triggerEvent, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent, float duration, byte interruptionSource)
        {
            var transition = new PlayableTransition(sourceState, destinationState, triggerEvent, durationEvent, duration, interruptionSource);
            return transition;
        }
        public override void AddTransitionFor(IPlayableState<T> state, IPlayableState<T> destinationState, Func<bool> triggerEvent)
        {
            var transition = NewTransition(state, destinationState, triggerEvent, null, 0, s_defaultInterruptionSource);
            AddTransitionFor(transition);
        }
        public void AddTransitionFor(IPlayableState<T> state, IPlayableState<T> destinationState, Func<bool> triggerEvent, byte interruptionSourceNum)
        {
            var transition = NewTransition(state, destinationState, triggerEvent, null, 0, interruptionSourceNum);
            AddTransitionFor(transition);
        }
        public IPlayableTransition<T> AddTransitionFor(IPlayableState<T> state, IPlayableState<T> destinationState, float duration, Func<bool> triggerEvent, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent)
        {
            //var transition = NewTransition(state, destinationState, triggerEvent, durationEvent, duration, 1);
            //AddTransitionFor(transition);
            //return transition;
            return AddTransitionFor(state, destinationState, duration, triggerEvent, durationEvent, s_defaultInterruptionSource);
        }

        public IPlayableTransition<T> AddTransitionFor(IPlayableState<T> state, IPlayableState<T> destinationState, float duration, Func<bool> triggerEvent, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent, byte interruptionSourceNum)
        {
            var transition = NewTransition(state, destinationState, triggerEvent, durationEvent, duration, interruptionSourceNum);
            AddTransitionFor(transition);
            return transition;
        }
        public IPlayableTransition<T> AddTransitionFor(IPlayableState<T> state, IPlayableState<T> destinationState, float duration, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent)
        {
            return AddTransitionFor(state, destinationState, duration, null, durationEvent);
        }
        public override void AddTransition(ITransition<T> transition)
        {
            base.AddTransitionFor(transition as IPlayableTransition<T>);
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

        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void OnExit()
        {
            base.OnExit();
        }

        public virtual void TransitionRunningWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void TransitionRunningWhichToNextState(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public void TransitionBeginWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition)
        {
            throw new NotImplementedException();
        }

        public void TransitionEndWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition)
        {
            throw new NotImplementedException();
        }

        public void TransitionBeginWhichToNextState(IReadonlyPlayableTransition<T> currentTransition)
        {
            throw new NotImplementedException();
        }

        public void TransitionEndWhichToNextState(IReadonlyPlayableTransition<T> currentTransition)
        {
            throw new NotImplementedException();
        }
    }

}
