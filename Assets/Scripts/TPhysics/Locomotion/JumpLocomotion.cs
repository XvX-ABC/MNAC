using System;
using MNAC.Utilities.Timeline;
using UnityEngine;

namespace MNAC.TPhysics.Locomotion
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
            _timeline = new Timeline(0);
        }
        public override Context OnStart(Context context)
        {
            var world = context.world;
            var jumpVelocity = Mathf.Sqrt(-2 * world.Gravity.y * _height);
            context.CurrentVelocity += Quaternion.FromToRotation(World.DefaultUp, world.Up) * new Vector3(0, jumpVelocity, 0);
            _timeline.UpdateLength(jumpVelocity / -world.Gravity.y);
            _timeline.Restart();
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