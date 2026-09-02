using MNAC.Characters.Humanoid;
using MNAC.TPhysics;
using MNAC.TPhysics.Environment;
using MNAC.Utilities.Blackboards;
using UnityEngine;
using Core = MNAC.TPhysics.Environment.EnvironmentCore;
using MNAC.Utilities;
namespace MNAC.Characters
{
    [RequireComponent(typeof(CapsuleCollider))]
    internal class EnvironmentCore : CharacterCollisionComponent
    {
        Rigidbody _rbody;
        Core _core;
        CapsuleCollider _collider;
        internal GroundDetector groundDetector => _core.GroundDetector;
        internal World world => _core.World;

        public Rigidbody Rbody { get => _rbody; set => _rbody = value; }
        public CapsuleCollider Collider { get => _collider; set => _collider = value; }

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<CapsuleCollider>();
            var definitions = GetComponent<IEnvironmentDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(IEnvironmentDefinitions));
            _core = new Core(definitions);
        }
        void OnEnable()
        {
            groundDetector.Enabled = true;
        }
        void OnDisable()
        {
            groundDetector.Enabled = false;
        }
        //private void OnCollisionEnter(Collision collision)
        //{

        //    groundDetector.OnCollisionEnter(collision);
        //}

        //private void OnCollisionExit(Collision collision)
        //{
        //    groundDetector.OnCollisionExit(collision);
        //}

        //private void OnCollisionStay(Collision collision)
        //{
        //    groundDetector.OnCollisionStay(collision);
        //}
        protected internal override void OnCollisionEnterImpl(Collision collision)
        {
            base.OnCollisionEnterImpl(collision);
            groundDetector.OnCollisionEnter(collision);
        }
        protected internal override void OnCollisionExitImpl(Collision collision)
        {
            base.OnCollisionExitImpl(collision);
            groundDetector.OnCollisionExit(collision);
        }
        protected internal override void OnCollisionStayImpl(Collision collision)
        {
            base.OnCollisionStayImpl(collision);
            groundDetector.OnCollisionStay(collision);
        }
        private void FixedUpdate()
        {
            groundDetector.Position = _rbody?.position ?? this.transform.position;
            groundDetector.OnFixedUpdate();
        }
        private void LateUpdate()
        {
            groundDetector.OnLateUpdate();
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.World, world);
            blackboard.TryRegisterField(CharacterBlackboardFields.GroundDetector, groundDetector);
            blackboard.TryReadValueOrThrowException(CharacterBlackboardFields.Rigidbody, out _rbody);
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(CharacterBlackboardFields.World, world);
            blackboard.TryUnregisterField(CharacterBlackboardFields.GroundDetector, groundDetector);
            base.Dispose();
        }
    }
}
