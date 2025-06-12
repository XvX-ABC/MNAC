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
using UnityEngine.InputSystem.XR.Haptics;

namespace Tests.BodyBehaviour.Arm.Weapons
{
    [RequireComponent(typeof(AimIK))]
    public class LauncherBehaviours_New : MonoBehaviour, IArmWeaponBehaviour, IAimer
    {
        ArmAimer _aimer;
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
        }
        void LateUpdate()
        {
            _aimer.OnUpdate();
        }
        public bool BEnd()
        {
            Debug.Log("Launcher behaviour ended.");
            return _aimer.BEnd();
        }

        public bool BStart()
        {
            return _aimer.BStart();
        }
    }
}
