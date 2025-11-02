using System;
using Tests.Characters.Interaction.Input;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Tests.Characters.Humanoid.Interaction.Input
{
    // BUG: 其他按键与开火键绑定后，还会触发开火
    [Serializable]
    public class ArmInput : IArmInput
    {
        [Serializable]
        class ControlInput : IWeaponControlInput
        {
            [SerializeField]
            internal KeyCode _fire;
            [SerializeField]
            internal KeyCode _reload;
            public bool Fire => UInput.GetKey(_fire);

            public bool Reload => UInput.GetKey(_reload);
        }
        [SerializeField]
        KeyCode _switch;
        [SerializeField]
        ControlInput _control;
        public bool WeaponSwitch => UInput.GetKey(_switch);

        public IWeaponControlInput WeaponControl => _control;
    }
}
