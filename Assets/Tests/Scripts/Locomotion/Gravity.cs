using System.Linq.Expressions;
using UnityEngine;
namespace Tests.Locomotion
{
    public class Gravity : IModule
    {

        public Gravity()
        {
        }

        public void OnUpdate(Context context)
        {
            if (context.State == State.Descending)
            {
                var currentVelocity = context.Velocity;

                currentVelocity.y += Physics.gravity.y * context.DeltaTime;
                context.Velocity = currentVelocity;
            }
        }
    }
}