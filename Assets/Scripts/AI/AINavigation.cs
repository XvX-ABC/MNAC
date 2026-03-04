using System;
using MNAC.Interaction;
using MNAC.Utilities.Blackboards;
using MNAC.Utilities.Extensions;
using UnityEngine;
using LocomotionCore = MNAC.Characters.Humanoid.Locomotion.LocomotionCore;
namespace MNAC.AI
{
    [Serializable]
    internal class AINavigation : AIComponent
    {
        [SerializeField]
        float _sampleInterval;
        LocomotionCore _locomotionCore;
        internal NavigationModule module;
        internal LocomotionCore locomotionCore
        {
            get => _locomotionCore;
            set
            {
                if (_locomotionCore != null)
                    _locomotionCore.internalCore.EvaluationModules = _locomotionCore.internalCore.EvaluationModules.Remove(module);
                if (value != null)
                {
                    value.internalCore.EvaluationModules = value.internalCore.EvaluationModules.Append(module);
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
                if (module != null)
                {
                    module.Enabled = value;
                }
            }
        }
        public override string Name => "ai_navigation";

        public IPositionTarget SteeringTarget => module.steeringTarget;
        public IPositionTarget Destination { get => module.destination; set => module.destination = value; }
        public bool IsMoving { get => module.IsMoving; }
        public override void Initialize(AIComponentContext context)
        {
            base.Initialize(context);

            module = new(context, _sampleInterval);
            module.Enabled = this.Enabled;


            var characterBlackboard = context.characterBlackboard;
            if (characterBlackboard.TryReadValue<LocomotionCore>(AIBlackboardFields.Character_LocomotionCore, out var lcore))
            {
                locomotionCore = lcore;
            }
            else
            {
                characterBlackboard.RegisterFieldChangeAction<LocomotionCore>(AIBlackboardFields.Character_LocomotionCore, WhenLocomotionCoreChange);
            }
            //module.destination = context.target;
            context.navigation = this;
        }
        public override void Dispose()
        {
            locomotionCore = null;
            var characterBlackboard = context.characterBlackboard;
            characterBlackboard.UnregisterFieldChangeAction<LocomotionCore>(AIBlackboardFields.Character_LocomotionCore, WhenLocomotionCoreChange);
            context.navigation = null;
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
