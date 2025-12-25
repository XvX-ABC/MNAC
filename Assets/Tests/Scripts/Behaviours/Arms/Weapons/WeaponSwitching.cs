using System;
using Tests.Utilities.MountPoints;
using Tests.Utilities.Timeline;
using Tests.Weapons;
using UnityEngine;
using Tests.Utilities.Timeline.Events.Point;
using Tests.Weapons_New;
using Tests.Characters.Weapons;
using WeaponBackpack = Tests.Weapons_New.WeaponBackpack;
namespace Tests.Behaviours.Arms.Weapons
{

    public class WeaponSwitching
    {
        internal IArmedWeaponArmDefinitions definitions;
        MountPoint _launcherMountPoint;
        MountPoint _swordMountPoint;
        WeaponBackpack _weaponBackpack;
        [Obsolete]
        Weapons_New.WeaponCore_Obsolete _weaponCore;
        Func<WeaponDescription[], string> _selectionFunc;

        internal ITimeline timeline;
        IWeapon _currentWeapon;
        GameObject _currentWeaponObj;
        DefaultWeaponSelector _defaultSelector;
        Func<IWeapon, IWeapon, IWeapon> _switchingEvent;

        public Func<IWeapon, IWeapon, IWeapon> SwitchingEvent
        {
            get => _switchingEvent;
            set
            {
                _switchingEvent = value;
                if (value != null)
                {

                    _swordMountPoint.LoadObjChangeFunc = _launcherMountPoint.LoadObjChangeFunc = (ol, nl) =>
                      {
                          var ow = default(IWeapon);
                          var nw = default(IWeapon);
                          if (ol != null)
                          {
                              var ob = ol.Obj;
                              ow = ob.GetComponent<IWeapon>() ?? throw new ComponentCantFindException(ob, typeof(IWeapon));
                              ob.SetActive(false);
                          }
                          if (nl != null)
                          {
                              var nb = nl.Obj;
                              nw = nb.GetComponent<IWeapon>() ?? throw new ComponentCantFindException(nb, typeof(IWeapon));
                              nb.SetActive(true);
                          }
                          var w = _switchingEvent?.Invoke(ow, nw);

                          //var result = default(GameObject);
                          //if (w == ow)
                          //{
                          //    result = ob;
                          //}
                          //else
                          //{
                          //    ob?.SetActive(false);
                          //    nb?.SetActive(true);
                          //    result = nb;
                          //}
                          return nl;
                      };
                }
                else
                    _launcherMountPoint.LoadObjChangeFunc = null;
            }
        }

        internal Func<WeaponDescription[], string> selectionFunc
        {
            get => _selectionFunc;
            set
            {
                if (value != null)
                    _selectionFunc = value;
                else
                    _selectionFunc = _defaultSelector.Select;
            }
        }

        public IWeapon CurrentWeapon { get => _currentWeapon; }

        [Obsolete]
        protected internal WeaponSwitching(IArmedWeaponArmDefinitions definitions, MountPoint launcherMountPoint, MountPoint swordMountPoint, Weapons_New.WeaponCore_Obsolete weaponCore, Func<WeaponDescription[], string> selectionFunc = null)
        {
            this.definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _launcherMountPoint = launcherMountPoint ?? throw new ArgumentNullException(nameof(launcherMountPoint));
            _swordMountPoint = swordMountPoint ?? throw new ArgumentNullException(nameof(swordMountPoint));
            timeline = new Timeline_V1(this.definitions.Switching.DurationTime);
            timeline.AddPointEvent(this.definitions.Switching.MountedProportion, ChangeWeapon);


            _weaponCore = weaponCore ?? throw new NullReferenceException(nameof(weaponCore));


            //foreach (var origin in definitions.Origins)
            //{
            //    if (!_weaponCore.ContainsOrigin(origin.Name))
            //        throw new WeaponNotContainsException(_weaponCore, origin.Name);
            //}


            if (selectionFunc == null)
            {
                _defaultSelector = new();
                _selectionFunc = _defaultSelector.Select;
            }
            else
                _selectionFunc = selectionFunc;
        }

        internal WeaponSwitching(IArmedWeaponArmDefinitions definitions, MountPoint launcherMountPoint, MountPoint swordMountPoint, WeaponBackpack weaponBackpack, Func<WeaponDescription[], string> selectionFunc = null)
        {
            this.definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _launcherMountPoint = launcherMountPoint ?? throw new ArgumentNullException(nameof(launcherMountPoint));
            _swordMountPoint = swordMountPoint ?? throw new ArgumentNullException(nameof(swordMountPoint));
            timeline = new Timeline_V1(this.definitions.Switching.DurationTime);
            timeline.AddPointEvent(this.definitions.Switching.MountedProportion, ChangeWeapon);


            _weaponBackpack = weaponBackpack ?? throw new NullReferenceException(nameof(weaponBackpack));


            //foreach (var origin in definitions.Origins)
            //{
            //    if (!_weaponCore.ContainsOrigin(origin.Name))
            //        throw new WeaponNotContainsException(_weaponCore, origin.Name);
            //}


            if (selectionFunc == null)
            {
                _defaultSelector = new();
                _selectionFunc = _defaultSelector.Select;
            }
            else
                _selectionFunc = selectionFunc;
        }
        public void SetDefaultWeapon()
        {
            ChangeWeapon(default);
        }
        protected void ChangeWeapon(TimelineContext _)
        {
            var newWeapon = GetWeapon();
            if (newWeapon == _currentWeapon)
                return;
            var newWeaponObj = newWeapon.Obj;
            var type = newWeapon.Type;
            switch (type)
            {
                case Weapons_New.WeaponType.Launcher:
                    _launcherMountPoint.Load = newWeaponObj.GetComponent<ILoad>() ?? throw new ComponentCantFindException(newWeaponObj, typeof(ILoad));
                    _swordMountPoint.Load = null;
                    break;
                case Weapons_New.WeaponType.Sword:
                    _swordMountPoint.Load = newWeaponObj.GetComponent<ILoad>() ?? throw new ComponentCantFindException(newWeaponObj, typeof(ILoad));
                    _launcherMountPoint.Load = null;
                    break;
            }
            if (_currentWeapon != null)
                PutBackWeapons(_currentWeapon);
            _currentWeapon = newWeapon;
        }
        [Obsolete]
        protected IWeapon GetWeapon_Obsolete()
        {
            var name = _selectionFunc(definitions.Origins);
            if (!_weaponCore.TryGetWeapon(name, out var weapon))
                throw new WeaponObjGetFailedByName(name);
            return weapon;
        }
        internal IWeapon GetWeapon()
        {
            var name = _selectionFunc(definitions.Origins);
            var weapon = _weaponBackpack.GetWeapon(name);
            if (weapon == null)
                throw new WeaponObjGetFailedByName(name);
            return weapon;
        }
        internal void PutBackWeapons(IWeapon weapon)
        {
            _weaponBackpack.PutWeapon(weapon.Name, weapon);
        }
        public void Begin()
        {
            timeline.Restart();
        }
        public void Update()
        {
            timeline.OnUpdate(Time.deltaTime);
        }
        //public void End()
        //{
        //    timeline.End();
        //    timeline.Reset();
        //}
        public void End()
        {
            timeline.EndEarly();
        }
    }
}
