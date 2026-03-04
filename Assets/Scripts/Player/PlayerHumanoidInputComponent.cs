using MNAC.Characters.Humanoid.Input;
using UnityEngine;

namespace MNAC.Player
{
    internal class PlayerHumanoidInputComponent : HumanoidInputComponent
    {
        [SerializeField]
        PlayerHumanoidInput _input;
        protected override IHumanoidInput humanInput => _input;
    }
}
