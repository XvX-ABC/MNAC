using Tests.Characters.Humanoid.Input;
using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.AI
{
    internal class HumanoidInputComponentAdapter : HumanoidInputComponent
    {
        [SerializeField]
        AIHumanoidInputComponent _component;

        protected override IHumanoidInput humanInput => _component.input;
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            Debug.Log("input initialized");
        }
    }
}
