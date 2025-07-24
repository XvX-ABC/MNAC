using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Tests.Scripts.Weapons;
using System;
using Tests.BT;
using Tests.Characters;
using Tests.Input;
using Tests.States;
using Tests.Utilities.MTrees;
using UnityEngine;

namespace Tests.Behaviours.Arm
{
    public class ArmWeaponSwitching : ArmBehaviourPlayableState
    {
        IArmWeaponDefinitions _definitions;
        MountPoint _mountPoint;
        WeaponCore _weaponCore;
        Func<ArmWeaponDescription[], string> _selectionFunc;


        GameObject _weaponObj;
        DefaultWeaponSelector _defaultSelector;



        public Func<ArmWeaponDescription[], string> SelectionFunc
        {
            get => _selectionFunc;
            set
            {
                if (value == null)
                    throw new NullReferenceException(nameof(SelectionFunc));
                _selectionFunc = value;
            }
        }
        public Func<GameObject, GameObject, GameObject> SwitchingEvent { get => _mountPoint.LoadObjChangeFunc; set => _mountPoint.LoadObjChangeFunc = value; }
        public ArmWeaponSwitching(IArmWeaponDefinitions definitions, MountPoint mountPoint, WeaponCore weaponCore, Func<ArmWeaponDescription[], string> selectionFunc) : base("switching", definitions.SwitchingDurationTime)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _mountPoint = mountPoint ?? throw new ArgumentNullException(nameof(mountPoint));
            timeline.AddPointEvent(_definitions.SwitchingMountedProportion, _ =>
            {
                _weaponObj = GetWeaponObj();
                _mountPoint.LoadObj = _weaponObj;
            });


            _weaponCore = weaponCore ?? throw new NullReferenceException(nameof(weaponCore));


            foreach (var origin in definitions.Origins)
            {
                if (!_weaponCore.ContainsOrigin(origin.Name))
                    throw new WeaponOriginNotContainsException(_weaponCore, origin.Name);
            }


            if (selectionFunc == null)
            {
                _defaultSelector = new();
                _selectionFunc = _defaultSelector.Select;
            }
            else
                _selectionFunc = selectionFunc;

        }


        public ArmWeaponSwitching(IArmWeaponDefinitions definitions, MountPoint mountPoint, WeaponCore weaponCore) : this(definitions, mountPoint, weaponCore, null) { }


        protected GameObject GetWeaponObj()
        {
            var name = _selectionFunc(_definitions.Origins);
            if (!_weaponCore.TryGetWeaponObj(name, out var obj))
                throw new WeaponObjGetFailedByName(name);
            return obj;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            if (timeline.IsRunning)
            {
                Debug.LogWarning("This weapon switching behaviour is still continuing");
                return;
            }

            timeline.Start();
        }


        public override void OnExit()
        {
            base.OnExit();
            timeline.Stop();
        }


        public override void OnUpdate()
        {
            timeline.OnUpdate(Time.deltaTime);
        }
        public void OnAnimatorIK(int layerIndex) { }
    }
}
