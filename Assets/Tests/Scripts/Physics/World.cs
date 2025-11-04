
using UnityEngine;

namespace Tests.TPhysics
{
    public class World
    {
        public static readonly Vector3 DefaultUp = Vector3.up;
        public static readonly Vector3 DefaultRight = Vector3.right;
        public static readonly Vector3 DefaultForward = Vector3.forward;
        public static readonly World Default;
        static World()
        {
            Default = new();
        }
        Vector3 _up;
        Vector3 _right;
        Vector3 _forward;
        Vector3 _gravity;
        internal Quaternion rotation;
        public World(Vector3 upwards, Vector3 gravity)
        {
            UpdateTransitions(upwards);
            _gravity = gravity;
        }
        public World() : this(DefaultUp, Physics.gravity)
        {

        }
        public Vector3 Up
        {
            get => _up;
        }
        public Vector3 Right
        {
            get => _right;
        }
        public Vector3 Forward
        {
            get => _forward;
        }
        public Vector3 Gravity
        {
            get => _gravity;
            set => _gravity = value;
        }
        public Quaternion Rotation { get => rotation; }
        void UpdateTransitions(Vector3 upwards)
        {
            rotation = Quaternion.FromToRotation(DefaultUp, upwards);
            if (upwards == DefaultUp)
            {
                _up = DefaultUp;
                _right = DefaultRight;
                _forward = DefaultForward;
                return;
            }
            _up = upwards;
            _right = rotation * Vector3.right;
            _forward = rotation * Vector3.forward;
        }
        public Vector3 InverseTransformVector(Vector3 vector)
        {
            return Quaternion.Inverse(rotation) * vector;
        }
        public Vector3 TransformVector3(Vector3 vector)
        {
            return rotation * vector;
        }
    }
}
