using Locomotion;
using System;
using System.Collections.Generic;
using UnityEngine;
using Ground = Tests.TPhysics.Environment.Ground;

namespace Tests.TPhysics.Locomotion
{
    public class HorizontalLocomotion : LocomotionModuleBase
    {
        [Obsolete]
        IBaseDefinitions _definitions;
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
            set => _maxSpeed = value < 0 ? 0 : value;
        }
        public float AcceleratedSpeed
        {
            get => _acceleratedSpeed;
            set => _acceleratedSpeed = value < 0 ? 0 : value;
        }

        [Obsolete]
        public HorizontalLocomotion(IBaseDefinitions definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }

        public HorizontalLocomotion(float maxSpeed, float acceleratedSpeed, Vector3 horizontalVector)
        {
            _maxSpeed = maxSpeed;
            _acceleratedSpeed = acceleratedSpeed;
            _horizontalVector = horizontalVector;
        }
        public HorizontalLocomotion(float maxSpeed, float acceleratedSpeed) : this(maxSpeed, acceleratedSpeed, Vector3.zero)
        {

        }

        Vector3 CalculateDirection(Context context)
        {
            var direction = _horizontalVector;
            var grounds = context.GroundDetector.Grounds;

            var groundNormal = CalculateNormalInGrounds(grounds);
            if (groundNormal == Vector3.zero)
                return direction;

            return world.rotation * Quaternion.FromToRotation(world.Up, groundNormal) * direction;


            Vector3 CalculateNormalInGrounds(IReadOnlyList<Ground> grounds)
            {
                if (grounds.Count == 0)
                    return Vector3.zero;

                var result = Vector3.zero;
                foreach (var g in grounds)
                {
                    result += g.Normal;
                }
                return result / grounds.Count;
            }
        }
        public override Context Start(Context context)
        {
            return Update(context);
        }

        public override Context Update(Context context)
        {
            var worldUp = world.Up;
            var direction = CalculateDirection(context);

            if (direction == Vector3.zero)
                return context;

            var currentVelocity = Vector3.ProjectOnPlane(context.CurrentVelocity, worldUp);
            var currentSpeed = currentVelocity.magnitude;

            var maxSpeed = _maxSpeed;
            currentSpeed = maxSpeed - Mathf.MoveTowards(currentSpeed, maxSpeed, _acceleratedSpeed);
            //Debug.Log($"max speed: {maxSpeed}, current speed: {currentSpeed}, accelerated speed: {_acceleratedSpeed}");
            context.CurrentVelocity += direction * currentSpeed * Time.deltaTime;
            return context;
        }
        public Context OnUpdate_0(Context context)
        {
            var worldUp = world.Up;
            var direction = CalculateDirection(context);
            if (direction == Vector3.zero)
                return context;

            var currentVelocity = context.CurrentVelocity;
            var currentSpeed = context.CurrentSpeed;


            var speed = _maxSpeed;
            if (currentSpeed <= _maxSpeed)
            {
                speed = Mathf.MoveTowards(currentSpeed, speed, _acceleratedSpeed);
            }
            var velocity = direction * speed - currentVelocity;
            context.CurrentVelocity += velocity;

            return context;
        }

        public override Context End(Context context)
        {
            return Update(context);
        }
    }
}