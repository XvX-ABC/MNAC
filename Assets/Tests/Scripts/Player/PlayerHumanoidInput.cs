using System;
using Tests.Behaviours.Input;
using Tests.Characters.Humanoid.Input;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Tests.Player
{
    [Serializable]
    public class PlayerHumanoidInput : IHumanoidInput
    {
        [SerializeField]
        PlayerBaseInput _baseInput;
        [SerializeField]
        KeyCode _jump;
        [SerializeField]
        KeyCode _quickBoost;
        [SerializeField]
        PlayerArmInput _arm_l;
        [SerializeField]
        PlayerArmInput _arm_r;
        [SerializeField]
        PlayerArmInput _arm_lb;
        [SerializeField]
        PlayerArmInput _arm_rb;
        public IBaseInput BaseInput => _baseInput;

        public bool Jump => UInput.GetKey(_jump);

        public bool QuickBoost => UInput.GetKey(_quickBoost);


        public IArmInput LArm => _arm_l;

        public IArmInput RArm => _arm_r;

    }
}
