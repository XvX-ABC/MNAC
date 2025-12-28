using System;
using Tests.Utilities.Blackboards;
using UnityEngine;
using LocomotionCore = Tests.Characters.Humanoid.Locomotion.LocomotionCore;
namespace Tests.AI
{
    internal class AIRotationControl : AIComponent
    {
        AINavigation _navigation;
        LocomotionCore _locomotionCore;
        AIRotationLocomotion _locomotion;

        internal LocomotionCore locomotionCore
        {
            get => _locomotionCore;
            set
            {
                if (value != null)
                {
                    if (_locomotion == null)
                    {
                        _locomotion = new(value.internalCore);
                        _locomotion.Enabled = this.Enabled;
                    }
                    value.rotationModule = _locomotion;
                }
                _locomotionCore = value;
            }
        }
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                if (_locomotion != null)
                    _locomotion.Enabled = value;
            }
        }
        public override string Name => "ai_rotation";
        internal void Update()
        {
            if (_locomotion == null)
                return;
            var target = _navigation?.SteeringTarget;
            _locomotion.steeringTarget = target;
        }
        public override void Initialize(AIComponentContext context)
        {
            base.Initialize(context);

            var characterBlackboard = context.characterBlackboard;
            if (characterBlackboard.TryReadValue<LocomotionCore>(AIBlackboardFields.Character_LocomotionCore, out var lcore))
            {
                locomotionCore = lcore;
            }
            else
            {
                characterBlackboard.RegisterFieldChangeAction<LocomotionCore>(AIBlackboardFields.Character_LocomotionCore, WhenLocomotionCoreChange);
            }
            _navigation = context.navigation ?? throw new NullReferenceException(nameof(context.navigation));
            locomotionCore = _navigation.locomotionCore;
        }
        public override void Dispose()
        {
            locomotionCore = null;
            var characterBlackboard = context.characterBlackboard;
            characterBlackboard.UnregisterFieldChangeAction<LocomotionCore>(AIBlackboardFields.Character_LocomotionCore, WhenLocomotionCoreChange);
            _navigation = null;
            base.Dispose();
        }
        void WhenLocomotionCoreChange(FieldEventType type, LocomotionCore ov, LocomotionCore nv)
        {
            if (type == FieldEventType.Reading)
                return;
            locomotionCore = nv;
        }
    }
}
