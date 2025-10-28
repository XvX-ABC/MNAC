using System;
using System.Diagnostics.CodeAnalysis;
using Tests.States;
using Tests.TPhysics;
using Tests.TPhysics.Locomotion;
using UnityEngine;

namespace Tests.Characters.Humanoid.Locomotion
{
    internal class LocomotionStatemachine : WithCallbackPlayableStatemachine<object>, IEvaluationModule, IState<object>
    {
        public LocomotionStatemachine(string name, [NotNull] LocomotionStateContext context, bool enabled = true) : base($"{name}_locomotion_statemachine", enabled)
        {
            Context = context;
        }

        public World World { get; set; }
        TPhysics.Locomotion.Context IEvaluationModule.Update(TPhysics.Locomotion.Context context)
        {
            OnUpdate();
            //Debug.Log(this);
            return context;
        }

        //void IState<object>.AddTransition(ITransition<object> transition)
        //{
        //    if (transition == null)
        //        throw new ArgumentNullException(nameof(transition), "Transition cannot be null.");
        //    var index = FindTransitionIndex(transition.DestinationState);
        //    if (index > -1)
        //        return;
        //    var transitions = this.transitions;
        //    if (transitions == null)
        //    {
        //        transitions = new ITransition<object>[] { transition };
        //    }
        //    else
        //    {
        //        Array.Resize(ref transitions, transitions.Length + 1);
        //        transitions[^1] = transition;
        //    }
        //    this.transitions = transitions;
        //}
        //void IState<object>.RemoveTransition(IState<object> destinationState)
        //{
        //    if (destinationState == null)
        //        throw new ArgumentNullException(nameof(destinationState), "Destination state cannot be null.");
        //    var transitions = this.transitions;
        //    if (transitions == null || transitions.Length == 0)
        //        return;
        //    int index = FindTransitionIndex(destinationState);
        //    if (index == -1)
        //        return;
        //    if (transitions.Length == 1)
        //        transitions = null;
        //    else
        //    {
        //        if (transitions.Length != index)
        //            Array.Copy(transitions, index + 1, transitions, index, transitions.Length - index - 1);
        //        Array.Resize(ref transitions, transitions.Length - 1);
        //    }
        //    this.transitions = transitions;
        //}
    }
}
