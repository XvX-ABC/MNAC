using Tests.Characters;
using UnityEngine;

namespace Tests.AI
{
    internal class CharacterComponentAdapter : CharacterComponent
    {
        [SerializeField]
        AIComponent _component;
    }
    //internal class AIHumanoidInputComponent : AIComponent
    //{
    //    AIHumanoidInput _input;
    //    AIHumanoidInput.BInput _baseInput => _input.baseInput;
    //    protected override void Awake()
    //    {
    //        base.Awake();

    //    }

    //}
}
