using Assets.Tests.Scripts.Weapons;
using NUnit.Framework.Api;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Schema;
using Tests.Input;
using Tests.Weapons;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm
{
    public class ArmBehaviourCore : MonoBehaviour, IArmBehaviour
    {

        [SerializeField]
        GameObject _bodyObj;
        [SerializeField]
        GameObject[] _weaponBehaviourObjs;
        //[SerializeField]
        //LauncherBehaviour _launcherBehaviour;
        [SerializeField]
        MountPoint[] _mountPoints;

        IArmBehaviourDefinitions _definitions;

        IArmWeaponBehaviour[] _behaviours;

        ITarget _target;
        IInput _input;


        ArmWeaponSwitching _weaponSwitching;
        ArmWeaponBehaviours _weaponBehaviours;
        public ITarget Target
        {
            get => _target;
            set
            {
                _target = value;
            }
        }

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
            //_launcherBehaviour = new(this.gameObject, _goal, _hint);

            _definitions = GetComponent<IArmBehaviourDefinitions>();
            _input = _bodyObj.GetComponent<IInput>() ?? throw new ComponentCantFindException(_bodyObj, typeof(IInput));

            var weaponCore = _bodyObj.GetComponent<WeaponCore>() ?? throw new ComponentCantFindException(this.gameObject, typeof(WeaponCore));
            var weaponDefinitions = _definitions.Weapon;
            var weaponMountPoint = FindMountPoint(weaponDefinitions.MountPointName) ?? throw new CantFindMountPointByNameException(weaponDefinitions.MountPointName);


            _weaponSwitching = new ArmWeaponSwitching(weaponDefinitions, weaponMountPoint, weaponCore);




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


            _weaponSwitching.WeaponSwitchFunc += (cobj, nobj) =>
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

        }
        private void Start()
        {
            var mp = FindMountPoint(_definitions.Weapon.MountPointName);
            var n = _definitions.Weapon.Origins[0].Name;
            var core = _bodyObj.GetComponent<WeaponCore>();
            LoadDefaultWeapon(mp, n, core);
        }

        void LoadDefaultWeapon(MountPoint weaponMountPoint, string weaponName, WeaponCore weaponCore)
        {
            if (!weaponCore.TryGetWeaponObj(weaponName, out var obj))
                throw new Exception();
            weaponMountPoint.LoadObj = obj;

        }

        public void Update()
        {
     
            if (_input.Reload)
            {
                Debug.Log("Start reload");
                if (_weaponBehaviours.Continuing && !_weaponBehaviours.End())
                    throw new Exception("Try to end weapon behaviours failed.");
                if (!_weaponSwitching.Continuing && !_weaponSwitching.Begin())
                    throw new Exception("Try to start weapon switching failed.");
            }
            _weaponSwitching.Update();
            _weaponBehaviours.Update();
        }
        public void OnAnimatorIK(int layerIndex)
        {

            _weaponSwitching.OnAnimatorIK(layerIndex);
            _weaponBehaviours.OnAnimatorIK(layerIndex);
        }

        bool IArmBehaviour.Begin()
        {
            enabled = true;
            return true;
        }

        bool IArmBehaviour.End()
        {
            enabled = false;
            return true;
        }
    }
}
