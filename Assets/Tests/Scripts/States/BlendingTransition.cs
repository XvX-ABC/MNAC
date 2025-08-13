using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using System;
using Tests.Behaviours.Arm;
using Tests.States;
using UnityEngine;
using UnityEngine.EventSystems;
internal interface IBlendingTransition<T> : IPlayableTransition<T>
{
    public float FixedExitTime { get; }
}
internal class BlendingTransition<T> : PlayableTransition<T>, IBlendingTransition<T>
{
    protected internal const float FIXED_EXIT_TIME_INVALID_VALUE = -1;
    float _oldLength;
    protected float offset;
    protected float fixedExitTime;

    public float FixedExitTime => fixedExitTime;

    public BlendingTransition(
        IWithCallbackPlayableState<T> sourceState,
        IWithCallbackPlayableState<T> destinationState,
        Func<bool> triggerEvent,
        Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent,
        float duration,
        float offset,
        float fixedExitTime = FIXED_EXIT_TIME_INVALID_VALUE,
        byte interruptionSource = INTERRUPTION_SOURCE_DEFAULT_CODE
       ) : base(
            sourceState,
            destinationState,
            triggerEvent,
            durationEvent,
            duration,
            interruptionSource)
    {
        this.offset = offset < 0 ? 0 : offset;
        if (fixedExitTime != FIXED_EXIT_TIME_INVALID_VALUE)
        {
            fixedExitTime = Mathf.Clamp01(fixedExitTime);
            this.fixedExitTime = fixedExitTime;
            //this.triggerEvent += () => this.sourceState.Timeline.NormalizedTime >= this.fixedExitTime;
            this.AddTriggerEvent(() => this.sourceState.Timeline.NormalizedTime >= this.fixedExitTime);
        }
        //this.timeline.AddPointEvent(0, Begin);
        this.timeline.StartAction += Begin;


        //destinationState.Timeline.EndAction += ResetDestinationState;
        destinationState.ExitAction += ResetDestinationState;
    }
    protected virtual void Begin(TimelineContext ctx)
    {
        Debug.Log($"Blending transtion name begin+ " + this.GetType().Name);
        var timeline = destinationState.Timeline;
        _oldLength = timeline.Length;
        var newLength = _oldLength - (offset + ctx.Duration);
        newLength = newLength < 0 ? 0 : newLength;


        timeline.UpdateLength(newLength);
    }
    protected virtual void ResetDestinationState()
    {
        Debug.Log("Blending transition ResetDestinationState");
        var timeline = destinationState.Timeline;
        timeline.UpdateLength(_oldLength);
    }

}

