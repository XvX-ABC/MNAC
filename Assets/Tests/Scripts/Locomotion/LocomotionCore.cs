using Locomotion;
using System;
using System.Text;
using Tests.Environment;
using Tests.Input;
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
    public class LocomotionCore : MonoBehaviour
    {
        [SerializeField]
        Camera _camera;

        Rigidbody _rb;
        internal HorizontalLocomotion horizontalLocomotion;
        internal BoostingLocomotion boostingLocomotion;
        internal HorizontalDrag horizontalDrag;
        internal JumpLocomotion jumpLocomotion;
        internal AirLocomotion airLocomotion;
        internal Gravity gravity;
        internal QuarterViewRotation rotation;
        internal PlatformLocomotion platformLocomotion;


        LocomotionAnimatorCore _locomotionAnimator;

        ILocomotionDefinitions _definitions;
        IGroundDetector _groundDetector;
        IHybridInput _input;
        Context _context;

        IModule[] _modules;
        void Awake()
        {
            _definitions = GetComponent<ILocomotionDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILocomotionDefinitions));
            _groundDetector = GetComponent<IGroundDetector>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IGroundDetector));
            _input = GetComponent<IHybridInput>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IInput));

            _rb = GetComponent<Rigidbody>();


            platformLocomotion = GetComponent<PlatformLocomotion>() ?? throw new ComponentCantFindException(this.gameObject, typeof(PlatformLocomotion));

            horizontalDrag = new(_definitions.Base);
            jumpLocomotion = new(_definitions.Jump);
            horizontalLocomotion = new(_definitions.Base, jumpLocomotion);
            boostingLocomotion = new(_definitions.Base, _definitions.Boosting, jumpLocomotion);
            airLocomotion = new(_definitions.Base, jumpLocomotion);
            gravity = new();
            rotation = new(this.gameObject);
            var collider = GetComponent<Collider>();

            var target = new Tests.Environment.Target(_camera);

            _context = new(_rb, _input, target, collider, _groundDetector);
            target.context = _context;


            _modules = new IModule[]
            {
                //platformLocomotion.AM,
                rotation,
                horizontalLocomotion,
                boostingLocomotion,
                horizontalDrag,
                jumpLocomotion,
                airLocomotion,
                //platformLocomotion.BM
            };



            _locomotionAnimator = GetComponent<LocomotionAnimatorCore>();
            if (_locomotionAnimator != null)
            {
                Array.Resize(ref _modules, _modules.Length + 1);
                Array.Copy(_modules, 0, _modules, 1, _modules.Length - 1);
                _modules[0] = _locomotionAnimator;
            }
        }
        void UpdateContext()
        {
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
            _rb.MovePosition(_context.Position);
            if (_context.Rotation != Quaternion.identity)
                _rb.MoveRotation(_context.Rotation);
        }
        void OnFixedUpdateOfChildrenModules()
        {
            for (int i = 0; i < _modules.Length; i++)
            {
                var module = _modules[i];
                module.OnFixedUpdate(_context);
            }
        }
        void OnUpdateOfChildrenModules()
        {
            for (int i = 0; i < _modules.Length; i++)
            {
                var m = _modules[i];
                m.OnUpdate(_context);
            }
        }

        void FixedUpdate()
        {
            UpdateContext();
            OnFixedUpdateOfChildrenModules();
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