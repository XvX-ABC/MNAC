using Locomotion;
using System;
using Tests.Locomotion;
using UnityEngine;
using Utilities.Timeline;

namespace Tests.TPhysics.Locomotion
{
    public class JumpLocomotion : LocomotionModuleBase
    {
        IJumpDefinitions _definitions;
        ITimeline _timeline;
        public JumpLocomotion(IJumpDefinitions definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _timeline = new Timeline(0);
        }

        public override Context Start(Context context)
        {
            var jv = Mathf.Sqrt(-2 * world.Gravity.y * _definitions.Height);
            var ov = context.CurrentVelocity;
            context.CurrentVelocity += Quaternion.FromToRotation(World.DefaultUp, world.Up) * new Vector3(0, jv, 0);
            var time = jv / -world.Gravity.y;
            _timeline.UpdateLength(time);
            _timeline.Start();
            var nv = context.CurrentVelocity;
            return context;
        }
        public override Context Update(Context context)
        {
            _timeline.OnUpdate(Time.deltaTime);
            return context;
        }



        public override Context End(Context context)
        {
            _timeline.End();
            return context;
        }
    }
}