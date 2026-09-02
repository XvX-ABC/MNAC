using System;
using MNAC.Characters.Humanoid.Input;
using MNAC.Behaviours.Input;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace MNAC.Player
{

    [Serializable]
    public class PlayerArmInput : IArmInput
    {
        class ControlInput : IWeaponControlInput
        {
            internal KeyCode _switch;
            internal KeyCode _fire;
            internal KeyCode _reload;

            public ControlInput(KeyCode @switch, KeyCode fire, KeyCode reload)
            {
                _switch = @switch;
                _fire = fire;
                _reload = reload;
            }

            public bool Fire => !UInput.GetKey(_switch) && !UInput.GetKey(_reload) && UInput.GetKey(_fire);

            public bool Reload => UInput.GetKey(_reload) && UInput.GetKey(_fire);
        }
        [SerializeField]
        KeyCode _switch;
        [SerializeField]
        KeyCode _fire;
        [SerializeField]
        KeyCode _reload;
        ControlInput _control;
        public bool WeaponSwitch => UInput.GetKey(_switch) && UInput.GetKey(_fire);

        public IWeaponControlInput WeaponControl
        {
            get
            {
                if (_control == null)
                {
                    _control = new(_switch, _fire, _reload);
                }
                return _control;
            }
        }
    }
}
