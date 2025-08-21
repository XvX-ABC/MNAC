using System;
using Tests.Weapons;
using UnityEngine;
using Utilities.Timeline;
using Utilities.Timeline.Events.Point;

namespace Tests.Behaviours.Arms.Weapons
{
  
    public class WeaponSwitching
    {
        internal IArmWeaponDefinitions definitions;
        MountPoint _mountPoint;
        WeaponCore _weaponCore;
        Func<WeaponDescription[], string> _selectionFunc;

        internal ITimeline timeline;
        GameObject _weaponObj;
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
                    _mountPoint.LoadObjChangeFunc = (ob, nb) =>
                    {
                        var ow = default(IWeapon);
                        var nw = default(IWeapon);
                        if (ob != null)
                        {
                            ow = ob.GetComponent<IWeapon>() ?? throw new ComponentCantFindException(ob, typeof(IWeapon));
                        }
                        if (nb != null)
                        {
                            nw = nb.GetComponent<IWeapon>() ?? throw new ComponentCantFindException(nb, typeof(IWeapon));
                        }
                        var w = _switchingEvent?.Invoke(ow, nw);

                        var result = default(GameObject);
                        if (w == ow)
                        {
                            result = ob;
                        }
                        else
                        {
                            ob?.SetActive(false);
                            nb?.SetActive(true);
                            result = nb;
                        }
                        return result;
                    };
                }
                else
                    _mountPoint.LoadObjChangeFunc = null;
            }
        }


        protected internal WeaponSwitching(IArmWeaponDefinitions definitions, MountPoint mountPoint, WeaponCore weaponCore, Func<WeaponDescription[], string> selectionFunc = null)
        {
            this.definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _mountPoint = mountPoint ?? throw new ArgumentNullException(nameof(mountPoint));
            timeline = new Timeline(this.definitions.SwitchingDurationTime);
            timeline.AddPointEvent(this.definitions.SwitchingMountedProportion, _ =>
            {
                _weaponObj = GetWeaponObj();
                _mountPoint.LoadObj = _weaponObj;
            });


            _weaponCore = weaponCore ?? throw new NullReferenceException(nameof(weaponCore));


            foreach (var origin in definitions.Origins)
            {
                if (!_weaponCore.ContainsOrigin(origin.Name))
                    throw new WeaponNotContainsException(_weaponCore, origin.Name);
            }


            if (selectionFunc == null)
            {
                _defaultSelector = new();
                _selectionFunc = _defaultSelector.Select;
            }
            else
                _selectionFunc = selectionFunc;
        }

        protected GameObject GetWeaponObj()
        {
            var name = _selectionFunc(definitions.Origins);
            if (!_weaponCore.TryGetWeaponObj(name, out var obj))
                throw new WeaponObjGetFailedByName(name);
            return obj;
        }
    }
}
