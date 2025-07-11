using Assets.Tests.Scripts.Weapons;
using System;
using System.Collections.Generic;
using Tests.BT;
using Tests.Input;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arm
{
    public interface IActionCore : ITask
    {
        public IInput Input { set; }
    }
    public class ArmActions : MonoBehaviour, IArmAction, IActionCore
    {

        [SerializeField]
        WeaponCore _weaponCore;
        IArmBehaviourDefinitions _definitions;
        [SerializeField]
        MountPoint[] _mountPoints;
        [SerializeField]
        GameObject[] _weaponActionObjs;
        ActionsSelector _selector;
        public IInput Input { set => _selector.Input = value; }

        public TaskState State => _selector.State;

        public class ActionsSelector : Selector, IArmAction
        {
            IArmWeaponDefinitions _definitions;
            WeaponCore _weaponCore;

            internal ArmWeaponActions _weaponActions;
            ArmWeaponSwitching _weaponSwitching;
            public IInput Input
            {
                set
                {
                    _weaponActions.Input = value;
                    _weaponSwitching.Input = value;
                }
            }
            public ActionsSelector(IArmWeaponDefinitions definitions, WeaponCore weaponCore, MountPoint mountPoint, IList<IArmWeaponAction> weaponActions)
            {
                this._definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
                this._weaponCore = weaponCore ?? throw new ArgumentNullException(nameof(weaponCore));
                _weaponSwitching = new ArmWeaponSwitching(definitions, mountPoint, weaponCore);


                _weaponActions = new(GetWeaponActionMappings(weaponActions));

                _weaponSwitching.SwitchingEvent += (cobj, nobj) =>
                {
                    if (cobj != null)
                    {
                        var cweapon = cobj.GetComponent<IWeapon>();
                        _weaponActions.UnactivateActionBy(cweapon);
                        cobj.SetActive(false);
                    }
                    if (nobj != null)
                    {
                        var nweapon = nobj.GetComponent<IWeapon>();
                        _weaponActions.ActivateActionBy(nweapon);
                        nobj.SetActive(true);
                    }

                    return nobj;
                };

                //children = new List<ITask>() { _weaponSwitching, _weaponActions };
            }
            (string, IArmWeaponAction)[] GetWeaponActionMappings(IList<IArmWeaponAction> weaponActions)
            {

                var list = new List<(string, IArmWeaponAction)>();
                foreach (var wa in weaponActions)
                {
                    var names = GetOriginNames(wa);
                    foreach (var n in names)
                    {
                        list.Add((n, wa));
                    }
                }

                return list.ToArray();


                string[] GetOriginNames(IArmWeaponAction action)
                {
                    var type = action.Type;
                    var list = new List<string>();
                    foreach (var od in _definitions.Origins)
                    {
                        if (od.Type == type || _weaponCore.ContainsOrigin(od.Name))
                        {
                            list.Add(od.Name);
                        }
                    }
                    return list.ToArray();
                }
            }

        }
        void Awake()
        {
            //_definitions = GetComponent<IArmBehaviourDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IArmBehaviourDefinitions));

            //var list = new List<IArmWeaponAction>();
            //foreach (var obj in _weaponActionObjs)
            //{
            //    var action = obj.GetComponent<IArmWeaponAction>() ?? throw new ComponentCantFindException(obj, typeof(IArmWeaponAction));
            //    list.Add(action);
            //}

            //_selector = new(_definitions.Weapon, _weaponCore, FindMountPoint(_definitions.Weapon.MountPointName), list.ToArray());
        }

        void Start()
        {
            var mp = FindMountPoint(_definitions.Weapon.MountPointName);
            var n = _definitions.Weapon.Origins[0].Name;
            SetDefaultWeapon(mp, n, _weaponCore);

        }
        void SetDefaultWeapon(MountPoint weaponMountPoint, string weaponName, WeaponCore weaponCore)
        {
            if (!weaponCore.TryGetWeaponObj(weaponName, out var obj))
                throw new Exception();
            var weapon = obj.GetComponent<IWeapon>();
            _selector._weaponActions.ActivateActionBy(weapon);
            weaponMountPoint.LoadObj = obj;
        }

        public TaskState Work()
        {
            if (!enabled)
                return TaskState.Failure;
            return _selector.Work();
        }

        MountPoint FindMountPoint(string name)
        {
            foreach (var m in _mountPoints)
            {
                if (m.Name == name)
                    return m;
            }
            return null;
        }

    }
}
