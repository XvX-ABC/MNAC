using System;
using Tests.Utilities.MountPoints;
using Tests.Utilities.Timeline;
using Tests.Weapons;
using UnityEngine;
using Tests.Utilities.Timeline.Events.Point;
namespace Tests.Behaviours.Arms.Weapons
{

    public class WeaponSwitching
    {
        internal IArmedWeaponArmDefinitions definitions;
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
                    _mountPoint.LoadObjChangeFunc = (ol, nl) =>
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
                    _mountPoint.LoadObjChangeFunc = null;
            }
        }


        protected internal WeaponSwitching(IArmedWeaponArmDefinitions definitions, MountPoint mountPoint, WeaponCore weaponCore, Func<WeaponDescription[], string> selectionFunc = null)
        {
            this.definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _mountPoint = mountPoint ?? throw new ArgumentNullException(nameof(mountPoint));
            timeline = new Timeline_V1(this.definitions.SwitchingDurationTime);
            timeline.AddPointEvent(this.definitions.SwitchingMountedProportion, ChangeWeapon);


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
        protected void ChangeWeapon(TimelineContext _)
        {
            _weaponObj = GetWeaponObj();
            _mountPoint.Load = _weaponObj.GetComponent<ILoad>() ?? throw new ComponentCantFindException(_weaponObj, typeof(ILoad));
        }
        protected GameObject GetWeaponObj()
        {
            var name = _selectionFunc(definitions.Origins);
            if (!_weaponCore.TryGetWeaponObj(name, out var obj))
                throw new WeaponObjGetFailedByName(name);
            return obj;
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
