using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Input;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm.Weapons
{
    [RequireComponent(typeof(AimIK))]
    public class LauncherBehaviour : MonoBehaviour, IArmWeaponBehaviour, IAimer
    {
        ArmAimer _aimer;
        [SerializeField]
        ArmAimingAndReloadTransition _transition;
        [SerializeField]
        ArmReloadAnimation _reloadAnimation;
        ILauncher _launcher;
        IInput _input;
        public WeaponType Type => WeaponType.Launcher;

        public IWeapon Weapon
        {
            get => _launcher;

            set
            {
                if (value is ILauncher launcher)
                {
                    _launcher = launcher;
                    _transition.Launcher = launcher;
                }
                else
                    throw new InvalidCastException($"This weapon '{value.Name}' is not a launcher.");
            }
        }
        public IInput Input { set => _input = value; }

        public bool Continuing => _aimer.Continuing;
        public ITarget Target
        {
            get => _aimer.Target;
            set => _aimer.Target = value;
        }
        void Awake()
        {
            _aimer = new(GetComponent<AimIK>());
            Target = GetComponent<ITarget>();
            _transition.Initialize(_aimer, _reloadAnimation);
        }
        public void OnUpdate()
        {
            if (!this.enabled)
                return;
            if (_input.Reload)
            {
                _transition.BStart();
            }

            _transition.OnUpdate();
        }
        void LateUpdate()
        {

            _aimer.OnUpdate();
        }
        public bool BEnd()
        {
            if (_transition.Continuing)
                _transition.BEnd();
            _aimer.BEnd();
            enabled = false;
            return true;
        }

        public bool BStart()
        {
            this.enabled = true;
            return _aimer.BStart();
        }
    }
}
