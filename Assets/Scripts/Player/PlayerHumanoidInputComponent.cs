using Tests.Characters.Humanoid.Input;
using UnityEngine;

namespace Tests.Player
{
    internal class PlayerHumanoidInputComponent : HumanoidInputComponent
    {
        [SerializeField]
        PlayerHumanoidInput _input;
        protected override IHumanoidInput humanInput => _input;
    }
}
