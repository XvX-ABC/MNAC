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
        VerticalPosture _posture;
        public Context(TPhysics.Context physicsContext, [NotNull] IGroundDetector groundDetector)
        {
            _physicsContext = physicsContext;
            _groundDetector = groundDetector;
            _posture = VerticalPosture.Holding;
        }

        internal Rigidbody rbody { get => _physicsContext.rbody; }
        public TPhysics.Context PhysicsContext { get => _physicsContext; set => _physicsContext = value; }

        public OriginalInfo Original { get => _physicsContext.Original; }
        public Vector3 CurrentVelocity { get => _physicsContext.CurrentVelocity; set => _physicsContext.CurrentVelocity = value; }
        public Vector3 CurrentPosition { get => _physicsContext.CurrentPosition; set => _physicsContext.CurrentPosition = value; }
        public Quaternion CurrentRotation { get => _physicsContext.CurrentRotation; set => _physicsContext.CurrentRotation = value; }
        public float CurrentSpeed { get => _physicsContext.CurrentSpeed; }
        public float CurrentSquareSpeed { get => _physicsContext.CurrentSquareSpeed; }
        public ushort UpdatedCount => _physicsContext.UpdatedCount;

        public IGroundDetector GroundDetector { get => _groundDetector; }

        internal VerticalPosture VerticalPosture { get => _posture; set => _posture = value; }

        public void Synchronise()
        {
            _physicsContext.ResetUpdatedCount();
            _physicsContext.SynchronizeFromRigidbody();
        }
        public void Apply()
        {
            _physicsContext.SynchronizeToRigidbody();

        }
    }
}