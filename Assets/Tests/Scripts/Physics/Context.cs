using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.TPhysics
{
    public static class TPhysicsHelper
    {
        internal static readonly Vector3 InvalidVelocity = Vector3.negativeInfinity;
        internal static readonly Vector3 InvalidPosition = Vector3.negativeInfinity;
        internal static readonly Quaternion InvalidRotation = Quaternion.identity;
        internal static readonly float InvalidSpeed = -1;
    }
    public struct Context
    {
        internal Rigidbody rbody;
        OriginalInfo _original;
        Vector3 _currentVelocity;
        Vector3 _currentPosition;
        float _currentSpeed;
        float _currentSquareSpeed;
        Quaternion _currentRotation;
        ushort _updatedCount;
        public struct OriginalInfo
        {
            Rigidbody _rb;

            public OriginalInfo(Rigidbody rb)
            {
                _rb = rb;
            }
            public Vector3 Velocity { get => _rb.velocity; }
            public Vector3 Position { get => _rb.position; }
            public Quaternion Rotation { get => _rb.rotation; }
        }
        public Context([NotNull] Rigidbody rigidbody)
        {
            rbody = rigidbody ?? throw new ArgumentNullException(nameof(rigidbody));
            _original = new(rigidbody);
            _currentSpeed = TPhysicsHelper.InvalidSpeed;
            _currentSquareSpeed = TPhysicsHelper.InvalidSpeed;
            _currentVelocity = TPhysicsHelper.InvalidVelocity;
            _currentPosition = TPhysicsHelper.InvalidPosition;
            _currentRotation = TPhysicsHelper.InvalidRotation;
            _updatedCount = 0;

            SynchronizeFromRigidbody();
        }
        public OriginalInfo Original { get => _original; }
        public Vector3 CurrentVelocity
        {
            get => _currentVelocity;
            set
            {
                _currentVelocity = value;
                if (value != TPhysicsHelper.InvalidVelocity)
                {
                    _currentSpeed = value.magnitude;
                    _currentSquareSpeed = value.magnitude;
                }
                _updatedCount++;
            }
        }
        public Vector3 CurrentPosition
        {
            get => _currentPosition;
            set
            {
                _currentPosition = value;
                _updatedCount++;
            }
        }
        public Quaternion CurrentRotation
        {
            get => _currentRotation;
            set
            {
                _currentRotation = value;
                _updatedCount++;
            }
        }
        public float CurrentSpeed { get => _currentSpeed; }
        public float CurrentSquareSpeed { get => _currentSquareSpeed; }
        public ushort UpdatedCount { get => _updatedCount; set => _updatedCount = value; }

        public void SynchronizeToRigidbody()
        {
            rbody.velocity = _currentVelocity;
            rbody.MovePosition(_currentPosition);
            if (_currentRotation != Quaternion.identity)
                rbody.MoveRotation(_currentRotation);
        }
        public void SynchronizeFromRigidbody()
        {
            _currentVelocity = rbody.velocity;
            _currentPosition = rbody.position;
            _currentRotation = rbody.rotation;
        }
        public void ResetUpdatedCount() => _updatedCount = 0;
    }
}
