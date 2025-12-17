using NUnit.Framework;
using System.Collections.Generic;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.Timeline;
using Tests.Utilities.Timeline.Events.Point;
using UnityEngine;
using UnityEngine.AI;

namespace Tests.AI
{
    internal class NavigationModule : EvaluationModuleBase
    {
        public interface IPositionChangeHandler
        {
            public void Handle(Vector3 currentPos, Vector3 nextPos);
        }
        AIComponentContext _context;
        float _positionSampleInterval;
        ITimeline _sampleTimeline;
        NavMeshAgent _navAgent;
        Vector3 _targetPosCache = Vector3.positiveInfinity;
        internal List<IPositionChangeHandler> handlers;
        public NavigationModule(AIComponentContext context, float positionSampleInterval)
        {
            _context = context;
            _positionSampleInterval = positionSampleInterval;
            handlers = new();
            navAgent = context.navAgent;
        }
        internal NavMeshAgent navAgent
        {
            get => _navAgent;
            set
            {
                if (value != null)
                {
                    value.updatePosition = false;
                    value.updateRotation = false;

                    _sampleTimeline = new Timeline_V2(_positionSampleInterval, true);
                    _sampleTimeline.AddPointEvent(0, UpdateTimelineLength);
                    _sampleTimeline.AddPointEvent(1, SetDestination);
                    _sampleTimeline.SetNormalizedTime(1);
                    _sampleTimeline.Start();
                }
                else
                    _sampleTimeline.End();
                _navAgent = value;
            }
        }
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                if (value)
                {
                    _sampleTimeline?.Restart();
                }
                else
                {
                    _sampleTimeline?.End();
                }
            }
        }

        public float PositionSampleInterval
        {
            get => _positionSampleInterval;
            set
            {
                _positionSampleInterval = value;
            }
        }
        void UpdateTimelineLength(TimelineContext ctx)
        {
            if (_positionSampleInterval != ctx.Duration)
                _sampleTimeline.UpdateLength(_positionSampleInterval, false);
        }
        public override Context Update(Context context)
        {
            Debug.Log($"up : {navAgent.updatePosition}, ur: {navAgent.updateRotation}");
            navAgent.speed = context.CurrentSpeed;
            var currentPos = context.CurrentPosition;
            var worldDeltaPosition = _navAgent.nextPosition - currentPos;

            var nextPosition = _navAgent.nextPosition;
            if (worldDeltaPosition.magnitude > _navAgent.radius)
                //nextPosition = _navAgent.nextPosition - 0.9f * worldDeltaPosition;
                nextPosition = currentPos + 0.9f * worldDeltaPosition;
            HandlePositionChange(currentPos, nextPosition);
            _sampleTimeline?.OnUpdate(Time.fixedDeltaTime);
            _navAgent.nextPosition = nextPosition;
            return base.Update(context);
        }
        void HandlePositionChange(Vector3 currentPos, Vector3 nextPos)
        {
            foreach (var h in handlers)
                h.Handle(currentPos, nextPos);
        }
        void SetDestination(TimelineContext _)
        {
            var target = _context.target;
            if (target == null || Vector3.Distance(_targetPosCache, target.Position) <= _navAgent.stoppingDistance)
                return;
            var pos = target.Position;
            if (!_navAgent.SetDestination(pos))
                Debug.LogWarning($"The path to '{pos}' set failed.");
            _targetPosCache = pos;
        }
    }
}
