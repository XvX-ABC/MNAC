using System;
using Tests.Behaviours.Arms;
using Tests.Environment;
using Tests.Input;
using UnityEngine;
namespace Tests.Locomotion
{
    public class Context
    {
        LocomotionContext _originalLocomotion;
        LocomotionContext _expectedLocomotion;
        public World World;
        public Vector3 Velocity
        {
            get => _expectedLocomotion.Velocity == Vector3.zero ? _originalLocomotion.Velocity : _expectedLocomotion.Velocity;
            set => _expectedLocomotion.Velocity = value;
        }
        public Quaternion Rotation
        {
            get => _expectedLocomotion.Rotation == default ? _originalLocomotion.Rotation : _expectedLocomotion.Rotation;
            set => _expectedLocomotion.Rotation = value;
        }
        public Vector3 Position
        {
            get => _expectedLocomotion.Position == default ? _originalLocomotion.Position : _expectedLocomotion.Position;
            set => _originalLocomotion.Position = value;
        }
        public float Speed { get => _expectedLocomotion.Speed == default ? _originalLocomotion.Speed : _expectedLocomotion.Speed; }
        public float SquareSpeed
        {
            get => _expectedLocomotion.SquareSpeed == default ? _originalLocomotion.SquareSpeed :
_expectedLocomotion.SquareSpeed;
        }

        public LocomotionContext OriginalLocomotion { get => _originalLocomotion; }
        public LocomotionContext ExpectedLocomotion { get => _expectedLocomotion; }
        public IHybridInput Input;
        public ITarget_Obsolete Target;
        public float DeltaTime;
        public IGround Ground;
        public IGroundDetector GroundDetector;
        public State State;
        public Transform Transform;
        public Context(Rigidbody rb, Transform transform, IHybridInput input, ITarget_Obsolete target, Collider collider, IGroundDetector groundDetector)
        {
            this.Transform = transform;
            _originalLocomotion = new(rb);
            _expectedLocomotion = new(null);
            World = new(input);
            Input = input;
            //Target = target;
            GroundDetector = groundDetector;

        }
        public void OnUpdate(IGround? ground)
        {
            Ground = GroundDetector.CollidedGround;
            _originalLocomotion.OnUpdate();
            _expectedLocomotion.OnUpdate();
            World.OnUpdate(ground, ground == null ? Vector3.up : ground.Normal);
            DeltaTime = Time.fixedDeltaTime;
        }
        public override string ToString()
        {
            return $"State: {State},\nGround: {{ {Ground} }}, \nLocomotionContext: {{ {_originalLocomotion} }}";
        }

    }
}