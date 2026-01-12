using Tests.Behaviours.Input;
using Tests.Characters.Humanoid.Input;
using Tests.Characters.Interaction.Input;
using UnityEngine;

namespace Tests.AI
{
    internal class AIHumanoidInput : IHumanoidInput
    {
        #region internal classes
        internal class BInput : IBaseInput
        {
            internal Vector3 horizontalVector;
            public Vector3 MousePosition => Vector3.zero;

            public Vector3 MousePositionDelta => Vector3.zero;

            public Vector3 HorizontalVector => horizontalVector;
        }
        internal class WInput : IWeaponControlInput
        {
            internal bool fire;
            internal bool reload;
            public bool Fire => fire;

            public bool Reload => reload;
        }
        internal class AInput : IArmInput
        {
            internal bool weaponSwitch;
            internal WInput weaponInput;
            public AInput()
            {
                weaponInput = new();
            }
            public bool WeaponSwitch => weaponSwitch;

            public IWeaponControlInput WeaponControl => weaponInput;
        }


        #endregion;
        internal BInput baseInput;
        internal AInput leftArmInput;
        internal AInput rightArmInput;

        public AIHumanoidInput()
        {
            baseInput = new();
            leftArmInput = new();
            rightArmInput = new();
        }
        public IBaseInput BaseInput => baseInput;

        public bool Jump => false;

        public bool QuickBoost => false;

        public IArmInput LArm => leftArmInput;

        public IArmInput RArm => rightArmInput;
    }
}
