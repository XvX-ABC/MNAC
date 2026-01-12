using System;
using UnityEngine;

namespace Tests.Characters.Humanoid.Locomotion.Animations
{
    [Serializable]
    public class LocomotionAnimatorDefinitions : ILocomotionAnimatorDefinitions
    {
        [SerializeField]
        string _velocity_x;
        [SerializeField]
        string _velocity_y;
        [SerializeField]
        float _jump_clip_length;
        [SerializeField]
        string _jump_trigger;
        [SerializeField]
        string _jump_multiplier;
        [SerializeField]
        string _inAir;
        [SerializeField]
        string _descending;
        [SerializeField]
        float _boosting_clip_length;
        [SerializeField]
        string _boosting_trigger;
        [SerializeField]
        string _boosting_multiplier;
        public string Velocity_X => _velocity_x;

        public string Velocity_Y => _velocity_y;

        public string InAir => _inAir;
        public string Jump_Trigger => _jump_trigger;

        public string Jump_Multiplier => _jump_multiplier;

        public float Jump_Clip_Length => _jump_clip_length;

        public string Descending => _descending;

        public string Boosting_Trigger => _boosting_trigger;

        public string Boosting_Multiplier => _boosting_multiplier;
        public float Boosting_Clip_Length => _boosting_clip_length;
    }
}
