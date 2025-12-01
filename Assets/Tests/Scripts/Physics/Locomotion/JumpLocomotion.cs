using System;
using Tests.Utilities.Timeline;
using UnityEngine;

namespace Tests.TPhysics.Locomotion
{
    public class JumpLocomotion : LocomotionModuleBase
    {
        ITimeline _timeline;
        float _height;
        public ITimeline Timeline { get => _timeline; }
        public float Height { get => _height; set => _height = Mathf.Max(0, value); }

        public JumpLocomotion(float height)
        {
            Height = height;
            _timeline = new Timeline_V1(0);
        }
        public override Context OnStart(Context context)
        {
            var jv = Mathf.Sqrt(-2 * world.Gravity.y * _height);
            var ov = context.CurrentVelocity;
            context.CurrentVelocity += Quaternion.FromToRotation(World.DefaultUp, world.Up) * new Vector3(0, jv, 0);
            var time = jv / -world.Gravity.y;
            _timeline.UpdateLength(time);
            _timeline.Restart();
            var nv = context.CurrentVelocity;
            return context;
        }
        public override Context OnUpdate(Context context)
        {
            _timeline.OnUpdate(Time.deltaTime);
            return context;
        }



        public override Context OnEnd(Context context)
        {
            _timeline.Pause();
            return context;
        }
    }
}