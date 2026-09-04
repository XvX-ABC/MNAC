using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 朝一个水平方向加速移动；有支撑面时把方向抬到斜坡平面。
    public class HorizontalLocomotion : LocomotionModuleBase
    {
        Vector3 _direction;
        float _maxSpeed;
        float _acceleratedSpeed;

        public Vector3 DirectionVector
        {
            get => _direction;
            set => _direction = value.normalized;
        }

        public float MaxSpeed
        {
            get => _maxSpeed;
            set => _maxSpeed = Mathf.Max(0f, value);
        }

        public float AcceleratedSpeed
        {
            get => _acceleratedSpeed;
            set => _acceleratedSpeed = Mathf.Max(0f, value);
        }

        public HorizontalLocomotion(float maxSpeed, float acceleratedSpeed) : this(maxSpeed, acceleratedSpeed, Vector3.zero)
        {
        }

        public HorizontalLocomotion(float maxSpeed, float acceleratedSpeed, Vector3 horizontalVector)
        {
            MaxSpeed = maxSpeed;
            AcceleratedSpeed = acceleratedSpeed;
            _direction = horizontalVector.normalized;
        }

        public override Context OnUpdate(Context context)
        {
            var direction = CalculateDirection(context);
            context.CurrentVelocity = Accelerate(direction, context.CurrentVelocity, _acceleratedSpeed, _maxSpeed);
            return context;
        }

        Vector3 CalculateDirection(Context context)
        {
            if (!context.GroundDetector.IsGrounded)
                return _direction;
            var groundNormal = context.GroundDetector.GroundsNormal;
            return context.world.rotation * Quaternion.FromToRotation(context.world.Up, groundNormal) * _direction;
        }

        Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float maxSpeed)
        {
            float projVel = Vector3.Dot(prevVelocity, accelDir);
            float accelVel = accelerate == 0 ? maxSpeed : accelerate * Time.deltaTime;
            if (projVel + accelVel > maxSpeed)
                accelVel = maxSpeed - projVel;
            var result = prevVelocity + accelDir * accelVel;
            if (result.magnitude >= maxSpeed)
                result = result.normalized * maxSpeed;
            return result;
        }
    }
}
