using System;
using MNAC.Animations;
using UnityEngine;
using UnityEngine.Playables;

namespace MNAC.Behaviours.Arms.Weapons.Sword.Animations
{
    internal class WholeBodyControllerPlayable : ControllerPlayable
    {
        struct Parameters
        {

            internal string switchName;
            internal string multiplierName;
            internal float clipLength;

            public Parameters(string switchName, string multiplierName, float clipLength)
            {
                this.switchName = switchName ?? throw new ArgumentNullException(nameof(switchName));
                this.multiplierName = multiplierName ?? throw new ArgumentNullException(nameof(multiplierName));
                this.clipLength = Mathf.Max(0, clipLength);
            }
        }
        Parameters _boostingParams;
        Parameters _slashParams;
        public WholeBodyControllerPlayable(
            PlayableGraph graph,
            string boostingSwitchName,
            string boostingMultiplierName,
            float boostingClipLength,
            string slashSwitchName,
            string slashMultiplierName,
            float slashClipLength,
            Animator animator) : base(graph, animator)
        {
            _boostingParams = new(boostingSwitchName, boostingMultiplierName, boostingClipLength);
            _slashParams = new(slashSwitchName, slashMultiplierName, slashClipLength);
        }

        public WholeBodyControllerPlayable(
            PlayableGraph graph,
            string boostingSwitchName,
            string boostingMultiplierName,
            float boostingClipLength,
            string slashSwitchName,
            string slashMultiplierName,
            float slashClipLength,
            RuntimeAnimatorController controller) : base(graph, controller)
        {
            _boostingParams = new(boostingSwitchName, boostingMultiplierName, boostingClipLength);
            _slashParams = new(slashSwitchName, slashMultiplierName, slashClipLength);
        }
        float CalculateMultiplier(float clipLength, float expectedLength)
        {
            return clipLength / (expectedLength > 0 ? expectedLength : 1);
        }
        public void SetBoostingMultiplier(float duration)
        {
            SetFloat(_boostingParams.multiplierName, CalculateMultiplier(_boostingParams.clipLength, duration));
        }
        public void SetSlashMultiplier(float duration)
        {
            SetFloat(_slashParams.multiplierName, CalculateMultiplier(_slashParams.clipLength, duration));
        }
        public bool GetBoostingSwitch()
        {
            return GetBool(_boostingParams.switchName);
        }
        public bool GetSlashSwitch()
        {
            return GetBool(_slashParams.switchName);
        }
        public void SetBoostingSwitch(bool value)
        {
            SetBool(_boostingParams.switchName, value);
        }
        public void SetSlashSwitch(bool value)
        {
            SetBool(_slashParams.switchName, value);
        }
    }
}
