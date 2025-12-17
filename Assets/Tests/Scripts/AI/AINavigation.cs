using Tests.Characters.Humanoid.Locomotion;
using Tests.Extensions;
using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.AI
{
    internal class AINavigation : AIComponent
    {
        [SerializeField]
        float _positionSampleInterval;
        LocomotionCore _locomotionCore;
        internal NavigationModule module;

        internal LocomotionCore locomotionCore
        {
            get => _locomotionCore;
            set
            {
                if (_locomotionCore != null)
                    _locomotionCore.core.EvaluationModules = _locomotionCore.core.EvaluationModules.Remove(module);
                if (value != null)
                {
                    value.core.EvaluationModules = value.core.EvaluationModules.Append(module);
                }
                _locomotionCore = value;
            }
        }

        public override void Initialize(AIComponentContext context)
        {
            base.Initialize(context);
            module = new(context, _positionSampleInterval);
            var characterBlackboard = context.characterBlackboard;
            if (characterBlackboard.TryReadValue<LocomotionCore>(AIBlackboardFields.LocomotionCore, out var lcore))
            {
                locomotionCore = lcore;
                module.Enabled = true;
            }
            else
                characterBlackboard.RegisterFieldChangeAction<LocomotionCore>(AIBlackboardFields.LocomotionCore, WhenLocomotionCoreWrite);
            context.navigation = this;
        }
        public override void Dispose()
        {
            context.navigation = null;
            base.Dispose();
        }
        void WhenLocomotionCoreWrite(FieldEventType type, LocomotionCore ov, LocomotionCore nv)
        {
            if (type == FieldEventType.Reading)
                return;
            locomotionCore = nv;
            module.Enabled = true;
        }
    }
}
