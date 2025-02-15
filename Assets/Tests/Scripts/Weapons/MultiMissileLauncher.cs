using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using FoundationStone.UI.Tests.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests;
using Tests.Weapons;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    [Serializable]
    public struct PrepareLaunch
    {
        public float CoverOpenDuration;
    }
    [Serializable]
    public struct Reload
    {
        public Vector2 CoverCloseProportion;
        public Vector2 MagazineEmptyProportion;
        public Vector2 MagazineFullProportion;
        public float CoverCloseTriggerProportion;
        public float MagazineEmptyTriggerProportion;
        public float MagazineFullTriggerProportion;
    }
    [Serializable]
    public struct Cover
    {
        public float OpenOrCloseDuration;
    }
    [Serializable]
    public struct MagazineModule
    {
        public float EmptyOrFullDuration;
    }
    //public class X0_MissileLauncherDefines : IMissileLauncherDefines, ILauncherAnimatorActionDefines
    //{
    //    [SerializeField]
    //    float _coverOpenOrCloseDuration;
    //    [SerializeField]
    //    float _magazineFullOrEmptyDuration;
    //    [SerializeField]
    //    GameObject _origin;
    //    [SerializeField]
    //    Vector3 _magazinePosition;
    //    [SerializeField]
    //    Vector3 _muzzlePosition;
    //    [SerializeField]
    //    float _launchRate;
    //    [SerializeField]
    //    ushort _ammoSpareQuantity;
    //    [SerializeField]
    //    ushort _ammoQuantityInMagazine;
    //    [SerializeField]
    //    float _reloadDuration;
    //    [SerializeField]
    //    Vector2 _launchDelay_New;
    //    [SerializeField]
    //    Reload _reloadAction;

    //    public float LaunchDelay => throw new NotImplementedException();

    //    public Vector2 LaunchDelay_New => _launchDelay_New;

    //    public GameObject AmmoOrigin => _origin;

    //    public Vector3 MagazinePosition => _magazinePosition;

    //    public Vector3 MuzzlePosition => _muzzlePosition;

    //    public float LaunchRate
    //    {
    //        get => Mathf.Max(_launchRate, ReloadDuration);
    //    }

    //    public ushort AmmoTotalQuantity => (ushort)(_ammoSpareQuantity + _ammoQuantityInMagazine);

    //    public ushort AmmoSpareQuantity => _ammoSpareQuantity;

    //    public ushort AmmoQuantityInMagazine => _ammoQuantityInMagazine;

    //    public float ReloadDuration
    //    {
    //        get
    //        {
    //            return _coverOpenOrCloseDuration + _reloadDuration;
    //        }
    //    }
    //    public Reload Reload { get => _reloadAction; }
    //}
    public class MultiMissileLauncher : MonoBehaviour, IMissileLauncher
    {
        class TimelinesGroup : ITimeline
        {
            ITimeline[] _timelines;
            ILauncher[] _launchers;
            Func<ILauncher, ITimeline> _timelineGetFunc;

            internal ITimeline[] timelines
            {
                get
                {
                    if (_timelines == null)
                    {
                        _timelines = new ITimeline[_launchers.Length];
                        for (int i = 0; i < _launchers.Length; i++)
                        {
                            var l = _launchers[i];
                            var t = _timelineGetFunc(l) ?? throw new NullReferenceException($"{nameof(_launchers)}[{i}]");
                            _timelines[i] = t;
                        }
                    }
                    return _timelines;
                }
            }
            public TimelinesGroup(params ITimeline[] timelines)
            {
                for (int i = 0; i < timelines.Length; i++)
                {
                    if (timelines[i] == null)
                        throw new ArgumentNullException($"timelines[{i}]");

                }
                _timelines = timelines;
            }
            public TimelinesGroup(Func<ILauncher, ITimeline> getFunc, params ILauncher[] launchers)
            {
                if (launchers.Any(l => l == null))
                    throw new ArgumentNullException($"There have a null element in the argument '{nameof(launchers)}'");
                this._launchers = launchers;
                this._timelineGetFunc = getFunc ?? throw new NullReferenceException(nameof(getFunc));
            }

            public bool IsRunning => timelines.Any(t => t.IsRunning);

            public float Time => throw new NotImplementedException();
            public float Length
            {
                get
                {
                    if (timelines == null || timelines.Length == 0)
                        return 0;
                    return timelines[0].Length;
                }
            }
            public Action<TimelineContext> StartAction
            {
                get
                {
                    if (timelines == null || timelines.Length == 0)
                        return null;
                    return timelines[0].StartAction;
                }
                set
                {
                    foreach (var t in timelines)
                        t.StartAction = value;
                }
            }
            public Action<float> UpdateAction
            {
                get
                {
                    if (timelines == null || timelines.Length == 0)
                        return null;
                    return timelines[0].UpdateAction;
                }
                set
                {
                    foreach (var t in timelines)
                        t.UpdateAction = value;
                }
            }
            public Action<TimelineContext> EndAction
            {
                get
                {
                    if (timelines == null || timelines.Length == 0)
                        return null;
                    return timelines[0].EndAction;
                }
                set
                {
                    foreach (var t in timelines)
                        t.EndAction = value;
                }
            }

            public bool AddEvent(ITimelineEvent evt)
            {
                foreach (var t in timelines)
                    if (!t.AddEvent(evt))
                    {
                        RemoveEvent(evt);
                        return false;
                    }
                return true;
            }

            public void Continue()
            {
                foreach (var l in timelines)
                    l.Continue();
            }

            public void OnUpdate(float deltaTime)
            {
                foreach (var l in timelines)
                    l.OnUpdate(deltaTime);
            }

            public bool RemoveEvent(ITimelineEvent evt)
            {
                foreach (var t in timelines)
                    if (!t.RemoveEvent(evt))
                        return false;
                return true;
            }

            public void Start()
            {
                foreach (var l in timelines)
                    l.Start();
            }

            public void Stop()
            {
                foreach (var l in timelines)
                    l.Stop();
            }
        }
        internal IMissileLauncher[] subLaunchers;
        ushort _ammoSpareQuantity;
        ushort _ammoQuantityInMagazine;
        IMissileLauncherDefines _defines;
        Action<ILauncher> _initializationAction;
        ITarget _target;
        ILauncherActionsLock _actionsLock;
        ITimeline _reloadTimeline;
        ITimeline _delayLaunchTimeline;
        public ushort SpareCount { get => _ammoSpareQuantity; }
        public ushort MagazineCount { get => _ammoQuantityInMagazine; }
        public ILauncherDefines Defines { get => _defines; }
        IMissileLauncherDefines IMissileLauncher.Defines => _defines;
        public Action<ILauncher> InitializationAction { get => _initializationAction; set => _initializationAction = value; }
        public ITarget Target
        {
            get => _target;
            set
            {
                _target = value;
                foreach (var l in subLaunchers)
                    l.Target = value;
            }
        }


        public ITimeline ReloadTimeline { get => _reloadTimeline; }
        public ITimeline DelayLaunchTimeline { get => _delayLaunchTimeline; }
        public Action<IMissileLauncher, ITarget> TargetChangedAction
        {
            get
            {
                if (subLaunchers == null || subLaunchers.Length == 0)
                    return null;
                return subLaunchers[0].TargetChangedAction;
            }
            set
            {
                foreach (var l in subLaunchers)
                    l.TargetChangedAction = value;
            }
        }

        ILauncherActionsLock ILauncher.actionsLock => _actionsLock;

        public IMissileLauncher this[int index]
        {
            get
            {
                if (index < 0 || index >= subLaunchers.Length)
                    throw new IndexOutOfRangeException("index: " + index);
                return subLaunchers[index];
            }
            set
            {
                if (index < 0 || index >= subLaunchers.Length)
                    throw new IndexOutOfRangeException("index: " + index);
                subLaunchers[index] = value;
            }
        }

        void Awake()
        {
            var list = GetComponentsInChildren<IMissileLauncher>().ToList();
            if (list.Contains(this))
                list.Remove(this);
            subLaunchers = list.ToArray();
            _defines = GetComponent<IMissileLauncherDefines>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILauncherDefines));


            foreach (var l in subLaunchers)
                InitializeSubLauncher(l);





        }
        
        protected void InitializeSubLauncher(IMissileLauncher l)
        {
            l.InitializationAction += launcher =>
            {
                launcher.Supply(-(launcher.Defines.AmmoSpareQuantity));
                if (launcher is not IMissileLauncher mlauncher)
                    throw new Exception($"The sublaunchers of the type '{this.GetType().Name}' must to implement the interface '{typeof(IMissileLauncher).Name}'");


                var defines = mlauncher.Defines;
                if (defines.AmmoSpareQuantity == 0)
                {
                    if ((object)mlauncher is GameObject obj)
                        throw new Exception($"The parameter 'AmmoSpareQuantity' of the subluncher '{obj.name}' can't less than or equals to zero.");
                    else
                        throw new Exception($"The parameter 'AmmoSpareQuantity' of the subluncher can't less than or equals to zero.");
                }

                var reloadTimeline = mlauncher.ReloadTimeline;
                reloadTimeline.EndAction += context =>
                {
                    EndReloadForSubLauncher(mlauncher);
                };

            };
        }
        protected void Start()
        {
            _ammoSpareQuantity = (ushort)(_defines.AmmoTotalQuantity - subLaunchers.Length);


            var quantity = subLaunchers.Length;
            _ammoQuantityInMagazine = (ushort)quantity;

            //var reloadTimelines = new ITimeline[quantity];
            //var delayLaunchTimelines = new ITimeline[quantity];
            //for (int i = 0; i < quantity; i++)
            //{
            //    var l = subLaunchers[i];
            //    reloadTimelines[i] = l.ReloadTimeline;
            //    delayLaunchTimelines[i] = l.DelayLaunchTimeline;
            //}


            //_reloadTimeline = new TimelinesGroup(reloadTimelines);
            //_delayLaunchTimeline = new TimelinesGroup(delayLaunchTimelines);
            _reloadTimeline = new TimelinesGroup(l => ((IMissileLauncher)l).ReloadTimeline, subLaunchers);
            _delayLaunchTimeline = new TimelinesGroup(l => ((IMissileLauncher)l).DelayLaunchTimeline, subLaunchers);

            var actionsLocks = subLaunchers.Select(launcher => launcher.actionsLock).ToArray();
            _actionsLock = new LauncherActionsLockGroup(actionsLocks);

            _initializationAction?.Invoke(this);
        }
        protected void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0))
                Launch();
            if (Input.GetKeyDown(KeyCode.R))
            {
                var result = StartReload();
                Debug.Log("Start reload result: " + result);
            }
            if (Input.GetKeyDown(KeyCode.Space))
                Supply(10);

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (Target == null)
                    Target = GetComponent<ITarget>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ITarget));
                else
                    Target = null;
            }

        }
        public bool Launch()
        {
            if (!enabled || _ammoQuantityInMagazine <= 0)
                return false;


            foreach (var l in subLaunchers)
            {
                if (_ammoQuantityInMagazine <= 0)
                    continue;
                if (l.Launch())
                    _ammoQuantityInMagazine--;
            }
            return true;
        }

        public bool StartReload()
        {
            if (!enabled)
                return false;

            foreach (var l in subLaunchers)
            {
                if (_ammoSpareQuantity <= 0)
                    return false;

                l.Supply(1);
                _ammoSpareQuantity--;
                if (!l.StartReload())
                {
                    l.Supply(-1);
                    _ammoSpareQuantity++;
                    return false;
                }
            }
            return true;
        }
        internal bool EndReloadForSubLauncher(IMissileLauncher launcher)
        {
            if (launcher.EndReload())
            {
                _ammoQuantityInMagazine++;
                return true;
            }
            launcher.Supply(-1);
            _ammoSpareQuantity++;
            return false;
        }
        public bool EndReload()
        {
            if (!enabled)
                return false;
            foreach (var l in subLaunchers)
            {
                EndReloadForSubLauncher(l);
            }
            return true;
        }

        public int Supply(int num)
        {
            if (_ammoSpareQuantity + num < 0)
                return 0;
            var suppNum = Mathf.Min(_defines.AmmoTotalQuantity - subLaunchers.Length - _ammoSpareQuantity, num);
            _ammoSpareQuantity += (ushort)suppNum;
            return suppNum;
        }

        void OnGUI()
        {
            var style = new GUIStyle();
            style.fontSize = 32;
            style.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0, 0, 100, 100), _ammoSpareQuantity.ToString(), style);
            GUI.Label(new Rect(0, 100, 100, 100), _ammoQuantityInMagazine.ToString(), style);
            if (Target == null)
                GUI.Label(new Rect(0, 200, 100, 100), "NULL", style);
            else
                GUI.Label(new Rect(0, 200, 100, 100), Target.Obj.name, style);
        }
    }
}
