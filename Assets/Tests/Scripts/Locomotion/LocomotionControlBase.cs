using Locomotion;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
namespace Tests.Locomotion
{
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

    }
    [RequireComponent(typeof(Rigidbody))]
    public class LocomotionControlBase : MonoBehaviour
    {
        [SerializeField]
        Camera _camera;

        Rigidbody _rb;
        HorizontalLocomotion _horizontalLocomotion;
        JumpLocomotion _jumpLocomotion;
        Gravity _gravity;
        QuarterViewRotation _rotation;
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
            _animator = GetComponent<ILocomotionAnimator>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(ILocomotionAnimator));
            _input = GetComponent<IInput>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(IInput));

            _rb = GetComponent<Rigidbody>();

            _horizontalLocomotion = new(_defines.Base);
            _jumpLocomotion = new(_defines.Jump);
            _gravity = new(_jumpLocomotion);
            _rotation = new(this.gameObject);


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
            _horizontalLocomotion,
            _jumpLocomotion,
            _gravity,
            };
        }
        void UpdateContext()
        {
            _context.Locomotion = new()
            {
                Position = _rb.position,
                Rotation = _rb.rotation,
                Velocity = _rb.velocity,
            };
            _context.DeltaTime = Time.fixedDeltaTime;
        }

        void ApplyContext()
        {
            _rb.velocity = _context.Velocity;
            _rb.MoveRotation(_context.Rotation);
            _animator.OnFixedUpdate(_context.Locomotion);
        }
        void FixedUpdate()
        {
            UpdateContext();
            for (int i = 0; i < _modules.Length; i++)
            {
                var module = _modules[i];
                module.Update(_context);
            }

            ApplyContext();
        }
    }
}