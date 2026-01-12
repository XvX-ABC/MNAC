using System;
using UnityEngine;

namespace Tests
{
    public class LocomotionContext
    {
        Rigidbody _rb;
        public LocomotionContext(Rigidbody rbody)
        {
            _rb = rbody;
        }
        float _sqrSpeed;
        float _speed;
        Vector3 _velocity;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity
        {
            get => _velocity;
            set
            {
                _speed = value.magnitude;
                _sqrSpeed = value.sqrMagnitude;
                _velocity = value;
            }
        }

        public float SquareSpeed => _sqrSpeed;
        public float Speed => _speed;
        public void OnUpdate()
        {
            if (_rb != null)
            {

            Position = _rb.position;
            Rotation = _rb.rotation;
            Velocity = _rb.velocity;
            }else
                Reset();
        }
        public void Reset()
        {
            Position = default;
            Rotation = default;
            Velocity = default;
        }
    }
}