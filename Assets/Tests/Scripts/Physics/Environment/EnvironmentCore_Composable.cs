using Tests.Characters;
using Tests.TPhysics;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;
using Core = Tests.TPhysics.Environment.EnvironmentCore;
namespace Tests.TPhysics.Environment
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnvironmentCore_Composable : ComponentBase_MonoComponent
    {
        Rigidbody _rbody;
        Core _core;
        internal GroundDetector groundDetector => _core.GroundDetector;
        internal World world => _core.World;
        protected override void Awake()
        {
            base.Awake();
            var definitions = GetComponent<IEnvironmentDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(IEnvironmentDefinitions));
            _core = new Core(definitions);
            _rbody = GetComponent<Rigidbody>();
        }
        void OnEnable()
        {
            groundDetector.Enabled = true;
        }
        void OnDisable()
        {
            groundDetector.Enabled = false;
        }
        private void OnCollisionEnter(Collision collision)
        {
            groundDetector.OnCollisionEnter(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            groundDetector.OnCollisionExit(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            groundDetector.OnCollisionStay(collision);
        }
        private void FixedUpdate()
        {

            groundDetector.Position = _rbody.position;
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
        }
        public override void Dispose()
        {
            base.Dispose();
            blackboard.TryUnregisterField(CharacterBlackboardFields.World, world);
            blackboard.TryUnregisterField(CharacterBlackboardFields.GroundDetector, groundDetector);
        }
    }
}
