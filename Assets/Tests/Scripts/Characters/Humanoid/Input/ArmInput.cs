using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
using System;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Tests.Characters.Interaction.Input
{
    [Serializable]
    public class ArmInput : IArmInput
    {
        [Serializable]
        class ControlInput : IWeaponControlInput
        {
            [SerializeField]
            KeyCode _fire;
            [SerializeField]
            KeyCode _reload;
            public bool Fire => UInput.GetKey(_fire);

            public bool Reload => UInput.GetKey(_reload) && UInput.GetKey(_reload);
        }
        [SerializeField]
        KeyCode _switch;
        [SerializeField]
        ControlInput _control;
        public bool WeaponSwitch => UInput.GetKey(_switch) && _control.Fire;

        public IWeaponControlInput WeaponControl => _control;
    }
}
