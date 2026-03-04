using MNAC.Characters.Humanoid.Input;
using MNAC.Utilities.Blackboards;
using UnityEngine;

namespace MNAC.AI
{
    internal class HumanoidInputComponentAdapter : HumanoidInputComponent
    {
        [SerializeField]
        AIHumanoidInput_Mono _component;

        protected override IHumanoidInput humanInput => _component.input;
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
        }
    }
}
