#define LOCOMOTION_JUMP_DIRECTION_KEEP
using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Locomotion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
namespace Tests.Locomotion
{
    class JumpLocomotion_New : IModule
    {
        public enum State
        {
            OnGround,
            InPreparation,
            Ascending,
            Descending
        }
        Vector3 _startVelocity;
        float _ascendingDurationTime;
        ITimeline _ascendingTimeline;
        State _state;
        IHybridInput.Mode _oldMode;
        Vector3[] _probes;
        List<RaycastHit> _probeResults;

        public State CurrentState => _state;

        public JumpLocomotion_New(IJumpDefinitions jumpDefinitions)
        {
            if (jumpDefinitions == null)
                throw new ArgumentNullException(nameof(jumpDefinitions));

            _startVelocity.y = Mathf.Sqrt(-2 * Physics.gravity.y * jumpDefinitions.Height);
            _ascendingDurationTime = _startVelocity.y / -Physics.gravity.y;
            _state = State.OnGround;

            _ascendingTimeline = new Timeline(_ascendingDurationTime);
            _ascendingTimeline.AddPointEvent(0, _ => _state = State.Ascending);
            _ascendingTimeline.AddPointEvent(1, _ => _state = State.Descending);
            _probes = new Vector3[]
            {
                Vector3.forward,
                Quaternion.Euler(0,30,0)*Vector3.forward,
                Quaternion.Euler(0,-30,0)*Vector3.forward,
                Quaternion.Euler(0,60,0)*Vector3.forward,
                Quaternion.Euler(0,-60,0)*Vector3.forward,
            };
        }
        public void EndJump(Context context)
        {
            if (_state == State.OnGround)
                return;

            EndJumpImpl(context);
        }

        void EndJumpImpl(Context context)
        {
            Debug.Log("end jump");
            if (_ascendingTimeline.IsRunning)
                _ascendingTimeline.Stop();
            _state = State.OnGround;

            //var input = context.Input;
            //input.CurrentMode = _oldMode;
            _probeResults.TrimExcess();
        }
        void StartJumpImpl(Context context)
        {
            context.Velocity += Quaternion.FromToRotation(World.DefaultUp, context.World.Up) * _startVelocity;
            _state = State.InPreparation;
            //_oldMode = input.CurrentMode;
            _probeResults = new();
        }
        void ProbesHandle(Context context)
        {
            var pos = context.Position;
            var rotation = Quaternion.LookRotation(context.Input.HorizontalDirection);
            _probeResults.Clear();
            foreach (var p in _probes)
            {
                var probe = rotation * p;

                //Debug.DrawLine(pos, pos + probe * 2.2f, Color.magenta);
                if (Physics.Raycast(pos, probe, out var hitInfo, 2.2f))
                {
                    _probeResults.Add(hitInfo);
                }
            }

            var v = Vector3.ProjectOnPlane(context.Velocity, context.World.Up);
            foreach (var hitInfo in _probeResults)
            {
                var normal = hitInfo.normal;
                //Debug.DrawLine(hitInfo.point, hitInfo.point + normal * 2, Color.blue);
                v = Vector3.ProjectOnPlane(v, normal);
            }
            v.y = context.Velocity.y;
            context.Velocity = v;
        }
        public void OnUpdate(Context context)
        {

            var input = context.Input;
            var ground = context.Ground;
            if (_state > State.InPreparation)
            {
                if (ground != null)
                    EndJumpImpl(context);
                else
                    ProbesHandle(context);
            }


            if (_state == State.InPreparation && ground == null)
                _ascendingTimeline.Start();

            if (_state == State.OnGround && ground != null && input.IsAscending)
            {
                StartJumpImpl(context);
                Debug.Log("Start Jump");
            }


            if (_ascendingTimeline.IsRunning)
                _ascendingTimeline.OnUpdate(context.DeltaTime);

        }
    }

}