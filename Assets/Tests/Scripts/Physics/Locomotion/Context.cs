using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using static Tests.TPhysics.Context;
using IGroundDetector = Tests.TPhysics.Environment.IGroundDetector;

namespace Tests.TPhysics.Locomotion
{
    public struct Context
    {
        TPhysics.Context _physicsContext;
        IGroundDetector _groundDetector;
        internal VerticalPosture verticalPosture;
        internal World world;
        Plane _worldPlane;
        Plane _groundPlane;
        Vector3 _forward;
        public Context(TPhysics.Context physicsContext, [NotNull] IGroundDetector groundDetector)
        {
            world = World.Default;
            _worldPlane = default;
            _groundPlane = default;
            _physicsContext = physicsContext;
            _groundDetector = groundDetector;
            verticalPosture = VerticalPosture.Holding;
            _forward = Vector3.forward;
        }
        public Context(World world, TPhysics.Context physicsContext, [NotNull] IGroundDetector groundDetector)
        {
            this.world = world ?? throw new ArgumentNullException(nameof(world));
            _physicsContext = physicsContext;
            _groundDetector = groundDetector;
            verticalPosture = VerticalPosture.Holding;
            _groundPlane = default;
            _worldPlane = default;
            _forward = Vector3.forward;

        }

        public Rigidbody Rbody { get => _physicsContext.rbody; }
        public IGroundDetector GroundDetector { get => _groundDetector; }

        public VerticalPosture VerticalPosture { get => verticalPosture; set => verticalPosture = value; }
        internal Vector3 groundNormal { get => _groundDetector.GroundsNormal == Vector3.zero ? world.Up : _groundDetector.GroundsNormal; }
        public TPhysics.Context PhysicsContext { get => _physicsContext; set => _physicsContext = value; }

        public OriginalInfo Original { get => _physicsContext.Original; }
        public Vector3 CurrentVelocity { get => _physicsContext.CurrentVelocity; set => _physicsContext.CurrentVelocity = value; }
        public Vector3 CurrentPosition { get => _physicsContext.CurrentPosition; set => _physicsContext.CurrentPosition = value; }
        public Quaternion CurrentRotation { get => _physicsContext.CurrentRotation; set => _physicsContext.CurrentRotation = value; }
        public float CurrentSpeed { get => _physicsContext.CurrentSpeed; }
        public float CurrentSquareSpeed { get => _physicsContext.CurrentSquareSpeed; }
        public ushort UpdatedCount => _physicsContext.UpdatedCount;

        public Plane GroundPlane { get => _groundPlane; }
        public Plane WorldPlane { get => _worldPlane; }
        public World World { get => world; }
        public Vector3 Forward { get => _forward; }

        internal void UpdatePlanes()
        {
            _groundPlane = new Plane(groundNormal, CurrentPosition);
            _worldPlane = new Plane(world.Up, CurrentPosition);
            _forward = CurrentRotation * Vector3.forward;
        }
        public void Synchronise()
        {
            _physicsContext.ResetUpdatedCount();
            _physicsContext.SynchronizeFromRigidbody();
            UpdatePlanes();
        }
        public void Apply()
        {
            _physicsContext.SynchronizeToRigidbody();

        }
    }
}