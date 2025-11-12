using System;
using System.Linq;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    [Serializable]
    public class ArmedSwordArmAnimationDefinitions : IArmedSwordArmAnimationDefinitions
    {
        [SerializeField]
        RuntimeAnimatorController _wholeBodyController;
        [SerializeField]
        RuntimeAnimatorController _armController;
        [SerializeField]
        string _velocityName_Y;
        [SerializeField]
        string _velocityName_X;
        [SerializeField]
        string _boostingSwitchName;
        [SerializeField]
        string _boostingSpeedMultiplierName;
        [SerializeField]
        float _boostingClipLength;
        [SerializeField]
        string _slashSwitchName;
        [SerializeField]
        string _slashSpeedMultiplierName;
        [SerializeField]
        float _slashClipLength;
        [SerializeField]
        StateTransitionOptions[] _transitionOptions;
        public ArmedSwordArmAnimationDefinitions()
        {

        }

        public ArmedSwordArmAnimationDefinitions(
            RuntimeAnimatorController wholeBodyController,
            RuntimeAnimatorController armController,
            string velocityName_Y,
            string velocityName_X,
            string boostingSwitchName,
            string boostingSpeedMultiplierName,
            float boostingClipLength,
            string slashSwitchName,
            string slashSpeedMultiplierName,
            float slashClipLength)
        {
            _wholeBodyController = wholeBodyController ?? throw new ArgumentNullException(nameof(wholeBodyController));
            _armController = armController ?? throw new ArgumentNullException(nameof(armController));
            _velocityName_Y = velocityName_Y ?? throw new ArgumentNullException(nameof(velocityName_Y));
            _velocityName_X = velocityName_X ?? throw new ArgumentNullException(nameof(velocityName_X));
            _boostingSwitchName = boostingSwitchName ?? throw new ArgumentNullException(nameof(boostingSwitchName));
            _slashSwitchName = slashSwitchName ?? throw new ArgumentNullException(nameof(slashSwitchName));
            _boostingSpeedMultiplierName = boostingSpeedMultiplierName ?? throw new ArgumentNullException(nameof(boostingSpeedMultiplierName));
            _boostingClipLength = Mathf.Max(0, boostingClipLength);
            _slashSpeedMultiplierName = slashSpeedMultiplierName ?? throw new ArgumentNullException(nameof(slashSpeedMultiplierName));
            _slashClipLength = Mathf.Max(0, slashClipLength);
        }

        public RuntimeAnimatorController WholeBodyController => _wholeBodyController;

        public RuntimeAnimatorController ArmController => _armController;

        public string VelocityName_Y => _velocityName_Y;

        public string VelocityName_X => _velocityName_X;

        public string BoostingSwitchName => _boostingSwitchName;
        public string BoostingSpeedMultiplierName => _boostingSpeedMultiplierName;
        public float BoostingClipLength => _boostingClipLength;
        public string SlashSwitchName => _slashSwitchName;
        public string SlashSpeedMultiplierName => _slashSpeedMultiplierName;
        public float SlashClipLength => _slashClipLength;

        public StateTransitionOptions GetTransitionOptions(IArmedSwordArmAnimationDefinitions.Transition transition)
        {
            return _transitionOptions.FirstOrDefault(t => t.Transition == transition);
        }
    }
}
