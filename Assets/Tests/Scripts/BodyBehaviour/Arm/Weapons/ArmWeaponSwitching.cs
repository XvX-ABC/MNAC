using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Tests.Scripts.Weapons;
using System;
using Tests.Input;
using Tests.States;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm
{
    public class ArmBehaviourState : IState<object>
    {
        IArmBehaviour _behaviour;
        string _name;
        Guid _id;
        Transition<object>[] _transitions;
        public string Name => _name;

        public Guid ID => _id;

        public Transition<object>[] Transitions { get => _transitions; set => _transitions = value; }
        public object Context { set { } }

        public ArmBehaviourState(string name, IArmBehaviour behaviour)
        {
            _name = name;
            _behaviour = behaviour ?? throw new ArgumentNullException(nameof(behaviour));
            _id = Guid.NewGuid();
        }
        public void OnEnter()
        {
            _behaviour.BStart();
        }

        public void OnExit()
        {
            _behaviour.BEnd();
        }

        public void OnUpdate()
        {
            _behaviour.OnUpdate();
        }
    }
    public class ArmWeaponSwitching : IArmBehaviour
    {
        IArmWeaponDefinitions _definitions;
        MountPoint _mountPoint;
        WeaponCore _weaponCore;
        Func<ArmWeaponDescription[], string> _selectionFunc;


        ITimeline _timeline;
        GameObject _weaponObj;
        DefaultWeaponSelector _defaultSelector;

        BehaviourState _state;


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
        BehaviourState IArmBehaviour.State { get => _state; }
        public bool Continuing { get => _timeline.IsRunning; }
        public ArmWeaponSwitching(IArmWeaponDefinitions definitions, MountPoint mountPoint, WeaponCore weaponCore, Func<ArmWeaponDescription[], string> selectionFunc)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _mountPoint = mountPoint ?? throw new ArgumentNullException(nameof(mountPoint));
            _timeline = new Timeline(_definitions.SwitchingDurationTime);
            _timeline.AddPointEvent(_definitions.SwitchingMountedProportion, _ =>
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



        public bool BStart()
        {

            if (_timeline.IsRunning)
            {
                Debug.LogWarning("This weapon switching behaviour is still continuing");
                return false;
            }

            _timeline.Start();
            return true;
        }


        public bool BEnd()
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
