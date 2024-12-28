
using System;
using System.Net;
using System.Net.Mime;
using Assets.Scripts.Utilities.Timeline;
using Locomotion;
using UnityEngine;

public interface IModule
{
    public void Update(Context context);
}
public class Input
{
    public Vector3 HorizontalDirection;
    public bool Ascending;
}
public class Ground
{
    public Vector3 Normal;
    public bool Collded;
}
public class Context
{
    public Vector3 Velocity;
    public ushort State;
    public Ground Ground;
    public Input Input;
}
public class JumpLocomotion : IModule
{
    public enum StateEnum
    {
        None,
        Preparting,
        Ascending,
        Descending
    }
    class PrepareCompleted : PointEvent
    {
        JumpLocomotion _jump;

        public PrepareCompleted(float triggerProportion, JumpLocomotion locomotion) : base(triggerProportion)
        {
            _jump = locomotion;
        }


        public override void Execute(TimelineContext context)
        {
            _jump._state = StateEnum.Ascending;
        }
    }
    class AscendingEvent : RangeEvent
    {

        JumpLocomotion _jump;
        float _time;

        public AscendingEvent(float triggeredProportion, float durationProportion, JumpLocomotion locomotion) : base(durationProportion, triggeredProportion)
        {
            _jump = locomotion;
        }

        public override void Execute(TimelineContext context)
        {
            _jump._currentVerticalVelocity = _jump.CalculateVelocityInAscendingStage(_time);
            _time += Time.fixedDeltaTime;
        }
        public override void Reset()
        {
            _time = 0;
        }
    }
    class AscendingStageEndEvent : PointEvent
    {
        JumpLocomotion _jump;

        public AscendingStageEndEvent(float triggeredProportion, JumpLocomotion locomotion) : base(triggeredProportion)
        {
            _jump = locomotion;
        }

        public override void Execute(TimelineContext context)
        {
            _jump._state = StateEnum.Descending;
        }
    }

    IJumpDefines _defines;
    float _startVelocity;
    float _ascendingDuration;
    StateEnum _state;
    float _currentVerticalVelocity;
    public StateEnum State
    {
        get => _state;
    }
    public JumpLocomotion(IJumpDefines defines)
    {
        _defines = defines;
        //v^2=2gh
        _startVelocity = Mathf.Sqrt(-2 * Physics.gravity.y * _defines.Height);
        _ascendingDuration = _startVelocity / -Physics.gravity.y + _defines.PreparationDuration;
    }

    float CalculateVelocityInAscendingStage(float time)
    {
        return _startVelocity + Physics.gravity.y * time;
    }
    void StartJump()
    {
        if (_state > StateEnum.None)
        {
            _state = StateEnum.Preparting;
            _currentVerticalVelocity = 0;

        }
    }

    public void Update(Context context)
    {
    }
}
public class LocomotionBase
{

}