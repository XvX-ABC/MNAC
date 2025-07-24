using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using Assets.Tests.Scripts.BDExtensions.Variables;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.BT;
using Tests.Weapons.Launcher;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace Tests.Behaviours.Arm.Weapons
{
    [Serializable]
    [Obsolete]
    public class ArmAimingAndReloadTransition : Action
    {
        [SerializeField]
        AnimationCurve _curve;
        [SerializeField]
        float _duration;
        [SerializeField, Range(0, 1)]
        float _offset;
        //internal ArmAimer _aimer;
        [SerializeField]
        SharedGameObject _owner;
        [SerializeField]
        SharedGameObject _weaponObj;
        [SerializeField]
        ArmReloadAnimation _reloadAnimation;
        [SerializeField]
        ArmAimerAction _aimer;
        ITimeline _t0;
        ITimeline _t1;
        ITimeline _reloadTimeline;
        ILauncher _launcher;
        //internal ILauncher Launcher
        //{
        //    get => _launcher;
        //    set
        //    {
        //        _launcher = value;
        //        _reloadTimeline = value?.ReloadTimeline;
        //        if (_reloadTimeline != null)
        //        {
        //            _reloadTimeline.AddPointEvent(1 - _offset, _ =>
        //            {
        //                _t1.Start();
        //            });
        //            _reloadTimeline.AddPointEvent(0, _ =>
        //            {
        //                _reloadAnimation.Continue();
        //            });
        //            _reloadAnimation.DurationTime = _reloadTimeline.Length;

        //        }
        //    }
        //}
        //internal void Initialize(ArmAimer aimer, ArmReloadAnimation reloadAnimation)
        //{
        //    this._aimer = aimer ?? throw new ArgumentNullException(nameof(aimer));
        //    this._reloadAnimation = reloadAnimation ?? throw new ArgumentNullException(nameof(reloadAnimation));
        //    InitializeTimelines();
        //}
        void InitializeTimelines()
        {
            _t0 = new Timeline(_duration);
            _t0.AddRangeEvent(0, 1, ctx =>
            {
                var weight = _curve.Evaluate(ctx.Proportion);
                _aimer.Weight = weight;
            });
            _t0.AddPointEvent(0, _ =>
            {
                _reloadAnimation.Play();
                _reloadAnimation.Pause();
            });
            _t0.AddPointEvent(1 - _offset, _ =>
            {
                _launcher.StartReload();
            });
            //_t0.AddPointEvent(1, _ =>
            //{
            //    _aimer.BEnd();

            //});

            _t1 = new Timeline(_duration);
            _t1.AddRangeEvent(0, 1, ctx =>
            {
                var weight = _curve.Evaluate(1 - ctx
                    .Proportion);
                _aimer.Weight = weight;
            });
            _t1.AddPointEvent(0, _ =>
            {
                //_aimer.BStart();
                _reloadAnimation.Pause();
                _reloadAnimation.Stop();
            });
        }
        public bool Continuing => _t0.IsRunning || _t1.IsRunning || _reloadTimeline?.IsRunning == true;
        public bool BStart()
        {
            if (Continuing)
                return false;
            _t0.Start();
            return true;
        }
        public bool BEnd()
        {
            if (_t0.IsRunning)
                _t0.Stop();
            if (_reloadTimeline.IsRunning)
                _launcher.EndReload();
            if (_t1.IsRunning)
                _t1.Stop();

            //if (!_aimer.Continuing)
            //    _aimer.BStart();
            _aimer.Weight = 1;
            return true;

        }
        //public void OnUpdate()
        //{
        //    _t0.OnUpdate(Time.deltaTime);
        //    _t1.OnUpdate(Time.deltaTime);
        //}
        void InitializeWhenWeaponChanged()
        {
            _reloadTimeline = _launcher?.ReloadTimeline;
            if (_reloadTimeline != null)
            {
                _reloadTimeline.AddPointEvent(1 - _offset, _ =>
                {
                    _t1.Start();
                });
                _reloadTimeline.AddPointEvent(0, _ =>
                {
                    _reloadAnimation.Continue();
                });
                _reloadAnimation.DurationTime = _reloadTimeline.Length;
            }
        }
        public override void OnAwake()
        {
            InitializeTimelines();
        }
        public override void OnStart()
        {
            _t0.Start();
            _launcher = _weaponObj.Value.GetComponent<ILauncher>();
            InitializeWhenWeaponChanged();
        }
        public override void OnEnd()
        {
            if (_t0.IsRunning)
                _t0.Stop();
            if (_reloadTimeline.IsRunning)
                _launcher.EndReload();
            if (_t1.IsRunning)
                _t1.Stop();

            //if (!_aimer.Continuing)
            //    _aimer.BStart();
            _aimer.Weight = 1;
        }
        //protected override TaskState OnWork()
        public override TaskStatus OnUpdate()
        {
            _t0.OnUpdate(Time.deltaTime);
            _t1.OnUpdate(Time.deltaTime);
            if (Continuing)
            {
                _t0.Start();
                return TaskStatus.Running;
            }
            return TaskStatus.Failure;
        }
    }
}
