using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using System;
using Tests.Weapons.Launcher;
using UnityEngine;
using UnityEngine.VFX;

namespace Tests.BodyBehaviour.Arm
{
    [Serializable]
    internal class ArmReloadAnimation
    {

        [SerializeField]
        float _clipLength;
        [SerializeField]
        internal string _speedMultiplierName;
        [SerializeField]
        internal string _enterParamName;
        [SerializeField]
        Animator _animator;
        float _expectedSpeedMultiplier;
        public float DurationTime
        {
            set
            {
                if (value > 0)
                    _expectedSpeedMultiplier = _clipLength / value;
                else
                    _expectedSpeedMultiplier = 1;
            }
        }
        public void Play()
        {
            _animator.SetBool(_enterParamName, true);
        }
        public void Stop()
        {
            _animator.SetBool(_enterParamName, false);
        }
        public void Pause()
        {
            _animator.SetFloat(_speedMultiplierName, 0);
        }
        public void Continue()
        {
            _animator.SetFloat(_speedMultiplierName, _expectedSpeedMultiplier);
        }
    }

}