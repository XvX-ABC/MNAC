using System;
using Tests.Characters.Humanoid.Locomotion;
using Tests.Interaction;
using Tests.TPhysics.Locomotion;
using LCore = Tests.TPhysics.Locomotion.LocomotionCore;
namespace Tests.AI
{

    internal class AIRotationLocomotion : RotationLocomotionBase
    {
        IPositionTarget _target;
        IPositionTarget _steeringTarget;
        LCore _core;
        RotationByTargetLocomotion _locomotion;
        public AIRotationLocomotion(LCore locomotionCore) : base(locomotionCore)
        {
            _core = locomotionCore ?? throw new ArgumentNullException(nameof(locomotionCore));
            _locomotion = new();
        }
        public override string Name => "rotation_ai";

        internal override IPositionTarget target
        {
            get => _target;
            set
            {
                _locomotion.Target = value == null ? _steeringTarget : value;
                _target = value;
            }
        }
        internal IPositionTarget steeringTarget
        {
            get => _steeringTarget;
            set
            {
                if (_target == null)
                    _locomotion.Target = value;
                _steeringTarget = value;
            }
        }
        protected override LocomotionModuleBase rotationLocomotionModule => _locomotion;
        public override void OnUpdate()
        {
        }
    }
}
