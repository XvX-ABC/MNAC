using Assets.Tests.Scripts.Weapons;
using Mono.Cecil;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Tests.Input;
using Tests.Locomotion;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using static UnityEngine.Rendering.DebugUI;
using Debug = UnityEngine.Debug;


namespace Tests.BodyBehaviour.Arm
{
    [Serializable]
    public class LauncherBehaviour : MonoBehaviour, IArmWeaponBehaviour, IAimer
    {

        [SerializeField]
        ArmAim _aim;
        [SerializeField]
        ArmReload _reload;
        [SerializeField]
        AimToReloadTransition _transition;

        IInput _input;
        ILauncher _launcher;
        public WeaponType Type => WeaponType.Launcher;
        public virtual IWeapon Weapon
        {
            get => _launcher;
            set
            {
                if (value is ILauncher launcher)
                {
                    _launcher = launcher;
                    _transition.Target = launcher;
                }
                else
                    throw new InvalidCastException($"This weapon '{value.Name}' is not a launcher.");
            }
        }
        public virtual ITarget Target
        {
            get => _aim.Target;
            set => _aim.Target = value;
        }
        IInput IArmBehaviour.Input { set => _input = value ?? throw new NullReferenceException(nameof(value)); }
        public bool Continuing { get => _transition.Continuing || _aim.Continuing; }

        protected virtual void Awake()
        {
            _aim.OnAwake();
            _transition.Initialize(_aim,_reload);
        }
        protected virtual void Start()
        {
            _aim.Target = GetComponent<ITarget>();
        }
        public void Update()
        {
            if (_input.Reload)
            {
                _transition.Begin();
            }
            else if (_transition.Continuing && _input.Fire)
            {
                _transition.End();
            }

            _aim.OnUpdate();
            _transition.OnUpdate();
        }
        public void OnAnimatorIK(int layerIndex)
        {
            _aim.OnAnimatorIK(layerIndex);
        }

        public bool Begin()
        {
            if (!enabled)
                enabled = true;
            return true;
        }

        public bool End()
        {
            if (enabled)
                enabled = false;
            if (_transition.Continuing)
                _transition.End();
            return true;
        }
        void OnDrawGizmos()
        {
            //_aim.OnDrawGizmos();
        }
    }
}
