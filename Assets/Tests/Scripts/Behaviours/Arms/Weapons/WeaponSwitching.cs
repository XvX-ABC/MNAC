using System;
using Tests.Characters.Humanoid.Arms;
using Tests.Utilities.MountPoints;
using Tests.Utilities.Timeline;
using Tests.Utilities.Timeline.Events.Point;
using Tests.Weapons;
using Tests.Weapons_New;
using UnityEngine;
using MountPoint = Tests.Utilities.MountPoints.MountPoint;
using WeaponBackpack = Tests.Characters.Weapons.WeaponBackpack;
namespace Tests.Behaviours.Arms.Weapons
{

    public class WeaponSwitching
    {
        internal IArmedWeaponArmDefinitions definitions;
        MountPoint _launcherMountPoint;
        MountPoint _swordMountPoint;
        WeaponBackpack _weaponBackpack;
        Func<WeaponDescription[], string> _selectionFunc;

        internal ITimeline timeline;
        IWeapon _currentWeapon;
        DefaultWeaponSelector _defaultSelector;
        Action<IWeapon, IWeapon> _switchedEvent_new;
        ArmController _ownerArmController;

        public Action<IWeapon, IWeapon> SwitchedEvent
        {
            get => _switchedEvent_new;
            set
            {
                _switchedEvent_new = value;
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


        internal WeaponSwitching(ArmController ownerArmController, IArmedWeaponArmDefinitions definitions, MountPoint launcherMountPoint, MountPoint swordMountPoint, WeaponBackpack weaponBackpack, Func<WeaponDescription[], string> selectionFunc = null)
        {
            _ownerArmController = ownerArmController ?? throw new ArgumentNullException(nameof(ownerArmController));
            this.definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _launcherMountPoint = launcherMountPoint ?? throw new ArgumentNullException(nameof(launcherMountPoint));
            _swordMountPoint = swordMountPoint ?? throw new ArgumentNullException(nameof(swordMountPoint));
            timeline = new Timeline_V1(this.definitions.Switching.DurationTime);
            timeline.AddPointEvent(this.definitions.Switching.MountedProportion, ChangeWeapon);


            _weaponBackpack = weaponBackpack ?? throw new NullReferenceException(nameof(weaponBackpack));

            if (selectionFunc == null)
            {
                _defaultSelector = new();
                _selectionFunc = _defaultSelector.Select;
            }
            else
                _selectionFunc = selectionFunc;


            _swordMountPoint.LoadObjChangeFunc = _launcherMountPoint.LoadObjChangeFunc = (ol, nl) =>
            {
                if (ol != null)
                {
                    var ob = ol.Obj;
                    ob.SetActive(false);
                }
                if (nl != null)
                {
                    var nb = nl.Obj;
                    nb.SetActive(true);
                }
                return nl;
            };
        }


        public void SetDefaultWeapon()
        {
            ChangeWeapon(default);
        }
        protected void ChangeWeapon(TimelineContext _)
        {
            var name = _selectionFunc(definitions.Origins);
            if (_currentWeapon != null)
            {
                if (name == _currentWeapon.Name)
                {
                    _switchedEvent_new?.Invoke(_currentWeapon, _currentWeapon);
                    return;
                }
                PutBackWeapons(_currentWeapon);
            }
            var newWeapon = GetWeaponBy(name);
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

            _switchedEvent_new?.Invoke(_currentWeapon, newWeapon);
            _currentWeapon = newWeapon;
        }
        internal IWeapon GetWeaponBy(string name)
        {
            var weapon = _weaponBackpack.GetWeapon(_ownerArmController.bodyPart, name);
            if (weapon == null)
                throw new WeaponObjGetFailedByName(name);
            return weapon;
        }
        internal void PutBackWeapons(IWeapon weapon)
        {
            _weaponBackpack.PutWeapon(_ownerArmController.bodyPart, weapon.Name, weapon);
        }
        public void Begin()
        {
            timeline.Restart();
        }
        public void Update()
        {
            timeline.OnUpdate(Time.deltaTime);
        }
        public void End()
        {
            timeline.EndEarly();
        }
    }
}
