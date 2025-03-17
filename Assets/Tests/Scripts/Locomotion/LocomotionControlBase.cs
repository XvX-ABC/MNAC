using Locomotion;
using System;
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
        public Rigidbody RigidBody;
        public Collider Collider;
        public Vector3 Velocity { get => Locomotion.Velocity; set => Locomotion.Velocity = value; }
        public Quaternion Rotation { get => Locomotion.Rotation; set => Locomotion.Rotation = value; }
        public Vector3 Position { get => Locomotion.Position; set => Locomotion.Position = value; }
        public ITarget Target;
        public float DeltaTime;
        public IHybridInput Input;
        public IGround Ground;
        public IGroundDetector GroundDetector;
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

            horizontalLocomotion = new(_definition.Base);
            horizontalDrag = new(_definition.Base);
            jumpLocomotion = new(_definition.Jump);
            jumpLocomotion_New = new(_definition.Jump, GetComponent<JumpCollision>());
            quickBoostLocomotion = new(_definition.QuickBoost, jumpLocomotion);
            airLocomotion = new(_definition.Base, jumpLocomotion);
            gravity = new();
            rotation = new(this.gameObject);
            var collider = GetComponent<Collider>();
            Debug.Log("Bounds: " + collider.bounds);

            var ground = new Ground(_groundDetector);
            var target = new Target(_camera);
            _context = new()
            {
                //Ground = ground,
                RigidBody = _rb,
                Collider = GetComponent<Collider>(),
                Input = _input,
                Target = target,
            };
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
                //quickBoostLocomotion,
                horizontalDrag,
                jumpLocomotion_New,
                //jumpLocomotion,
                //airLocomotion,
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
            _context.Ground = _groundDetector.CollidedGround;
            _context.Locomotion = new()
            {
                Position = _rb.position,
                Rotation = _rb.rotation,
                Velocity = _rb.velocity,
            };
            var ground = _context.Ground;
            var state = _context.State;
            if (ground != null && _rb.velocity.y <= 0)
                _context.State = State.OnGround;
            else if (ground == null && state != State.Ascending)
            {
                _context.State = State.Descending;
            }
            _context.DeltaTime = Time.fixedDeltaTime;

        }

        void ApplyContext()
        {
            _rb.velocity = _context.Velocity;
            _rb.MovePosition(_context.Position);
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
        void CollisionContextInitialize(Collision collision)
        {
            _collisionContext.Collision = collision;
            collision.GetContacts(_collisionContext.ContactPoints);
        }
        private void OnCollisionEnter(Collision collision)
        {
            CollisionContextInitialize(collision);
            foreach (var c in _collisionModules)
            {
                c.OnColliderEnter(_collisionContext);
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            CollisionContextInitialize(collision);
            foreach (var c in _collisionModules)
            {
                c.OnColliderStay(_collisionContext);
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            CollisionContextInitialize(collision);
            foreach (var c in _collisionModules)
            {
                c.OnColliderExit(_collisionContext);
            }
        }
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            var velocity = _rb.velocity;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_rb.position, velocity);
        }
    }
}