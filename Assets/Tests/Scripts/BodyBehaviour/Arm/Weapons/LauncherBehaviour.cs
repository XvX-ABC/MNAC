using Assets.Tests.Scripts.Weapons;
using Mono.Cecil;
using RootMotion.FinalIK;
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
    public class ArmAimer : IArmBehaviour, IAimer
    {
        AimIK _ik;
        ITarget _target;
        bool _endabled;
        public ArmAimer(AimIK ik)
        {
            this._ik = ik ?? throw new NullReferenceException(nameof(ik));
        }

        public IInput Input { set => throw new NotImplementedException(); }

        public bool Continuing => _endabled;
        public ITarget Target { get => _target; set => _target = value; }
        public bool BEnd()
        {
            if (_target == null)
            {
                return false;
            }
            _ik.solver.IKPositionWeight = 0f;
            _endabled = true;
            return true;
        }

        public bool BStart()
        {
            if (_target == null)
            {
                return false;
            }
            _ik.solver.IKPositionWeight = 1f;
            _endabled = false;
            return true;

        }
        public void OnUpdate()
        {
            if (!_endabled)
                return;
            _ik.solver.IKPosition = _target.Position;
        }
    }
    [Serializable]
    public class LauncherBehaviour : MonoBehaviour, IArmWeaponBehaviour, IAimer
    {

        [SerializeField]
        ArmAim _aim_old;
        ArmAimer _aim;
        [SerializeField]
        ArmReloadAnimation _reloadAnimation;
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
            get => _aim_old.Target;
            set => _aim_old.Target = value;
        }
        IInput IArmBehaviour.Input { set => _input = value ?? throw new NullReferenceException(nameof(value)); }
        public bool Continuing { get => _transition.Continuing || _aim_old.Continuing; }

        protected virtual void Awake()
        {
            //_aim_old.OnAwake();
            _transition.Initialize(_aim_old, _reloadAnimation);
            var aimIk = GetComponent<AimIK>() ?? throw new ComponentCantFindException(this.gameObject, typeof(AimIK));
            _aim = new(aimIk);
        }
        protected virtual void Start()
        {
            _aim.Target = GetComponent<ITarget>();
        }
        public void Update()
        {
            if (_aim.Continuing || _transition.Continuing)
                _reloadAnimation.Play();
            else
                _reloadAnimation.Stop();

            if (_input.Reload)
            {
                _transition.Begin();
            }
            else if (_transition.Continuing && _input.Fire)
            {
                _transition.End();
            }

            _aim_old.OnUpdate();
            _transition.OnUpdate();
        }
        void LateUpdate()
        {
            _aim.OnUpdate();
        }
        public void OnAnimatorIK(int layerIndex)
        {
            _aim_old.OnAnimatorIK(layerIndex);
        }

        public bool BStart()
        {
            if (!enabled)
                enabled = true;
            return true;
        }

        public bool BEnd()
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
