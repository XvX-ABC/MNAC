using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Tests.Scripts.Weapons;
using System;
using Tests.BT;
using Tests.Input;
using Tests.States;
using UnityEngine;

namespace Tests.Behaviours.Arm
{
    public abstract class ArmBehaviorState : StateBase, IArmBehaviour
    {
        protected ArmBehaviorState(string name) : base($"arm_{name}")
        {
        }
        Action _entryAction;
        Action _updateAction;
        Action _exitAction;
        public abstract IInput Input { set; }
        public abstract bool Continuing { get; }
        public Action EntryAction { get => _entryAction; set => _entryAction = value; }
        public Action UpdateAction { get => _updateAction; set => _updateAction = value; }
        public Action ExitAction { get => _exitAction; set => _exitAction = value; }
        public override void OnEnter()
        {
            _entryAction?.Invoke();
        }
        public override void OnExit()
        {
            _exitAction?.Invoke();
        }
        public override void OnUpdate()
        {
            _updateAction?.Invoke();
        }
    }



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
        //public override void OnStop()
        //{
        //    _timeline.Stop();
        //}
        //protected override TaskState OnWork()
        //{
        //    if (_timeline.IsRunning)
        //    {
        //        _timeline.OnUpdate(Time.deltaTime);
        //        return TaskState.Running;
        //    }
        //    else if (input.Supply)
        //    {
        //        _timeline.Start();
        //        return TaskState.Running;
        //    }
        //    return TaskState.Failure;
        //}
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
