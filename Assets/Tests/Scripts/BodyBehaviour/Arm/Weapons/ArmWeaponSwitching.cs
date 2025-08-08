using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Tests.Scripts.Weapons;
using System;
using Tests.BodyBehaviour.Arm.Animations;
using Tests.BT;
using Tests.Characters;
using Tests.Input;
using Tests.States;
using Tests.Utilities.MTrees;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arm
{
    public class ArmWeaponSwitching : ArmBehaviourPlayableState
    {
        IArmWeaponDefinitions _definitions;
        MountPoint _mountPoint;
        WeaponCore _weaponCore;
        Func<WeaponDescription[], string> _selectionFunc;


        GameObject _weaponObj;
        DefaultWeaponSelector _defaultSelector;
        Func<IWeapon, IWeapon, IWeapon> _switchingEvent;

        ArmAnimationCore_New _animationCore;
        public Func<WeaponDescription[], string> SelectionFunc
        {
            get => _selectionFunc;
            set
            {
                if (value == null)
                    throw new NullReferenceException(nameof(SelectionFunc));
                _selectionFunc = value;
            }
        }
        //public Func<GameObject, GameObject, GameObject> SwitchingEvent { get => _mountPoint.LoadObjChangeFunc; set => _mountPoint.LoadObjChangeFunc = value; }
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

        internal ArmAnimationCore_New AnimationCore { get => _animationCore; set => _animationCore = value; }

        public ArmWeaponSwitching(IArmWeaponDefinitions definitions, MountPoint mountPoint, WeaponCore weaponCore, Func<WeaponDescription[], string> selectionFunc) : base("switching", definitions.SwitchingDurationTime)
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
            if (_animationCore != null)
                _animationCore.StatusNum = 0;
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

    }
}
