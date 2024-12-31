using Locomotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Locomotion.Animation;
using Unity.VisualScripting;
using UnityEngine;
namespace Tests.Locomotion
{
    public enum State
    {
        OnGround,
        Ascending,
        Descending
    }
    public class Context
    {
        public LocomotionContext Locomotion;
        public Vector3 Velocity { get => Locomotion.Velocity; set => Locomotion.Velocity = value; }
        public Quaternion Rotation { get => Locomotion.Rotation; set => Locomotion.Rotation = value; }
        public Vector3 Position { get => Locomotion.Position; set => Locomotion.Position = value; }
        public ITarget Target;
        public float DeltaTime;
        public IInput Input;
        public IGround Ground;
        public State State;
        public override string ToString()
        {
            return $"State: {State},\nGround: {{ {Ground} }}, \nLocomotionContext: {{ {Locomotion} }}";
        }

    }
    [RequireComponent(typeof(Rigidbody))]
    public class LocomotionControlBase : MonoBehaviour
    {
        [SerializeField]
        Camera _camera;

        Rigidbody _rb;
        internal HorizontalLocomotion horizontalLocomotion;
        internal QuickBoostLocomotion quickBoostLocomotion;
        internal HorizontalDrag horizontalDrag;
        internal JumpLocomotion jumpLocomotion;
        internal AirLocomotion airLocomotion;
        internal Gravity gravity;
        internal QuarterViewRotation rotation;


        LocomotionAnimator _locomotionAnimator;

        ILocomotionDefine _defines;
        IGroundSampler _groundSampler;
        ILocomotionAnimator _animator;
        IInput _input;
        Context _context;

        IModule[] _modules;

        void Awake()
        {
            _defines = GetComponent<ILocomotionDefine>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(ILocomotionDefine));
            _groundSampler = GetComponent<IGroundSampler>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(IGroundSampler));
            //_animator = GetComponent<ILocomotionAnimator>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(ILocomotionAnimator));
            _input = GetComponent<IInput>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(IInput));


            _rb = GetComponent<Rigidbody>();


            horizontalLocomotion = new(_defines.Base);
            horizontalDrag = new(_defines.Base);
            jumpLocomotion = new(_defines.Jump);
            quickBoostLocomotion = new(_defines.QuickBoost, jumpLocomotion);
            airLocomotion = new(_defines.Base, jumpLocomotion);
            gravity = new();
            rotation = new(this.gameObject);


            var ground = new Ground(_groundSampler);
            var target = new Target(_camera);
            _context = new()
            {
                Ground = ground,
                Input = _input,
                Target = target,
            };
            target.context = _context;


            _modules = new IModule[]
            {
                rotation,
                horizontalLocomotion,
                quickBoostLocomotion,
                horizontalDrag,
                jumpLocomotion,
                airLocomotion,
                gravity,
            };


            _locomotionAnimator = GetComponent<LocomotionAnimator>();
            if (_locomotionAnimator != null)
            {
                Array.Resize(ref _modules, _modules.Length + 1);
                _modules[^1] = _locomotionAnimator;
            }
        }
        void UpdateContext()
        {
            _context.Locomotion = new()
            {
                Position = _rb.position,
                Rotation = _rb.rotation,
                Velocity = _rb.velocity,
            };
            var ground = _context.Ground;
            var state = _context.State;
            if (ground.Touched)
                _context.State = State.OnGround;
            else if (!ground.Touched && state != State.Ascending)
                _context.State = State.Descending;
            _context.DeltaTime = Time.fixedDeltaTime;

        }

        void ApplyContext()
        {
            _rb.velocity = _context.Velocity;
            _rb.MoveRotation(_context.Rotation);
        }
        void DebugRun()
        {
            var sbuilder = new StringBuilder();
            for (int i = 0; i < _modules.Length; i++)
            {
                var module = _modules[i];
                module.OnUpdate(_context);
                sbuilder.AppendLine($"[{module.GetType().Name}]->{_context}\t");
            }
            Debug.Log(sbuilder.ToString());
        }
        void Run()
        {
            for (int i = 0; i < _modules.Length; i++)
            {
                var module = _modules[i];
                module.OnUpdate(_context);
            }
        }
        void FixedUpdate()
        {
            _groundSampler.Sample();
            UpdateContext();
            //for (int i = 0; i < _modules.Length; i++)
            //{
            //    var module = _modules[i];
            //    module.OnUpdate(_context);
            //}
            Run();
            //DebugRun();
            ApplyContext();
        }
    }
}