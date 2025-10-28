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

            public bool Reload => UInput.GetKey(_reload);
        }
        [SerializeField]
        KeyCode _switch;
        [SerializeField]
        ControlInput _control;
        public bool Switch => UInput.GetKey(_switch);

        public IWeaponControlInput Control => _control;
    }
}
