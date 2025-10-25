using UnityEngine;

namespace Tests.TPhysics.Locomotion
{
    public class HorizontalLocomotion : LocomotionModuleBase
    {
        Vector3 _horizontalVector;
        float _maxSpeed;
        float _acceleratedSpeed;
        public Vector3 HorizontalVector
        {
            get => _horizontalVector;
            set => _horizontalVector = value;
        }
        public float MaxSpeed
        {
            get => _maxSpeed;
            set => _maxSpeed = Mathf.Max(0, value);
        }
        public float AcceleratedSpeed
        {
            get => _acceleratedSpeed;
            set => _acceleratedSpeed = Mathf.Max(0, value);
        }

        public HorizontalLocomotion(float maxSpeed, float acceleratedSpeed, Vector3 horizontalVector)
        {
            MaxSpeed = maxSpeed;
            AcceleratedSpeed = acceleratedSpeed;
            _horizontalVector = horizontalVector;
        }
        public HorizontalLocomotion(float maxSpeed, float acceleratedSpeed) : this(maxSpeed, acceleratedSpeed, Vector3.zero)
        {

        }

        Vector3 CalculateDirection(Context context)
        {
            var direction = _horizontalVector;
            var grounds = context.groundDetector.Grounds;

            var groundNormal = context.groundDetector.GroundsNormal;
            if (groundNormal == Vector3.zero)
                return direction;

            return world.rotation * Quaternion.FromToRotation(world.Up, groundNormal) * direction;

        }
        public override Context OnStart(Context context)
        {
            //return OnUpdate(context);
            return context;
        }
        public override Context OnUpdate(Context context)
        {
            //return OnUpdate_0(context);
            return OnUpdate_1(context);
        }
        public Context OnUpdate_0(Context context)
        {
            var up = world.Up;
            if (context.groundDetector.Grounds.Count > 0)
            {
                up = context.groundDetector.GroundsNormal;
            }
            var direction = CalculateDirection(context);


            if (direction == Vector3.zero)
                return context;
            var velocity = context.CurrentVelocity;
            var speed = velocity.magnitude;

            //TODO: 完善实现方式
            var dv = _maxSpeed - speed;
            var fs = Mathf.Max(0, dv);
            if (_acceleratedSpeed > 0)
                fs = Mathf.Min(fs, _acceleratedSpeed) * Time.deltaTime;
            context.CurrentVelocity += direction * fs;
            Debug.Log($"dv: {dv}, as: {_acceleratedSpeed}, ms: {_maxSpeed}, fs: {fs},  iv: {(direction * fs).magnitude / Time.deltaTime}, velocity: {context.CurrentVelocity.magnitude}");
            return context;
        }
        public Context OnUpdate_1(Context context)
        {
            var up = world.Up;
            if (context.groundDetector.Grounds.Count > 0)
            {
                up = context.groundDetector.GroundsNormal;
            }
            var direction = CalculateDirection(context);
            context.CurrentVelocity = Accelerate(direction, context.CurrentVelocity, _acceleratedSpeed, _maxSpeed);
            return context;
        }
        Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float maxSpeed)
        {
            float projVel = Vector3.Dot(prevVelocity, accelDir);
            float accelVel = accelerate == 0 ? _maxSpeed : accelerate * Time.deltaTime;
            if (projVel + accelVel > maxSpeed)
            {
                accelVel = maxSpeed - projVel;
            }
            var r = prevVelocity + accelDir * accelVel;
            if (r.magnitude >= maxSpeed)
                r = r.normalized * maxSpeed;
            return r;
        }
        public override Context OnEnd(Context context)
        {
            //return OnUpdate(context);
            return context;
        }
    }
}