using Locomotion;
using System;
using System.Text;
using Tests.Locomotion.Animation;
using UnityEngine;
namespace Tests.Locomotion
{
    public enum State
    {
        OnGround,
        Ascending,
        Descending
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
        internal JumpLocomotion_New jumpLocomotion_New;
        internal AirLocomotion airLocomotion;
        internal Gravity gravity;
        internal QuarterViewRotation rotation;
        internal PlatformLocomotion platformLocomotion;


        LocomotionAnimator _locomotionAnimator;

        ILocomotionDefinitions _definition;
        IGroundDetector _groundDetector;
        ILocomotionAnimator _animator;
        IHybridInput _input;
        Context _context;
        CollisionContext _collisionContext;

        IModule[] _modules;
        ICollisionDetector[] _collisionModules;
        void Awake()
        {
            _definition = GetComponent<ILocomotionDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILocomotionDefinitions));
            _groundDetector = GetComponent<IGroundDetector>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IGroundDetector));
            //_animator = GetComponent<ILocomotionAnimator>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(ILocomotionAnimator));
            _input = GetComponent<IHybridInput>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IInput));

            _rb = GetComponent<Rigidbody>();


            platformLocomotion = GetComponent<PlatformLocomotion>() ?? throw new ComponentCantFindException(this.gameObject, typeof(PlatformLocomotion));

            horizontalDrag = new(_definition.Base, _rb);
            jumpLocomotion_New = new(_definition.Jump);
            horizontalLocomotion = new(_definition.Base, jumpLocomotion_New);
            quickBoostLocomotion = new(_definition.Base, _definition.QuickBoost, jumpLocomotion_New);
            airLocomotion = new(_definition.Base, jumpLocomotion_New);
            gravity = new();
            rotation = new(this.gameObject);
            var collider = GetComponent<Collider>();

            //var ground = new Ground(_groundDetector);
            var target = new Target(_camera);
            //_context = new()
            //{
            //    //Ground = ground,
            //    RigidBody = _rb,
            //    Collider = GetComponent<Collider>(),
            //    Input = _input,
            //    Target = target,
            //};

            _context = new(_rb, _input, target, collider, _groundDetector);
            target.context = _context;


            _collisionContext = new()
            {
                Collision = null,
                ContactPoints = new(),
            };

            _modules = new IModule[]
            {
                //platformLocomotion.AM,
                rotation,
                horizontalLocomotion,
                quickBoostLocomotion,
                horizontalDrag,
                jumpLocomotion_New,
                //jumpLocomotion,
                airLocomotion,
                //gravity,
                //platformLocomotion.BM
            };

            _collisionModules = new ICollisionDetector[]
            {
                _groundDetector,
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
            //_context.Ground = _groundDetector.CollidedGround;
            //_context.Locomotion = new()
            //{
            //    Position = _rb.position,
            //    Rotation = _rb.rotation,
            //    Velocity = _rb.velocity,
            //};
            //var ground = _context.Ground;
            //var state = _context.State;

            //_context.World = ground == null ? World.Default : World.NewTranslation(ground.Normal);

            _context.OnUpdate(_groundDetector.CollidedGround);


            var ground = _context.Ground;
            var state = _context.State;

            if (ground != null)
            {
                if (_rb.velocity.y <= 0)
                    _context.State = State.OnGround;
            }
            else if (ground == null && state != State.Ascending)
            {
                _context.State = State.Descending;
            }


        }

        void ApplyContext()
        {
            _rb.velocity = _context.Velocity;
            Debug.Log("context.speed :" + Vector3.ProjectOnPlane(_context.Velocity, Vector3.up).magnitude);
            _rb.MovePosition(_context.Position);
            _rb.MoveRotation(_context.Rotation);
            //_rb.MoveRotation(_context.Rotation * Quaternion.Euler(0, 35 * Time.deltaTime, 0));
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
            UpdateContext();
            Run();
            //DebugRun();
            ApplyContext();
        }
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            var velocity = _rb.velocity;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_rb.position, _rb.position + velocity);
        }
    }
}