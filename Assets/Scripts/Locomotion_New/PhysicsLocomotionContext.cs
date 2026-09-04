using UnityEngine;
using MNAC.TPhysics;
using MNAC.TPhysics.Environment;
using MNAC.Locomotion.Interfaces;

namespace MNAC.Locomotion.Adapters
{
    /// <summary>
    /// 物理系统上下文适配器，将物理系统适配到运动上下文接口
    /// </summary>
    public class PhysicsLocomotionContext : ILocomotionContext
    {
        private World _world;
        private TPhysics.Context _physicsContext;

        public PhysicsLocomotionContext(World world, Rigidbody rigidbody, IGroundDetector groundDetector)
        {
            _world = world;
            _physicsContext = new TPhysics.Context(rigidbody);
        }

        public Vector3 Position
        {
            get => _physicsContext.CurrentPosition;
            set => _physicsContext.CurrentPosition = value;
        }

        public Quaternion Rotation
        {
            get => _physicsContext.CurrentRotation;
            set => _physicsContext.CurrentRotation = value;
        }

        public Vector3 Velocity
        {
            get => _physicsContext.CurrentVelocity;
            set => _physicsContext.CurrentVelocity = value;
        }

        public float Speed => _physicsContext.CurrentSpeed;

        public Vector3 Forward => _physicsContext.CurrentRotation * Vector3.forward;
        public Vector3 Up => _world.Up;
        public Vector3 Right => Vector3.Cross(Forward, Up);

        public float DeltaTime => Time.deltaTime;
        public float FixedDeltaTime => Time.fixedDeltaTime;
        public int UpdateCount => _physicsContext.UpdatedCount;

        public void Synchronize()
        {
            _physicsContext.SynchronizeFromRigidbody();
        }

        public void Apply()
        {
            _physicsContext.SynchronizeToRigidbody();
        }
    }
}