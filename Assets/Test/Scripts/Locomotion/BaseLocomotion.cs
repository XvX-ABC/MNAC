using Locomotion;
using UnityEngine;

public class BaseLocomotion : IModule
{
    IBaseDefines _defines;
    public void Update(Context context)
    {
        var direction = context.Input.HorizontalDirection;
        var velocity = context.Velocity;
        var speed = _defines.Speed;
        var ground = context.Ground;
        var groundNormal = ground.Normal;
        var collded = ground.Collded;
        if (collded)
            direction = Vector3.ProjectOnPlane(direction, groundNormal);
        velocity = Vector3.MoveTowards(velocity, direction * speed, _defines.AccelerationSpeed);
        context.Velocity = velocity;
    }
}
