using System;
using Tests.Behaviours.Input;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Tests.Characters.Interaction.Input
{
    [Serializable]
    public class HumanInput : IHumanInput
    {
        [SerializeField]
        BaseInput _baseInput;
        [SerializeField]
        KeyCode _jump;
        [SerializeField]
        KeyCode _quickBoost;
        [SerializeField]
        ArmInput _arm_l;
        [SerializeField]
        ArmInput _arm_r;
        [SerializeField]
        ArmInput _arm_lb;
        [SerializeField]
        ArmInput _arm_rb;
        public IBaseInput BaseInput => _baseInput;

        public bool Jump => UInput.GetKey(_jump);

        public bool QuickBoost => UInput.GetKey(_quickBoost);;


        public IArmInput LArm => _arm_l;

        public IArmInput RArm => _arm_r;

        public IArmInput LBArm => _arm_lb;

        public IArmInput RBArm => _arm_rb;
    }
}
