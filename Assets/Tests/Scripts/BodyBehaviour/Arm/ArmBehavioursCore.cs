using Assets.Tests.Scripts.Weapons;
using NUnit.Framework.Api;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Schema;
using Tests.Input;
using Tests.States;
using Tests.Weapons;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Tests.BodyBehaviour.Arm
{
    [RequireComponent(typeof(ArmBehaviourDefinitions))]
    public class ArmBehavioursCore : MonoBehaviour, IArmBehaviour
    {

        [SerializeField]
        WeaponCore _weaponCore;
        [SerializeField]
        HybridInput _input;
        [SerializeField]
        GameObject[] _weaponBehaviourObjs;
        [SerializeField]
        MountPoint[] _mountPoints;

        IArmBehaviourDefinitions _definitions;

        IArmWeaponBehaviour[] _behaviours;


        ArmWeaponSwitching _weaponSwitching;
        ArmWeaponBehaviours _weaponBehaviours;

        StateMachine<object> _stateMachine;
        IInput IArmBehaviour.Input
        {
            set
            {
                foreach (var b in _behaviours)
                    b.Input = value;
            }
        }

        bool IArmBehaviour.Continuing => IArmBehaviour.AnyBehaviourIsContinuing(_behaviours);

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


            _weaponSwitching = new ArmWeaponSwitching(weaponDefinitions, weaponMountPoint, _weaponCore);



            var length = _weaponBehaviourObjs.Length;
            _behaviours = new IArmWeaponBehaviour[length];
            for (int i = 0; i < length; i++)
            {
                _behaviours[i] = _weaponBehaviourObjs[i].GetComponent<IArmWeaponBehaviour>() ?? throw new ComponentCantFindException(_weaponBehaviourObjs[i], typeof(IArmWeaponBehaviour));
            }



            var l = new List<(string, IArmWeaponBehaviour)>();
            foreach (var od in _definitions.Weapon.Origins)
            {
                var n = od.Name;
                var t = od.Type;
                foreach (var b in _behaviours)
                {
                    if (b.Type == t)
                    {
                        l.Add((n, b));
                        break;
                    }
                }
            }
            _weaponBehaviours = new ArmWeaponBehaviours(l.ToArray());


            _weaponBehaviours.Input = _input;


            _weaponSwitching.WeaponSwitchingFunc += (cobj, nobj) =>
            {
                if (cobj != null)
                {
                    var cweapon = cobj.GetComponent<IWeapon>();
                    _weaponBehaviours.UnactivateBehaviourBy(cweapon);
                    cobj.SetActive(false);
                }
                if (nobj != null)
                {
                    var nweapon = nobj.GetComponent<IWeapon>();
                    _weaponBehaviours.ActivateBehaviourBy(nweapon);
                    nobj.SetActive(true);
                }

                return nobj;
            };

            _stateMachine = new();
            var switchingState = new ArmBehaviourState("switching", _weaponSwitching);
            var behavioursState = new ArmBehaviourState("behaviours", _weaponBehaviours);
            var estate = new EmptyState<object>();
            _stateMachine.AddState(behavioursState);
            _stateMachine.AddState(switchingState);
            _stateMachine.AddState(estate);
            _stateMachine.AddTransitionFor(behavioursState, () => _input.Supply && _weaponBehaviours.BEnd(), switchingState);
            _stateMachine.AddTransitionFor(switchingState, () => (!_weaponSwitching.Continuing || (_input.Fire && _weaponSwitching.BEnd())) && _weaponBehaviours.BStart(), behavioursState);
            _stateMachine.AddTransitionFor(behavioursState, () => UInput.GetKeyDown(KeyCode.S), estate);
        }
        private void Start()
        {
            var mp = FindMountPoint(_definitions.Weapon.MountPointName);
            var n = _definitions.Weapon.Origins[0].Name;
            SetDefaultWeapon(mp, n, _weaponCore);
        }

        void SetDefaultWeapon(MountPoint weaponMountPoint, string weaponName, WeaponCore weaponCore)
        {
            if (!weaponCore.TryGetWeaponObj(weaponName, out var obj))
                throw new Exception();
            weaponMountPoint.LoadObj = obj;
        }

        public void OnUpdate()
        {

            //if (_input.Supply)
            //{
            //    if (_weaponBehaviours.Continuing && !_weaponBehaviours.BEnd())
            //        throw new Exception("Try to end weapon behaviours failed.");
            //    if (!_weaponSwitching.Continuing && !_weaponSwitching.BStart())
            //        throw new Exception("Try to start weapon switching failed.");
            //}
            //_weaponSwitching.OnUpdate();
            //_weaponBehaviours.OnUpdate();
            if (UInput.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Debug point");
            }
            _stateMachine.OnUpdate();

        }
        public void OnAnimatorIK(int layerIndex)
        {

            _weaponSwitching.OnAnimatorIK(layerIndex);
            _weaponBehaviours.OnAnimatorIK(layerIndex);
        }

        bool IArmBehaviour.BStart()
        {
            enabled = true;
            return IArmBehaviour.TryBeginAllBehaviours(_behaviours);
        }

        bool IArmBehaviour.BEnd()
        {
            enabled = false;
            return IArmBehaviour.TryEndAllBehaviours(_behaviours);
        }
    }
}
