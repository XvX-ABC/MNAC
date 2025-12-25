using NUnit.Framework;
using System.Collections.Generic;
using Tests.Interaction;
using Tests.TPhysics.Locomotion;
using Tests.Utilities;
using Tests.Utilities.Timeline;
using Tests.Utilities.Timeline.Events.Point;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Tests.AI
{
    internal interface IValidTarget
    {
        public bool IsValid { get; }
    }
    internal interface IDestination : IPositionTarget, IValidTarget
    {

    }
    internal class PTarget : IDestination
    {
        internal Vector3 pos;
        internal bool isValid;
        public Vector3 Position => pos;

        public bool IsValid => IsValid;
    }
    internal class NavigationModule : EvaluationModuleBase
    {
        public interface INavigationHandler
        {
            public void SetDestination(Vector3 pos);
            public void PositionChange(Vector3 currentPos, Vector3 nextPos);
            public void Stop();
        }


        AIComponentContext _context;
        float _positionSampleInterval;
        ITimeline _sampleTimeline;
        NavMeshAgent _navAgent;
        Vector3 _targetPosCache = Vector3.positiveInfinity;
        PTarget _steeringTarget;
        IPositionTarget _destination;
        internal List<INavigationHandler> handlers;
        Context _lcontext;
        bool _isMoving;
        public NavigationModule(AIComponentContext context, float positionSampleInterval)
        {
            _context = context;
            _positionSampleInterval = positionSampleInterval;
            handlers = new();
            navAgent = context.navAgent;
            _steeringTarget = new();
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
                    _sampleTimeline.AddPointEvent(1, UpdateNavAgent);
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
                    _sampleTimeline?.Start();
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

        internal IPositionTarget steeringTarget
        {
            get
            {
                return navAgent.hasPath ? _steeringTarget : null;
            }
        }
        internal IPositionTarget destination
        {
            get => _destination;
            set
            {
                if (value == null)
                    _targetPosCache = Vector3.positiveInfinity;
                _destination = value;
                _isMoving = true;
            }
        }
        internal bool IsMoving
        {
            get => _isMoving;
        }
        void UpdateTimelineLength(TimelineContext ctx)
        {
            if (_positionSampleInterval != ctx.Duration)
                _sampleTimeline.UpdateLength(_positionSampleInterval, false);
        }
        public override Context Update(Context context)
        {
            _lcontext = context;
            navAgent.speed = context.CurrentSpeed;
            var currentPos = context.CurrentPosition;
            var worldDeltaPosition = _navAgent.nextPosition - currentPos;

            var nextPosition = _navAgent.nextPosition;
            if (worldDeltaPosition.magnitude > _navAgent.radius)
                //nextPosition = _navAgent.nextPosition - 0.9f * worldDeltaPosition;
                nextPosition = currentPos + 0.9f * worldDeltaPosition;
            if (navAgent.hasPath)
                HandlePositionChange(currentPos, nextPosition);

            _steeringTarget.pos = navAgent.steeringTarget;
            _sampleTimeline?.OnUpdate(Time.fixedDeltaTime);
            _navAgent.nextPosition = nextPosition;

            return base.Update(context);
        }
        void HandlePositionChange(Vector3 currentPos, Vector3 nextPos)
        {
            foreach (var h in handlers)
                h.PositionChange(currentPos, nextPos);
        }
        void WhenDestinationChange(Vector3 pos)
        {
            foreach (var h in handlers)
                h.SetDestination(pos);
        }
        void WhenStop()
        {
            foreach (var h in handlers)
                h.Stop();
        }
        void UpdateNavAgent(TimelineContext _)
        {
            var target = _destination;
            var agent = _navAgent;
            if (target == null || agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
            {
                Stop();
            }
            else if (target != null)
            {
                Debug.Log("navigation target is not null");
                if (_targetPosCache == Vector3.positiveInfinity || Vector3.Distance(_targetPosCache, target.Position) > _navAgent.stoppingDistance)
                    SetDestination();
                else
                {
                    Debug.Log("target pos is too closest ");
                }
                _targetPosCache = _destination.Position;
            }
        }
        void SetDestination()
        {
            Debug.Log("set destination");
            var pos = _destination.Position;
            if (!_navAgent.SetDestination(pos))
            {
                Debug.Log($"The path to {pos} set failed");
            }
            else
            {
                WhenDestinationChange(pos);
                _steeringTarget.isValid = true;
                _isMoving = true;
            }
        }
        void Stop()
        {
            Debug.Log("stop");
            var currentPos = _lcontext.CurrentPosition;
            _navAgent.ResetPath();
            WhenStop();
            _steeringTarget.isValid = false;
            _isMoving = false;
        }

    }
}
