using System;
using Tests.Environment;
using Tests.Input;
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
            public Vector3 HorizontalVector { get => _horizontalDirection; set => _horizontalDirection = value; }
            public bool Jump { get => _input.Jump; set => _input.Jump = value; }
            public bool Boost { get => _input.Boost; set => _input.Boost = value; }

            public bool QuickBoost => throw new NotImplementedException();

            bool IInput_Obsolete.Fire => throw new NotImplementedException();

            bool IInput_Obsolete.Reload => throw new NotImplementedException();

            bool IInput_Obsolete.Supply => throw new NotImplementedException();

            public void OnUpdate(IGround? ground)
            {
                var direction = _input.HorizontalVector;
                if (direction == Vector3.zero)
                    _horizontalDirection = Vector3.zero;
                else
                    _horizontalDirection = ground == null ? _input.HorizontalVector : Vector3.ProjectOnPlane(direction, _world.Up).normalized;

            }
        }
        InputWrapper _input;
        public Vector3 Up;
        public Vector3 Right;
        public Vector3 Forward;
        public Vector3 Gravity;
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
            var rotation = Quaternion.FromToRotation(DefaultUp, upwards);
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