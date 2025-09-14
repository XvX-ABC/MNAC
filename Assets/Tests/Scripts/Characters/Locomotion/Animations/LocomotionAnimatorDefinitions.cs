using UnityEngine;

namespace Tests.Characters.Locomotion.Animations
{
    public class LocomotionAnimatorDefinitions : MonoBehaviour, ILocomotionAnimatorDefinitions
    {
        string _velocity_x;
        string _velocity_y;
        string _jump_trigger;

        public string Velocity_X => _velocity_x;

        public string Velocity_Y => _velocity_y;

        public string Jump_Trigger => _jump_trigger;
    }
}
