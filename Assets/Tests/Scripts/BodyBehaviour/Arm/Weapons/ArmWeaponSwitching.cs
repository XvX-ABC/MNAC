using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Tests.Scripts.Weapons;
using System;
using Tests.Input;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm
{
    public class ArmWeaponSwitching : IArmBehaviour
    {
        IArmWeaponDefinitions _definitions;
        MountPoint _mountPoint;
        WeaponCore _weaponCore;
        Func<ArmWeaponDescription[], string> _selectionFunc;


        ITimeline _timeline;
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
        public Func<GameObject, GameObject, GameObject> WeaponSwitchingFunc { get => _mountPoint.LoadObjChangeFunc; set => _mountPoint.LoadObjChangeFunc = value; }
        IInput IArmBehaviour.Input { set => throw new NotImplementedException(); }
        public bool Continuing { get => _timeline.IsRunning; }
        public ArmWeaponSwitching(IArmWeaponDefinitions definitions, MountPoint mountPoint, WeaponCore weaponCore, Func<ArmWeaponDescription[], string> selectionFunc)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _mountPoint = mountPoint ?? throw new ArgumentNullException(nameof(mountPoint));
            _timeline = new Timeline(_definitions.SwitchingDurationTime);
            _timeline.AddPointEvent(_definitions.SwitchingMountedProportion, _ =>
            {
                Debug.Log("change the weapon obj");
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
                throw new GetWeaponObjByNameFailedException(name);
            return obj;
        }



        public bool Begin()
        {

            if (_timeline.IsRunning)
            {
                Debug.LogWarning("This weapon switching behaviour is still continuing");
                return false;
            }

            _timeline.Start();
            return true;
        }


        public bool End()
        {
            _timeline.Stop();
            return true;
        }


        public void OnUpdate()
        {
            if (_timeline.IsRunning)
                _timeline.OnUpdate(Time.deltaTime);
        }
        public void OnAnimatorIK(int layerIndex) { }
    }
}
