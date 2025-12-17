using Tests.Characters.Humanoid.Input;
using UnityEngine;

namespace Tests.AI
{
    internal class HumanoidInputComponentAdapter : HumanoidInputComponent
    {
        [SerializeField]
        AIHumanoidInputComponent _component;

        protected override IHumanoidInput humanInput => _component.input;
    }
}
