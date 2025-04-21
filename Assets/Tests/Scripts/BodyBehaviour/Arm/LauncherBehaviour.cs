using Assets.Tests.Scripts.Weapons;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Tests.Input;
using Tests.Locomotion;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using Debug = UnityEngine.Debug;

namespace Tests.BodyBehaviour.Arm
{
    [Serializable]
    public class LauncherBehaviour : MonoBehaviour, IArmWeaponBehaviour
    {

        [SerializeField]
        ArmAim _aim;

        IInput _input;
        GameObject _launcherObj;
        ILauncher _launcher;
        IArmBehaviour[] _behaviours;
        public WeaponType Type => WeaponType.Launcher;
        IWeapon IArmWeaponBehaviour.Weapon
        {
            get => _launcher;
            set
            {
                if (value is ILauncher launcher)
                    _launcher = launcher;
                else
                    throw new InvalidCastException($"This weapon '{value.Name}' is not a launcher.");
            }
        }
        IInput IArmBehaviour.Input { set => _input = value ?? throw new NullReferenceException(nameof(value)); }
        public bool Continuing { get => IArmBehaviour.AnyBehaviourIsContinuing(_behaviours); }

        void Awake()
        {
            _aim.OnAwake();
            _behaviours = new IArmBehaviour[]
            {
                _aim,
            };
        }
        void Start()
        {
            _aim.Target = GetComponent<ITarget>();
        }
        public void Update()
        {
            foreach (var b in _behaviours)
            {
                b.Update();
            }
        }
        public void OnAnimatorIK(int layerIndex)
        {

            foreach (var m in _behaviours)
                m.OnAnimatorIK(layerIndex);
        }

        public bool Begin()
        {
            return IArmBehaviour.TryBeginAllBehaviours(_behaviours);
        }

        public bool End()
        {
            return IArmBehaviour.TryEndAllBehaviours(_behaviours);
        }
    }
}
