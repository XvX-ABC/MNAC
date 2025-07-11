using Assets.Tests.Scripts.BodyBehaviour.Arm.Animations;
using Assets.Tests.Scripts.Weapons;
using NUnit.Framework.Api;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Schema;
using Tests.BodyBehaviour.Arm.Animations;
using Tests.Input;
using Tests.States;
using Tests.Weapons;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using static Tests.BodyBehaviour.Arm.Animations.ArmWeaponBehavioursAnimator;
using UInput = UnityEngine.Input;

namespace Tests.Behaviours.Arm
{

    [RequireComponent(typeof(ArmBehaviourDefinitions))]
    public class ArmBehavioursCore : MonoStateBase, IArmBehaviour
    {
        public static implicit operator StateBase<object>(ArmBehavioursCore core)
        {
            return core._stateMachine;
        }
        [SerializeField]
        Animator _animator;
        [SerializeField]
        WeaponCore _weaponCore;
        [SerializeField]
        HybridInput _input;
        //[SerializeField]
        //GameObject[] _weaponBehaviourObjs;
        [SerializeField]
        MountPoint[] _mountPoints;

        IArmBehaviourDefinitions _definitions;

        //IArmWeaponBehaviour[] _behaviours;


        internal ArmWeaponSwitching weaponSwitching;
        internal ArmWeaponHoldingBehaviours holdingBehaviours;
        internal ArmWeaponAnimator weaponAnimator;
        PlayableStateMachine _stateMachine;
        IInput IArmBehaviour.Input
        {
            set
            {
                weaponSwitching.Input = value;
                holdingBehaviours.Input = value;
            }
        }

        bool IArmBehaviour.Continuing => false;

        public MountPoint FindMountPoint(string name)
        {
            foreach (var m in _mountPoints)
            {
                if (m.Name == name)
                    return m;
            }
            return null;
        }

        private void Awake()
        {
            _definitions = GetComponent<ArmBehaviourDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ArmBehaviourDefinitions));
            var weaponDefinitions = _definitions.Weapon;
            var weaponMountPoint = FindMountPoint(weaponDefinitions.MountPointName) ?? throw new CantFindMountPointByNameException(weaponDefinitions.MountPointName);


            weaponSwitching = new ArmWeaponSwitching(weaponDefinitions, weaponMountPoint, _weaponCore);
            weaponSwitching.ExitWhenEnd = true;


            var alist = new List<(string, IArmWeaponHoldingBehavioursAnimator)>();
            var blist = new List<(string, IArmWeaponHoldingBehaviour)>();
            foreach (var od in _definitions.Weapon.Origins)
            {
                if (!_weaponCore.TryGetWeaponOrigin(od.Name, out var origin))
                {
                    Debug.LogWarning(new WeaponOriginNotContainsException(_weaponCore, od.Name));
                    continue;
                }
                var behaviour = origin.GetComponent<IArmWeaponHoldingBehaviour>();
                var animator = behaviour.Animator;
                if (behaviour == null)
                {
                    Debug.LogWarning(new ComponentCantFindException(origin, typeof(IArmWeaponHoldingBehaviour)));
                    continue;
                }
                blist.Add((od.Name, behaviour));
            }
            holdingBehaviours = new ArmWeaponHoldingBehaviours(this.gameObject, blist.ToArray());



            holdingBehaviours.Input = _input;


            weaponSwitching.SwitchingEvent += (cobj, nobj) =>
            {
                if (cobj != null)
                {
                    var cweapon = cobj.GetComponent<IWeapon>();
                    holdingBehaviours.UnactivateBehaviourBy(cweapon);
                    cobj.SetActive(false);
                }
                if (nobj != null)
                {
                    var nweapon = nobj.GetComponent<IWeapon>();
                    holdingBehaviours.ActivateBehaviourBy(nweapon);
                    nobj.SetActive(true);
                }

                return nobj;
            };

            _stateMachine = new(this.name);
            _stateMachine.AddState(holdingBehaviours);
            _stateMachine.AddState(weaponSwitching);
            //_stateMachine.AddTransitionFor(holdingBehaviours, () => _input.Supply, weaponSwitching);
            //_stateMachine.AddTransitionFor(weaponSwitching, () => weaponSwitching.NormalizedTime >= 1, holdingBehaviours);
            _stateMachine.AddTransitionFor(weaponSwitching, holdingBehaviours, 1, () => _input.Supply, (s, d, t) => { });
            weaponAnimator = new(this, _animator, GetComponent<IArmAnimationDefinitions>(), alist.ToArray());
        }
        private void Start()
        {
            var mp = FindMountPoint(_definitions.Weapon.MountPointName);
            var n = _definitions.Weapon.Origins[0].Name;
            SetDefaultWeapon(mp, n, _weaponCore);
            this.OnEnter();
        }

        void SetDefaultWeapon(MountPoint weaponMountPoint, string weaponName, WeaponCore weaponCore)
        {
            if (!weaponCore.TryGetWeaponObj(weaponName, out var obj))
                throw new Exception();
            var weapon = obj.GetComponent<IWeapon>();
            holdingBehaviours.ActivateBehaviourBy(weapon);
            weaponMountPoint.LoadObj = obj;
        }

        public override void OnUpdate()
        {
            _stateMachine.OnUpdate();
            weaponAnimator.OnUpdate();
        }
        public void OnAnimatorIK(int layerIndex)
        {

            weaponSwitching.OnAnimatorIK(layerIndex);
            holdingBehaviours.OnAnimatorIK(layerIndex);
        }

        public override void OnEnter()
        {
            enabled = true;
            //IArmBehaviour.TryBeginAllBehaviours(_behaviours);
        }

        public override void OnExit()
        {
            enabled = false;
            //IArmBehaviour.TryEndAllBehaviours(_behaviours);
        }
        void Update()
        {
            this.OnUpdate();
        }
    }
}
