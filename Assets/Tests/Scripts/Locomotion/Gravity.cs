using System.Linq.Expressions;
using UnityEngine;
namespace Tests.Locomotion
{
    public class Gravity : IModule
    {
        float _time;
        float _v;
        public Gravity()
        {
        }

        public void OnFixedUpdate(Context context)
        {
            //if (context.State == State.Descending)
            //{
            //    Debug.Log("Gravit working");
            //    var currentVelocity = context.Velocity;

            //    currentVelocity.y += Physics.gravity.y * context.DeltaTime;
            //    context.Velocity = currentVelocity;
            //}

            if (context.State == State.Descending)
            {
                var velocity = context.Velocity;
                velocity.y = Physics.gravity.y * _time;


                context.Velocity = velocity;


                _time += context.DeltaTime;
            }
            else if (_time > 0)
                _time = 0;
        }
    }
}