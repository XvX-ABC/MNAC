using System;
using UnityEngine;
namespace Tests.Locomotion
{
    public class World
    {
        public static Vector3 DefaultUp = Vector3.up;
        public static Vector3 DefaultRight = Vector3.right;
        public static Vector3 DefaultForward = Vector3.forward;

        protected internal struct InputWrapper : IHybridInput
        {
            public IHybridInput _input;
            Vector3 _horizontalDirection;
            World _world;
            public InputWrapper(IHybridInput input, World world)
            {
                _input = input ?? throw new ArgumentNullException(nameof(input));
                _world = world ?? throw new ArgumentNullException(nameof(world));
                _horizontalDirection = Vector3.zero;
            }

            public IHybridInput.Mode CurrentMode { get => _input.CurrentMode; set => _input.CurrentMode = value; }
            public Vector3 HorizontalDirection { get => _horizontalDirection; set => _horizontalDirection = value; }
            public bool IsAscending { get => _input.IsAscending; set => _input.IsAscending = value; }
            public bool IsBoosting { get => _input.IsBoosting; set => _input.IsBoosting = value; }
            public void OnUpdate(IGround? ground)
            {
                var direction = _input.HorizontalDirection;
                if (direction == Vector3.zero)
                    _horizontalDirection = Vector3.zero;
                else
                    _horizontalDirection = ground == null ? _input.HorizontalDirection : Vector3.ProjectOnPlane(direction, _world.Up).normalized;

            }
        }
        InputWrapper _input;
        public Vector3 Up;
        public Vector3 Right;
        public Vector3 Forward;
        private World() { }
        public World(IHybridInput input)
        {
            _input = new(input, this);
        }

        public IHybridInput Input { get => _input; }
        public void UpdateTranslations(Vector3 upwards)
        {
            if (upwards == DefaultUp)
            {
                Up = DefaultUp;
                Right = DefaultRight;
                Forward = DefaultForward;
                return;
            }
            var rotation = Quaternion.FromToRotation(DefaultUp,upwards);
            Up = upwards;
            Right = rotation * Vector3.right;
            Forward = rotation * Vector3.forward;
        }
        public void OnUpdate(IGround? ground, Vector3 upwards)
        {
            UpdateTranslations(upwards);
            _input.OnUpdate(ground);
        }
    }
}